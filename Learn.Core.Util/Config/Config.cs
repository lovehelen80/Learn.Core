using Microsoft.Extensions.Configuration;
using System.IO;

namespace Learn.Core.Util
{
    public class Config
    {
        private static IConfiguration config = null;
        static Config()
        {
            // Microsoft.Extensions.Configuration扩展包提供的
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                    .AddJsonFile("appsettings.json");
            config = builder.Build();
        }

        public static IConfiguration AppSettings
        {
            get
            {
                return config;
            }
        }

        public static string GetValue(string key)
        {
            return config[key];
        }
    }
}
