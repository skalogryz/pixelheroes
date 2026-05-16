using System.Collections.Generic;

namespace PixelHeroes.Homm2.Sprites
{
    public sealed class Homm2SpriteSet
    {
        public Homm2SpriteSet(string name, IReadOnlyList<Homm2SpriteFrame> frames)
        {
            Name = name;
            Frames = frames;
        }

        public string Name { get; }
        public IReadOnlyList<Homm2SpriteFrame> Frames { get; }
    }
}
