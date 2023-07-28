using Recreate.Core;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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

    #endregion

    #region Constructor

    public MainWindow()
    {
        InitializeComponent();

        // Set width and height of the canvas
        canvas.Width = Boids.ScreenWidth;
        canvas.Height = Boids.ScreenHeight;

        // Set canvas background
        canvas.Background = new SolidColorBrush(Color.FromArgb(255,44, 44, 44));

        // Initialize the game
        mBoidsRuntime.Initialize((boids) =>
        {
            // Run the draw method on UI Thread
            Application.Current.Dispatcher.Invoke(() => Draw(boids));
            
            // Return result
            return Task.CompletedTask;
        });
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
                var shape = new BoidShape(15, boid.Color, boid);

                // Add it to the list of shapes
                mBoidShapes.Add(shape);

                // Set it's initial position
                shape.UpdatePosition();

                // Add the shape to the canvas
                canvas.Children.Add(shape.Polygon);
            }
        }
        // Otherwise...
        else
        {
            // For each shape in the boid shapes
            foreach(var shape in mBoidShapes)
            {
                // Update it's position
                shape.UpdatePosition();
            }
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
