using System;
using System.Globalization;
using System.IO;
using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Xunit;

namespace FileUpload
{
    public class AwsStorage
    {
        private readonly AmazonS3Client _amazonS3Client;
        private readonly ServiceSettings _serviceSettings;

        public AwsStorage()
        {
            _serviceSettings = ServiceSettings.FromAppSettings();
            var awsCredentials = new BasicAWSCredentials(_serviceSettings.Amazon.AccessKey, _serviceSettings.Amazon.SecretAccessKey);
            _amazonS3Client = new AmazonS3Client(awsCredentials, RegionEndpoint.EUWest1);
        }
        
        [Fact]
        public void ThenCanPutFile()
        {
            var fileTransferUtility = new
                TransferUtility(_amazonS3Client);

            using (var fileToUpload =
                new FileStream(GetFileDirectory("test.html"), FileMode.Open, FileAccess.Read))
            {
                var request = new TransferUtilityUploadRequest
                { 
                    BucketName = _serviceSettings.Amazon.BucketName,
                    InputStream = fileToUpload,
                    PartSize = 1,
                    Key = "test/TestKey.html",
                    StorageClass = S3StorageClass.ReducedRedundancy,
                    Headers =
                    {
                        ContentLength = fileToUpload.Length,
                        ContentType = "text/html"
                    }
                };

                fileTransferUtility.Upload(request);
            }
        }

        [Fact]
        public void ThenCanEncryptFiles()
        {
            var fileTransferUtility = new
                TransferUtility(_amazonS3Client);

            using (var fileToUpload =
                new FileStream(GetFileDirectory("test.html"), FileMode.Open, FileAccess.Read))
            {
                var request = new TransferUtilityUploadRequest
                {
                    BucketName = _serviceSettings.Amazon.BucketName,
                    
                    ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256,
//                    ServerSideEncryptionCustomerProvidedKey = "Q3VzdG9tZXIgS2V5",
                    InputStream = fileToUpload,
                    PartSize = 1,
                    Key = "TestKey2.html",
                    StorageClass = S3StorageClass.ReducedRedundancy,
                    Headers =
                    {
                        ContentLength = fileToUpload.Length,
                        ContentType = "text/html",
                        Expires = DateTime.Now.AddDays(1)
                    }
                };

                fileTransferUtility.Upload(request);
            }
        }

        [Fact]
        public void ThenCanExpireFile()
        {
            var fileTransferUtility = new
                TransferUtility(_amazonS3Client);

            using (var fileToUpload =
                new FileStream(GetFileDirectory("test.html"), FileMode.Open, FileAccess.Read))
            {
                var metadataCollection = new MetadataCollection();
                metadataCollection.Add("Expires", DateTime.Now.AddHours(1).ToString(CultureInfo.InvariantCulture));

                var request = new TransferUtilityUploadRequest
                {
                    BucketName = _serviceSettings.Amazon.BucketName,
                    InputStream = fileToUpload,
                    PartSize = 1,
                    AutoCloseStream = true,
                    Key = "TestKey2.html",
                    StorageClass = S3StorageClass.ReducedRedundancy,
                    Headers = 
                    {
                        ContentLength = fileToUpload.Length,
                        ContentType = "text/html",
                        Expires = DateTime.Now.AddDays(1)
                    }
                };

                fileTransferUtility.Upload(request);
            }
        }

        [Fact]
        public void ThenCanGetFile()
        {
            var request = new GetObjectRequest
            {
                BucketName = _serviceSettings.Amazon.BucketName,
                Key = "testingdownload.txt",
                ResponseExpires = DateTime.Now.AddDays(1)
            };
            var result = _amazonS3Client.GetObjectAsync(request).Result;

            var fileStream = File.Create("c:\\git\\file.txt");
            result.ResponseStream.CopyTo(fileStream);
            fileStream.Close();
        }

        [Fact]
        public void ThenCanSetPermissions()
        {
        }

        public string GetFileDirectory(string fileName) 
            => Path.Combine(Directory.GetCurrentDirectory(), fileName);
    }
}
