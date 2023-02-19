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
    }
}