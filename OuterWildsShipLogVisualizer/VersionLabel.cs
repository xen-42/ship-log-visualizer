using Godot;
using System;

public partial class VersionLabel : Label
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        using var file = FileAccess.Open("res://version.txt", FileAccess.ModeFlags.Read);
        var version = file.GetAsText().Trim();
        Text = $"Ship Log Visualizer {version} by xen-42";
    }
}
