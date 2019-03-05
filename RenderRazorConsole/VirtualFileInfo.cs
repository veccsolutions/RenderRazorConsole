using System;
using System.IO;
using Microsoft.Extensions.FileProviders;

namespace RenderRazorConsole
{
    public class VirtualFileInfo : IFileInfo
    {
        public Lazy<byte[]> Contents { get; }

        public bool Exists => Contents.Value != null;

        public long Length => Contents.Value.Length;

        public string PhysicalPath { get; }

        public string Name { get; }

        public DateTimeOffset LastModified { get; }

        public bool IsDirectory { get; }

        public Stream CreateReadStream() => new MemoryStream(Contents.Value);

        public VirtualFileInfo(string physicalPath, string name, DateTimeOffset lastModified, bool isDirectory, Func<IFileInfo, byte[]> getContents)
        {
            Contents = new Lazy<byte[]>(() => getContents(this));
            PhysicalPath = physicalPath;
            Name = name;
            LastModified = lastModified;
            IsDirectory = isDirectory;
        }
    }
}
