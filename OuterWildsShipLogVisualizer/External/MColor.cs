using Godot;
using Newtonsoft.Json;

namespace OuterWildsShipLogVisualizer.External;

public class MColor
{
    [JsonProperty]
    public int r;
    [JsonProperty]
    public int g;
    [JsonProperty]
    public int b;
    [JsonProperty]
    public int a;

    public Color ToColor() => new(r / 255f, g / 255f, b / 255f, a / 255f);
}
