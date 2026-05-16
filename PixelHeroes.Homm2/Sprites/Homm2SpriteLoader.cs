using System.IO;
using PixelHeroes.Homm2.Archives;

namespace PixelHeroes.Homm2.Sprites
{
    public sealed class Homm2SpriteLoader
    {
        public Homm2SpriteResource LoadFromFiles(string icnPath, string palettePath)
        {
            var spriteSet = new Homm2IcnReader().Read(icnPath);
            var palette = new Homm2PaletteReader().Read(palettePath);
            return new Homm2SpriteResource(spriteSet, palette);
        }

        public Homm2SpriteResource LoadFromAgg(string aggPath, string icnName, string paletteName = "KB.PAL")
        {
            var archive = Homm2AggArchive.Open(aggPath);
            var icnBytes = archive.ReadBytes(NormalizeResourceName(icnName, ".ICN"));
            var paletteBytes = archive.ReadBytes(NormalizeResourceName(paletteName, ".PAL"));

            using (var icnStream = new MemoryStream(icnBytes))
            using (var paletteStream = new MemoryStream(paletteBytes))
            {
                var spriteSet = new Homm2IcnReader().Read(icnStream, icnName);
                var palette = new Homm2PaletteReader().Read(paletteStream);
                return new Homm2SpriteResource(spriteSet, palette);
            }
        }

        private static string NormalizeResourceName(string name, string extension)
        {
            if (Path.HasExtension(name))
            {
                return name;
            }

            return name + extension;
        }
    }
}
