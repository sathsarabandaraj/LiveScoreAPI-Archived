using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDBDemo
{
    public class MongoCRUD
    {
        private IMongoDatabase db;
        private string collectionName;

        public MongoCRUD(IConfiguration config, string collection)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDB"));
            db = client.GetDatabase("match1");
            collectionName = collection;
        }

        public async Task<(bool, string)> InsertRecord<T>(T record)
        {
            try
            {
                var collection = db.GetCollection<T>(collectionName);
                await collection.InsertOneAsync(record);
                return (true, "inserted successfully");
            }
            catch (Exception mongoInsertEx)
            {
                return (false, mongoInsertEx.Message);
            }
        }

        public async Task<(bool, string, List<T>?)> LoadRecords<T>()
        {
            try
            {
                var collection = db.GetCollection<T>(collectionName);
                var doc = collection.Find(new BsonDocument()).ToList();
                return (true, "", doc);
            }
            catch (Exception mongoGetEx)
            {
                return (false, mongoGetEx.Message, null);
            }
        }
    }
}
