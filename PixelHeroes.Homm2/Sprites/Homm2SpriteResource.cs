namespace PixelHeroes.Homm2.Sprites
{
    public sealed class Homm2SpriteResource
    {
        public Homm2SpriteResource(Homm2SpriteSet spriteSet, Homm2Palette palette)
        {
            SpriteSet = spriteSet;
            Palette = palette;
        }

        public Homm2SpriteSet SpriteSet { get; }
        public Homm2Palette Palette { get; }
    }
}
