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
    public List<ShipLogEntryDisplay> Entries { get; private set; } = new();

    public static ShipLogsRoot Instance { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        Instance = this;
    }

    public bool Load(string rootFolder, string starSystem)
    {
        Entries.Clear();
        ShipLogEntryDisplay.ClearCache();
        StarSystems.Clear();

        try
        {
            foreach (var child in this.GetChildren())
            {
                child.QueueFree();
            }

            this.Position = Vector2.Zero;

            var systemsPath = $"{rootFolder}/systems";
            var planetsPath = $"{rootFolder}/planets";

            var planetShipLogModules = new List<ShipLogModule>();

            using var systemDir = DirAccess.Open(systemsPath);
            foreach (var file in systemDir.GetFiles())
            {
                if (!file.EndsWith(".json")) continue;

                using var systemFile = FileAccess.Open(System.IO.Path.Combine(systemsPath, file), FileAccess.ModeFlags.Read);
                try
                {
                    var starSystemName = System.IO.Path.GetFileNameWithoutExtension(file);
                    StarSystems[starSystemName] = (JsonConvert.DeserializeObject<StarSystemConfig>(systemFile.GetAsText()));
                    GD.Print("Loaded star system " + starSystemName);
                }
                catch (Exception e)
                {
                    GD.PrintErr($"Couldn't load star system {file} - {e}");
                }
            }

            // Default to first system if our selection doesn't exist
            if (!StarSystems.ContainsKey(starSystem))
            {
                starSystem = StarSystems.Keys.First();
            }

            using var planetsDir = DirAccess.Open(planetsPath);
            foreach (var file in planetsDir.GetAllFiles())
            {
                if (!file.EndsWith(".json")) continue;

                var path = System.IO.Path.Combine(planetsPath, file);
                using var planetFile = FileAccess.Open(path, FileAccess.ModeFlags.Read);
                try
                {
                    var planetConfig = JsonConvert.DeserializeObject<Dictionary<string, object>>(planetFile.GetAsText());
                    var planetStarSystem = "SolarSystem";
                    if (planetConfig.ContainsKey("starSystem"))
                    {
                        planetStarSystem = planetConfig["starSystem"].ToString();
                    }
                    if (planetStarSystem == starSystem)
                    {
                        planetShipLogModules.Add(JsonConvert.DeserializeObject<ShipLogModule>(planetConfig["ShipLog"].ToString()));
                    }
                }
                catch (Exception e)
                {
                    GD.PrintErr($"Couldn't load ship logs for planet {path} - {e}");
                }
            }

            foreach (var shipLogModule in planetShipLogModules)
            {
                if (string.IsNullOrEmpty(shipLogModule.xmlFile)) continue;

                var path = System.IO.Path.Combine(rootFolder, shipLogModule.xmlFile);
                try
                {
                    using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
                    var xml = new XmlDocument();
                    xml.LoadXml(file.GetAsText());
                    XElement xelement = XElement.Parse(xml.DocumentElement.OuterXml);
                    string astroObjectID = xelement.Element("ID").Value;
                    foreach (XElement entryNode in xelement.Elements("Entry"))
                    {
                        var entry = new ShipLogEntry(astroObjectID, entryNode, "");

                        var shipLogEntryDisplay = ShipLogEntryScene.Instantiate<ShipLogEntryDisplay>();
                        this.AddChild(shipLogEntryDisplay);
                        shipLogEntryDisplay.SetShipLogEntry(rootFolder, shipLogModule, entry, StarSystems[starSystem]);

                        entry.childEntries = new List<ShipLogEntry>();
                        foreach (XElement childEntryXML in entryNode.Elements("Entry"))
                        {
                            var childEntry = new ShipLogEntry(astroObjectID, childEntryXML, entry.id);
                            entry.childEntries.Add(childEntry);

                            var childShipLogEntryDisplay = ShipLogEntryScene.Instantiate<ShipLogEntryDisplay>();
                            this.AddChild(childShipLogEntryDisplay);
                            childShipLogEntryDisplay.SetShipLogEntry(rootFolder, shipLogModule, childEntry, StarSystems[starSystem]);
                            childShipLogEntryDisplay.ZIndex++;

                            Entries.Add(childShipLogEntryDisplay);
                        }

                        Entries.Add(shipLogEntryDisplay);
                    }
                }
                catch (Exception e)
                {
                    GD.PrintErr($"Couldn't load ship log xml at {path} - {e}");
                }
            }

            if (starSystem == "SolarSystem")
            {
                StockSystemLoader.Load();
            }

            foreach (var entry in Entries)
            {
                entry.PostInit();
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

    private void DisplayStockShipLogs()
    {

    }

}
