using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MyFileSystem.Data;
using MyFileSystem.Data.Repository;
using MyFileSystem.Infrastructure;
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
        byte[] GetFileContent(FileSystemItem file);
    }

    internal class FileSystem : IFileSystem
    {
        [Inject]
        public IFileSystemRepository FileSystemRepository { get; set; }
        [Inject]
        public IDataFileRepository DataFileRepository { get; set; }
        [Inject]
        public ITempFileManager TempFileManager { get; set; }
        [Inject]
        public IGetFileLogic GetFileLogic { get; set; }
        [Inject]
        public IAddFilesLogic AddFilesLogic { get; set; }
        [Inject]
        public ICreateDirectoryLogic CreateDirectoryLogic { get; set; }
        [Inject]
        public IDeleteFileSystemItemLogic DeleteFileSystemItemLogic { get; set; }

        private FileSystemItem _root;
        public FileSystemItem Root => _root ??= new FileSystemItem(FileSystemRepository, 0, FileSystemItemKind.Directory, "Корневая папка", null, 0);

        public void OpenFile(FileSystemItem file)
        {
            var tmpFilePath = TempFileManager.GetTempFilePath(file.Name);
            using var tmpFileStream = File.OpenWrite(tmpFilePath);
            GetFileLogic.GetFileContent(file.Id, tmpFileStream);
            using var fileOpener = new Process();
            fileOpener.StartInfo.FileName = "explorer";
            fileOpener.StartInfo.Arguments = "\"" + tmpFilePath + "\"";
            fileOpener.Start();
        }

        public void AddFiles(FileSystemItem parent, IEnumerable<string> filePathes)
        {
            var result = AddFilesLogic.AddFiles(parent.Id, filePathes);
            parent.AddChildren(result.FileSystemItems.Select(poco => FileSystemItemConverter.ToFileSystemItem(FileSystemRepository, poco, parent)));
        }

        public void Rename(FileSystemItem item, string newName)
        {
            FileSystemRepository.Rename(item.Id, newName);
            item.Name = newName;
        }

        public FileSystemItem CreateDirectory(FileSystemItem parentDirectory)
        {
            var newDirectoryPoco = CreateDirectoryLogic.CreateDirectory(parentDirectory.Id, parentDirectory.Children.Select(x => x.Name).ToList());
            var newDirectory = FileSystemItemConverter.ToFileSystemItem(FileSystemRepository, newDirectoryPoco, parentDirectory);
            parentDirectory.AddChildren(new[] { newDirectory });

            return newDirectory;
        }

        public void MoveTo(FileSystemItem itemToMove, FileSystemItem parentDirectory)
        {
            var poco = FileSystemRepository.GetById(itemToMove.Id);
            poco.ParentId = parentDirectory.Id;
            FileSystemRepository.SaveFileSystemItems(new[] { poco });
            itemToMove.Parent.ChildrenMoveFrom(new[] { itemToMove });
            parentDirectory.ChildrenMoveTo(new[] { itemToMove });
        }

        public void DeleteItems(IEnumerable<FileSystemItem> itemsToDelete)
        {
            DeleteFileSystemItemLogic.DeleteItems(itemsToDelete);
        }

        public byte[] GetFileContent(FileSystemItem file)
        {
            byte[] result = null;
            DataFileRepository.OpenDataFile(file.DataFileNumber);
            using (var recordStream = DataFileRepository.OpenDataRecord(file.Id.ToString()))
            {
                result = new byte[recordStream.Length];
                recordStream.Read(result, 0, result.Length);
            }
            DataFileRepository.CloseCurrentDataFile();

            return result;
        }
    }
}
