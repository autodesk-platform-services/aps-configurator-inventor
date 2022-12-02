using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ForgeControllers.Controllers
{
    public static class ConfiguratorController
    {

        public static string BaseURL
        {
            get
            {
                string appsettingsName = "appsettings.json";
#if DEBUG
                appsettingsName = "appsettings.debug.json";
#endif

                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string executingPath = Uri.UnescapeDataString(uri.Path);

                string appsettingsDirectory = Path.GetDirectoryName(executingPath);

                string appsettingsPath = Path.Combine(appsettingsDirectory, appsettingsName);
                Console.WriteLine($"Base appsettings file path: {appsettingsPath}");

                string appsettingsData = File.ReadAllText(appsettingsPath);
                JObject appsettings = JObject.Parse(appsettingsData);

                return appsettings["ApplicationUrl"].ToString();
            }
        }
    }
}
