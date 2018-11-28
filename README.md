# Sftp to azure blob using azure functions
Copy files from SFTP server to Azure Blob Storage using Azure Functions and Azure Key Vault
Using Azure Functions v1.x

## Pre-requisites
- Azure key valut and SFTP password stored in Secret [Reference Article here](https://blogs.msdn.microsoft.com/benjaminperkins/2018/06/13/create-an-azure-key-vault-and-secret/).
- Azure Function should have a connection string pre-created with **storage_con** [Reference Article here](https://docs.microsoft.com/en-us/azure/azure-functions/functions-scenario-database-table-cleanup).
  - This will be used to connect to the Azure Blob Storage
- For a complete end to end live demo refer to my blog with video [here](https://www.jasjitchopra.com/url).
## Guided Steps
1. Create the Azure function and pick a trigger of your choice with c# as the language
2. In the files section create a new file called project.json
3. Copy the code present in the project.json from this github repo in that file
4. Open the run.csx file and completely replace the code from run.csx file from this github repo
5. Replace the hyperlink **{URL to Azure Secret with SFTP Password}** on line 19 with your link to the Azure Key Vault secret
6. Replace the SFTP server **{SFTP Server IP address or FQDN}** on line 31 with the ip address or FQDN of your SFTP server
7. Replace the SFTP username **{SFTP Ussr Name}** on line 33 with the username required to connect to the SFTP server
8. Change the root level direcry path to pick your zip file on line 45
9. Make sure the path referenced in Azure Blob matches with the name and path stated on lines 53 and 73
