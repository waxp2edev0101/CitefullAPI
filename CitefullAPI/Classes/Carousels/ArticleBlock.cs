using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CitefullAPI.Classes.Carousels
{
    public class ArticleBlock
    {
        private object recordId;
        public object _id { get => recordId.ToString(); set => recordId = value; }
        public string outlet_identity { get; set; }
        public string url { get; set; }
        public string title { get; set; }
        public string top_image { get; set; }
        public string snippet { get; set; }
        public string[] authors { get; set; }
        public string favicon { get; set; }
        public double outlet_score { get; set; }
        public int group { get; set; }
        public int matchsize { get; set; }
        public int pcnt { get; set; }
        public Guid instance { get; set; }
    }
}
