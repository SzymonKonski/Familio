using Application.Common.Interfaces;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;

public class AzureStorage : IAzureStorage
{
    private readonly string _storageConnectionString;

    public AzureStorage(IConfiguration configuration)
    {
        _storageConnectionString = configuration.GetConnectionString("AzureStorage");
    }

    public async Task<string> UploadUserAvatarAsync(IFormFile file, string blobContainerName, string? avatarUrl)
    {
        var container = new BlobContainerClient(_storageConnectionString, blobContainerName);
        var createResponse = await container.CreateIfNotExistsAsync();
        if (createResponse != null && createResponse.GetRawResponse().Status == 201)
            await container.SetAccessPolicyAsync(PublicAccessType.Blob);

        var newBlobName = Guid.NewGuid().ToString();
        if (avatarUrl != null)
        {
            var oldBlobName = avatarUrl.Split('/').Last();
            var oldBlob = container.GetBlobClient(oldBlobName);
            await oldBlob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
        }

        var newBlob = container.GetBlobClient(newBlobName);
        await newBlob.UploadAsync(file.OpenReadStream(), new BlobHttpHeaders {ContentType = file.ContentType});
        return newBlob.Uri.ToString();
    }

    public async Task<string> UploadChatImageAsync(IFormFile file, string blobContainerName, string blobName)
    {
        var container = new BlobContainerClient(_storageConnectionString, blobContainerName);
        var createResponse = await container.CreateIfNotExistsAsync();
        if (createResponse != null && createResponse.GetRawResponse().Status == 201)
            await container.SetAccessPolicyAsync(PublicAccessType.Blob);
        var blob = container.GetBlobClient(blobName);
        await blob.UploadAsync(file.OpenReadStream(), new BlobHttpHeaders {ContentType = file.ContentType});
        return blob.Uri.ToString();
    }

    public async Task DeleteAvatarImageAsync(string blobContainerName, string avatarUrl)
    {
        var container = new BlobContainerClient(_storageConnectionString, blobContainerName);
        var createResponse = await container.CreateIfNotExistsAsync();
        if (createResponse != null && createResponse.GetRawResponse().Status == 201)
            await container.SetAccessPolicyAsync(PublicAccessType.Blob);
        var blobName = avatarUrl.Split('/').Last();
        var blob = container.GetBlobClient(blobName);
        await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
    }
}