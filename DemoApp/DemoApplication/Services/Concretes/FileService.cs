using DemoApplication.Contracts.File;
using DemoApplication.Services.Abstracts;

namespace DemoApplication.Services.Concretes
{
    public class FileService : IFileService
    {
        public async Task<string> UploadAsync(IFormFile formFile, UploadDirectory uploadDirectory)
        {
           string directoryPath = GetUploadDirectory(uploadDirectory);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var ImageNameSystem = GenerateUniqueFileName(formFile.FileName);

            var filePath = Path.Combine(directoryPath, ImageNameSystem);

            using FileStream fileStream = new FileStream(filePath,FileMode.Create);

            await formFile.CopyToAsync(fileStream);

            return ImageNameSystem;

        }
        public string GetUploadDirectory(UploadDirectory uploadDirectory) 
        {
            string startPath = Path.Combine("wwwroot","client","custom-file");
            switch (uploadDirectory)
            {
                case UploadDirectory.Book:
                    return Path.Combine(startPath,"books");
                default:
                    throw new Exception("Something went wrong");
            }
        }
        private string GenerateUniqueFileName(string fileName)
        {
            return $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        }
    }
}
