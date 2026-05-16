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

        // The map creator specific sprite name, to distringuish from the TerrainType.
        // it's used for the drawing purposes only!
        //
        // Note: Sprite isn't necessary a static image, it could be an animation as well
        // or just a reference to a certain drawer.
        public string Sprite { get; set; } = "";

        public MapPosition Position { get; }
        public TerrainType Terrain { get; set; }
        public bool IsPassable { get; set; }
    }
}
