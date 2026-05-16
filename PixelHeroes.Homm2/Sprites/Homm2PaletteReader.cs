using System;
using System.IO;

namespace PixelHeroes.Homm2.Sprites
{
    public sealed class Homm2PaletteReader
    {
        private const int PaletteColorCount = 256;
        private const int PaletteByteLength = PaletteColorCount * 3;

        public Homm2Palette Read(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            using (var stream = File.OpenRead(filePath))
            {
                return Read(stream);
            }
        }

        public Homm2Palette Read(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var bytes = new byte[PaletteByteLength];
            var read = stream.Read(bytes, 0, bytes.Length);
            if (read != bytes.Length)
            {
                throw new InvalidDataException("HoMM2 palette must be 768 bytes long.");
            }

            var scaleSixBitColors = IsSixBitPalette(bytes);
            var red = new byte[PaletteColorCount];
            var green = new byte[PaletteColorCount];
            var blue = new byte[PaletteColorCount];

            for (var i = 0; i < PaletteColorCount; i++)
            {
                red[i] = ConvertColorComponent(bytes[i * 3], scaleSixBitColors);
                green[i] = ConvertColorComponent(bytes[i * 3 + 1], scaleSixBitColors);
                blue[i] = ConvertColorComponent(bytes[i * 3 + 2], scaleSixBitColors);
            }

            return new Homm2Palette(red, green, blue);
        }

        private static bool IsSixBitPalette(byte[] bytes)
        {
            foreach (var value in bytes)
            {
                if (value > 63)
                {
                    return false;
                }
            }

            return true;
        }

        private static byte ConvertColorComponent(byte value, bool scaleSixBitColors)
        {
            if (!scaleSixBitColors)
            {
                return value;
            }

            return (byte)Math.Min(255, value * 4);
        }
    }
}
