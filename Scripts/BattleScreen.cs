using Godot;

public class BattleScreen : Control
{
    public override void _Ready()
    {
        GetNode<Button>("TopBar/MapButton").Connect("pressed", this, nameof(OpenMap));
        GetNode<Button>("TopBar/MenuButton").Connect("pressed", this, nameof(OpenMenu));
    }

    private void OpenMap()
    {
        GetTree().ChangeScene("res://Scenes/GameMap.tscn");
    }

    private void OpenMenu()
    {
        GetTree().ChangeScene("res://Scenes/MainMenu.tscn");
    }
}
