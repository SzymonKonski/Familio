using System.Text;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Shouldly;

namespace Infrastructure.UnitTests.Services;

public class FileValidatorTests
{
    private readonly IConfiguration _configuration;

    public FileValidatorTests()
    {
        var inMemorySettings = new Dictionary<string, string>();

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
    }

    [Test]
    public void FileValidatorShouldReturnTrueWhenFileIsCorrect()
    {
        // Arrange 
        //Setup mock file using a memory stream
        var content = generateStringSize(1 * 1024);
        var fileName = "test.jpg";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;

        //create FormFile with desired data
        IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);

        // Act
        var fileValidator = new FileValidator(_configuration);

        // Assert
        var result = fileValidator.IsValid(file);
        result.ShouldBe(true);
    }


    [Test]
    public void FileValidatorShouldReturnFalseWhenFileIsBiggerThan1Mb()
    {
        // Arrange 
        //Setup mock file using a memory stream
        var content = generateStringSize(1 * 1024 * 1024 + 100);
        var fileName = "test.pdf";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;

        //create FormFile with desired data
        IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);

        // Act
        var fileValidator = new FileValidator(_configuration);

        // Assert
        var result = fileValidator.IsValid(file);
        result.ShouldBe(false);
    }

    [Test]
    public void FileValidatorShouldReturnFalseWhenFileNameLengthIsGreaterThan255()
    {
        // Arrange 
        //Setup mock file using a memory stream
        var content = "Hello World from a Fake File";
        var fileName = generateStringSize(300);
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;

        //create FormFile with desired data
        IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);

        // Act
        var fileValidator = new FileValidator(_configuration);

        // Assert
        var result = fileValidator.IsValid(file);
        result.ShouldBe(false);
    }

    [Test]
    public void FileValidatorShouldReturnFalseWhenFileHasNotAllowedExtension()
    {
        // Arrange 
        //Setup mock file using a memory stream
        var content = "Hello World from a Fake File";
        var fileName = "test.pdf";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;

        //create FormFile with desired data
        IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);

        // Act
        var fileValidator = new FileValidator(_configuration);

        // Assert
        var result = fileValidator.IsValid(file);
        result.ShouldBe(false);
    }

    [Test]
    public void FileValidatorShouldReturnFalseWhenFileIsEmpty()
    {
        // Arrange 
        //Setup mock file using a memory stream
        var fileName = "test.pdf";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        stream.Position = 0;

        //create FormFile with desired data
        IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);

        // Act
        var fileValidator = new FileValidator(_configuration);

        // Assert
        var result = fileValidator.IsValid(file);
        result.ShouldBe(false);
    }


    private string generateStringSize(long sizeByte)
    {
        var sb = new StringBuilder();
        var rd = new Random();
        var numOfChars = sizeByte;
        var allows = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var maxIndex = allows.Length - 1;
        for (var i = 0; i < numOfChars; i++)
        {
            var index = rd.Next(maxIndex);
            var c = allows[index];
            sb.Append(c);
        }

        return sb.ToString();
    }
}