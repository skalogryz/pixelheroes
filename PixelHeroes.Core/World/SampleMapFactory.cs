namespace PixelHeroes.Core.World
{
    public static class SampleMapFactory
    {
        public static AdventureMap CreateStartingMap()
        {
            var map = new AdventureMap("Valley of Dawn", 32, 20);

            for (var y = 0; y < map.Height; y++)
            {
                for (var x = 0; x < map.Width; x++)
                {
                    var terrain = TerrainType.Grass;
                    var passable = true;

                    if ((y == 4 || y == 12) && x > 1 && x < map.Width - 2)
                    {
                        terrain = TerrainType.Road;
                    }
                    else if (x == 18 && y > 4 && y < 13)
                    {
                        terrain = TerrainType.Road;
                    }
                    else if (x == 0 || y == 0 || x == map.Width - 1 || y == map.Height - 1)
                    {
                        terrain = TerrainType.Mountain;
                        passable = false;
                    }
                    else if ((x == 4 && y < 10) || (x == 11 && y > 5) || (x == 22 && y > 8))
                    {
                        terrain = TerrainType.Forest;
                    }
                    else if (x > 24 && y < 7)
                    {
                        terrain = TerrainType.Water;
                        passable = false;
                    }
                    else if ((x > 6 && x < 10 && y > 5) || (x > 17 && x < 25 && y > 13))
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
            map.AddLocation(new SpecialLocation("castle_blue", "Blue Hold", SpecialLocationType.Castle, new MapPosition(28, 12), false));
            map.AddLocation(new SpecialLocation("dwelling_01", "Wolf Den", SpecialLocationType.Dwelling, new MapPosition(22, 15), false));

            map.AddItem(new MapItem("gold_01", "Gold Pile", ResourceType.Gold, 750, new MapPosition(6, 4)));
            map.AddItem(new MapItem("wood_01", "Wood Bundle", ResourceType.Wood, 8, new MapPosition(3, 7)));
            map.AddItem(new MapItem("ore_01", "Ore Cart", ResourceType.Ore, 6, new MapPosition(10, 4)));
            map.AddItem(new MapItem("gems_01", "Gems", ResourceType.Gems, 4, new MapPosition(20, 12)));
            map.AddItem(new MapItem("crystal_01", "Crystal", ResourceType.Crystal, 3, new MapPosition(27, 6)));

            map.AddUnit(new MapUnit("peasants_01", "Peasants", "Neutral", 24, new MapPosition(8, 3)));
            map.AddUnit(new MapUnit("wolves_01", "Wolves", "Neutral", 12, new MapPosition(12, 7)));
            map.AddUnit(new MapUnit("goblins_01", "Goblins", "Neutral", 20, new MapPosition(18, 10)));
            map.AddUnit(new MapUnit("dwarves_01", "Dwarves", "Neutral", 10, new MapPosition(25, 16)));

            return map;
        }
    }
}
