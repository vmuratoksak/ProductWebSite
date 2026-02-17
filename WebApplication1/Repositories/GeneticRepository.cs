using MongoDB.Driver;
using Microsoft.Extensions.Options;
using WebApplication1.Models;

namespace WebApplication1.Repositories
{
    public class GenericRepository<T> : IRepository<T>
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
            return _collection.Find(_ => true).ToList();
        }

        public T GetById(string id)
        {
            return _collection.Find("_id", id).FirstOrDefault();
        }

        public void Insert(T entity)
        {
            _collection.InsertOne(entity);
        }

        public void Update(string id, T entity)
        {
            _collection.ReplaceOne("_id", id, entity);
        }

        public void Delete(string id)
        {
            _collection.DeleteOne("_id", id);
        }
    }
}
