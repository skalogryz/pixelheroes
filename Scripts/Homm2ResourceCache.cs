using System.Collections.Generic;
using System.IO;
using Godot;
using PixelHeroes.Homm2.Sprites;
using IoFile = System.IO.File;
using IoPath = System.IO.Path;

public static class Homm2ResourceCache
{
    private static readonly Dictionary<string, Homm2GodotSpriteFrame> terrainTiles = new Dictionary<string, Homm2GodotSpriteFrame>();
    private static readonly Dictionary<string, Homm2GodotSpriteFrame> transformedTerrainTiles = new Dictionary<string, Homm2GodotSpriteFrame>();
    private static bool terrainTilesLoaded;
    private static string loadedAggPath;

    public static bool TerrainTilesLoaded => terrainTilesLoaded;
    public static string LoadedAggPath => loadedAggPath;

    public static void LoadForMap(string mapPath)
    {
        if (terrainTilesLoaded)
        {
            return;
        }

        var aggPath = FindHeroes2Agg(mapPath);
        if (string.IsNullOrEmpty(aggPath))
        {
            GD.Print("HoMM2 resources were not found near the selected map. Terrain will use fallback colors.");
            return;
        }

        LoadFromAgg(aggPath);
    }

    public static void LoadFromAgg(string aggPath)
    {
        if (terrainTilesLoaded)
        {
            return;
        }

        var resource = new Homm2SpriteLoader().LoadTileSetFromAgg(aggPath);
        var frames = Homm2SpriteTextureConverter.ConvertResource(resource);
        terrainTiles.Clear();
        transformedTerrainTiles.Clear();

        for (var i = 0; i < frames.Count; i++)
        {
            terrainTiles[i.ToString()] = frames[i];
        }

        loadedAggPath = aggPath;
        terrainTilesLoaded = true;
        GD.Print($"Loaded {frames.Count} HoMM2 terrain tiles from {aggPath}.");
    }

    public static bool TryGetTerrainTile(string spriteKey, out Homm2GodotSpriteFrame frame)
    {
        if (string.IsNullOrEmpty(spriteKey))
        {
            frame = null;
            return false;
        }

        if (!TryParseTerrainSpriteKey(spriteKey, out var index, out var verticalFlip, out var horizontalFlip))
        {
            return terrainTiles.TryGetValue(spriteKey, out frame);
        }

        if (!verticalFlip && !horizontalFlip)
        {
            return terrainTiles.TryGetValue(index, out frame);
        }

        if (transformedTerrainTiles.TryGetValue(spriteKey, out frame))
        {
            return true;
        }

        if (!terrainTiles.TryGetValue(index, out var baseFrame))
        {
            frame = null;
            return false;
        }

        frame = Homm2SpriteTextureConverter.CreateFlippedFrame(baseFrame, verticalFlip, horizontalFlip);
        transformedTerrainTiles[spriteKey] = frame;
        return true;
    }

    private static bool TryParseTerrainSpriteKey(string spriteKey, out string index, out bool verticalFlip, out bool horizontalFlip)
    {
        index = null;
        verticalFlip = false;
        horizontalFlip = false;

        var parts = spriteKey.Split(':');
        if (parts.Length != 4 || parts[0] != "terrain")
        {
            return false;
        }

        index = parts[1];
        verticalFlip = parts[2] == "v";
        horizontalFlip = parts[3] == "h";
        return true;
    }

    private static string FindHeroes2Agg(string mapPath)
    {
        var startDirectory = IoFile.Exists(mapPath) ? IoPath.GetDirectoryName(mapPath) : mapPath;
        if (string.IsNullOrEmpty(startDirectory))
        {
            return null;
        }

        var directory = new DirectoryInfo(startDirectory);
        while (directory != null)
        {
            var candidates = new[]
            {
                IoPath.Combine(directory.FullName, "HEROES2.AGG"),
                IoPath.Combine(directory.FullName, "heroes2.agg"),
                IoPath.Combine(directory.FullName, "DATA", "HEROES2.AGG"),
                IoPath.Combine(directory.FullName, "DATA", "heroes2.agg"),
                IoPath.Combine(directory.FullName, "data", "HEROES2.AGG"),
                IoPath.Combine(directory.FullName, "data", "heroes2.agg")
            };

            foreach (var candidate in candidates)
            {
                if (IoFile.Exists(candidate))
                {
                    return candidate;
                }
            }

            directory = directory.Parent;
        }

        return null;
    }
}
