using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.Extensions.Options;
using SatisSitesi.Application.Interfaces.Repositories;
using SatisSitesi.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SatisSitesi.Infrastructure.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        private readonly IMongoCollection<T> _collection;

        public GenericRepository(
            IMongoClient client,
            IOptions<MongoSettings> settings,
            string collectionName)
        {
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _collection = database.GetCollection<T>(collectionName);
        }

        public List<T> GetAll()
        {
            // Eğer T tipi BaseEntity'den kalıtım alıyorsa (ki artık alıyor), silinmemiş ve aktif olanları getir.
            // Eski kayıtlarda IsDeleted alanı henüz olmadığı için Ne(true) (True Olmayanlar) kullanıyoruz.
            if (typeof(SatisSitesi.Domain.Entities.BaseEntity).IsAssignableFrom(typeof(T)))
            {
                var filter = Builders<T>.Filter.Ne("IsDeleted", true);
                return _collection.Find(filter).ToList();
            }

            return _collection.Find(_ => true).ToList();
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter)
        {
            return _collection.Find(filter).FirstOrDefault();
        }

        public T GetById(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
                return null;

            if (typeof(SatisSitesi.Domain.Entities.BaseEntity).IsAssignableFrom(typeof(T)))
            {
                var filter = Builders<T>.Filter.And(
                    Builders<T>.Filter.Eq("_id", objectId),
                    Builders<T>.Filter.Ne("IsDeleted", true)
                );
                return _collection.Find(filter).FirstOrDefault();
            }
            
            var defaultFilter = Builders<T>.Filter.Eq("_id", objectId);
            return _collection.Find(defaultFilter).FirstOrDefault();
        }

        public void Insert(T entity)
        {
            _collection.InsertOne(entity);
        }

        public void Update(string id, T entity)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
                return;

            var filter = Builders<T>.Filter.Eq("_id", objectId);
            _collection.ReplaceOne(filter, entity);
        }

        public void Delete(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
                return;

            if (typeof(SatisSitesi.Domain.Entities.BaseEntity).IsAssignableFrom(typeof(T)))
            {
                var filter = Builders<T>.Filter.Eq("_id", objectId);
                var entity = _collection.Find(filter).FirstOrDefault();
                
                if (entity != null)
                {
                    // Soft delete update
                    var update = Builders<T>.Update
                        .Set("IsDeleted", true)
                        .Set("DeletedAt", DateTime.Now)
                        .Set("IsActive", false);
                    
                    _collection.UpdateOne(filter, update);
                }
            }
            else 
            {
                // Hard delete fallback
                var filter = Builders<T>.Filter.Eq("_id", objectId);
                _collection.DeleteOne(filter);
            }
        }
    }
}
