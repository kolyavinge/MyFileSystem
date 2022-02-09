using System;
using System.Collections.Generic;
using System.Text;

namespace MyFileSystem.Data.Poco
{
    public class FileSystemItemPoco
    {
        public uint Id { get; set; }

        public uint ParentId { get; set; }

        public byte Kind { get; set; }

        public string Name { get; set; }

        public ushort DataFileNumber { get; set; }
    }
}
