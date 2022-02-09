using System.IO;
using MyFileSystem.Data;
using MyFileSystem.Logic;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataDirectoryPath = @"D:\Projects\MyFileSystem\TestApp\bin\Debug\data";
            var filesPath = @"D:\Projects\MyFileSystem\Files";
            var dataContext = new DataContext(dataDirectoryPath);
            var addFilesLogic = new AddFilesLogic(dataContext.FileSystemRepository, dataContext.DataFileRepository);
            addFilesLogic.AddFiles(0, Directory.GetFiles(filesPath));

            var getFileLogic = new GetFileLogic(dataContext.FileSystemRepository, dataContext.DataFileRepository);
        }
    }
}
