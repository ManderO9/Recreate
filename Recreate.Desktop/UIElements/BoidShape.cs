using Recreate.Core;
using System.Collections.Generic;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Recreate.Desktop;

/// <summary>
/// Represents a shape that will get drawn on the canvas for a boid
/// </summary>
public partial class BoidShape
{

    /// <summary>
    /// The boid that we want to display
    /// </summary>
    public Boid ReferenceBoid { get; }

    public Polygon Polygon { get; }

    private float mSize;


    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="size">The size of the boid</param>
    /// <param name="color">The color of the boid</param>
    /// <param name="referenceBoid">The boid that we are drawing</param>
    public BoidShape(double size, System.Drawing.Color color, Boid referenceBoid)
    {
        // Set private members
        ReferenceBoid = referenceBoid;

        // Create a new UI Element to represent the boid in the canvas
        Polygon = new Polygon();

        mSize = (float)size;

        var bird = new Bird(
            new Vector2((float)referenceBoid.XPosition, (float)referenceBoid.YPosition),
            mSize,
            referenceBoid.HeadingAngle);

        Polygon.Points.Add(new Point(bird.HeadPoint.X, bird.HeadPoint.Y));
        Polygon.Points.Add(new Point(bird.LeftWingPoint.X, bird.LeftWingPoint.Y));
        Polygon.Points.Add(new Point(bird.RightWingPoint.X, bird.RightWingPoint.Y));

        // Set its color
        Polygon.Fill = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
    }

    /// <summary>
    /// Updates the position of this shape according to the boid current position
    /// </summary>
    public void UpdatePosition()
    {

        var bird = new Bird(
            new Vector2((float)ReferenceBoid.XPosition, (float)ReferenceBoid.YPosition),
            mSize,
            ReferenceBoid.HeadingAngle);


        Polygon.Points.Clear();

        Polygon.Points.Add(new Point(bird.HeadPoint.X, bird.HeadPoint.Y));
        Polygon.Points.Add(new Point(bird.LeftWingPoint.X, bird.LeftWingPoint.Y));
        Polygon.Points.Add(new Point(bird.StomachPoint.X, bird.StomachPoint.Y));
        Polygon.Points.Add(new Point(bird.RightWingPoint.X, bird.RightWingPoint.Y));

        // Set the circle position
        Canvas.SetLeft(Polygon, ReferenceBoid.XPosition);
        Canvas.SetTop(Polygon, ReferenceBoid.YPosition);
    }

}
