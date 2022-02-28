# Azure Storage Account- .NET

This simple application provides basic overwiew of uploading and downloading files in Azure portal.



## Development

Upload files to the Azure. 

```bash
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
```
Download files from Azure.  

```bash
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

```
