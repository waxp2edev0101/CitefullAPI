using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitefullAPI.Classes.Carousels;

namespace CitefullAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CarouselController : ControllerBase
    {
        // GET <CarouselController>
        [HttpGet]
        public ControlBlocks Get()
        {
            if (Request.Query.ContainsKey("blockname"))
            {
                return new ControlBlocks(Request.Query["blockname"]);
            }

            return ControlBlocks.GetBlocks("default");

        }

        // GET <CarouselController>/Name
        [HttpGet("{profile}")]
        public ControlBlocks Get(string profile)
        {
            return ControlBlocks.GetBlocks(profile);
        }

        // POST <CarouselController>
        [HttpPost]
        public CarouselResponse Post([FromBody] ControlBlocks controlBlocks)
        {
            return new CarouselResponse(controlBlocks);
        }
    }
}
