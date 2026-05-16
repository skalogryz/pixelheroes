using System;
using System.IO;
using PixelHeroes.Core.World;

namespace PixelHeroes.Homm2.Maps
{
    public sealed class Homm2MapReader
    {
        private const int HeaderSize = 428;
        private const uint Magic = 0x5C000000;
        private const int MapNameOffset = 58;
        private const int MapNameLength = 16;

        public AdventureMap Read(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            using (var stream = File.OpenRead(filePath))
            {
                return Read(stream, Path.GetFileNameWithoutExtension(filePath));
            }
        }

        public AdventureMap Read(Stream stream, string fallbackName)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (!stream.CanRead || !stream.CanSeek)
            {
                throw new ArgumentException("HoMM2 map stream must support reading and seeking.", nameof(stream));
            }

            var reader = new BinaryReader(stream);
            stream.Position = 0;

            if (ReadUInt32BigEndian(reader) != Magic)
            {
                throw new InvalidDataException("File does not look like a HoMM2 MP2/MX2 map.");
            }

            stream.Position = 6;
            var width = reader.ReadByte();
            var height = reader.ReadByte();
            if (width <= 0 || height <= 0)
            {
                throw new InvalidDataException("HoMM2 map has invalid dimensions.");
            }

            var name = ReadFixedString(stream, MapNameOffset, MapNameLength);
            if (string.IsNullOrWhiteSpace(name))
            {
                name = string.IsNullOrWhiteSpace(fallbackName) ? "HoMM2 Map" : fallbackName;
            }

