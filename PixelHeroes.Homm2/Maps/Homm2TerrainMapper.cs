using PixelHeroes.Core.World;

namespace PixelHeroes.Homm2.Maps
{
    internal static class Homm2TerrainMapper
    {
        public static TerrainType ToTerrainType(ushort terrainImageIndex)
        {
            var terrainGroup = terrainImageIndex / 32;
            switch (terrainGroup)
            {
                case 0:
                    return TerrainType.Grass;
                case 1:
                    return TerrainType.Water;
                case 2:
                    return TerrainType.Dirt;
                case 3:
                    return TerrainType.Forest;
                case 4:
                    return TerrainType.Mountain;
                case 5:
                case 6:
                case 7:
                    return TerrainType.Dirt;
                default:
                    return TerrainType.Grass;
            }
        }
    }
}
