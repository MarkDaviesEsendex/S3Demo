using System.IO;
using Microsoft.Extensions.Configuration;

namespace FileUpload
{
    internal class ServiceSettings
    {
        public AmazonS3Settings Amazon { get; private set; }

        private static ServiceSettings FromAppSettings(IConfigurationRoot settings)
        {
            return new ServiceSettings
            {
                Amazon = AmazonS3Settings.FromAppSettings(settings),
            };
        }

        public static ServiceSettings FromAppSettings()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            return FromAppSettings(config);
        }
    }
}