using System;
using System.Configuration;
using System.IO;

namespace WebpageScreenshotTaker.Data
{
    public static class ConnectionString
    {
        public static string Get(string id = "Default")
        {
#if(DEBUG)
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
#endif
            string path = @"\Webpage Screenshot Taker\Data\ScreenshotCaptureDB.db;Version=3;";
            return $"Data Source={Path.GetPathRoot(Environment.SystemDirectory)}" + path;
        }
    }
}
