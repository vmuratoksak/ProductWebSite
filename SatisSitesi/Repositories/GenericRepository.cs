using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.Extensions.Options;
using SatisSitesi.Repositories.Interfaces;
using SatisSitesi.Models;
using System.Collections.Generic;

namespace SatisSitesi.Repositories
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
            var result =  _collection.Find(_ => true).ToList();
            return result;
        }

        //sadece email ile veri çekelim

        public T GetById(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
                return null;

            var filter = Builders<T>.Filter.Eq("_id", objectId);
            return _collection.Find(filter).FirstOrDefault();
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

            var filter = Builders<T>.Filter.Eq("_id", objectId);
            _collection.DeleteOne(filter);
        }
    }
}
