using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CitefullAPI.Classes.Carousels
{
    public class DataBlockTopStory : DataBlock
    {
        public string category { get; set; }
        public DateTime mindate { get; set; }
        public DateTime maxdate { get; set; }

        public DataBlockTopStory()
        {
            category = "None";
        }
    }
}
