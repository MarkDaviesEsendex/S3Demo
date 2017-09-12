using Microsoft.Extensions.Configuration;

namespace FileUpload
{
    internal class AmazonS3Settings
    {
        public string AccessKey { get; private set; }
        public string SecretAccessKey { get; private set; }
        public string BucketName { get; private set; }

        public static AmazonS3Settings FromAppSettings(IConfigurationRoot settings) => new AmazonS3Settings
        {
            AccessKey = settings["Amazon:S3:AccessKey"],
            SecretAccessKey = settings["Amazon:S3:SecretAccessKey"],
            BucketName = settings["Amazon:S3:BucketName"],
        };
    }
}