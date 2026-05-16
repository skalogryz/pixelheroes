using Godot;
using PixelHeroes.Core.World;

public class GameMap : Control
{
    private const int TileSize = 48;
    private AdventureMap adventureMap;

    public override void _Ready()
    {
        GetNode<Button>("Header/TownButton").Connect("pressed", this, nameof(OpenTown));
        GetNode<Button>("Header/BattleButton").Connect("pressed", this, nameof(OpenBattle));
        GetNode<Button>("Header/SettingsButton").Connect("pressed", this, nameof(OpenSettings));
        GetNode<Button>("Header/MenuButton").Connect("pressed", this, nameof(OpenMenu));

        adventureMap = SampleMapFactory.CreateStartingMap();
        GetNode<Label>("Header/Title").Text = $"Adventure Map - {adventureMap.Name}";
        RenderAdventureMap();
    }

    private void RenderAdventureMap()
    {
        var mapRoot = GetNode<Control>("MapPlaceholder");
        foreach (Node child in mapRoot.GetChildren())
        {
            child.QueueFree();
        }

        var grid = new GridContainer
        {
            Name = "RenderedMap",
            Columns = adventureMap.Width,
            RectMinSize = new Vector2(adventureMap.Width * TileSize, adventureMap.Height * TileSize)
        };

        grid.Set("custom_constants/hseparation", 2);
        grid.Set("custom_constants/vseparation", 2);
        grid.AnchorLeft = 0.5f;
        grid.AnchorTop = 0.5f;
        grid.AnchorRight = 0.5f;
        grid.AnchorBottom = 0.5f;
        grid.MarginLeft = -adventureMap.Width * TileSize / 2f;
        grid.MarginTop = -adventureMap.Height * TileSize / 2f;
        grid.MarginRight = adventureMap.Width * TileSize / 2f;
        grid.MarginBottom = adventureMap.Height * TileSize / 2f;

        mapRoot.AddChild(grid);

        for (var y = 0; y < adventureMap.Height; y++)
        {
            for (var x = 0; x < adventureMap.Width; x++)
            {
                grid.AddChild(CreateTileCell(adventureMap.GetTile(x, y)));
            }
        }
    }

    private Control CreateTileCell(MapTile tile)
    {
        var cell = new ColorRect
        {
            RectMinSize = new Vector2(TileSize, TileSize),
            Color = GetTerrainColor(tile.Terrain)
        };

        var label = new Label
        {
            AnchorRight = 1,
            AnchorBottom = 1,
            Align = Label.AlignEnum.Center,
            Valign = Label.VAlign.Center,
            Text = GetMarkerText(tile.Position)
        };

        label.AddColorOverride("font_color", tile.IsPassable ? Colors.White : new Color(0.78f, 0.82f, 0.88f));
        cell.AddChild(label);
        return cell;
    }

    private string GetMarkerText(MapPosition position)
    {
        foreach (var location in adventureMap.Locations)
        {
            if (location.Position.X == position.X && location.Position.Y == position.Y)
            {
                return GetLocationMarker(location.Type);
            }
        }

        foreach (var unit in adventureMap.Units)
        {
            if (unit.Position.X == position.X && unit.Position.Y == position.Y)
            {
                return "U";
            }
        }

        foreach (var item in adventureMap.Items)
        {
            if (item.Position.X == position.X && item.Position.Y == position.Y)
            {
                return GetResourceMarker(item.ResourceType);
            }
        }

        return string.Empty;
    }

    private static string GetLocationMarker(SpecialLocationType type)
    {
        switch (type)
        {
            case SpecialLocationType.Castle:
                return "C";
            case SpecialLocationType.Mine:
                return "M";
            case SpecialLocationType.Sawmill:
                return "S";
            case SpecialLocationType.Shrine:
                return "+";
            case SpecialLocationType.Dwelling:
                return "D";
            case SpecialLocationType.Treasure:
                return "T";
            default:
                return "?";
        }
    }

    private static string GetResourceMarker(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Gold:
                return "G";
            case ResourceType.Wood:
                return "W";
            case ResourceType.Ore:
                return "O";
            case ResourceType.Crystal:
                return "C";
            case ResourceType.Gems:
                return "J";
            case ResourceType.Sulfur:
                return "S";
            case ResourceType.Mercury:
                return "M";
            default:
                return "?";
        }
    }

    private static Color GetTerrainColor(TerrainType terrain)
    {
        switch (terrain)
        {
            case TerrainType.Grass:
                return new Color(0.18f, 0.42f, 0.18f);
            case TerrainType.Dirt:
                return new Color(0.45f, 0.31f, 0.16f);
            case TerrainType.Road:
                return new Color(0.58f, 0.49f, 0.31f);
            case TerrainType.Water:
                return new Color(0.12f, 0.32f, 0.58f);
            case TerrainType.Mountain:
                return new Color(0.30f, 0.32f, 0.36f);
            case TerrainType.Forest:
                return new Color(0.08f, 0.25f, 0.12f);
            default:
                return Colors.Magenta;
        }
    }

    private void OpenTown()
    {
        GetTree().ChangeScene("res://Scenes/TownScreen.tscn");
    }

    private void OpenBattle()
    {
        GetTree().ChangeScene("res://Scenes/BattleScreen.tscn");
    }

    private void OpenSettings()
    {
        GetTree().ChangeScene("res://Scenes/Settings.tscn");
    }

    private void OpenMenu()
    {
        GetTree().ChangeScene("res://Scenes/MainMenu.tscn");
    }
}
