using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CitefullAPI.Classes.Carousels
{
    public class DataBlockKeyword : DataBlock
    {
        public DateTime mindate { get; set; }
        public DateTime maxdate { get; set; }

        public DataBlockKeyword()
        {
            _id = Guid.NewGuid();
            articles = new List<ArticleBlock>();
            heading = "";
        }
    }
}
