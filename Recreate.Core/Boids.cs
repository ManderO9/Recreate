using System.Drawing;
using System.Numerics;

namespace Recreate.Core;

public class Boids : IDisposable
{
    #region Consts

    /// <summary>
    /// The width of the screen in pixels
    /// </summary>
    public const int ScreenWidth = 800;

    /// <summary>
    /// The height of the screen in pixels
    /// </summary>
    public const int ScreenHeight = 600;

    #endregion

    #region Private Members

    /// <summary>
    /// The timer that will run a function every time an interval elapses
    /// </summary>
    private System.Timers.Timer? mTimer;

    /// <summary>
    /// The list of boids to work with
    /// </summary>
    private List<Boid> mBoids = new();

    /// <summary>
    /// The method that will draw the boids to the screen
    /// </summary>
    private Func<List<Boid>, Task> mDrawImplementation = default!;

    #endregion

    #region Public Methods

    /// <summary>
    /// Initializes the class objects and starts the loop timer
    /// </summary>
    /// <param name="draw">The method that will draw the boids to the screen</param>
    public void Initialize(Func<List<Boid>, Task> draw)
    {
        // When the element gets initialized

        // Set the draw method
        mDrawImplementation = draw;

        mBoids.AddRange(
        new Boid[]
        {
            new (new (50), 0,Color.FromArgb(255, 255, 0, 255)),
            new (new (50, 100), 0,Color.FromArgb(255, 255, 0, 255)),
            //new (new (50, 150), 0,Color.FromArgb(255, 255, 0, 255)),
            //new (new (50, 200), 0,Color.FromArgb(255, 255, 0, 255)),
            //new (new (50, 250), 0,Color.FromArgb(255, 255, 0, 255)),
            //new (new (120, 120), 0, Color.FromArgb(255, 0, 55, 0)),
            //new (new (100,100), 0,Color.FromArgb(255, 155, 255, 0)),
            //new (new (150, 200), Math.PI, Color.FromArgb(255, 255, 255, 0)),
            //new (new (450, 450), Math.PI, Color.FromArgb(255, 0, 255, 0)),
            //new (new (350, 350), Math.PI, Color.FromArgb(255, 255, 0, 0)),
            new (new (150, 150), Math.PI/2, Color.FromArgb(255, 255, 255, 0))
        });

        // Initiate the timer
        mTimer = new();

        // Set the interval to run 30 times per second
        mTimer.Interval = 1000 / 30;

        // Each time the set interval elapses
        mTimer.Elapsed += async (s, e) =>
        {
            // Update game state
            Update();

            // Draw the boids into the screen
            await mDrawImplementation(mBoids);
        };

        // Start the timer
        mTimer.Start();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Updates the state of the boids
    /// </summary>
    private void Update()
    {
        // Rules for flocking

        // Separation
        //    Avoid crowding neighbors(short range repulsion)
        // Alignment
        //    Steer towards average heading of neighbors
        // Cohesion
        //    Steer towards average position of neighbors(long range attraction)





        for(var i = 0; i < mBoids.Count; i++)
        {
            mBoids[i].HeadingAngle += 0.0001;
        }
        //foreach(var boid in mBoids)
        //{
        //    var otherBoids = mBoids.Where(x => x != boid);
        //    var averageX = otherBoids.Average(x => x.XPosition);
        //    var averageY = otherBoids.Average(x => x.YPosition);

        //    boid.HeadingAngle = Math.Atan2(boid.YPosition - averageY, boid.XPosition - averageX);
        //}

        //foreach(var boid in mBoids)
        //{
        //    boid.XPosition = boid.XPosition + Math.Cos(boid.HeadingAngle) * 0.5;
        //    boid.YPosition = boid.YPosition + Math.Sin(boid.HeadingAngle) * 0.5;
        //}



    }


    #endregion

    #region IDisposable Implementation

    public void Dispose()
    {
        // Stop the timer
        mTimer?.Stop();

        // Clear it's resources
        mTimer?.Close();
        mTimer = null;
    }

    #endregion

}
