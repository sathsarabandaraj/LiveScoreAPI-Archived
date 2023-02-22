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
                var doc = await collection.Find(new BsonDocument()).ToListAsync();
                return (true, "", doc);
            }
            catch (Exception mongoGetEx)
            {
                return (false, mongoGetEx.Message, null);
            }
        }

        public async Task<(bool, string, List<T>?)> LoadRecordsByQuery<T>(FilterDefinition<T> query)
        {
            try
            {
                var collection = db.GetCollection<T>(collectionName);
                var documents = await collection.Find(query).ToListAsync();
                return (true, "", documents);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public async Task<(bool, string)> UpdateRecordById<T>(string id, T record)
        {
            try
            {
                var filter = Builders<T>.Filter.Eq("_id", int.Parse(id));
                var getQueryStatus = await LoadRecordsByQuery(filter);

                if (!getQueryStatus.Item1)
                {
                    return (false, getQueryStatus.Item2);
                }

                if (getQueryStatus.Item3 == null || getQueryStatus.Item3.Count == 0)
                {
                    return (false, "not found.");
                }

                var collection = db.GetCollection<T>(collectionName);
                var result = await collection.ReplaceOneAsync(filter, record);

                if (result.ModifiedCount == 0)
                {
                    return (false, "not updated.");
                }

                return (true, "updated successfully");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

    }
}
