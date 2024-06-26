using Godot;

public partial class DisplayManager : Node
{
    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Input.IsActionJustPressed("Quit"))
        {
            GetTree().Quit();
        }

        if (Input.IsActionJustPressed("Fullscreen"))
        {
            ToggleFullscreen();
        }
    }

    private void ToggleFullscreen()
    {
        var screen = DisplayServer.WindowGetCurrentScreen();

        var screenRootPosition = DisplayServer.ScreenGetPosition(screen);

        var screenSize = DisplayServer.ScreenGetSize(screen);

        if (DisplayServer.WindowGetMode() != DisplayServer.WindowMode.Windowed)
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
            var size = new Vector2I(1280, 720);
            DisplayServer.WindowSetSize(size);
            DisplayServer.WindowSetPosition(screenRootPosition + (screenSize / 2 - size / 2));
        }
        else
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
        }
    }
}
