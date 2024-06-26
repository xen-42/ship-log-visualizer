using Godot;
using Newtonsoft.Json;

namespace OuterWildsShipLogVisualizer.External;

public class MVector2
{
    [JsonProperty]
    public float x;

    [JsonProperty]
    public float y;

    public MVector2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public MVector2 InvertY() => new(x, -y);
    public Vector2 ToVector2() => new(x, y);
}
