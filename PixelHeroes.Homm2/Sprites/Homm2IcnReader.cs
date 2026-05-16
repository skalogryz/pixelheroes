using System;
using System.Collections.Generic;
using System.IO;

namespace PixelHeroes.Homm2.Sprites
{
    public sealed class Homm2IcnReader
    {
        private const byte NormalSpriteType = 0;
        private const byte MonochromeSpriteType = 32;

        public Homm2SpriteSet Read(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            using (var stream = File.OpenRead(filePath))
            {
                return Read(stream, Path.GetFileName(filePath));
            }
        }

        public Homm2SpriteSet Read(Stream stream, string name)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (!stream.CanRead || !stream.CanSeek)
            {
                throw new ArgumentException("ICN stream must support reading and seeking.", nameof(stream));
            }

            var reader = new BinaryReader(stream);
            stream.Position = 0;
            var frameCount = reader.ReadUInt16();
            var declaredSize = reader.ReadUInt32();
            if (declaredSize + 6 > stream.Length)
            {
                throw new InvalidDataException("ICN declared size is larger than the stream.");
            }

            var headers = new Homm2IcnFrameHeader[frameCount];
            for (var i = 0; i < frameCount; i++)
            {
                headers[i] = new Homm2IcnFrameHeader
                {
                    OffsetX = reader.ReadInt16(),
                    OffsetY = reader.ReadInt16(),
                    Width = reader.ReadUInt16(),
                    Height = reader.ReadUInt16(),
                    Type = reader.ReadByte(),
                    DataOffset = reader.ReadUInt32()
                };
            }

            var dataStart = 6 + frameCount * 13;
            var frames = new List<Homm2SpriteFrame>(frameCount);
            foreach (var header in headers)
            {
                stream.Position = ResolveDataOffset(stream.Length, dataStart, header.DataOffset);
                frames.Add(DecodeFrame(reader, header));
            }

            return new Homm2SpriteSet(name, frames);
        }

        private static Homm2SpriteFrame DecodeFrame(BinaryReader reader, Homm2IcnFrameHeader header)
        {
            var pixelCount = header.Width * header.Height;
            var paletteIndexes = new byte[pixelCount];
            var alpha = new byte[pixelCount];

            if (header.Type == MonochromeSpriteType)
            {
                DecodeMonochrome(reader, header.Width, header.Height, paletteIndexes, alpha);
            }
            else if (header.Type == NormalSpriteType)
            {
                DecodeNormal(reader, header.Width, header.Height, paletteIndexes, alpha);
            }
            else
            {
                DecodeNormal(reader, header.Width, header.Height, paletteIndexes, alpha);
            }

            return new Homm2SpriteFrame(header.OffsetX, header.OffsetY, header.Width, header.Height, header.Type, paletteIndexes, alpha);
        }

        private static void DecodeNormal(BinaryReader reader, int width, int height, byte[] paletteIndexes, byte[] alpha)
        {
            var x = 0;
            var y = 0;

            while (y < height && reader.BaseStream.Position < reader.BaseStream.Length)
            {
                var command = reader.ReadByte();

                if (command == 0x00)
                {
                    x = 0;
                    y++;
                }
                else if (command <= 0x7F)
                {
                    for (var i = 0; i < command; i++)
                    {
                        WritePixel(width, height, paletteIndexes, alpha, ref x, y, reader.ReadByte(), Homm2Palette.OpaqueAlpha);
                    }
                }
                else if (command == 0x80)
                {
                    break;
                }
                else if (command <= 0xBF)
                {
                    x += command - 0x80;
                }
                else if (command == 0xC0)
                {
                    var lengthByte = reader.ReadByte();
                    var count = lengthByte % 4;
                    if (count == 0)
                    {
                        count = reader.ReadByte();
                    }

                    for (var i = 0; i < count; i++)
                    {
                        WritePixel(width, height, paletteIndexes, alpha, ref x, y, 0, Homm2Palette.ShadowAlpha);
                    }
                }
                else if (command == 0xC1)
                {
                    var count = reader.ReadByte();
                    var color = reader.ReadByte();
                    for (var i = 0; i < count; i++)
                    {
                        WritePixel(width, height, paletteIndexes, alpha, ref x, y, color, Homm2Palette.OpaqueAlpha);
                    }
                }
                else
                {
                    var count = command - 0xC0;
                    var color = reader.ReadByte();
                    for (var i = 0; i < count; i++)
                    {
                        WritePixel(width, height, paletteIndexes, alpha, ref x, y, color, Homm2Palette.OpaqueAlpha);
                    }
                }
            }
        }

        private static void DecodeMonochrome(BinaryReader reader, int width, int height, byte[] paletteIndexes, byte[] alpha)
        {
            var x = 0;
            var y = 0;

            while (y < height && reader.BaseStream.Position < reader.BaseStream.Length)
            {
                var command = reader.ReadByte();

                if (command == 0x00)
                {
                    x = 0;
                    y++;
                }
                else if (command <= 0x7F)
                {
                    for (var i = 0; i < command; i++)
                    {
                        WritePixel(width, height, paletteIndexes, alpha, ref x, y, 0, Homm2Palette.OpaqueAlpha);
                    }
                }
                else if (command == 0x80)
                {
                    break;
                }
                else
                {
                    x += command - 0x80;
                }
            }
        }

        private static void WritePixel(int width, int height, byte[] paletteIndexes, byte[] alpha, ref int x, int y, byte color, byte pixelAlpha)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                var index = y * width + x;
                paletteIndexes[index] = color;
                alpha[index] = pixelAlpha;
            }

            x++;
        }

        private static long ResolveDataOffset(long streamLength, int dataStart, uint rawOffset)
        {
            var relativeToData = dataStart + rawOffset;
            if (relativeToData >= dataStart && relativeToData < streamLength)
            {
                return relativeToData;
            }

            var relativeToPayload = 6 + rawOffset;
            if (relativeToPayload >= 6 && relativeToPayload < streamLength)
            {
                return relativeToPayload;
            }

            if (rawOffset < streamLength)
            {
                return rawOffset;
            }

            throw new InvalidDataException("ICN frame data offset is outside the stream.");
        }

        private sealed class Homm2IcnFrameHeader
        {
            public int OffsetX { get; set; }
            public int OffsetY { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public byte Type { get; set; }
            public uint DataOffset { get; set; }
        }
    }
}
