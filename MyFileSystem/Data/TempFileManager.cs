using System;
using System.Collections.Generic;
using System.IO;

namespace MyFileSystem.Data
{
    public interface ITempFileManager
    {
        string GetTempFilePath(string fileName);
    }

    public class TempFileManager : ITempFileManager, IDisposable
    {
        private readonly string _tempDirectoryPath;
        private readonly List<string> _tempFiles;

        public TempFileManager(string dataDirectoryPath)
        {
            _tempFiles = new List<string>();
            _tempDirectoryPath = Path.Combine(dataDirectoryPath, "tmp");
            if (!Directory.Exists(_tempDirectoryPath))
            {
                Directory.CreateDirectory(_tempDirectoryPath);
            }
            else
            {
                foreach (var file in Directory.GetFiles(_tempDirectoryPath)) File.Delete(file);
            }
        }

        public void Dispose()
        {
            foreach (var tempFile in _tempFiles)
            {
                File.Delete(tempFile);
            }
        }

        public string GetTempFilePath(string fileName)
        {
            var path = Path.Combine(_tempDirectoryPath, DateTime.Now.ToBinary() + "_" + fileName);
            _tempFiles.Add(path);

            return path;
        }
    }
}
