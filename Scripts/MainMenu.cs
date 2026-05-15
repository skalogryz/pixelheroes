using Godot;

public class MainMenu : Control
{
	public override void _Ready()
	{
		GetNode<Button>("Menu/NewGameButton").Connect("pressed", this, nameof(OpenGameMap));
		GetNode<Button>("Menu/MapButton").Connect("pressed", this, nameof(OpenGameMap));
		GetNode<Button>("Menu/BattleButton").Connect("pressed", this, nameof(OpenBattle));
		GetNode<Button>("Menu/TownButton").Connect("pressed", this, nameof(OpenTown));
		GetNode<Button>("Menu/SettingsButton").Connect("pressed", this, nameof(OpenSettings));
		GetNode<Button>("Menu/QuitButton").Connect("pressed", this, nameof(QuitGame));
	}

	private void OpenGameMap()
	{
		GetTree().ChangeScene("res://Scenes/GameMap.tscn");
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
