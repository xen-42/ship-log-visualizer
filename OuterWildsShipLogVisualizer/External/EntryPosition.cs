using Newtonsoft.Json;

namespace OuterWildsShipLogVisualizer.External;

public class EntryPosition
{
    [JsonProperty]
    public string id;

    [JsonProperty]
    public MVector2 position;
}
