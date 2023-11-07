using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace CitefullAPI.Classes.Carousels
{
    public class ControlBlockKeyword : ControlBlock, IControlBlockInterface
    {
        public ControlBlockKeyword()
        {
            name = "New Keyword Search";
            classname = "keyword";
            querylimit = 50;
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

            string querystring = "{'scan_date': {'$gte': new Date('%0'), '$lte': new Date('%1')}}";
            string keywords = "";

            DataBlockKeyword ResultBlock = new DataBlockKeyword();
            ResultBlock.instance = instance;
            ResultBlock.title = controlBlock.name ?? name;
            ResultBlock.type = "keyword";

            foreach (QueryParams qp in controlBlock.parameters)
            {
                if (qp.TranslateDefault() != null)
                {
                    querystring = querystring.Replace(qp.key, qp.currentvalue);
                    if (qp.name == "$text")
                    {
                        if (ResultBlock.title == "")
                            ResultBlock.title = qp.currentvalue;

                        foreach (string keyword in qp.TranslateDefault().Split())
                        {
                            keywords += ((keywords == "") ? "" : ", ") + "'" + keyword + "'";
                        }
                        querystring = querystring.Remove(querystring.LastIndexOf('}')) + ", 'keywords': { '$all': [" + keywords + "]}}";
                    }
                }

            }
            if (string.IsNullOrEmpty(keywords))
            {
                return new List<DataBlock>();
            }
            else
            {
                ResultBlock.articles = db.GetCollection<ArticleBlock>("article")
                    .Find<ArticleBlock>(MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(querystring))
                    .Project<ArticleBlock>(MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>("{_id: 1, outlet_identity: 1, url: 1, title: 1, top_image: 1, snippet: 1, authors: 1, outlet_score: 1}"))
                    .Sort(MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>("{outlet_score: -1}"))
                    .Skip(controlBlock.length).Limit(controlBlock.querylimit).ToList<ArticleBlock>();
                foreach (ArticleBlock a in ResultBlock.articles)
                    try { a.favicon = DataCache.Favicons[a.outlet_identity]; } catch { }
                ResultBlock.heading = "Keyword Search";

                List<DataBlock> ResultList = new List<DataBlock> { ResultBlock };
                return ResultList;
            }
        }
    }
}
