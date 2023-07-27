using Recreate.Core;
using System;
using System.Data.Common;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Recreate.Desktop
{
    /// <summary>
    /// Represents a shape that will get drawn on the canvas for a boid
    /// </summary>
    public class BoidShape
    {
        public Polygon shape { get; set; }
        public Ellipse Ellipse { get; }
        
        /// <summary>
        /// The boid that we want to display
        /// </summary>
        public Boid ReferenceBoid { get; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="radius">The size of the boid</param>
        /// <param name="color">The color of the boid</param>
        /// <param name="referenceBoid">The boid that we are drawing</param>
        public BoidShape(double radius, System.Drawing.Color color, Boid referenceBoid)
        {
            // TODO: change the Ellipse into a Polygon

            // Set private members
            ReferenceBoid = referenceBoid;

            // Create a new UI Element to represent the boid in the canvas
            Ellipse = new Ellipse();
            
            // Set its size
            Ellipse.Width = radius;
            Ellipse.Height = radius;
            
            // Set its color
            Ellipse.Fill = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
        }

        /// <summary>
        /// Updates the position of this shape according to the boid current position
        /// </summary>
        public void UpdatePosition()
        {


            // Set the circle position
            Canvas.SetLeft(Ellipse, ReferenceBoid.XPosition);
            Canvas.SetTop(Ellipse, ReferenceBoid.YPosition);
        }

    }
}
