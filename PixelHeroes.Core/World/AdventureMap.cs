using System;
using System.Collections.Generic;

namespace PixelHeroes.Core.World
{
    public sealed class AdventureMap
    {
        private readonly MapTile[,] tiles;
        private readonly List<MapItem> items = new List<MapItem>();
        private readonly List<MapUnit> units = new List<MapUnit>();
        private readonly List<SpecialLocation> locations = new List<SpecialLocation>();

        public AdventureMap(string name, int width, int height)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width));
            }

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height));
            }

            Name = name ?? throw new ArgumentNullException(nameof(name));
            Width = width;
            Height = height;
            tiles = new MapTile[width, height];
        }

        public string Name { get; }
        public int Width { get; }
        public int Height { get; }
        public IReadOnlyList<MapItem> Items => items;
        public IReadOnlyList<MapUnit> Units => units;
        public IReadOnlyList<SpecialLocation> Locations => locations;

        public MapTile GetTile(int x, int y)
        {
            EnsureInsideMap(x, y);
            return tiles[x, y];
        }

        public void SetTile(MapTile tile)
        {
            if (tile == null)
            {
                throw new ArgumentNullException(nameof(tile));
            }

            EnsureInsideMap(tile.Position.X, tile.Position.Y);
            tiles[tile.Position.X, tile.Position.Y] = tile;
        }

        public void AddItem(MapItem item)
        {
            EnsureInsideMap(item.Position.X, item.Position.Y);
            items.Add(item);
        }

        public void AddUnit(MapUnit unit)
        {
            EnsureInsideMap(unit.Position.X, unit.Position.Y);
            units.Add(unit);
        }

        public void AddLocation(SpecialLocation location)
        {
            EnsureInsideMap(location.Position.X, location.Position.Y);
            locations.Add(location);
        }

        private void EnsureInsideMap(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
            {
                throw new ArgumentOutOfRangeException($"Position {x}:{y} is outside map {Width}x{Height}.");
            }
        }
    }
}
