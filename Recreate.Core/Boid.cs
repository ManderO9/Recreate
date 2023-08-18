using System.Drawing;
using System.Numerics;

namespace Recreate.Core;

public class Boid
{

    public double vx;
    public double vy;
    public double x;
    public double y;

    #region Private Members

    /// <summary>
    /// The angle the boid is heading toward
    /// </summary>
    private double mHeadingAngle = 0;

    /// <summary>
    /// The size of this boid
    /// </summary>
    private const float size = 15;

    /// <summary>
    /// The head of the boid
    /// </summary>
    private Vector2 mHead = new Vector2(0, size / 2);

    /// <summary>
    /// The left wing of the boid
    /// </summary>
    private Vector2 mLeftWing = new Vector2(-size / 3, -size / 2);

    /// <summary>
    /// The right wing of the boid
    /// </summary>
    private Vector2 mRightWing = new Vector2(size / 3, -size / 2);

    /// <summary>
    /// The point that will create the emptiness in the back of the boid
    /// </summary>
    private Vector2 mStomach = new Vector2(0, (-size / 2) / 3);

    /// <summary>
    /// Returns the center of the boid
    /// </summary>
    /// <returns></returns>
    private Vector2 mCenter => (mHead + mLeftWing + mRightWing) / 3;

    #endregion

    #region Public Properties

    /// <summary>
    /// The color of this boid
    /// </summary>
    public Color Color { get; set; }

    ///// <summary>
    ///// The position of this boid
    ///// </summary>
    //public Vector2 Position { get; set; }

    /// <summary>
    /// The angle the boid is heading toward
    /// </summary>
    public double HeadingAngle
    {
        set
        {
            // Rotate the boid the provided value
            Rotate(value);
            mHeadingAngle = value;
        }
        get => mHeadingAngle;
    }

    /// <summary>
    /// The head of the boid
    /// </summary>
    public Vector2 HeadPoint { get; private set; } 

    /// <summary>
    /// The left wing of the boid
    /// </summary>
    public Vector2 LeftWingPoint { get; private set; }

    /// <summary>
    /// The right wing of the boid
    /// </summary>
    public Vector2 RightWingPoint { get; private set; } 

    /// <summary>
    /// The point that will create the emptiness in the back of the boid
    /// </summary>
    public Vector2 StomachPoint { get; private set; }

    #endregion

    #region Constructor

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="position">The center point of this boid</param>
    /// <param name="initialAngle">The angle at which this boid points (0 is pointing right)</param>
    public Boid(Vector2 position, double initialAngle, Color color)
    {
        // Set the color of the boid
        Color = color;

        // Set the position of the boid
        //Position = position;
        x = position.X;
        y = position.Y;


        // Set angle of boid
        HeadingAngle = initialAngle;

        // Initiate properties
        HeadPoint = mHead;
        LeftWingPoint = mLeftWing;
        RightWingPoint = mRightWing;
        StomachPoint = mStomach;

        // Rotate the boid according to the passed in angle
        Rotate(initialAngle);
    }

    #endregion

    #region Private Helpers

    /// <summary>
    /// Rotates the boid according to an angle
    /// </summary>
    /// <param name="angle">The angle that the boid will get rotated by</param>
    private void Rotate(double angle)
    {
        // Make the boid point to the right when the angle is equal to 0
        angle = angle - Math.PI / 2;

        // Rotate each point in the boid according to its center
        HeadPoint = RotatePoint(mHead, mCenter, angle);
        LeftWingPoint = RotatePoint(mLeftWing, mCenter, angle);
        RightWingPoint = RotatePoint(mRightWing, mCenter, angle);
        StomachPoint = RotatePoint(mStomach, mCenter, angle);
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

    #endregion
}
