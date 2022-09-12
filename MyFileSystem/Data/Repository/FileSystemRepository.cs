using System.Collections.Generic;
using System.Linq;
using MyFileSystem.Data.Poco;
using SimpleDB;

namespace MyFileSystem.Data.Repository
{
    internal interface IFileSystemRepository
    {
        FileSystemItemPoco GetById(uint id);
        IEnumerable<FileSystemItemPoco> GetChildren(uint parentId);
        void SaveFileSystemItems(IReadOnlyCollection<FileSystemItemPoco> fileSystemItems);
        IEnumerable<DataFilePoco> GetDataFiles();
        void SaveDataFiles(IReadOnlyCollection<DataFilePoco> dataFiles);
        LastItemIdPoco GetLastItemId();
        void SaveLastItemId(LastItemIdPoco item);
        void Rename(uint itemId, string newName);
        void DeleteFileSystemItems(IEnumerable<uint> idList);
    }

    internal class FileSystemRepository : IFileSystemRepository
    {
        private readonly IDBEngine _engine;

        public FileSystemRepository(IDBEngine engine)
        {
            _engine = engine;
            if (_engine.GetCollection<LastItemIdPoco>().Count() == 0)
            {
                _engine.GetCollection<LastItemIdPoco>().Insert(new LastItemIdPoco { Id = 0, Value = 1 });
            }
        }

        public FileSystemItemPoco GetById(uint id)
        {
            return _engine.GetCollection<FileSystemItemPoco>().Get(id);
        }

        public IEnumerable<FileSystemItemPoco> GetChildren(uint parentId)
        {
            return _engine.GetCollection<FileSystemItemPoco>().Query()
                .Where(x => x.ParentId == parentId)
                .ToList();
        }

        public void SaveFileSystemItems(IReadOnlyCollection<FileSystemItemPoco> fileSystemItems)
        {
            _engine.GetCollection<FileSystemItemPoco>().InsertOrUpdateRange(fileSystemItems);
        }

        public IEnumerable<DataFilePoco> GetDataFiles()
        {
            return _engine.GetCollection<DataFilePoco>().GetAll();
        }

        public void SaveDataFiles(IReadOnlyCollection<DataFilePoco> dataFiles)
        {
            _engine.GetCollection<DataFilePoco>().InsertOrUpdateRange(dataFiles);
        }

        public LastItemIdPoco GetLastItemId()
        {
            return _engine.GetCollection<LastItemIdPoco>().GetAll().First();
        }

        public void SaveLastItemId(LastItemIdPoco item)
        {
            _engine.GetCollection<LastItemIdPoco>().Update(item);
        }

        public void Rename(uint itemId, string newName)
        {
            _engine.GetCollection<FileSystemItemPoco>()
                .Query()
                .Update(x => new FileSystemItemPoco { Name = newName }, x => x.Id == itemId);
        }

        public void DeleteFileSystemItems(IEnumerable<uint> idList)
        {
            _engine.GetCollection<FileSystemItemPoco>().Delete(idList.Cast<object>().ToList());
        }
    }
}
