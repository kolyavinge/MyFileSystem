using System.IO;
using MyFileSystem.Data.Poco;
using MyFileSystem.Data.Repository;
using SimpleDB;

namespace MyFileSystem.Data
{
    internal interface IDataContext
    {
        IFileSystemRepository FileSystemRepository { get; }

        IDataFileRepository DataFileRepository { get; }
    }

    internal class DataContext : IDataContext
    {
        private readonly IDBEngine _engine;

        public DataContext(string dataDirectoryPath)
        {
            if (!Directory.Exists(dataDirectoryPath)) Directory.CreateDirectory(dataDirectoryPath);
            _engine = BuildEngine(Path.Combine(dataDirectoryPath, "fs"));
            FileSystemRepository = new FileSystemRepository(_engine);
            DataFileRepository = new DataFileRepository(dataDirectoryPath);
        }

        private IDBEngine BuildEngine(string databaseFilePath)
        {
            var builder = DBEngineBuilder.Make();
            builder.DatabaseFilePath(databaseFilePath);

            builder.Map<FileSystemItemPoco>()
                .PrimaryKey(x => x.Id)
                .Field(1, x => x.ParentId)
                .Field(2, x => x.Kind)
                .Field(3, x => x.Name)
                .Field(4, x => x.DataFileNumber);

            builder.Map<DataFilePoco>()
                .PrimaryKey(x => x.Id)
                .Field(1, x => x.RecordsCount);

            builder.Map<LastItemIdPoco>()
                .PrimaryKey(x => x.Id)
                .Field(1, x => x.Value);

            return builder.BuildEngine();
        }

        public IFileSystemRepository FileSystemRepository { get; }

        public IDataFileRepository DataFileRepository { get; }
    }
}
