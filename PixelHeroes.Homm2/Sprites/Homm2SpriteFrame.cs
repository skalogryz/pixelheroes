using System;

namespace PixelHeroes.Homm2.Sprites
{
    public sealed class Homm2SpriteFrame
    {
        public Homm2SpriteFrame(
            int offsetX,
            int offsetY,
            int width,
            int height,
            byte type,
            byte[] paletteIndexes,
            byte[] alpha)
        {
            if (width < 0 || height < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (paletteIndexes == null || alpha == null)
            {
                throw new ArgumentNullException();
            }

            if (paletteIndexes.Length != width * height || alpha.Length != width * height)
            {
                throw new ArgumentException("Sprite pixel arrays must match width * height.");
            }

            OffsetX = offsetX;
            OffsetY = offsetY;
            Width = width;
            Height = height;
            Type = type;
            PaletteIndexes = paletteIndexes;
            Alpha = alpha;
        }

        public int OffsetX { get; }
        public int OffsetY { get; }
        public int PivotX => -OffsetX;
        public int PivotY => -OffsetY;
        public int CenterX => Width / 2;
        public int CenterY => Height / 2;
        public int Width { get; }
        public int Height { get; }
        public byte Type { get; }
        public byte[] PaletteIndexes { get; }
        public byte[] Alpha { get; }

        public byte[] ToRgbaBytes(Homm2Palette palette)
        {
            if (palette == null)
            {
                throw new ArgumentNullException(nameof(palette));
            }

            var rgba = new byte[Width * Height * 4];
            for (var i = 0; i < PaletteIndexes.Length; i++)
            {
                var paletteIndex = PaletteIndexes[i];
                var offset = i * 4;
                rgba[offset] = palette.Red[paletteIndex];
                rgba[offset + 1] = palette.Green[paletteIndex];
                rgba[offset + 2] = palette.Blue[paletteIndex];
                rgba[offset + 3] = Alpha[i];
            }

            return rgba;
        }
    }
}
