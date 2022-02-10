using System.Collections.Generic;

namespace MyFileSystem.Utils
{
    public static class ImageFileFormats
    {
        public static readonly HashSet<string> Extensions = new HashSet<string>
        {
            ".tif",
            ".tiff",
            ".gif",
            ".jpeg",
            ".jpg",
            ".jif",
            ".jfif",
            ".jp2",
            ".jpx",
            ".j2k",
            ".j2c",
            ".fpx",
            ".pcd",
            ".png",
            ".pdf"
        };
    }
}
