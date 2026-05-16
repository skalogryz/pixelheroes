using Godot;
using PixelHeroes.Homm2.Maps;
using System;

public class MainMenu : Control
{
	public override void _Ready()
	{
		GetNode<Button>("Menu/NewGameButton").Connect("pressed", this, nameof(StartNewGame));
		GetNode<Button>("Menu/LoadMapButton").Connect("pressed", this, nameof(OpenLoadMapDialog));
		GetNode<Button>("Menu/MapButton").Connect("pressed", this, nameof(OpenGameMap));
		GetNode<Button>("Menu/BattleButton").Connect("pressed", this, nameof(OpenBattle));
		GetNode<Button>("Menu/TownButton").Connect("pressed", this, nameof(OpenTown));
		GetNode<Button>("Menu/SettingsButton").Connect("pressed", this, nameof(OpenSettings));
		GetNode<Button>("Menu/QuitButton").Connect("pressed", this, nameof(QuitGame));

		var loadMapDialog = GetNode<FileDialog>("LoadMapDialog");
		loadMapDialog.Mode = FileDialog.ModeEnum.OpenFile;
		loadMapDialog.Access = FileDialog.AccessEnum.Filesystem;
		loadMapDialog.Filters = new string[]
		{
			"*.mp2 ; HoMM2 Succession Wars Map",
			"*.mx2 ; HoMM2 Price of Loyalty Map"
		};
		loadMapDialog.Connect("file_selected", this, nameof(LoadHomm2Map));
	}

	private void OpenGameMap()
	{
		GetTree().ChangeScene("res://Scenes/GameMap.tscn");
	}

	private void StartNewGame()
	{
		GameSession.UseDefaultMap();
		OpenGameMap();
	}

	private void OpenLoadMapDialog()
	{
		GetNode<FileDialog>("LoadMapDialog").PopupCenteredRatio(0.75f);
	}

	private void LoadHomm2Map(string path)
	{
		try
		{
			var map = new Homm2MapReader().Read(path);
			GameSession.UseMap(map);
			OpenGameMap();
		}
		catch (Exception exception)
		{
			var errorDialog = GetNode<AcceptDialog>("ErrorDialog");
			errorDialog.DialogText = $"Could not load HoMM2 map:\n{exception.Message}";
			errorDialog.PopupCentered();
		}
	}

	private void OpenBattle()
	{
		GetTree().ChangeScene("res://Scenes/BattleScreen.tscn");
	}

	private void OpenTown()
	{
		GetTree().ChangeScene("res://Scenes/TownScreen.tscn");
	}

	private void OpenSettings()
	{
		GetTree().ChangeScene("res://Scenes/Settings.tscn");
	}

	private void QuitGame()
	{
		GetTree().Quit();
	}
}
