using System.Collections.Generic;
using System.Linq;
using MyFileSystem.Data.Repository;
using MyFileSystem.Model;
using MyFileSystem.Utils;

namespace MyFileSystem.Logic
{
    public interface IDeleteFileSystemItemLogic
    {
        void DeleteItems(IEnumerable<FileSystemItem> itemsToDelete);
    }

    public class DeleteFileSystemItemLogic : IDeleteFileSystemItemLogic
    {
        private readonly IFileSystemRepository _fileSystemRepository;
        private readonly IDataFileRepository _dataFileRepository;

        public DeleteFileSystemItemLogic(IFileSystemRepository fileSystemRepository, IDataFileRepository dataFileRepository)
        {
            _fileSystemRepository = fileSystemRepository;
            _dataFileRepository = dataFileRepository;
        }

        public void DeleteItems(IEnumerable<FileSystemItem> itemsToDelete)
        {
            var allItemsToDelete = GetAllItemsToDelete(itemsToDelete).ToList();
            _fileSystemRepository.DeleteFileSystemItems(allItemsToDelete.Select(x => x.Id));
            foreach (var itemsGroupedByDataFiles in allItemsToDelete.Where(x => x.Kind == FileSystemItemKind.File).GroupBy(x => x.DataFileNumber))
            {
                _dataFileRepository.OpenDataFile(itemsGroupedByDataFiles.Key);
                _dataFileRepository.DeleteRecords(itemsGroupedByDataFiles.Select(x => x.Id.ToString()));
                _dataFileRepository.CloseCurrentDataFile();
            }
            allItemsToDelete.GroupBy(x => x.Parent).Each(x => x.Key.DeleteChildren(x.ToList()));
        }

        private List<FileSystemItem> GetAllItemsToDelete(IEnumerable<FileSystemItem> itemsToDelete)
        {
            var result = new List<FileSystemItem>();
            var idset = new HashSet<uint>();

            foreach (var item in itemsToDelete.OrderBy(x => x.Depth))
            {
                if (!idset.Contains(item.Id))
                {
                    var allChildren = item.GetAllChildren();
                    idset.Add(item.Id);
                    idset.AddRange(allChildren.Select(x => x.Id));
                    result.Add(item);
                    result.AddRange(allChildren);
                }
            }

            return result;
        }
    }
}
