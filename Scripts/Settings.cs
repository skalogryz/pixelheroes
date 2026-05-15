using Godot;

public class Settings : Control
{
    public override void _Ready()
    {
        var fullscreen = GetNode<CheckBox>("Panel/FullscreenCheckBox");
        fullscreen.Pressed = OS.WindowFullscreen;
        fullscreen.Connect("toggled", this, nameof(SetFullscreen));

        GetNode<Button>("Panel/MapButton").Connect("pressed", this, nameof(OpenMap));
        GetNode<Button>("Panel/MenuButton").Connect("pressed", this, nameof(OpenMenu));
    }

    private void SetFullscreen(bool enabled)
    {
        OS.WindowFullscreen = enabled;
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
