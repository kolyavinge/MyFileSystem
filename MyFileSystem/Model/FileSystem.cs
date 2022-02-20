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
        void Rename(FileSystemItem item, string newName);
        FileSystemItem CreateDirectory(FileSystemItem parentDirectory);
        void MoveTo(FileSystemItem itemToMove, FileSystemItem parentDirectory);
        void DeleteItems(IEnumerable<FileSystemItem> itemsToDelete);
    }

    public class FileSystem : IFileSystem
    {
        private readonly IFileSystemRepository _fileSystemRepository;
        private readonly ITempFileManager _tempFileManager;
        private readonly IGetFileLogic _getFileLogic;
        private readonly IAddFilesLogic _addFilesLogic;
        private readonly ICreateDirectoryLogic _createDirectoryLogic;
        private readonly IDeleteFileSystemItemLogic _deleteFileSystemItemLogic;

        public FileSystemItem Root { get; }

        public FileSystem(
            IFileSystemRepository fileSystemRepository,
            ITempFileManager tempFileManager,
            IGetFileLogic getFileLogic,
            IAddFilesLogic addFilesLogic,
            ICreateDirectoryLogic createDirectoryLogic,
            IDeleteFileSystemItemLogic deleteFileSystemItemLogic)
        {
            _fileSystemRepository = fileSystemRepository;
            _tempFileManager = tempFileManager;
            _getFileLogic = getFileLogic;
            _addFilesLogic = addFilesLogic;
            _createDirectoryLogic = createDirectoryLogic;
            _deleteFileSystemItemLogic = deleteFileSystemItemLogic;
            Root = new FileSystemItem(_fileSystemRepository, 0, FileSystemItemKind.Directory, "Корневая папка", null, 0);
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
            parent.AddChildren(result.FileSystemItems.Select(poco => FileSystemItemConverter.ToFileSystemItem(_fileSystemRepository, poco, parent)));
        }

        public void Rename(FileSystemItem item, string newName)
        {
            _fileSystemRepository.Rename(item.Id, newName);
            item.Name = newName;
        }

        public FileSystemItem CreateDirectory(FileSystemItem parentDirectory)
        {
            var newDirectoryPoco = _createDirectoryLogic.CreateDirectory(parentDirectory.Id, parentDirectory.Children.Select(x => x.Name).ToList());
            var newDirectory = FileSystemItemConverter.ToFileSystemItem(_fileSystemRepository, newDirectoryPoco, parentDirectory);
            parentDirectory.AddChildren(new[] { newDirectory });

            return newDirectory;
        }

        public void MoveTo(FileSystemItem itemToMove, FileSystemItem parentDirectory)
        {
            var poco = _fileSystemRepository.GetById(itemToMove.Id);
            poco.ParentId = parentDirectory.Id;
            _fileSystemRepository.SaveFileSystemItems(new[] { poco });
            itemToMove.Parent.ChildrenMoveFrom(new[] { itemToMove });
            parentDirectory.ChildrenMoveTo(new[] { itemToMove });
        }

        public void DeleteItems(IEnumerable<FileSystemItem> itemsToDelete)
        {
            _deleteFileSystemItemLogic.DeleteItems(itemsToDelete);
        }
    }
}