            var map = new AdventureMap(name, width, height);
            stream.Position = HeaderSize;

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var tile = ReadTile(reader);
                    var position = new MapPosition(x, y);
                    var terrain = Homm2TerrainMapper.ToTerrainType(tile.TerrainImageIndex);
                    map.SetTile(new MapTile(position, terrain, terrain != TerrainType.Water && terrain != TerrainType.Mountain, CreateTerrainSpriteKey(tile)));
                    AddObject(map, position, tile);
                }
            }

            return map;
        }

        private static Homm2TileRecord ReadTile(BinaryReader reader)
        {
            return new Homm2TileRecord
            {
                TerrainImageIndex = reader.ReadUInt16(),
                ObjectName1 = reader.ReadByte(),
                BottomIcnImageIndex = reader.ReadByte(),
                Quantity1 = reader.ReadByte(),
                Quantity2 = reader.ReadByte(),
                ObjectName2 = reader.ReadByte(),
                TopIcnImageIndex = reader.ReadByte(),
                TerrainFlags = reader.ReadByte(),
                MapObjectType = reader.ReadByte(),
                NextAddonIndex = reader.ReadUInt16(),
                Level1ObjectUid = reader.ReadUInt32(),
                Level2ObjectUid = reader.ReadUInt32()
            };
        }

        private static string CreateTerrainSpriteKey(Homm2TileRecord tile)
        {
            var verticalFlip = (tile.TerrainFlags & 1) != 0;
            var horizontalFlip = (tile.TerrainFlags & 2) != 0;
            return "terrain:" + tile.TerrainImageIndex + ":" + (verticalFlip ? "v" : "-") + ":" + (horizontalFlip ? "h" : "-");
        }

        private static void AddObject(AdventureMap map, MapPosition position, Homm2TileRecord tile)
        {
            var rawObjectType = tile.MapObjectType >= 128 ? tile.MapObjectType - 128 : tile.MapObjectType;
            var objectType = (Homm2MapObjectType)rawObjectType;
            switch (objectType)
            {
                case Homm2MapObjectType.Castle:
                case Homm2MapObjectType.RandomTown:
                case Homm2MapObjectType.RandomCastle:
                    map.AddLocation(new SpecialLocation(CreateId("location", position), objectType.ToString(), SpecialLocationType.Castle, position, false));
                    break;
                case Homm2MapObjectType.Mine:
                    map.AddLocation(new SpecialLocation(CreateId("location", position), "Mine", SpecialLocationType.Mine, position, false));
                    break;
                case Homm2MapObjectType.Sawmill:
                    map.AddLocation(new SpecialLocation(CreateId("location", position), "Sawmill", SpecialLocationType.Sawmill, position, false));
                    break;
                case Homm2MapObjectType.ShrineFirstCircle:
                case Homm2MapObjectType.ShrineSecondCircle:
                case Homm2MapObjectType.ShrineThirdCircle:
                    map.AddLocation(new SpecialLocation(CreateId("location", position), objectType.ToString(), SpecialLocationType.Shrine, position, false));
                    break;
                case Homm2MapObjectType.ArcherHouse:
                case Homm2MapObjectType.GoblinHut:
                case Homm2MapObjectType.DwarfCottage:
                case Homm2MapObjectType.PeasantHut:
                case Homm2MapObjectType.DragonCity:
                case Homm2MapObjectType.WagonCamp:
                case Homm2MapObjectType.TreeHouse:
                case Homm2MapObjectType.TreeCity:
                case Homm2MapObjectType.Ruins:
                case Homm2MapObjectType.HalflingHole:
                case Homm2MapObjectType.CityOfDead:
                case Homm2MapObjectType.Cave:
                    map.AddLocation(new SpecialLocation(CreateId("location", position), objectType.ToString(), SpecialLocationType.Dwelling, position, false));
                    break;
                case Homm2MapObjectType.Resource:
                case Homm2MapObjectType.RandomResource:
                case Homm2MapObjectType.TreasureChest:
                case Homm2MapObjectType.Campfire:
                case Homm2MapObjectType.SeaChest:
                    map.AddItem(new MapItem(CreateId("item", position), objectType.ToString(), GuessResourceType(tile), GetQuantity(tile), position));
                    break;
                case Homm2MapObjectType.Monster:
                case Homm2MapObjectType.RandomMonster:
                case Homm2MapObjectType.RandomMonsterWeak:
                case Homm2MapObjectType.RandomMonsterMedium:
                case Homm2MapObjectType.RandomMonsterStrong:
                case Homm2MapObjectType.RandomMonsterVeryStrong:
                    map.AddUnit(new MapUnit(CreateId("unit", position), objectType.ToString(), "Neutral", GetQuantity(tile), position));
                    break;
            }
        }

        private static int GetQuantity(Homm2TileRecord tile)
        {
            var value = (tile.Quantity1 & 0x1F) | (tile.Quantity2 << 5);
            return value <= 0 ? 1 : value;
        }

        private static ResourceType GuessResourceType(Homm2TileRecord tile)
        {
            var index = tile.TopIcnImageIndex != 255 ? tile.TopIcnImageIndex : tile.BottomIcnImageIndex;
            switch (index % 7)
            {
                case 0:
                    return ResourceType.Wood;
                case 1:
                    return ResourceType.Mercury;
                case 2:
                    return ResourceType.Ore;
                case 3:
                    return ResourceType.Sulfur;
                case 4:
                    return ResourceType.Crystal;
                case 5:
                    return ResourceType.Gems;
                default:
                    return ResourceType.Gold;
            }
        }

        private static string CreateId(string prefix, MapPosition position)
        {
            return prefix + "_" + position.X + "_" + position.Y;
        }

        private static string ReadFixedString(Stream stream, int offset, int length)
        {
            stream.Position = offset;
            var bytes = new byte[length];
            stream.Read(bytes, 0, bytes.Length);

            var count = Array.IndexOf(bytes, (byte)0);
            if (count < 0)
            {
                count = bytes.Length;
            }

            return System.Text.Encoding.ASCII.GetString(bytes, 0, count).Trim();
        }

        private static uint ReadUInt32BigEndian(BinaryReader reader)
        {
            var bytes = reader.ReadBytes(4);
            if (bytes.Length != 4)
            {
                throw new EndOfStreamException();
            }

            return ((uint)bytes[0] << 24) | ((uint)bytes[1] << 16) | ((uint)bytes[2] << 8) | bytes[3];
        }
    }
}
