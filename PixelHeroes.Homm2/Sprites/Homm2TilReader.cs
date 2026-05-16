using System;
using System.Collections.Generic;
using System.IO;

namespace PixelHeroes.Homm2.Sprites
{
    public sealed class Homm2TilReader
    {
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

            var reader = new BinaryReader(stream);
            var count = reader.ReadUInt16();
            var width = reader.ReadUInt16();
            var height = reader.ReadUInt16();
            var pixelCount = checked(width * height);
            var frames = new List<Homm2SpriteFrame>(count);

            for (var i = 0; i < count; i++)
            {
                var paletteIndexes = reader.ReadBytes(pixelCount);
                if (paletteIndexes.Length != pixelCount)
                {
                    throw new EndOfStreamException("TIL tile data ended unexpectedly.");
                }

                var alpha = new byte[pixelCount];
                for (var pixel = 0; pixel < alpha.Length; pixel++)
                {
                    alpha[pixel] = Homm2Palette.OpaqueAlpha;
                }

                frames.Add(new Homm2SpriteFrame(0, 0, width, height, 0, paletteIndexes, alpha));
            }

            return new Homm2SpriteSet(name, frames);
        }
    }
}
