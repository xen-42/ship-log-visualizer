using Godot;
using System;

public partial class ShipLogLink : Node2D
{
    [Export] private ColorRect _rect;
    [Export] private TextureRect _arrow;

	public void SetExtents(Vector2 start, Vector2 end)
    {
        this.GlobalRotation = (start - end).Angle();
        var length = start.DistanceTo(end);
        _rect.Size = new Vector2(length, _rect.Size.Y);
        _arrow.Position = new Vector2((length + _arrow.Size.X) / 2f, _arrow.Position.Y);
        this.Position = end;
    }
}
