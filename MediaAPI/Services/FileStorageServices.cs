namespace MediaAPI.Services
{
    using MediaAPI.Modules.FileStorage;
    using System.Collections.Generic;
    using System.IO;

    public class FileStorageServices : IFileStorage
    {
        private const string StoragePath = "./MediaFiles";

        public void SaveFile(string fileName, byte[] content)
        {
            string filePath = Path.Combine(StoragePath, fileName);
            File.WriteAllBytes(filePath, content);
        }

        public byte[] GetFile(string fileName)
        {
            string filePath = Path.Combine(StoragePath, fileName);
            return File.Exists(filePath) ? File.ReadAllBytes(filePath) : null;
        }

        public IEnumerable<object> GetFiles()
        {
            var files = Directory.GetFiles(StoragePath);

            var fileList = files.Select(file =>
            {
                var fileInfo = new FileInfo(file);
                return new
                {
                    FileName = Path.GetFileName(file),
                    Size = Math.Round(((double)fileInfo.Length)/ (1024 * 1024),2) + " MB",
                    Date = fileInfo.LastWriteTimeUtc.ToString("yyyy-MM-dd")
            };
            });

            return fileList;            
        }
    }

}
