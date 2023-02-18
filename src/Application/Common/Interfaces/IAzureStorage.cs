using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces;

public interface IAzureStorage
{
    Task<string> UploadUserAvatarAsync(IFormFile file, string blobContainerName, string? avatarUrl);

    Task<string> UploadChatImageAsync(IFormFile file, string blobContainerName, string blobName);

    Task DeleteAvatarImageAsync(string blobContainerName, string avatarUrl);
}