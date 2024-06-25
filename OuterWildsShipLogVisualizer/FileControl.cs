using Godot;
using System;
using System.Linq;

public partial class FileControl : Control
{
    [Export] private Button _openFileButton;
    [Export] private FileDialog _openFileDialog;
    [Export] private Label _label;
    [Export] private ShipLogsRoot _shipLogsRoot;
    [Export] private Button _reloadButton;
    [Export] private OptionButton _options;

    private string _currentDir;

	public override void _Ready()
	{
        _openFileButton.Pressed += () => _openFileDialog.Show();
        _openFileDialog.DirSelected += LoadDir;
        _reloadButton.Pressed += () => LoadDir(_currentDir);
        _options.ItemSelected += (_) => LoadDir(_currentDir);

        _reloadButton.Disabled = true;
    }

    private void LoadDir(string dir)
    {
        _currentDir = dir;
        if (FileAccess.FileExists(System.IO.Path.Combine(dir, "manifest.json")))
        {
            _reloadButton.Disabled = false;
            var selectedSolarSystem = "SolarSystem";
            try
            {
                selectedSolarSystem = _shipLogsRoot.StarSystems.Keys.ElementAt(_options.Selected);
            }
            catch { }
            if (_shipLogsRoot.Load(dir, selectedSolarSystem))
            {
                _label.Text = System.IO.Path.GetFileName(dir);
                _options.Clear();
                foreach (var system in _shipLogsRoot.StarSystems.Keys)
                {
                    GD.Print(system + " GRUH");
                    _options.AddItem(system);
                }
                _options.Selected = _shipLogsRoot.StarSystems.Keys.ToList().IndexOf(selectedSolarSystem);
            }
            else
            {
                _label.Text = "Failed to load any ship logs in that mod.";
            }
        }
        else
        {
            _reloadButton.Disabled = true;
            _label.Text = "That is not a valid mod folder (should be where your manifest.json file is contained)";
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
