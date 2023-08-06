using Recreate.Core;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Recreate.Desktop;

/// <summary>
/// Represents a shape that will get drawn on the canvas for a boid
/// </summary>
public class BoidShape
{

    /// <summary>
    /// The boid that we want to display
    /// </summary>
    public Boid ReferenceBoid { get; }

    /// <summary>
    /// The polygon that will get drawn on screen representing the boid
    /// </summary>
    public Polygon Polygon { get; }


    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="referenceBoid">The boid that we are drawing</param>
    public BoidShape(Boid referenceBoid)
    {
        // Set private members
        ReferenceBoid = referenceBoid;

        // Create a new UI Element to represent the boid in the canvas
        Polygon = new Polygon();

        // Set its color
        Polygon.Fill = new SolidColorBrush(
        Color.FromArgb(referenceBoid.Color.A, referenceBoid.Color.R, referenceBoid.Color.G, referenceBoid.Color.B));
    }

    /// <summary>
    /// Updates the position of this shape according to the boid current position
    /// </summary>
    public void UpdatePosition()
    {

        Polygon.Points.Clear();

        Polygon.Points.Add(new Point(ReferenceBoid.HeadPoint.X, ReferenceBoid.HeadPoint.Y));
        Polygon.Points.Add(new Point(ReferenceBoid.LeftWingPoint.X, ReferenceBoid.LeftWingPoint.Y));
        Polygon.Points.Add(new Point(ReferenceBoid.StomachPoint.X, ReferenceBoid.StomachPoint.Y));
        Polygon.Points.Add(new Point(ReferenceBoid.RightWingPoint.X, ReferenceBoid.RightWingPoint.Y));

        // Set the circle position
        Canvas.SetLeft(Polygon, ReferenceBoid.Position.X);
        Canvas.SetTop(Polygon, ReferenceBoid.Position.Y);
    }

}
