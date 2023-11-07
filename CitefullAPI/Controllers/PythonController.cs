using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using CitefullAPI.Classes;

namespace CitefullAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PythonController : ControllerBase
    {
        // GET: <CategoryController>
        [HttpGet("scrape")]
        public ContentResult Get([FromQuery] String cmd)
        {
            string StandardResult = "", ErrorResult = "";

            try
            {
                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = ApplicationSettings.PythonExe;
                start.Arguments = $"demand.scrape.py {cmd}";
                start.WorkingDirectory = ApplicationSettings.WorkingDir;
                start.UseShellExecute = false;
                start.RedirectStandardOutput = true;
                start.RedirectStandardError = true;
                using (Process process = Process.Start(start))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        StandardResult = $"<div>{reader.ReadToEnd()}</div>";
                    }
                    using (StreamReader reader = process.StandardError)
                    {
                        ErrorResult = $"<div style='background-color:red'>{reader.ReadToEnd()}</div>";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorResult = ex.Message;
            }

            return new ContentResult() { ContentType = "text/html", Content = StandardResult+ErrorResult };
        }
    }
}
