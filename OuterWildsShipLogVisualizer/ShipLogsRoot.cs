using Godot;
using Newtonsoft.Json;
using OuterWildsShipLogVisualizer;
using OuterWildsShipLogVisualizer.External;
using OuterWildsShipLogVisualizer.OuterWildsXML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

public partial class ShipLogsRoot : Node2D
{
    [Export] public PackedScene ShipLogEntryScene { get; private set; }

    public Dictionary<string, StarSystemConfig> StarSystems { get; private set; } = new();
    public Dictionary<string, ShipLogEntryDisplay> Entries { get; private set; } = new();

    private static string _currentStarSystem;

    public static ShipLogsRoot Instance { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        Instance = this;
    }

    public bool Load(string rootFolder, string starSystem)
    {
        Entries.Clear();
        StarSystems.Clear();

        _currentStarSystem = starSystem;

        try
        {
            foreach (var child in this.GetChildren())
            {
                child.QueueFree();
            }

            this.Position = Vector2.Zero;

            var systemsPath = $"{rootFolder}/systems";
            var planetsPath = $"{rootFolder}/planets";

            using var systemDir = DirAccess.Open(systemsPath);
            foreach (var file in systemDir.GetFiles())
            {
                if (!file.EndsWith(".json")) continue;

                var path = System.IO.Path.Combine(systemsPath, file);
                var name = System.IO.Path.GetFileNameWithoutExtension(file);
                if (TryLoadStarSystem(path, out var starSystemConfig))
                {
                    StarSystems[name] = starSystemConfig;
                }
            }

            // Default to first system if our selection doesn't exist
            if (!StarSystems.ContainsKey(_currentStarSystem))
            {
                _currentStarSystem = StarSystems.Keys.First();
            }

            using var planetsDir = DirAccess.Open(planetsPath);
            foreach (var file in planetsDir.GetAllFiles())
            {
                if (!file.EndsWith(".json")) continue;

                var path = System.IO.Path.Combine(planetsPath, file);
                if (TryLoadShipLogModule(path, out var shipLogModule))
                {
                    if (string.IsNullOrEmpty(shipLogModule.xmlFile)) continue;

                    LoadShipLogXML(rootFolder, shipLogModule);
                }
            }

            if (_currentStarSystem == "SolarSystem")
            {
                StockSystemLoader.Load();
            }

            foreach (var entry in Entries.Values)
            {
                entry.LinkRumors();
            }
        }
        catch (Exception e)
        {
            foreach (var child in GetChildren())
            {
                child.QueueFree();
            }
            GD.PrintErr(e);
            return false;
        }
        return Entries.Any();
    }

    private static bool TryLoadStarSystem(string path, out StarSystemConfig config)
    {
        using var systemFile = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        try
        {
            config = JsonConvert.DeserializeObject<StarSystemConfig>(systemFile.GetAsText());
            return true;
        }
        catch (Exception e)
        {
            GD.PrintErr($"Couldn't load star system {path} - {e}");
        }

        config = null;
        return false;
    }

    private static bool TryLoadShipLogModule(string path, out ShipLogModule shipLogModule)
    {
        try
        {
            using var planetFile = FileAccess.Open(path, FileAccess.ModeFlags.Read);
            var planetConfig = JsonConvert.DeserializeObject<Dictionary<string, object>>(planetFile.GetAsText());

            // NH defaults the null starSystem value to SolarSystem
            var planetStarSystem = "SolarSystem";
            if (planetConfig.ContainsKey("starSystem"))
            {
                planetStarSystem = planetConfig["starSystem"].ToString();
            }

            if (planetStarSystem == _currentStarSystem)
            {
                shipLogModule = JsonConvert.DeserializeObject<ShipLogModule>(planetConfig["ShipLog"].ToString());
                return true;
            }
        }
        catch (Exception e)
        {
            GD.PrintErr($"Couldn't load ship logs for planet {path} - {e}");
        }

        shipLogModule = null;
        return false;
    }

    private void LoadShipLogXML(string rootFolder, ShipLogModule shipLogModule)
    {
        var xmlPath = System.IO.Path.Combine(rootFolder, shipLogModule.xmlFile);

        try
        {
            // To properly search through the XML file we must convert it to an XElement
            using var file = FileAccess.Open(xmlPath, FileAccess.ModeFlags.Read);
            var xml = new XmlDocument();
            xml.LoadXml(file.GetAsText());
            XElement xelement = XElement.Parse(xml.DocumentElement.OuterXml);

            var astroObjectID = xelement.Element("ID").Value;

            // Reading each ship log entry in the xml file
            foreach (XElement entryNode in xelement.Elements("Entry"))
            {
                var entry = new ShipLogEntry(astroObjectID, entryNode, "");

                // Create a display for the root entry and add it to our scene
                var shipLogEntryDisplay = ShipLogEntryScene.Instantiate<ShipLogEntryDisplay>();
                this.AddChild(shipLogEntryDisplay);
                shipLogEntryDisplay.SetShipLogEntry(rootFolder, shipLogModule, entry, StarSystems[_currentStarSystem]);

                // Go through all its children and add them if applicable
                entry.childEntries = new List<ShipLogEntry>();
                foreach (XElement childEntryXML in entryNode.Elements("Entry"))
                {
                    var childEntry = new ShipLogEntry(astroObjectID, childEntryXML, entry.id);
                    entry.childEntries.Add(childEntry);

                    var childShipLogEntryDisplay = ShipLogEntryScene.Instantiate<ShipLogEntryDisplay>();
                    this.AddChild(childShipLogEntryDisplay);
                    childShipLogEntryDisplay.SetShipLogEntry(rootFolder, shipLogModule, childEntry, StarSystems[_currentStarSystem]);

                    // Want to make sure it appears over its parent if there is overlap
                    childShipLogEntryDisplay.ZIndex++;

                    Entries[childEntry.id] = childShipLogEntryDisplay;
                }

                Entries[entry.id] = shipLogEntryDisplay;
            }
        }
        catch (Exception e)
        {
            GD.PrintErr($"Couldn't load ship log xml at {xmlPath} - {e}");
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        var speed = 1000f;
        var cameraSpeed = 1.5f;

        if (Input.IsActionPressed("Left"))
        {
            this.Position += Vector2.Right * (float)delta * speed;
        }
        if (Input.IsActionPressed("Right"))
        {
            this.Position += Vector2.Left * (float)delta * speed;
        }
        if (Input.IsActionPressed("Up"))
        {
            this.Position += Vector2.Down * (float)delta * speed;
        }
        if (Input.IsActionPressed("Down"))
        {
            this.Position += Vector2.Up * (float)delta * speed;
        }

        var prevScale = Scale;

        void AdjustForScale()
        {
            Position /= prevScale.X / Scale.X;
        }

        if (Input.IsActionPressed("ZoomIn"))
        {
            Scale = Vector2.One * Mathf.Min(Scale.X * (1f + (float)delta * cameraSpeed), 8f);
            AdjustForScale();
        }
        if (Input.IsActionPressed("ZoomOut"))
        {
            Scale = Vector2.One * Mathf.Max(Scale.X * (1f - (float)delta * cameraSpeed), 0.5f);
            AdjustForScale();
        }
    }
}
