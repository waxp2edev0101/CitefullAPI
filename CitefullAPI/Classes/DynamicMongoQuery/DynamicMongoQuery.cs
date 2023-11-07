using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using MongoDB.Bson;

namespace CitefullAPI.Classes
{
    [BsonIgnoreExtraElements]
    public class DynamicMongoQuery
    {
        public string querytype;
        public string name;
        public string description;
        public string source;
        public string returns;
        public string query;
        public string project = "{}";
        public string sort = "{}";
        public QueryParams[] parameters;

        public async Task<List<string>> Execute(HttpContext httpQuery)
        {
            List<string> RemoveElements = new List<string>();

            String limit = "";
            String skip = "";
            if (this.parameters != null)
            {
                foreach (QueryParams qp in this.parameters)
                {
                    string defaultValue = httpQuery.Request.Query[qp.name].ToString();
                    defaultValue = (defaultValue == String.Empty) ? qp.TranslateDefault() : defaultValue; // Use the query value encapsulated in quotes first, otherwise take the default
                    switch (qp.name)
                    {
                        case "limit":
                            limit = defaultValue;
                            break;
                        case "skip":
                            skip = defaultValue;
                            break;
                        default:
                            if (defaultValue != null && defaultValue != string.Empty)
                                query = query.Replace(qp.key, defaultValue);
                            else if (!qp.required)
                                RemoveElements.Add(qp.name);
                            break;
                    }
                }
            }
            limit = (limit == String.Empty) ? "20" : limit; // Limit will be 20 by default for safety

            List<string> resultslist = new List<string>();
            IAsyncCursor<BsonDocument> results = this.RunQuery(RemoveElements, limit, skip);

            await results.ForEachAsync(b => resultslist.Add(b.ToJson(new MongoDB.Bson.IO.JsonWriterSettings() { OutputMode = MongoDB.Bson.IO.JsonOutputMode.CanonicalExtendedJson })));
            return resultslist;
        }

        protected virtual IAsyncCursor<BsonDocument> RunQuery(List<string> removeElements, string limit, string skip)
        {
            throw new Exception($"Query of type {this.querytype} not supported");
        }
    }
}