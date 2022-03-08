using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AzureStorage
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var config = GetConfiguration();
            var files = GetFiles(config["AzureStorage:SourceFolder"]);
            
            if (!files.Any())
            {
                Console.WriteLine("Nothing to process");
                
            }
            Upload(files, config["AzureStorage:ConnectionString"], config["AzureStorage:Container"]);
            Download(config["AzureStorage:ConnectionString"], config["AzureStorage:Container"], config["AzureStorage:DownloadFolder"]);
            Saas(config["AzureStorage:ConnectionString"], config["AzureStorage:Container"]);




        }
        static IConfigurationRoot GetConfiguration()
            => new ConfigurationBuilder()
            .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
            .AddJsonFile("appSettings.json")
            .Build();
        static IEnumerable<FileInfo> GetFiles(string sourceFolder)
        => new DirectoryInfo(sourceFolder)
            .GetFiles()
            .Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden));
    static void Upload(
            IEnumerable <FileInfo> files,
            string ConnectionString,
            string Container
            )
        {

            var containerClient = new BlobContainerClient(ConnectionString, Container);
            Console.WriteLine("Uploading files to the storage");
            foreach(var file in files)
            {
                try
                {
                    var blobClient = containerClient.GetBlobClient(file.Name);
                    using(var fileStream = File.OpenRead(file.FullName))
                    {
                        blobClient.Upload(fileStream);
                    }
                    Console.WriteLine($"{file.Name} uploaded");
                    File.Delete(file.FullName);
                }
                catch
                {

                }
            }
        }
        static void Download(
          
            string ConnectionString,
            string Container,
            string DownloadFolder
            )
        {

            BlobServiceClient blobServiceClient = new BlobServiceClient(ConnectionString);


            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(Container);
            var blobs = containerClient.GetBlobs();
            foreach(var blob in blobs)
            {
                Console.WriteLine(blob.Name);
                BlobClient blobClient = containerClient.GetBlobClient(blob.Name);
                blobClient.DownloadTo(DownloadFolder+blob.Name);

            }
            Console.WriteLine("Download Compleled!");


        }
        static void Saas(
             string ConnectionString,
            string Container)
        {
            Console.WriteLine(ConnectionString);
            Console.WriteLine(Container);
            BlobServiceClient blobServiceClient = new BlobServiceClient(ConnectionString);


            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(Container);
            SharedAccessBlobPolicy sharedPolicy = new SharedAccessBlobPolicy()
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessStartTime = DateTimeOffset.UtcNow,
                SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddHours(7),



            };
           
        }

    }
}
