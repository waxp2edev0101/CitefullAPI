using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Bson;

namespace CitefullAPI.Classes
{
    [BsonIgnoreExtraElements]
    public class CollectionDynamicMongoQuery : DynamicMongoQuery
    {
        protected override IAsyncCursor<BsonDocument> RunQuery(List<string> removeElements, string limit, string skip)
        {
            IMongoClient client = new MongoClient(ApplicationSettings.MongoConnection);
            IMongoDatabase db = client.GetDatabase(ApplicationSettings.DatabaseName);
            IMongoCollection<BsonDocument> collection = db.GetCollection<BsonDocument>(this.source);

            BsonDocument bdoc = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(this.query);
            foreach (string r in removeElements)
                bdoc.RemoveElement(bdoc.GetElement(r));

            skip = (skip == null || skip == String.Empty) ? "0" : skip;
            return (collection.Find<BsonDocument>(MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(this.query)).Project(this.project).Sort(this.sort).Skip(Convert.ToInt16(skip)).Limit(Convert.ToInt16(limit)).ToCursor());            
        }
    }
}
