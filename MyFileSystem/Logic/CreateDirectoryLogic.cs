using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MyFileSystem.Data.Poco;
using MyFileSystem.Data.Repository;

namespace MyFileSystem.Logic
{
    internal interface ICreateDirectoryLogic
    {
        FileSystemItemPoco CreateDirectory(uint parentDirectoryId, IReadOnlyCollection<string> childrenNames);
    }

    internal class CreateDirectoryLogic : ICreateDirectoryLogic
    {
        private readonly IFileSystemRepository _fileSystemRepository;

        public CreateDirectoryLogic(IFileSystemRepository fileSystemRepository)
        {
            _fileSystemRepository = fileSystemRepository;
        }

        public FileSystemItemPoco CreateDirectory(uint parentDirectoryId, IReadOnlyCollection<string> childrenNames)
        {
            var lastItemId = _fileSystemRepository.GetLastItemId();
            var newDirectoryPoco = new FileSystemItemPoco
            {
                Id = lastItemId.Value++,
                Kind = (int)FileSystemItemKind.Directory,
                ParentId = parentDirectoryId,
                Name = GetUniqueNewName(childrenNames),
                DataFileNumber = 0,
            };
            _fileSystemRepository.SaveFileSystemItems(new[] { newDirectoryPoco });
            _fileSystemRepository.SaveLastItemId(lastItemId);

            return newDirectoryPoco;
        }

        private string GetUniqueNewName(IReadOnlyCollection<string> childrenNames)
        {
            if (!childrenNames.Any(x => String.Equals("Новая папка", x, StringComparison.OrdinalIgnoreCase))) return "Новая папка";
            var regex = new Regex(@"^Новая папка \((\d+)\)$", RegexOptions.IgnoreCase);
            var matches = childrenNames.Select(x => regex.Match(x)).Where(x => x.Success).ToList();
            if (!matches.Any()) return "Новая папка (1)";
            var next = matches.Select(x => Int32.Parse(x.Groups[1].Value)).Max() + 1;

            return $"Новая папка ({next})";
        }
    }
}
