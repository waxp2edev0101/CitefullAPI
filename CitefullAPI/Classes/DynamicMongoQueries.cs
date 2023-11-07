using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace CitefullAPI.Classes
{
    public static class DynamicMongoQueries
    {
        public static Dictionary<string, DynamicMongoQuery> QueryList = new Dictionary<string, DynamicMongoQuery>(StringComparer.OrdinalIgnoreCase);

        public static int Refresh()
        {            
            IMongoClient client = new MongoClient(ApplicationSettings.MongoConnection);
            IMongoDatabase db = client.GetDatabase(ApplicationSettings.DatabaseName);
            IMongoCollection<BsonDocument> mongoQuery = db.GetCollection<BsonDocument>("query");
            IAsyncCursor<BsonDocument> csrQuery = db.GetCollection<BsonDocument>("query").Find<BsonDocument>(new BsonDocument()).ToCursor();
            QueryList.Clear();
            csrQuery.ForEachAsync(c => QueryList.Add(c.GetValue("name").AsString, QueryFactory(c)));
            return QueryList.Count();
        }

        public static IEnumerable<object> Display()
        {
            List<object> DisplayList = new List<object>();
            foreach (DynamicMongoQuery q in QueryList.Values)
            {
                List<object> qParams = new List<object>();
                if (q.parameters != null)
                {
                    foreach (QueryParams cp in q.parameters)
                    {
                        qParams.Add(new { key = cp.key, name = cp.name, description = cp.description, vartype = cp.vartype, vardefault = cp.vardefault });
                    }
                }
                object queryDef = new { name = q.name, description = q.description, returns = q.returns, parameters = qParams };
                DisplayList.Add(queryDef);
            }
            return DisplayList;
        }

        private static DynamicMongoQuery QueryFactory(BsonDocument bsonDoc)
        {            
            switch (bsonDoc.GetValue("querytype").AsString)
            {
                case "collection":
                    return BsonSerializer.Deserialize<CollectionDynamicMongoQuery>(bsonDoc);
                case "aggregate":
                    return BsonSerializer.Deserialize<AggregateDynamicMongoQuery>(bsonDoc);
                default:
                    return BsonSerializer.Deserialize<DynamicMongoQuery>(bsonDoc);
            }
        }
    }
}
