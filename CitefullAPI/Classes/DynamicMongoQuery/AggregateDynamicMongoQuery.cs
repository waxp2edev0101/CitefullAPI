using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Bson;

namespace CitefullAPI.Classes
{
    [BsonIgnoreExtraElements]
    public class AggregateDynamicMongoQuery : DynamicMongoQuery
    {
        protected override IAsyncCursor<BsonDocument> RunQuery(List<string> removeElements, string limit, string skip)
        {
            IMongoClient client = new MongoClient(ApplicationSettings.MongoConnection);
            IMongoDatabase db = client.GetDatabase(ApplicationSettings.DatabaseName);
            IMongoCollection<BsonDocument> collection = db.GetCollection<BsonDocument>(source);

            BsonArray query = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonArray>(this.query);
            BsonDocument bdoc = query.ElementAt(0).AsBsonDocument.GetValue("$match").ToBsonDocument();
            foreach (string r in removeElements)
                bdoc.RemoveElement(bdoc.GetElement(r));

            PipelineDefinition<BsonDocument, BsonDocument> pipeline = PipelineDefinition<BsonDocument, BsonDocument>.Create(query.Select(val => val.AsBsonDocument));
            pipeline = pipeline.Limit(Convert.ToInt16(limit));
            if (skip != null && skip != string.Empty)
                pipeline = pipeline.Skip(Convert.ToInt16(skip));

            return (collection.Aggregate<BsonDocument>(pipeline));
        }
    }
}
