using Godot;

public class GameMap : Control
{
    public override void _Ready()
    {
        GetNode<Button>("Header/TownButton").Connect("pressed", this, nameof(OpenTown));
        GetNode<Button>("Header/BattleButton").Connect("pressed", this, nameof(OpenBattle));
        GetNode<Button>("Header/SettingsButton").Connect("pressed", this, nameof(OpenSettings));
        GetNode<Button>("Header/MenuButton").Connect("pressed", this, nameof(OpenMenu));
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
