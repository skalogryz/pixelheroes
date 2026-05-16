namespace PixelHeroes.Core.World
{
    public sealed class MapUnit
    {
        public MapUnit(string id, string name, string faction, int count, MapPosition position)
        {
            Id = id;
            Name = name;
            Faction = faction;
            Count = count;
            Position = position;
        }

        public string Id { get; }
        public string Name { get; }
        public string Faction { get; }
        public int Count { get; }
        public MapPosition Position { get; }
    }
}
