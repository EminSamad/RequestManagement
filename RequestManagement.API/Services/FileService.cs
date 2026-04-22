namespace RequestManagement.API.Services;

public class FileService
{
    private readonly string _uploadPath;

    public FileService(IConfiguration configuration, IWebHostEnvironment env)
    {
        _uploadPath = Path.Combine(env.ContentRootPath, 
            configuration["FileStorage:UploadPath"] ?? "Uploads");
        
        if (!Directory.Exists(_uploadPath))
            Directory.CreateDirectory(_uploadPath);
    }

    public async Task<string> SaveFileAsync(IFormFile file)
    {
        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(_uploadPath, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return fileName;
    }

    public void DeleteFile(string fileName)
    {
        var filePath = Path.Combine(_uploadPath, fileName);
        if (File.Exists(filePath))
            File.Delete(filePath);
    }
}