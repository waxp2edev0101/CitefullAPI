using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using CitefullAPI.Classes;

namespace CitefullAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoryController : ControllerBase
    {
        // GET: <CategoryController>
        [HttpGet]
        public async Task<ContentResult> Get()
        {
            IMongoClient client = new MongoClient(ApplicationSettings.MongoConnection);
            IMongoDatabase db = client.GetDatabase("citefull");
            IMongoCollection<BsonDocument> col = db.GetCollection<BsonDocument>("category");

            IAsyncCursor<BsonDocument> results = col.Find<BsonDocument>(MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>("{}")).Sort("{\"title\": 1}").ToCursor();
            List<string> resultslist = new List<string>();
            await results.ForEachAsync(b => resultslist.Add(b.ToJson(new MongoDB.Bson.IO.JsonWriterSettings() { OutputMode = MongoDB.Bson.IO.JsonOutputMode.CanonicalExtendedJson })));
            Response.Headers.Add("Cache-Control", ApplicationSettings.CacheControl);
            return new ContentResult() { ContentType = "application/json", Content = $"[{String.Join(",", resultslist.ToArray())}]" };
        }


        // POST <CategoryController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT <CategoryController>/5
        [HttpPut("{collection}")]
        public ContentResult Put(string collection, [FromQuery] string id, [FromQuery] string category)
        {
            IMongoClient client = new MongoClient(ApplicationSettings.MongoConnection);
            IMongoDatabase db = client.GetDatabase("citefull");
            IMongoCollection<BsonDocument> col = db.GetCollection<BsonDocument>("category");
            if (col.CountDocuments($"{{\"_id\": \"{category}\"}}") != 1)
            { 
                return new ContentResult() { ContentType = "application/json", Content = $"{{\"Error\": \"Category {category} not found.\"}}", StatusCode = 500 };
            }

            collection = collection.ToLower();
            String[] collections = { "article", "carousel" };
            if (Array.Find(collections, e => e==collection) != collection)
            {
                return new ContentResult() { ContentType = "application/json", Content = $"{{\"Error\": \"Collection {collection} not found or category assignment not allowed.\"}}", StatusCode = 500 };
            }

            try
            {
                String jsonFilter = $"{{_id: ObjectId('{id}')}}";
                String jsonUpdate = $"{{\"$set\": {{\"category\": \"{category}\"}}}}";
                col = db.GetCollection<BsonDocument>(collection);
                BsonDocument newdoc = col.FindOneAndUpdate(jsonFilter, jsonUpdate);
                if (newdoc == null)
                    return new ContentResult() { ContentType = "application/json", Content = "{\"Error\": \"Document not found.\"}" };

                return new ContentResult() { ContentType = "application/json", Content = "{\"Success\": \"Category assigned.\"}" };
            }
            catch(Exception ex)
            {
                return new ContentResult() { ContentType = "application/json", Content = $"{{\"Error\": \"{ex.Message}\"}}", StatusCode = 500};
            }
        }

        // DELETE api/<CategoryController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
