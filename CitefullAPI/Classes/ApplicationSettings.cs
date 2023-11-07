using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CitefullAPI.Classes
{
    public static class ApplicationSettings
    {
        public static string MongoConnection { get; set; }
        public static string DatabaseName { get; set; }
        public static string CacheControl { get; set; }
        public static string WorkingDir { get; set; }
        public static string PythonExe { get; set; }
    }
}
