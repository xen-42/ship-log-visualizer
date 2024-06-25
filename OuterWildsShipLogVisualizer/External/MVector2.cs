using Godot;
using Newtonsoft.Json;

namespace OuterWildsShipLogVisualizer.External;

public class MVector2
{
    [JsonProperty]
    public int x;

    [JsonProperty]
    public int y;

    public MVector2 InvertY() => new() { x = x, y = -y };
    public Vector2 ToVector2() => new(x, y);
}
