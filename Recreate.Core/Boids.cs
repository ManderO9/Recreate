using System.Diagnostics.CodeAnalysis;
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
    /// The velocity of the boids
    /// </summary>
    private Vector2 mBoidVelocity = new Vector2(4);

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

        for(int i = 0; i < 30; i++)
        {
            mBoids.Add(new(new(Random.Shared.Next(ScreenWidth),
                Random.Shared.Next(ScreenHeight)),
                Random.Shared.NextSingle() * 2 * Math.PI,
                Color.FromArgb(255, 197, 66, 245)));
        }

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

    public Boid AddBoid(float x, float y, double direction, Color color)
    {
        lock(mBoids)
        {
            var boid = new Boid(new Vector2(x, y), direction, color);
            mBoids.Add(boid);
            return boid;
        }
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


        var longDistance = 60;
        var shortDistance = 20;

        var longDistanceAttractionRate = 0.2;

        lock(mBoids)
        {
            foreach(var boid in mBoids)
            {
                var neighbors = mBoids.Where(x => x != boid && Vector2.Distance(boid.Position, x.Position) < longDistance);

                if(neighbors.Count() > 0)
                {
                    boid.HeadingAngle = neighbors.Average(x => x.HeadingAngle % (2 * Math.PI));

                    var longRangeNeighbors = neighbors.Where(x => Vector2.Distance(boid.Position, x.Position) > shortDistance);

                    if(longRangeNeighbors.Count() > 0)
                    {
                        var positionX = neighbors.Average(x => x.Position.X);
                        var positionY = neighbors.Average(x => x.Position.Y);

                        double alpha = Math.Atan((boid.Position.Y - positionY) / (boid.Position.X - positionX));
                        boid.HeadingAngle = alpha * longDistanceAttractionRate + (1 - longDistanceAttractionRate) * boid.HeadingAngle;
                    }


                    var shortRangeNeighbors = neighbors.Where(x => Vector2.Distance(boid.Position, x.Position) < shortDistance);

                    if(shortRangeNeighbors.Count() > 0)
                    {
                        boid.HeadingAngle = shortRangeNeighbors.Average(x => x.HeadingAngle % (2 * Math.PI)) + (Random.Shared.NextSingle() - 0.5) * 0.1;
                    }
                }

                // Add randomness to movement
                boid.HeadingAngle += (Random.Shared.NextSingle() - 0.5) * 0.03;

            }

            // For each boid
            foreach(var boid in mBoids)
            {
                // Get new position after applying velocity
                var newPosition = new Vector2(
                    (float)(boid.Position.X + mBoidVelocity.X * Math.Cos(boid.HeadingAngle)),
                    (float)(boid.Position.Y + mBoidVelocity.Y * Math.Sin(boid.HeadingAngle)));

                // Limit the boid within screen bounds
                if(newPosition.Y > ScreenHeight) newPosition.Y = 0;
                if(newPosition.Y < 0) newPosition.Y = ScreenHeight;
                if(newPosition.X < 0) newPosition.X = ScreenWidth;
                if(newPosition.X > ScreenWidth) newPosition.X = 0;

                // Set new position of boid
                boid.Position = newPosition;
            }
        }
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
