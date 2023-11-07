using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace CitefullAPI.Classes.Carousels
{
    public class DataBlock
    {
        private object recordId;
        public object _id { get => recordId.ToString(); set => recordId = value; }
        public int index { get; set; }
        public string type { get; set; }
        public string title { get; set; }
        public string heading { get; set; }
        public Guid instance { get; set; }
        public List<ArticleBlock> articles { get; set; }

        public void SetArticleInstance()
        {
            foreach (ArticleBlock a in articles)
                a.instance = instance;
        }
    }
}
