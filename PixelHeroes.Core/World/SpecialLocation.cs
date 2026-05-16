namespace PixelHeroes.Core.World
{
    public sealed class SpecialLocation
    {
        public SpecialLocation(string id, string name, SpecialLocationType type, MapPosition position, bool isOwned)
        {
            Id = id;
            Name = name;
            Type = type;
            Position = position;
            IsOwned = isOwned;
        }

        public string Id { get; }
        public string Name { get; }
        public SpecialLocationType Type { get; }
        public MapPosition Position { get; }
        public bool IsOwned { get; set; }
    }
}
