using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Bson;

namespace CitefullAPI.Classes.Carousels
{
    public class ControlBlocks
    {
        public string _id { get; set; }
        public bool complete { get; set; }
        public List<ControlBlock> controlBlocks { get; set; }

        public ControlBlocks()
        {
        }

        public ControlBlocks(string[] BlockList)
        {
            controlBlocks = new List<ControlBlock>();
            foreach(string Block in BlockList)
            {
                switch(Block)
                {
                    case "topstories":
                        controlBlocks.Add(new ControlBlockTopStory());
                        break;
                    case "keyword":
                        controlBlocks.Add(new ControlBlockKeyword());
                        break;
                }
            }
            _id = "custom";
            complete = true;
        }

        public static ControlBlocks GetBlocks(string name)
        {
            IMongoClient client = new MongoClient(ApplicationSettings.MongoConnection);
            IMongoDatabase db = client.GetDatabase(ApplicationSettings.DatabaseName);
            string querystring = string.Format("{{\"_id\": \"{0}\"}}", name);
            return db.GetCollection<ControlBlocks>("profile").Find<ControlBlocks>(MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(querystring)).First<ControlBlocks>();
        }
    }
}
