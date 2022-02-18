using System.Collections.Generic;
using System.IO;
using System.Linq;
using MyFileSystem.Data.Poco;
using MyFileSystem.Data.Repository;

namespace MyFileSystem.Logic
{
    public interface IAddFilesLogic
    {
        AddFilesResult AddFiles(uint parentDirectoryId, IEnumerable<string> filePathes);
    }

    public class AddFilesLogic : IAddFilesLogic
    {
        private readonly IFileSystemRepository _fileSystemRepository;
        private readonly IDataFileRepository _dataFileRepository;

        public AddFilesLogic(IFileSystemRepository fileSystemRepository, IDataFileRepository dataFileRepository)
        {
            _fileSystemRepository = fileSystemRepository;
            _dataFileRepository = dataFileRepository;
        }

        public AddFilesResult AddFiles(uint parentDirectoryId, IEnumerable<string> filePathes)
        {
            var lastItemId = _fileSystemRepository.GetLastItemId();
            var dataFiles = _fileSystemRepository.GetDataFiles().OrderBy(x => x.Id).ToList();
            var currentDataFile = GetNextFreeDataFileOrCreateNew(dataFiles);
            var fileSystemItems = new List<FileSystemItemPoco>();
            var dataRecords = new List<DataRecord>();
            foreach (var filePath in filePathes)
            {
                if (currentDataFile.RecordsCount + 1 > DataFilePoco.MaxRecords)
                {
                    currentDataFile = GetNextFreeDataFileOrCreateNew(dataFiles);
                }
                currentDataFile.RecordsCount++;
                var fileSystemItem = new FileSystemItemPoco
                {
                    Id = lastItemId.Value++,
                    ParentId = parentDirectoryId,
                    Kind = (byte)FileSystemItemKind.File,
                    Name = Path.GetFileName(filePath),
                    DataFileNumber = currentDataFile.Id
                };
                fileSystemItems.Add(fileSystemItem);
                dataRecords.Add(new DataRecord
                {
                    FilePath = filePath,
                    RecordName = fileSystemItem.Id.ToString(),
                    DataFileNumber = fileSystemItem.DataFileNumber
                });
            }
            _fileSystemRepository.SaveFileSystemItems(fileSystemItems);
            _fileSystemRepository.SaveDataFiles(dataFiles);
            _fileSystemRepository.SaveLastItemId(lastItemId);
            var fileBuffer = new byte[10 * 1024 * 1024];
            foreach (var groupedDataRecords in dataRecords.GroupBy(x => x.DataFileNumber))
            {
                _dataFileRepository.CreateDataFileIfNotExist(groupedDataRecords.Key);
                _dataFileRepository.OpenDataFile(groupedDataRecords.Key);
                foreach (var dataRecord in groupedDataRecords)
                {
                    _dataFileRepository.CreateRecord(dataRecord.RecordName);
                    using (var fileStream = File.OpenRead(dataRecord.FilePath))
                    using (var recordStream = _dataFileRepository.OpenDataRecord(dataRecord.RecordName))
                    {
                        int count;
                        while ((count = fileStream.Read(fileBuffer, 0, fileBuffer.Length)) > 0)
                        {
                            recordStream.Write(fileBuffer, 0, count);
                        }
                    }
                }
                _dataFileRepository.CloseCurrentDataFile();
            }

            return new AddFilesResult
            {
                FileSystemItems = fileSystemItems
            };
        }

        private DataFilePoco GetNextFreeDataFileOrCreateNew(List<DataFilePoco> dataFiles)
        {
            var dataFile = dataFiles.FirstOrDefault(x => x.RecordsCount < DataFilePoco.MaxRecords);
            if (dataFile == null)
            {
                dataFile = new DataFilePoco { Id = (ushort)(dataFiles.Count + 1), RecordsCount = 0 };
                dataFiles.Add(dataFile);
            }

            return dataFile;
        }
    }

    public class AddFilesResult
    {
        public List<FileSystemItemPoco> FileSystemItems { get; set; }
    }

    class DataRecord
    {
        public string FilePath;
        public string RecordName;
        public ushort DataFileNumber;
    }
}
