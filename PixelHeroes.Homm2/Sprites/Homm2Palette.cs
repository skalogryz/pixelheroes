using System;

namespace PixelHeroes.Homm2.Sprites
{
    public sealed class Homm2Palette
    {
        public const byte TransparentAlpha = 0;
        public const byte ShadowAlpha = 64;
        public const byte OpaqueAlpha = 255;

        public Homm2Palette(byte[] red, byte[] green, byte[] blue)
        {
            if (red == null || green == null || blue == null)
            {
                throw new ArgumentNullException();
            }

            if (red.Length != 256 || green.Length != 256 || blue.Length != 256)
            {
                throw new ArgumentException("HoMM2 palette must contain exactly 256 colors.");
            }

            Red = red;
            Green = green;
            Blue = blue;
        }

        public byte[] Red { get; }
        public byte[] Green { get; }
        public byte[] Blue { get; }

        public byte[] ToRgbaBytes()
        {
            var rgba = new byte[256 * 4];
            for (var i = 0; i < 256; i++)
            {
                var offset = i * 4;
                rgba[offset] = Red[i];
                rgba[offset + 1] = Green[i];
                rgba[offset + 2] = Blue[i];
                rgba[offset + 3] = OpaqueAlpha;
            }

            return rgba;
        }
    }
}
