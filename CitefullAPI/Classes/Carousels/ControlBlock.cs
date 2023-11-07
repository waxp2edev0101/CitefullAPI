using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CitefullAPI.Classes.Carousels
{
    interface IControlBlockInterface
    {
        public abstract List<DataBlock> GetData(ControlBlock controlBlock);
    }
    public class ControlBlock
    {
        public string name { get; set; }
        public string classname { get; set; }
        public int querylimit { get; set; }
        public Guid instance { get; set; }
        public int length { get; set; }
        public bool complete { get; set; }
        public QueryParams[] parameters { get; set; }

        public ControlBlock()
        {
            this.instance = Guid.NewGuid();
        }
    }
}
