#r "Microsoft.WindowsAzure.Storage"

using System;
using System.Configuration;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Renci.SshNet;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;

public static async Task Run(TimerInfo myTimer, TraceWriter log)
{
    log.Info($"C# Timer trigger function executed at: {DateTime.Now}");

    log.Info("Retrieving secret from keyvault");

    string sftpPasswordUrl = "{URL to Azure Secret with SFTP Password}";
    
    AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
    
    var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
    
    var sftpPasswordSecret = await keyVaultClient.GetSecretAsync(sftpPasswordUrl).ConfigureAwait(false);
    
    string sftpPasswordString = sftpPasswordSecret.Value;
    
    log.Info($"Secret Read successfully. Now connecting to SFTP Server");

    string host = @"azfdemoftp.eastus2.cloudapp.azure.com";

    string username = "{SFTP Ussr Name}";

    var sftp = new SftpClient(host, username, sftpPasswordString);

    sftp.Connect();

    log.Info($"SFTP Connect Success");

    string date = DateTime.Now.ToString("yyyyMMdd");

    log.Info($"Date: {date}");

    string remoteDirectory = "/Level1/Level2/Level3/Daily/";

    var files = sftp.ListDirectory(remoteDirectory);

    var TFile = "";

    string StorageConnectionString = ConfigurationManager.ConnectionStrings["storage_con"].ConnectionString;

    string targetShareReference = "tempdaily";

    foreach (var file in files)
    {
        log.Info($"Processing File: {file.Name}");
        if (file.Name.Contains(date))
        {
            TFile = file.Name;
            log.Info($"This is Today's file: {TFile}");

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference(targetShareReference);

            var memoryStream = new MemoryStream();
            sftp.DownloadFile(remoteDirectory + TFile,memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            CloudBlockBlob blockBlob = container.GetBlockBlobReference("zip/"+TFile);
            blockBlob.Properties.ContentType = "application/zip";
            //blockBlob.SetProperties();
            blockBlob.UploadFromStream(memoryStream);

            /* This is for file share upload
            CloudFileClient fileClient = storageAccount.CreateCloudFileClient();
            CloudFileShare fileShare = fileClient.GetShareReference(targetShareReference);
            CloudFileDirectory rootDirectory = fileShare.GetRootDirectoryReference();
            var cloudFile = rootDirectory.GetFileReference(TFile);
            //Stream stream = new MemoryStream(file);
            //cloudFile.UploadFromStream(sftp.DownloadFile(remoteDirectory + TFile));
            var memoryStream = new MemoryStream();
            sftp.DownloadFile(remoteDirectory + TFile,memoryStream);
            cloudFile.UploadFromStream(memoryStream);
            */
            log.Info("Upload Done!");   
        }
    }
}
