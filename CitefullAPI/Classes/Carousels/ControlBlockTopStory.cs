using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace CitefullAPI.Classes.Carousels
{
    public class ControlBlockTopStory : ControlBlock, IControlBlockInterface
    {
        public ControlBlockTopStory()
        {
            name = "New Top Story Search";
            classname = "topstories";
            querylimit = 20;
            parameters = new QueryParams[] { 
                new QueryParams() { key = "%0", name = "mindate", vartype = "DateTime", vardefault = "48H" },
                new QueryParams() { key = "%1", name = "maxdate", vartype = "DateTime", vardefault = "0" },
                new QueryParams() { key = "%2", name = "$text", vartype = "String"}
            };
            foreach (QueryParams p in parameters)
                p.TranslateDefault();
        }

        public List<DataBlock> GetData(ControlBlock controlBlock)
        {
            IMongoClient client = new MongoClient(ApplicationSettings.MongoConnection);
            IMongoDatabase db = client.GetDatabase(ApplicationSettings.DatabaseName);

            string querystring = "{'type': 'topstory2.0', 'mindate': {'$gte': new Date('%0'), '$lte': new Date('%1')}}";
            List<DataBlock> ResultList = new List<DataBlock>();

            foreach (QueryParams qp in controlBlock.parameters)
            {
                querystring = querystring.Replace(qp.key, qp.TranslateDefault());
                if (qp.name == "$text" && (!string.IsNullOrEmpty(qp.currentvalue) || qp.vardefault != null))
                {
                    // ** Someday this will be text **
                    //querystring = querystring.Remove(querystring.LastIndexOf('}')) +  ", '$text': { '$search': '" + qp.TranslateDefault() + "'}})";

                    string keywords = "";
                    foreach(string keyword in qp.TranslateDefault().Split())
                    {
                        keywords += ((keywords == "") ? "" : ", ") + "'" + keyword + "'";
                    }
                    querystring = querystring.Remove(querystring.LastIndexOf('}')) + ", 'keywords': { '$in': [" + keywords + "]}})";
                }
            }
            return CursorToList(db.GetCollection<BsonDocument>("carousel")
                .Find<BsonDocument>(MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(querystring))
                .Project(MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>("{_id: 1, type: 1, category: 1, mindate: 1, articles: 1}")).Skip(controlBlock.length).Limit(controlBlock.querylimit).ToCursor()).Result;
        }

        private async Task<List<DataBlock>> CursorToList(IAsyncCursor<BsonDocument> results)
        {
            List<DataBlock> ResultList = new List<DataBlock>();            
            await results.ForEachAsync(b => ResultList.Add(DataBlockInit(MongoDB.Bson.Serialization.BsonSerializer.Deserialize<DataBlockTopStory>(b.ToJson(new MongoDB.Bson.IO.JsonWriterSettings() { OutputMode = MongoDB.Bson.IO.JsonOutputMode.CanonicalExtendedJson })))));
            return ResultList;
        }

        private DataBlockTopStory DataBlockInit(DataBlockTopStory datablock)
        {
            if (datablock.category == "")
            {
                datablock.category = "unassigned";
                datablock.title = "None";
            }
            else
                datablock.title = DataCache.Categories[datablock.category];
            datablock.heading = string.Format("Top Stories {0:D}", datablock.mindate);
            datablock.instance = instance;
            return datablock;
        }

    }
}
