using System;
using System.Numerics;

namespace Recreate.Desktop;

public struct Bird
{
    public Vector2 HeadPoint { get; set; }
    public Vector2 LeftWingPoint { get; set; }
    public Vector2 RightWingPoint { get; set; }

    public Vector2 StomachPoint { get; set; }

    public Vector2 Center() => (HeadPoint + LeftWingPoint + RightWingPoint) / 3;

    public Bird(Vector2 center, float size, double initialAngle)
    {
        HeadPoint = new Vector2(center.X, center.Y + size / 2);
        LeftWingPoint = new Vector2(center.X - size / 3, center.Y - size / 2);
        RightWingPoint = new Vector2(center.X + size / 3, center.Y - size / 2);
        StomachPoint = center;

        Rotate(initialAngle);
    }

    public void Rotate(double angle)
    {
        var center = Center();
        HeadPoint = RotatePoint(HeadPoint, center, angle);
        LeftWingPoint = RotatePoint(LeftWingPoint, center, angle);
        RightWingPoint = RotatePoint(RightWingPoint, center, angle);
        StomachPoint = RotatePoint(StomachPoint, center, angle);

    }


    private Vector2 RotatePoint(Vector2 point, Vector2 center, double angle)
    {
        return new Vector2(
                (float)(Math.Cos(angle) * (point.X - center.X) - Math.Sin(angle) * (point.Y - center.Y) + center.X),
                (float)(Math.Sin(angle) * (point.X - center.X) + Math.Cos(angle) * (point.Y - center.Y) + center.Y));
    }



}

