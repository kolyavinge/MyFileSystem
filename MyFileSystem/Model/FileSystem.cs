using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MyFileSystem.Data;
using MyFileSystem.Data.Repository;
using MyFileSystem.Logic;

namespace MyFileSystem.Model
{
    public interface IFileSystem
    {
        FileSystemItem Root { get; }
        void OpenFile(FileSystemItem file);
        void AddFiles(FileSystemItem parent, IEnumerable<string> filePathes);
    }

    public class FileSystem : IFileSystem
    {
        private readonly IFileSystemRepository _fileSystemRepository;
        private readonly ITempFileManager _tempFileManager;
        private readonly IGetFileLogic _getFileLogic;
        private readonly IAddFilesLogic _addFilesLogic;

        public FileSystemItem Root { get; }

        public FileSystem(
            IFileSystemRepository fileSystemRepository,
            ITempFileManager tempFileManager,
            IGetFileLogic getFileLogic,
            IAddFilesLogic addFilesLogic)
        {
            _fileSystemRepository = fileSystemRepository;
            _tempFileManager = tempFileManager;
            _getFileLogic = getFileLogic;
            _addFilesLogic = addFilesLogic;
            Root = new FileSystemItem(_fileSystemRepository, 0, FileSystemItemKind.Directory, "Корневая папка");
        }

        public void OpenFile(FileSystemItem file)
        {
            var tmpFilePath = _tempFileManager.GetTempFilePath(file.Name);
            using var tmpFileStream = File.OpenWrite(tmpFilePath);
            _getFileLogic.GetFileContent(file.Id, tmpFileStream);
            using var fileOpener = new Process();
            fileOpener.StartInfo.FileName = "explorer";
            fileOpener.StartInfo.Arguments = "\"" + tmpFilePath + "\"";
            fileOpener.Start();
        }

        public void AddFiles(FileSystemItem parent, IEnumerable<string> filePathes)
        {
            var result = _addFilesLogic.AddFiles(parent.Id, filePathes);
            parent.AddChildren(result.FileSystemItems.Select(x => FileSystemItemConverter.ToFileSystemItem(_fileSystemRepository, x)));
        }
    }
}
