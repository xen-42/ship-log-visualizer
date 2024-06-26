using Godot;
using System;
using System.Linq;

public partial class ShipLogVisualizerUI : Control
{
    [Export] private Button _selectFolderButton;
    [Export] private FileDialog _selectFolderDialog;
    [Export] private Label _modLabel;
    [Export] private OptionButton _starSystemSelection;
    [Export] private Button _reloadButton;

    private string _currentDir;

	public override void _Ready()
	{
        _selectFolderButton.Pressed += () => _selectFolderDialog.Show();
        _selectFolderDialog.DirSelected += LoadDir;
        _reloadButton.Pressed += () => LoadDir(_currentDir);
        _starSystemSelection.ItemSelected += (_) => LoadDir(_currentDir);

        _reloadButton.Disabled = true;
    }

    private void LoadDir(string dir)
    {
        _currentDir = dir;

        if (!IsFolderValid(dir))
        {
            _reloadButton.Disabled = true;
            _modLabel.Text = "That is not a valid mod folder (missing manifest.json)";
            return;
        }

        var modName = System.IO.Path.GetFileName(dir);

        var selectedSolarSystem = GetSelectedSolarSystem();

        _reloadButton.Disabled = false;

        if (ShipLogsRoot.Instance.Load(dir, selectedSolarSystem))
        {
            _modLabel.Text = modName;
        }
        else
        {
            _modLabel.Text = "Failed to load any ship logs in that mod/system.";
        }
        UpdateOptions(selectedSolarSystem);
    }

    private string GetSelectedSolarSystem()
    {
        var systems = ShipLogsRoot.Instance.StarSystems.Keys.ToList();

        if (_starSystemSelection.Selected >= 0 && _starSystemSelection.Selected < systems.Count)
        {
            return systems.ElementAt(_starSystemSelection.Selected);
        }
        else
        {
            return "SolarSystem";
        }
    }

    private void UpdateOptions(string selectedSolarSystem)
    {
        _starSystemSelection.Clear();
        ShipLogsRoot.Instance.StarSystems.Keys.ToList().ForEach(x => _starSystemSelection.AddItem(x));
        _starSystemSelection.Selected = ShipLogsRoot.Instance.StarSystems.Keys.ToList().IndexOf(selectedSolarSystem);
        if (_starSystemSelection.Selected == -1 && _starSystemSelection.ItemCount > 0)
        {
            _starSystemSelection.Selected = 0;
        }
    }

    private static bool IsFolderValid(string path)
    {
        return FileAccess.FileExists(System.IO.Path.Combine(path, "manifest.json"));
    }
}
