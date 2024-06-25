using Godot;
using Newtonsoft.Json;
using OuterWildsShipLogVisualizer;
using OuterWildsShipLogVisualizer.External;
using OuterWildsShipLogVisualizer.OuterWildsXML;
using System.Collections.Generic;
using System.Linq;

public partial class ShipLogEntryDisplay : Node2D
{
    private static Dictionary<string, ShipLogEntryDisplay> _entries = new();
    public static void ClearCache() => _entries.Clear();

    [ExportCategory("Nodes")]
    [Export] private Label _label;
    [Export] private ColorRect _nameBackground;
    [Export] private TextureRect _revealedImage;
    [Export] private TextureRect _border;

    [ExportCategory("Scenes")]
    [Export] private PackedScene _shipLogLinkScene;

    private ShipLogEntry _entry;

    public void SetShipLogEntry(string rootFolder, ShipLogModule shipLogModule, ShipLogEntry entry, StarSystemConfig starSystem)
    {
        this._entry = entry;

        // Try load translations
        var name = entry.name;

        var englishPath = System.IO.Path.Combine(rootFolder, "translations/english.json");
        if (FileAccess.FileExists(englishPath))
        {
            try
            {
                using var englishFile = FileAccess.Open(englishPath, FileAccess.ModeFlags.Read);
                var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(englishFile.GetAsText());
                var shipLogDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(dict["ShipLogDictionary"].ToString());
                name = shipLogDict[name];
            }
            catch { }
        }

        _label.Text = name;
        Position = starSystem.entryPositions.First(x => x.id == entry.id).position.InvertY().ToVector2();
        var color = starSystem.curiosities?.FirstOrDefault(x => x.id == entry.curiosity)?.color?.ToColor() ?? new Color(0.41f, 0.41f, 0.41f);

        _nameBackground.Color = color;
        _border.Modulate = color;

        var texturePath = System.IO.Path.Combine(rootFolder, shipLogModule.spriteFolder, entry.id + ".png");
        if (FileAccess.FileExists(texturePath))
        {
            GD.Print($"Loading file at {texturePath}");
            var img = new Image();
            img.Load(texturePath);
            var texture = new ImageTexture();
            texture.SetImage(img);
            _revealedImage.Texture = texture;
        }
        else
        {
            GD.PrintErr($"Couldn't find texture at {texturePath}");
            _revealedImage.Visible = false;
        }

        if (!string.IsNullOrEmpty(entry.parentID))
        {
            this.Scale *= entry.isCuriosity ? 0.8f : 0.6f;
        }
        else if (entry.isCuriosity)
        {
            this.Scale *= 2f;
        }

        _entries.Add(_entry.id, this);
    }

    public void PostInit()
    {
        foreach (var arrowSource in _entry.rumorFacts.Select(x => x.sourceID).Where(x => !string.IsNullOrEmpty(x)))
        {
            GD.Print($"{_entry.id} pointed to from {arrowSource}");

            if (!_entries.ContainsKey(arrowSource))
            {
                GD.PrintErr($"Couldn't find ship log {arrowSource}");
            }
            else
            {
                var shipLogLink = _shipLogLinkScene.Instantiate<ShipLogLink>();
                this.GetParent().AddChild(shipLogLink);
                this.GetParent().MoveChild(shipLogLink, 0);
                var source = _entries[arrowSource];
                shipLogLink.SetExtents(source.Position, this.Position);
            }
        }
    }
}