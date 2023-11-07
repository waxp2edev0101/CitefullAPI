using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CitefullAPI.Classes.Carousels
{
    public class CarouselResponse
    {
        public ControlBlocks ControlBlock { get; set; }
        public List<object> DataBlocks { get; set; }

        public CarouselResponse()
        {
            DataBlocks = new List<object>();
        }

        public CarouselResponse(ControlBlocks controlBlocks)
        {
            DataBlocks = new List<object>();
            this.ControlBlock = controlBlocks;
            PopulateAll();
        }

        public void PopulateAll()
        {
            this.ControlBlock.complete = true;
            foreach(ControlBlock blk in this.ControlBlock.controlBlocks)
            {
                if (!blk.complete)
                {
                    switch (blk.classname)
                    {
                        case "topstories":
                            List<DataBlock> TopStory = new ControlBlockTopStory().GetData(blk);
                            // topstories generates multiple blocks
                            blk.complete = (TopStory.Count == 0);
                            if (!blk.complete)
                                this.ControlBlock.complete = false;

                            blk.length += TopStory.Count;
                            foreach (DataBlockTopStory ts in TopStory)
                                DataBlocks.Add(ts);
                            break;
                        case "keyword":
                            List<DataBlock> KeyWord = new ControlBlockKeyword().GetData(blk);
                            //keywords generates a single block
                            blk.complete = true;
                            blk.length += KeyWord.Count;
                            foreach (DataBlockKeyword kw in KeyWord)
                                DataBlocks.Add(kw);
                            break;
                    }
                }
            }
        }
    }
}
