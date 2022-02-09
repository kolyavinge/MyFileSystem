using System.Collections.Generic;
using System.Linq;
using MyFileSystem.Data.Poco;
using SimpleDB;

namespace MyFileSystem.Data.Repository
{
    public interface IFileSystemRepository
    {
        FileSystemItemPoco GetById(uint id);
        IEnumerable<FileSystemItemPoco> GetChildren(uint parentId);
        void SaveFileSystemItems(IEnumerable<FileSystemItemPoco> fileSystemItems);
        IEnumerable<DataFilePoco> GetDataFiles();
        void SaveDataFiles(IEnumerable<DataFilePoco> dataFiles);
        LastItemIdPoco GetLastItemId();
        void SaveLastItemId(LastItemIdPoco item);
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

        public void SaveFileSystemItems(IEnumerable<FileSystemItemPoco> fileSystemItems)
        {
            _engine.GetCollection<FileSystemItemPoco>().Insert(fileSystemItems);
        }

        public IEnumerable<DataFilePoco> GetDataFiles()
        {
            return _engine.GetCollection<DataFilePoco>().GetAll();
        }

        public void SaveDataFiles(IEnumerable<DataFilePoco> dataFiles)
        {
            _engine.GetCollection<DataFilePoco>().InsertOrUpdate(dataFiles);
        }

        public LastItemIdPoco GetLastItemId()
        {
            return _engine.GetCollection<LastItemIdPoco>().GetAll().First();
        }

        public void SaveLastItemId(LastItemIdPoco item)
        {
            _engine.GetCollection<LastItemIdPoco>().Update(item);
        }
    }
}
