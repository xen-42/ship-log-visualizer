using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace OuterWildsShipLogVisualizer.OuterWildsXML;

public class ShipLogEntry
{
    public string id;
    public string parentID;
    public string astroObjectID;
    public string name;
    public string altPhotoConditionID;
    public string curiosity;
    public bool isCuriosity;
    public bool ignoreMoreToExplore;
    public bool parentIgnoreNotRevealed;
    public string ignoreMoreToExploreCondition;
    public bool extraLargeMoreToExploreIcon;
    public List<ShipLogFact> rumorFacts;
    public List<ShipLogFact> exploreFacts;
    public List<ShipLogFact> completionFacts;

    [NonSerialized]
    public List<ShipLogEntry> childEntries;

    public ShipLogEntry(string astroObjectID, XElement entryNode, string parentID = "")
    {
        this.astroObjectID = astroObjectID;
        this.completionFacts = new List<ShipLogFact>();
        this.id = entryNode.Element("ID").Value;
        this.parentID = parentID;
        this.name = entryNode.Element("Name").Value;
        if (this.id == "IPDREAMLAKE")
        {
            this.extraLargeMoreToExploreIcon = true;
        }
        this.ignoreMoreToExplore = (entryNode.Element("IgnoreMoreToExplore") != null);
        this.parentIgnoreNotRevealed = (entryNode.Element("ParentIgnoreNotRevealed") != null);
        this.isCuriosity = (entryNode.Element("IsCuriosity") != null);
        XElement xelement = entryNode.Element("IgnoreMoreToExploreCondition");
        this.ignoreMoreToExploreCondition = ((xelement != null) ? xelement.Value : string.Empty);
        XElement xelement2 = entryNode.Element("AltPhotoCondition");
        this.altPhotoConditionID = ((xelement2 != null) ? xelement2.Value : string.Empty);
        XElement xelement3 = entryNode.Element("Curiosity");
        this.curiosity = (xelement3 != null) ? xelement3.Value : "None";
        IEnumerable<XElement> enumerable = entryNode.Elements("RumorFact");
        this.rumorFacts = new List<ShipLogFact>();
        foreach (XElement factNode in enumerable)
        {
            ShipLogFact shipLogFact = new(this.id, true, factNode, this.name);
            this.rumorFacts.Add(shipLogFact);
        }
        IEnumerable<XElement> enumerable2 = entryNode.Elements("ExploreFact");
        this.exploreFacts = new List<ShipLogFact>();
        foreach (XElement factNode2 in enumerable2)
        {
            ShipLogFact shipLogFact2 = new(this.id, false, factNode2, this.name);
            this.exploreFacts.Add(shipLogFact2);
            this.completionFacts.Add(shipLogFact2);
        }
        IEnumerable<XElement> enumerable3 = entryNode.Elements("Entry");
        this.childEntries = new List<ShipLogEntry>();
        foreach (XElement entryNode2 in enumerable3)
        {
            this.childEntries.Add(new ShipLogEntry(astroObjectID, entryNode2, this.id));
        }
    }
}
