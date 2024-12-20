﻿using Godot;
using Newtonsoft.Json;
using OuterWildsShipLogVisualizer.External;
using OuterWildsShipLogVisualizer.OuterWildsXML;
using System.Collections.Generic;
using System.Linq;

namespace OuterWildsShipLogVisualizer;

public static class StockSystemLoader
{
    public static void Load()
    {
        // We are deserializing the data from Outer Wilds Ventures (https://github.com/nottldr/outer-wilds-ventures) and converting it to our format 
        // Comparing it to base game it seems its actually wrong, but close enough and I'm lazy!
        using var file = FileAccess.Open("res://assets/stockShiplogs.json", FileAccess.ModeFlags.Read);
        var stockShipLogInfos = JsonConvert.DeserializeObject<StockShipLogInfo[]>(file.GetAsText());

        // We can define the curiosity colours in advance
        var curiosities = new Curiosity[]
        {
            new Curiosity() { id = "TIME_LOOP", color = new MColor(180, 118, 74), highlightColor = new MColor(255, 167, 104)},
            new Curiosity() { id = "QUANTUM_MOON", color = new MColor(79, 58, 255), highlightColor = new MColor(144, 104, 255)},
            new Curiosity() { id = "VESSEL", color = new MColor(180, 74, 74), highlightColor = new MColor(255, 104, 104)},
            new Curiosity() { id = "INVISIBLE_PLANET", color = new MColor(29, 74, 148), highlightColor = new MColor(50, 128, 255)},
            new Curiosity() { id = "SUNKEN_MODULE", color = new MColor(56, 138, 93), highlightColor = new MColor(104, 255, 172)},
        };

        // We read the entry positions (for the StarSystemConfig) and the regular entries from the Outer Wilds Ventures data
        var entryPositions = new List<EntryPosition>();
        var entries = new Dictionary<string, ShipLogEntry>();
        foreach (var stockShipLog in stockShipLogInfos)
        {
            entryPositions.Add(new EntryPosition() 
            { 
                id = stockShipLog.id, position = new MVector2(stockShipLog.position[0], stockShipLog.position[1]) 
            });

            entries[stockShipLog.id] = new ShipLogEntry()
            {
               id = stockShipLog.id,
               parentID = stockShipLog.parent,
               astroObjectID = stockShipLog.astroObject,
               name = stockShipLog.name,
               curiosity = stockShipLog.curiosity,
               isCuriosity = stockShipLog.isCuriosity,
               rumorFacts = stockShipLog.facts.rumor.Select(x => x.ToShipLogFact()).ToList(),
               exploreFacts = stockShipLog.facts.explore.Select(x => x.ToShipLogFact()).ToList(),
            };
        }

        // We loop through the entries a second time to update the childEntries
        foreach (var entry in entries.Values)
        {
            if (!string.IsNullOrEmpty(entry.parentID))
            {
                entries[entry.parentID].childEntries ??= new();
                entries[entry.parentID].childEntries.Add(entry);
            }
        }

        var solarSystemConfig = new StarSystemConfig()
        {
            curiosities = curiosities,
            entryPositions = entryPositions.ToArray()
        };

        // The only purpose of the ShipLogModule here is to point out where the sprites are
        var shipLogModule = new ShipLogModule() { spriteFolder = "res://assets/stockShiplogSprites" };

        // We add all the entries to the ShipLogsRoot of the visualizer
        foreach (var entry in entries.Values)
        {
            var shipLogEntryDisplay = ShipLogsRoot.Instance.ShipLogEntryScene.Instantiate<ShipLogEntryDisplay>();
            ShipLogsRoot.Instance.AddChild(shipLogEntryDisplay);
            shipLogEntryDisplay.SetShipLogEntry(string.Empty, shipLogModule, entry, solarSystemConfig);
            ShipLogsRoot.Instance.Entries[entry.id] = shipLogEntryDisplay;
        }
    }

    private class StockShipLogInfo
    {
        [JsonProperty]
        public string id;

        [JsonProperty]
        public string name;

        [JsonProperty]
        public string astroObject;

        [JsonProperty]
        public float[] position;

        [JsonProperty]
        public string curiosity;

        [JsonProperty]
        public bool isCuriosity;

        [JsonProperty]
        public StockShipLogFacts facts;

        [JsonProperty]
        public string parent;
    }

    private class StockShipLogFacts
    {
        [JsonProperty]
        public StockShipLogExploreFact[] explore;

        [JsonProperty]
        public StockShipLogRumorFact[] rumor;
    }

    private class StockShipLogExploreFact
    {
        [JsonProperty]
        public string id;

        public ShipLogFact ToShipLogFact()
        {
            return new ShipLogFact(id, false);
        }
    }

    private class StockShipLogRumorFact
    {
        [JsonProperty]
        public string id;

        [JsonProperty]
        public string sourceId;

        public ShipLogFact ToShipLogFact()
        {
            return new ShipLogFact(id, true) { sourceID = sourceId };
        }
    }
}


