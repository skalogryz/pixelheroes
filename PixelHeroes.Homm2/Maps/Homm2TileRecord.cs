namespace PixelHeroes.Homm2.Maps
{
    internal sealed class Homm2TileRecord
    {
        public ushort TerrainImageIndex { get; set; }
        public byte ObjectName1 { get; set; }
        public byte BottomIcnImageIndex { get; set; }
        public byte Quantity1 { get; set; }
        public byte Quantity2 { get; set; }
        public byte ObjectName2 { get; set; }
        public byte TopIcnImageIndex { get; set; }
        public byte TerrainFlags { get; set; }
        public byte MapObjectType { get; set; }
        public ushort NextAddonIndex { get; set; }
        public uint Level1ObjectUid { get; set; }
        public uint Level2ObjectUid { get; set; }
    }
}
