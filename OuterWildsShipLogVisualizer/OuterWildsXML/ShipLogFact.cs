using System.Xml.Linq;

namespace OuterWildsShipLogVisualizer.OuterWildsXML;

public class ShipLogFact
{
    public string entryName;

    public string entryID;

    public string sourceID;

    public string entryRumorName;

    public int entryRumorNamePriority;

    public string text;

    public bool rumor;

    public bool ignoreMoreToExplore;

    public XElement altTextNode;

    public string altText;

    // public SlideCollection slideCollection;

    public bool startedTextReveal;

    public float startRevealTime;

    public ShipLogFact(string entryID, bool rumor, XElement factNode, string entryName)
    {
        this.entryID = entryID;
        this.rumor = rumor;
        this.entryName = entryName;
        string value = factNode.Element("ID").Value;
        XElement xelement = factNode.Element("Text");
        this.text = ((xelement != null) ? xelement.Value : "");
        this.altTextNode = factNode.Element("AltText");
        XElement xelement2 = factNode.Element("SourceID");
        this.sourceID = ((xelement2 != null) ? xelement2.Value : "");
        XElement xelement3 = factNode.Element("RumorName");
        this.entryRumorName = ((xelement3 != null) ? xelement3.Value : "");
        this.entryRumorNamePriority = -1;
        if (this.entryRumorName.Length > 0)
        {
            XElement xelement4 = factNode.Element("RumorNamePriority");
            this.entryRumorNamePriority = ((xelement4 != null) ? int.Parse(xelement4.Value) : 0);
        }
        this.ignoreMoreToExplore = (factNode.Element("IgnoreMoreToExplore") != null);
    }
}
