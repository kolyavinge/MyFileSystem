using System.IO;
using MyFileSystem.Data.Repository;

namespace MyFileSystem.Logic
{
    internal interface IGetFileLogic
    {
        void GetFileContent(uint fileId, Stream output);
    }

    internal class GetFileLogic : IGetFileLogic
    {
        private readonly IFileSystemRepository _fileSystemRepository;
        private readonly IDataFileRepository _dataFileRepository;

        public GetFileLogic(IFileSystemRepository fileSystemRepository, IDataFileRepository dataFileRepository)
        {
            _fileSystemRepository = fileSystemRepository;
            _dataFileRepository = dataFileRepository;
        }

        public void GetFileContent(uint fileId, Stream output)
        {
            var buffer = new byte[10 * 1024 * 1024];
            var filePoco = _fileSystemRepository.GetById(fileId);
            _dataFileRepository.OpenDataFile(filePoco.DataFileNumber);
            using (var recordStream = _dataFileRepository.OpenDataRecord(filePoco.Id.ToString()))
            {
                int count;
                while ((count = recordStream.Read(buffer)) > 0)
                {
                    output.Write(buffer, 0, count);
                }
            }
            _dataFileRepository.CloseCurrentDataFile();
        }
    }
}
