using System;
using System.Numerics;
using System.Windows.Diagnostics;

namespace Recreate.Desktop;

public struct Bird
{
    /// <summary>
    /// The head of the boid
    /// </summary>
    public Vector2 HeadPoint { get; set; }

    /// <summary>
    /// The left wing of the boid
    /// </summary>
    public Vector2 LeftWingPoint { get; set; }

    /// <summary>
    /// The right wing of the boid
    /// </summary>
    public Vector2 RightWingPoint { get; set; }

    /// <summary>
    /// The point that will create the emptiness in the back of the boid
    /// </summary>
    public Vector2 StomachPoint { get; set; }

    /// <summary>
    /// Returns the center of the boid
    /// </summary>
    /// <returns></returns>
    public Vector2 Center() => (HeadPoint + LeftWingPoint + RightWingPoint) / 3;

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="center">The center point of this boid</param>
    /// <param name="size">The size of this boid</param>
    /// <param name="initialAngle">The angle at which this boid points (0 is pointing right)</param>
    public Bird(Vector2 center, float size, double initialAngle)
    {
        // Set the head of the boid
        HeadPoint = new Vector2(center.X, center.Y + size / 2);
        
        // Set the left wing point
        LeftWingPoint = new Vector2(center.X - size / 3, center.Y - size / 2);
        
        // Set the right wing point
        RightWingPoint = new Vector2(center.X + size / 3, center.Y - size / 2);

        // Calculate the point that is gonna make the emptiness in the boid from the back
        StomachPoint = new Vector2(
            (LeftWingPoint.X + HeadPoint.X + RightWingPoint.X) / 3,
            (LeftWingPoint.Y + HeadPoint.Y + RightWingPoint.Y) / 3);

        // Rotate the boid according to the passed in angle
        Rotate(initialAngle);
    }

    /// <summary>
    /// Rotates the boid according to an angle
    /// </summary>
    /// <param name="angle">The angle that the boid will get rotated by</param>
    public void Rotate(double angle)
    {
        // Make the boid point to the right when the angle is equal to 0
        angle = angle - Math.PI / 2;

        // Get the center of the boid
        var center = Center();

        // Rotate each point in the boid according to its center
        HeadPoint = RotatePoint(HeadPoint, center, angle);
        LeftWingPoint = RotatePoint(LeftWingPoint, center, angle);
        RightWingPoint = RotatePoint(RightWingPoint, center, angle);
        StomachPoint = RotatePoint(StomachPoint, center, angle);
    }

    /// <summary>
    /// Rotates a point according to an angle around a center point
    /// </summary>
    /// <param name="point">The point to rotate</param>
    /// <param name="center">The point that we will rotate around</param>
    /// <param name="angle">The amount that we want to rotate the point (in radians)</param>
    /// <returns></returns>
    private Vector2 RotatePoint(Vector2 point, Vector2 center, double angle)
    {
        // Rotate the point according to the passed in angle around the center point
        return new Vector2(
                (float)(Math.Cos(angle) * (point.X - center.X) - Math.Sin(angle) * (point.Y - center.Y) + center.X),
                (float)(Math.Sin(angle) * (point.X - center.X) + Math.Cos(angle) * (point.Y - center.Y) + center.Y));
    }
}

