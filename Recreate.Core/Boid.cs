using System.Drawing;
using System.Numerics;

namespace Recreate.Core;

public class Boid
{
    #region Private Members

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

    /// <summary>
    /// The x velocity of the boid
    /// </summary>
    public double VelocityX { get; set; }

    /// <summary>
    /// The y velocity of the boid
    /// </summary>
    public double VelocityY { get; set; }

    /// <summary>
    /// The current x position of the boid according to the screen
    /// </summary>
    public double X { get; set; }

    /// <summary>
    /// The current y position of the boid according to the screen
    /// </summary>
    public double Y { get; set; }

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
        X = position.X;
        Y = position.Y;

        // Set initial direction of the boid
        VelocityX = Math.Cos(initialAngle);
        VelocityY = Math.Sin(initialAngle);

        // Initiate properties
        HeadPoint = mHead;
        LeftWingPoint = mLeftWing;
        RightWingPoint = mRightWing;
        StomachPoint = mStomach;

        // Rotate the boid according to the passed in angle
        Rotate(initialAngle);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Rotates the boid according to an angle
    /// </summary>
    /// <param name="angle">The angle that the boid will get rotated by</param>
    public void Rotate(double angle)
    {
        // Make the boid point to the right when the angle is equal to 0
        angle = angle - Math.PI / 2;

        // Rotate each point in the boid according to its center
        HeadPoint = RotatePoint(mHead, mCenter, angle);
        LeftWingPoint = RotatePoint(mLeftWing, mCenter, angle);
        RightWingPoint = RotatePoint(mRightWing, mCenter, angle);
        StomachPoint = RotatePoint(mStomach, mCenter, angle);
    }

    #endregion

    #region Private Helpers

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
