using System.Collections.Generic;
using System.IO;
using SingleFileStorage;

namespace MyFileSystem.Data.Repository
{
    public interface IDataFileRepository
    {
        void CreateDataFile(ushort dataFileNumber);
        void OpenDataFile(ushort dataFileNumber);
        void CloseCurrentDataFile();
        void CreateRecord(string recordName);
        Stream OpenDataRecord(string recordName);
    }

    internal class DataFileRepository : IDataFileRepository
    {
        private readonly string _dataDirectoryPath;
        private IStorage _storage;

        public DataFileRepository(string dataDirectoryPath)
        {
            _dataDirectoryPath = dataDirectoryPath;
        }

        public void CreateDataFile(ushort dataFileNumber)
        {
            StorageFile.Create(Path.Combine(_dataDirectoryPath, dataFileNumber.ToString()));
        }

        public void OpenDataFile(ushort dataFileNumber)
        {
            _storage = StorageFile.Open(Path.Combine(_dataDirectoryPath, dataFileNumber.ToString()), Access.Modify);
        }

        public void CloseCurrentDataFile()
        {
            _storage.Dispose();
            _storage = null;
        }

        public void CreateRecord(string recordName)
        {
            _storage.CreateRecord(recordName);
        }

        public Stream OpenDataRecord(string recordName)
        {
            return _storage.OpenRecord(recordName);
        }
    }
}
