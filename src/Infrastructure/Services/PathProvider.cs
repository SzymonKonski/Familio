using Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace Infrastructure.Services;

public class PathProvider : IPathProvider
{
    private readonly IWebHostEnvironment _hostEnvironment;

    public PathProvider(IWebHostEnvironment environment)
    {
        _hostEnvironment = environment;
    }

    public string MapPath(string path)
    {
        var filePath = Path.Combine(_hostEnvironment.WebRootPath, path);
        return filePath;
    }
}