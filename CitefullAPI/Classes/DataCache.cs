using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace CitefullAPI.Classes
{
    static public class DataCache
    {
        static public Dictionary<string, string> Categories;
        static public Dictionary<string, string> Favicons;
        static public void Load()
        {
            IMongoClient client = new MongoClient(ApplicationSettings.MongoConnection);
            IMongoDatabase db = client.GetDatabase(ApplicationSettings.DatabaseName);

            Categories = new Dictionary<string, string>();
            db.GetCollection<BsonDocument>("category").Find<BsonDocument>(MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>("{}"))
                .ToCursor().ForEachAsync(b => Categories.Add(b.GetValue("_id").AsString, b.GetValue("title").AsString));

            Favicons = new Dictionary<string, string>();
            db.GetCollection<BsonDocument>("outlet").Find<BsonDocument>(MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>("{favicon: {'$ne': null}}}"))
                .Project<BsonDocument>(MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>("{outlet_identity: 1, favicon: 1}")).ToCursor()
                .ForEachAsync(b => Favicons.Add(b.GetValue("outlet_identity").AsString, b.GetValue("favicon").AsString));
        }
    }
}
