using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using CitefullAPI.Classes;

namespace CitefullAPI.Controllers
{
    [ApiController]
    [Route("")]
    [Route("[controller]")]
    public class QueryController : ControllerBase
    {
        // GET: Query
        public IEnumerable<object> Index([FromQuery]string Refresh = "")
        {
            if (Refresh != string.Empty)
            {
                DynamicMongoQueries.Refresh();
                DataCache.Load();
            }
            return DynamicMongoQueries.Display();
        }
        // GET: Query//Name
        [HttpGet("{name}")]
        public async Task<ContentResult> Get(string name)
        {
            try
            {
                List<string> ResultsList = await DynamicMongoQueries.QueryList[name].Execute(HttpContext);
                Response.Headers.Add("Cache-Control", ApplicationSettings.CacheControl);
                return new ContentResult() { ContentType = "application/json", Content = $"[{String.Join(",", ResultsList.ToArray())}]" };

            }
            catch (Exception ex)
            {
                return new ContentResult() { ContentType = "application/json", Content = $"{{\"Error\": \"{ex.Message}\"}}", StatusCode = 404 };                
            }
        }
    }
}
