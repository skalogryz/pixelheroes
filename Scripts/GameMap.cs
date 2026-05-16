using System;
using System.Collections.Generic;
using Godot;
using PixelHeroes.Core.World;

public class GameMap : Control
{
	private const int TileSize = 48;
	private const float MinZoom = 0.25f;
	private const float MaxZoom = 2.0f;
	private const float ZoomStep = 0.1f;
	private const int EdgeScrollSize = 32;
	private const int EdgeScrollSpeed = 420;
	private AdventureMap adventureMap;
	private GridContainer renderedMap;
	private ScrollContainer mapScroll;
	private HSlider zoomSlider;
	private Label zoomLabel;
	private float zoom = 1.0f;
	private bool isPanning;
	private Vector2 lastPanMousePosition;

	List<Node> added = new List<Node>();

	public override void _Ready()
	{
		SetProcess(true);
		SetProcessInput(true);

		GetNode<Button>("Header/TownButton").Connect("pressed", this, nameof(OpenTown));
		GetNode<Button>("Header/BattleButton").Connect("pressed", this, nameof(OpenBattle));
		GetNode<Button>("Header/SettingsButton").Connect("pressed", this, nameof(OpenSettings));
		GetNode<Button>("Header/MenuButton").Connect("pressed", this, nameof(OpenMenu));

		mapScroll = GetNode<ScrollContainer>("MapPlaceholder/MapScroll");
		zoomSlider = GetNode<HSlider>("ZoomPanel/ZoomSlider");
		zoomLabel = GetNode<Label>("ZoomPanel/ZoomLabel");
		zoomSlider.Connect("value_changed", this, nameof(SetZoomFromSlider));
		GetNode<Button>("ZoomPanel/ZoomOutButton").Connect("pressed", this, nameof(ZoomOut));
		GetNode<Button>("ZoomPanel/ZoomInButton").Connect("pressed", this, nameof(ZoomIn));

		adventureMap = GameSession.TakeMapOrDefault();
		GetNode<Label>("Header/Title").Text = $"Adventure Map - {adventureMap.Name}";
		RenderAdventureMap();
		ApplyZoom();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton && IsMouseInsideMap(mouseButton.Position))
		{
			if (mouseButton.ButtonIndex == (int)ButtonList.WheelUp && mouseButton.Pressed)
			{
				ChangeZoom(ZoomStep);
				GetTree().SetInputAsHandled();
			}
			else if (mouseButton.ButtonIndex == (int)ButtonList.WheelDown && mouseButton.Pressed)
			{
				ChangeZoom(-ZoomStep);
				GetTree().SetInputAsHandled();
			}
			else if (mouseButton.ButtonIndex == (int)ButtonList.Middle || mouseButton.ButtonIndex == (int)ButtonList.Right)
			{
				isPanning = mouseButton.Pressed;
				lastPanMousePosition = mouseButton.Position;
				GetTree().SetInputAsHandled();
			}
		}
		else if (@event is InputEventMouseMotion mouseMotion && isPanning)
		{
			var delta = mouseMotion.Position - lastPanMousePosition;
			PanBy(-delta);
			lastPanMousePosition = mouseMotion.Position;
			GetTree().SetInputAsHandled();
		}
	}

	public override void _Process(float delta)
	{
		if (isPanning)
		{
			return;
		}

		var mousePosition = GetGlobalMousePosition();
		if (!IsMouseInsideMap(mousePosition))
		{
			return;
		}

		var mapRect = GetNode<Control>("MapPlaceholder").GetGlobalRect();
		var scrollDelta = Vector2.Zero;

		if (mousePosition.x <= mapRect.Position.x + EdgeScrollSize)
		{
			scrollDelta.x -= EdgeScrollSpeed * delta;
		}
		else if (mousePosition.x >= mapRect.End.x - EdgeScrollSize)
		{
			scrollDelta.x += EdgeScrollSpeed * delta;
		}

		if (mousePosition.y <= mapRect.Position.y + EdgeScrollSize)
		{
			scrollDelta.y -= EdgeScrollSpeed * delta;
		}
		else if (mousePosition.y >= mapRect.End.y - EdgeScrollSize)
		{
			scrollDelta.y += EdgeScrollSpeed * delta;
		}

		if (scrollDelta != Vector2.Zero)
		{
			PanBy(scrollDelta);
		}
	}

	private void RenderAdventureMap()
	{
		var mapRoot = GetNode<Control>("MapPlaceholder/MapScroll");
		var chlist = mapRoot.GetChildren();
		foreach (Node child in added)
		{
			child.QueueFree();
		}

		renderedMap = new GridContainer
		{
			Name = "RenderedMap",
			Columns = adventureMap.Width
		};
		renderedMap.Set("custom_constants/hseparation", 2);
		renderedMap.Set("custom_constants/vseparation", 2);
		mapRoot.AddChild(renderedMap);
		added.Add(renderedMap);

		for (var y = 0; y < adventureMap.Height; y++)
		{
			for (var x = 0; x < adventureMap.Width; x++)
			{
				var node = CreateTileCell(adventureMap.GetTile(x, y));
				renderedMap.AddChild(node);
				added.Add(node);
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

	private void SetZoomFromSlider(float value)
	{
		zoom = Mathf.Clamp(value / 100.0f, MinZoom, MaxZoom);
		ApplyZoom();
	}

	private void ZoomOut()
	{
		ChangeZoom(-ZoomStep);
	}

	private void ZoomIn()
	{
		ChangeZoom(ZoomStep);
	}

	private void ChangeZoom(float delta)
	{
		zoom = Mathf.Clamp(zoom + delta, MinZoom, MaxZoom);
		ApplyZoom();
	}

	private void ApplyZoom()
	{
		var tileSize = Mathf.RoundToInt(TileSize * zoom);
		var separation = Mathf.Max(1, Mathf.RoundToInt(2 * zoom));

		if (renderedMap != null)
		{
			renderedMap.Set("custom_constants/hseparation", separation);
			renderedMap.Set("custom_constants/vseparation", separation);
			renderedMap.RectMinSize = new Vector2(adventureMap.Width * (tileSize + separation), adventureMap.Height * (tileSize + separation));

			foreach (Node child in renderedMap.GetChildren())
			{
				if (child is Control control)
				{
					control.RectMinSize = new Vector2(tileSize, tileSize);
				}
			}
		}

		if (zoomSlider != null && !Mathf.IsEqualApprox((float)zoomSlider.Value, zoom * 100.0f))
		{
			zoomSlider.Value = zoom * 100.0f;
		}

		if (zoomLabel != null)
		{
			zoomLabel.Text = $"{Mathf.RoundToInt(zoom * 100.0f)}%";
		}
	}

	private void PanBy(Vector2 delta)
	{
		mapScroll.ScrollHorizontal = Mathf.Max(0, mapScroll.ScrollHorizontal + Mathf.RoundToInt(delta.x));
		mapScroll.ScrollVertical = Mathf.Max(0, mapScroll.ScrollVertical + Mathf.RoundToInt(delta.y));
	}

	private bool IsMouseInsideMap(Vector2 globalPosition)
	{
		return GetNode<Control>("MapPlaceholder").GetGlobalRect().HasPoint(globalPosition);
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
