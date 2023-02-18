using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces;

public interface IFileValidator
{
    bool IsValid(IFormFile file);
}