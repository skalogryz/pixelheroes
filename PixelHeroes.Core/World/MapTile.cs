namespace PixelHeroes.Core.World
{
    public sealed class MapTile
    {
        public MapTile(MapPosition position, TerrainType terrain, bool isPassable = true)
        {
            Position = position;
            Terrain = terrain;
            IsPassable = isPassable;
        }

        public MapPosition Position { get; }
        public TerrainType Terrain { get; set; }
        public bool IsPassable { get; set; }
    }
}
