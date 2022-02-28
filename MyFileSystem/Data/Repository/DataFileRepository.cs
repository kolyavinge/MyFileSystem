using System.Collections.Generic;
using System.IO;
using MyFileSystem.Utils;
using SingleFileStorage;

namespace MyFileSystem.Data.Repository
{
    internal interface IDataFileRepository
    {
        void CreateDataFileIfNotExist(ushort dataFileNumber);
        void OpenDataFile(ushort dataFileNumber);
        void CloseCurrentDataFile();
        void CreateRecord(string recordName);
        Stream OpenDataRecord(string recordName);
        void DeleteRecords(IEnumerable<string> recordNames);
    }

    internal class DataFileRepository : IDataFileRepository
    {
        private readonly string _dataDirectoryPath;
        private IStorage _storage;

        public DataFileRepository(string dataDirectoryPath)
        {
            _dataDirectoryPath = dataDirectoryPath;
        }

        public void CreateDataFileIfNotExist(ushort dataFileNumber)
        {
            var path = Path.Combine(_dataDirectoryPath, dataFileNumber.ToString());
            if (!File.Exists(path))
            {
                StorageFile.Create(path);
            }
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

        public void DeleteRecords(IEnumerable<string> recordNames)
        {
            recordNames.Each(_storage.DeleteRecord);
        }
    }
}
