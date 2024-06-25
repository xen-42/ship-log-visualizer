using Newtonsoft.Json;
using OuterWildsShipLogVisualizer.External;

namespace OuterWildsShipLogVisualizer;

public class StarSystemConfig
{
    [JsonProperty]
    public Curiosity[] curiosities;

    [JsonProperty]
    public EntryPosition[] entryPositions;
}
