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

    public MColor(int r, int g, int b, int a = 255)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    public Color ToColor() => new(r / 255f, g / 255f, b / 255f, a / 255f);
}
