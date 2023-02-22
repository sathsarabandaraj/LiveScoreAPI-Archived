using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDBDemo
{
    public class MongoCRUD
    {
        private readonly IMongoDatabase _db;
        private readonly string _collectionName;

        public MongoCRUD(IConfiguration config, string collection)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDB"));
            _db = client.GetDatabase("match1");
            _collectionName = collection;
        }

        public async Task<(bool, string)> InsertRecord<T>(T record)
        {
            try
            {
                var collection = _db.GetCollection<T>(_collectionName);
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
                var collection = _db.GetCollection<T>(_collectionName);
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
                var collection = _db.GetCollection<T>(_collectionName);
                var documents = await collection.Find(query).ToListAsync();
                return (true, "", documents);
            }
            catch (Exception mongoLoadQEx)
            {
                return (false, mongoLoadQEx.Message, null);
            }
        }

        public async Task<(bool, string)> UpdateRecordById<T>(int id, T record)
        {
            try
            {
                var filter = Builders<T>.Filter.Eq("_id", id);
                var getQueryStatus = await LoadRecordsByQuery(filter);

                if (!getQueryStatus.Item1)
                {
                    return (false, getQueryStatus.Item2);
                }

                if (getQueryStatus.Item3 == null || getQueryStatus.Item3.Count == 0)
                {
                    return (false, "not found.");
                }

                var collection = _db.GetCollection<T>(_collectionName);
                var result = await collection.ReplaceOneAsync(filter, record);

                if (result.ModifiedCount == 0)
                {
                    return (false, "not updated.");
                }

                return (true, "updated successfully");
            }
            catch (Exception monogoUpdateEx)
            {
                return (false, monogoUpdateEx.Message);
            }
        }

        public async Task<(bool, string)> DeleteRecord<T>(int? id)
        {
            try
            {
                var collection = _db.GetCollection<T>(_collectionName);
                var filter = Builders<T>.Filter.Eq("_id", id);
                var result = await collection.DeleteOneAsync(filter);
                if (result.DeletedCount > 0)
                {
                    return (true, "deleted successfully");
                }
                return (false, "no record found");
            }
            catch (Exception mongoDeleteEx)
            {
                return (false, mongoDeleteEx.Message);
            }
        }
    }
}
