using System;
using System.IO;

namespace FileGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = @"D:\Projects\MyFileSystem\Files";
            int filesCount = 1000;
            var fileContent = new byte[100];

            for (int i = 1; i <= filesCount; i++)
            {
                var fileName = i.ToString();
                if (fileName.Length == 1) fileName = "000" + fileName;
                else if (fileName.Length == 2) fileName = "00" + fileName;
                else if (fileName.Length == 3) fileName = "0" + fileName;
                var file = File.Create(Path.Combine(path, fileName));
                file.Write(fileContent, 0, fileContent.Length);
                file.Dispose();
            }
        }
    }
}
