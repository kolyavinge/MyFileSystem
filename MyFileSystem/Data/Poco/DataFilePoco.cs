using System;
using System.Collections.Generic;
using System.Text;

namespace MyFileSystem.Data.Poco
{
    public class DataFilePoco
    {
        public const int MaxRecords = 1000;

        public ushort Id { get; set; }

        public ushort RecordsCount { get; set; }
    }
}
