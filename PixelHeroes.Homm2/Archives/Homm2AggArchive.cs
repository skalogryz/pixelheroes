using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PixelHeroes.Homm2.Archives
{
    public sealed class Homm2AggArchive
    {
        private readonly Dictionary<string, Homm2AggEntry> entries;

        private Homm2AggArchive(string filePath, Dictionary<string, Homm2AggEntry> entries)
        {
            FilePath = filePath;
            this.entries = entries;
        }

        public string FilePath { get; }

        public IReadOnlyCollection<string> Names => entries.Keys;

        public static Homm2AggArchive Open(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            using (var stream = File.OpenRead(filePath))
            using (var reader = new BinaryReader(stream))
            {
                var count = reader.ReadUInt16();
                var rawEntries = new Homm2AggEntry[count];

                for (var i = 0; i < count; i++)
                {
                    rawEntries[i] = new Homm2AggEntry
                    {
                        Id = reader.ReadUInt32(),
                        Offset = reader.ReadUInt32(),
                        Size = reader.ReadUInt32()
                    };
                }

                stream.Position = stream.Length - count * 15;
                var result = new Dictionary<string, Homm2AggEntry>(StringComparer.OrdinalIgnoreCase);
                for (var i = 0; i < count; i++)
                {
                    var nameBytes = reader.ReadBytes(13);
                    reader.ReadBytes(2);
                    var nullIndex = Array.IndexOf(nameBytes, (byte)0);
                    if (nullIndex < 0)
                    {
                        nullIndex = nameBytes.Length;
                    }

                    var name = Encoding.ASCII.GetString(nameBytes, 0, nullIndex);
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        result[name] = rawEntries[i];
                    }
                }

                return new Homm2AggArchive(filePath, result);
            }
        }

        public bool Contains(string name)
        {
            return entries.ContainsKey(name);
        }

        public byte[] ReadBytes(string name)
        {
            if (!entries.TryGetValue(name, out var entry))
            {
                throw new FileNotFoundException("Resource was not found in HoMM2 AGG archive.", name);
            }

            using (var stream = File.OpenRead(FilePath))
            using (var reader = new BinaryReader(stream))
            {
                stream.Position = entry.Offset;
                return reader.ReadBytes(checked((int)entry.Size));
            }
        }

        private sealed class Homm2AggEntry
        {
            public uint Id { get; set; }
            public uint Offset { get; set; }
            public uint Size { get; set; }
        }
    }
}
