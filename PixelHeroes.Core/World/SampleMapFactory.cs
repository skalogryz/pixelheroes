namespace PixelHeroes.Core.World
{
    public static class SampleMapFactory
    {
        public static AdventureMap CreateStartingMap()
        {
            var map = new AdventureMap("Valley of Dawn", 16, 10);

            for (var y = 0; y < map.Height; y++)
            {
                for (var x = 0; x < map.Width; x++)
                {
                    var terrain = TerrainType.Grass;
                    var passable = true;

                    if (y == 4 && x > 1 && x < 14)
                    {
                        terrain = TerrainType.Road;
                    }
                    else if (x == 0 || y == 0 || x == map.Width - 1 || y == map.Height - 1)
                    {
                        terrain = TerrainType.Mountain;
                        passable = false;
                    }
                    else if ((x == 4 && y < 4) || (x == 11 && y > 5))
                    {
                        terrain = TerrainType.Forest;
                    }
                    else if (x > 12 && y < 4)
                    {
                        terrain = TerrainType.Water;
                        passable = false;
                    }
                    else if (x > 6 && x < 10 && y > 5)
                    {
                        terrain = TerrainType.Dirt;
                    }

                    map.SetTile(new MapTile(new MapPosition(x, y), terrain, passable));
                }
            }

            map.AddLocation(new SpecialLocation("castle_red", "Red Keep", SpecialLocationType.Castle, new MapPosition(2, 4), true));
            map.AddLocation(new SpecialLocation("sawmill_01", "Old Sawmill", SpecialLocationType.Sawmill, new MapPosition(5, 2), false));
            map.AddLocation(new SpecialLocation("mine_01", "Ore Pit", SpecialLocationType.Mine, new MapPosition(9, 7), false));
            map.AddLocation(new SpecialLocation("shrine_01", "Shrine of Speed", SpecialLocationType.Shrine, new MapPosition(13, 5), false));

            map.AddItem(new MapItem("gold_01", "Gold Pile", ResourceType.Gold, 750, new MapPosition(6, 4)));
            map.AddItem(new MapItem("wood_01", "Wood Bundle", ResourceType.Wood, 8, new MapPosition(3, 7)));
            map.AddItem(new MapItem("ore_01", "Ore Cart", ResourceType.Ore, 6, new MapPosition(10, 4)));

            map.AddUnit(new MapUnit("peasants_01", "Peasants", "Neutral", 24, new MapPosition(8, 3)));
            map.AddUnit(new MapUnit("wolves_01", "Wolves", "Neutral", 12, new MapPosition(12, 7)));

            return map;
        }
    }
}
