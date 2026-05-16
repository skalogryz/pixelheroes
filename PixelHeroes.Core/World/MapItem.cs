namespace PixelHeroes.Core.World
{
    public sealed class MapItem
    {
        public MapItem(string id, string name, ResourceType resourceType, int amount, MapPosition position)
        {
            Id = id;
            Name = name;
            ResourceType = resourceType;
            Amount = amount;
            Position = position;
        }

        public string Id { get; }
        public string Name { get; }
        public ResourceType ResourceType { get; }
        public int Amount { get; }
        public MapPosition Position { get; }
    }
}
