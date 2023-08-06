using Recreate.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Recreate.Desktop;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, IDisposable
{
    #region Private Members

    /// <summary>
    /// The object that will contain all the logic related to boids and will manage game loop
    /// </summary>
    private Boids mBoidsRuntime = new();

    /// <summary>
    /// List of boids to display on the canvas
    /// </summary>
    private List<BoidShape>? mBoidShapes;

    /// <summary>
    /// A list containing older values of boid position to add a trail to the boids
    /// </summary>
    private Queue<List<Polygon>> mOlderBoids = new();

    #endregion

    #region Constructor

    public MainWindow()
    {
        InitializeComponent();

        // Set width and height of the canvas
        canvas.Width = Boids.ScreenWidth;
        canvas.Height = Boids.ScreenHeight;

        // Set canvas background
        canvas.Background = new SolidColorBrush(Color.FromArgb(255, 44, 44, 44));

        // Initialize the game
        mBoidsRuntime.Initialize((boids) =>
        {
            // Run the draw method on UI Thread
            Application.Current.Dispatcher.Invoke(() => Draw(boids));

            // Return result
            return Task.CompletedTask;
        });

        canvas.MouseDown += (sender, eventArgs) =>
        {
            // Get mouse position
            var mousePosition = Mouse.GetPosition(canvas);

            // Create a new boid at that position
            var boid = mBoidsRuntime.AddBoid(
                 (float)mousePosition.X,
                 (float)mousePosition.Y,
                 Random.Shared.NextSingle() * 2 * Math.PI,
                 System.Drawing.Color.FromArgb(255,
                     Random.Shared.Next(256),
                     Random.Shared.Next(256),
                     Random.Shared.Next(256)));

            // Create the shape representing this boid
            var shape = new BoidShape(boid);

            // Add the shape to the list of drawable shapes
            mBoidShapes!.Add(shape);

            // Add the shape to the canvas
            canvas.Children.Add(shape.Polygon);
        };

    }

    #endregion

    #region Private Methosd

    /// <summary>
    /// Draws the boids into the screen
    /// </summary>
    /// <param name="boids">The boid to draw</param>
    private void Draw(List<Boid> boids)
    {
        // If the list of boid shapes is null
        if(mBoidShapes is null)
        {
            // Create a new one
            mBoidShapes = new();

            // For each boid 
            foreach(var boid in boids)
            {
                // Create a new shape
                var shape = new BoidShape(boid);

                // Add it to the list of shapes
                mBoidShapes.Add(shape);

                // Add the shape to the canvas
                canvas.Children.Add(shape.Polygon);
            }
        }
        // Otherwise...
        else
        {
            // Create a list that will contain the current values of boid position to add the to 
            // the older boids list, that will be used to create the trail behind boids
            var list = new List<Polygon>();

            // For each shape in the boid shapes
            foreach(var shape in mBoidShapes)
            {
                // Update it's position
                shape.UpdatePosition();

                // Create a new polygon using the current points of the boid
                var newShape = new Polygon()
                {
                    Fill = new SolidColorBrush(Color.FromArgb(
                        shape.ReferenceBoid.Color.A,
                        shape.ReferenceBoid.Color.R,
                        shape.ReferenceBoid.Color.G,
                        shape.ReferenceBoid.Color.B)),
                    Points = new(shape.Polygon.Points)
                };

                // Add the new polygon to the list of shapes
                list.Add(newShape);

                // Set it's position
                Canvas.SetLeft(newShape, shape.ReferenceBoid.Position.X);
                Canvas.SetTop(newShape, shape.ReferenceBoid.Position.Y);

                // Add it to the canvas
                canvas.Children.Add(newShape);
            }

            // Add the current values of boid positions to the list of old values
            mOlderBoids.Enqueue(list);

            // If the count of old values is greater than 5
            if(mOlderBoids.Count > 5)
            {
                // Remove the first element
                var toDelete = mOlderBoids.Dequeue();

                // For each item in the list to delete
                foreach(var element in toDelete)
                {
                    // Remove it from the canvas
                    canvas.Children.Remove(element);
                }
            }

            // For each old value of the boids positions
            foreach(var olderBoids in mOlderBoids)
                foreach(var item in olderBoids)
                    // Give it less opacity
                    item.Fill.Opacity -= 0.3;
        }
    }

    #endregion

    #region IDisposable Implementation

    public void Dispose()
    {
        // Dispose of resources in the game
        mBoidsRuntime.Dispose();
    }

    #endregion
}
