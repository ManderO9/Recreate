using System.Diagnostics;
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
            mBoids.Add(new(new(
                Random.Shared.Next(ScreenWidth),
                Random.Shared.Next(ScreenHeight)),
                Random.Shared.NextSingle() * 2 * Math.PI,
                Color.FromArgb(255, 197, 66, 245)));
        }

        //mBoids.Add(new(new(ScreenWidth / 2, ScreenHeight / 2), -Math.PI / 2, Color.FromArgb(255, 197, 66, 245)));

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
        var boid = new Boid(new Vector2(x, y), direction, color);

        lock(mBoids)
        {
            mBoids.Add(boid);
        }

        return boid;
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



        lock(mBoids)
        {
            //var longDistance = 60;
            //var shortDistance = 20;

            //var longDistanceAttractionRate = 0.2;

            //foreach(var boid in mBoids)
            //{
            //    var neighbors = mBoids.Where(x => x != boid && Vector2.Distance(boid.Position, x.Position) < longDistance);

            //    if(neighbors.Count() > 0)
            //    {
            //        boid.HeadingAngle = neighbors.Average(x => x.HeadingAngle % (2 * Math.PI));

            //        var longRangeNeighbors = neighbors.Where(x => Vector2.Distance(boid.Position, x.Position) > shortDistance);

            //        if(longRangeNeighbors.Count() > 0)
            //        {
            //            var positionX = neighbors.Average(x => x.Position.X);
            //            var positionY = neighbors.Average(x => x.Position.Y);

            //            double alpha = Math.Atan((boid.Position.Y - positionY) / (boid.Position.X - positionX));
            //            boid.HeadingAngle = alpha * longDistanceAttractionRate + (1 - longDistanceAttractionRate) * boid.HeadingAngle;
            //        }


            //        var shortRangeNeighbors = neighbors.Where(x => Vector2.Distance(boid.Position, x.Position) < shortDistance);

            //        if(shortRangeNeighbors.Count() > 0)
            //        {
            //            boid.HeadingAngle = shortRangeNeighbors.Average(x => x.HeadingAngle % (2 * Math.PI)) + (Random.Shared.NextSingle() - 0.5) * 0.1;
            //        }
            //    }

            //    // Add randomness to movement
            //    boid.HeadingAngle += (Random.Shared.NextSingle() - 0.5) * 0.03;

            //}

            // For each boid
            foreach(var boid in mBoids)
            {
                // The distance from the screen edge before we turn
                var offset = 30;

                // The angle at which we will turn each frame
                var additionValue = 0.3;

                // Get the direction we are heading to modulus 2 PI
                var heading = boid.HeadingAngle % (2 * Math.PI);

                // Whether we are heading to the right
                var headingRight = (heading >= -Math.PI / 2d && heading <= Math.PI / 2d) || heading <= -3d / 2d * Math.PI || heading >= 3d / 2d * Math.PI;

                // Whether we are heading to the left
                var headingLeft = (heading > Math.PI / 2d && heading < Math.PI * 3d / 2d) || (heading > -3d / 2d * Math.PI && heading < -Math.PI / 2d);

                // Whether we are heading to the bottom
                var headingDown = (heading >= 0 && heading <= Math.PI) || (heading >= -2d * Math.PI && heading <= -Math.PI);

                // Whether we are heading to the top
                var headingUp = (heading < 0 && heading > -Math.PI) || (heading > Math.PI && heading < 2d * Math.PI);

                // If we are close to the right edge and heading right
                if(boid.Position.X + offset >= ScreenWidth && headingRight)
                {    // If we are heading to the bottom
                    if(headingDown)
                        // Turn to the bottom
                        boid.HeadingAngle += additionValue;

                    // Otherwise...
                    else
                        // Turn towards the top
                        boid.HeadingAngle -= additionValue;
                }
                // Otherwise if we are close to the left edge and heading left
                else if(boid.Position.X - offset <= 0 && headingLeft)
                {   // If we are heading to the top
                    if(!headingDown)
                        // Turn toward the top
                        boid.HeadingAngle += additionValue;

                    // Otherwise...
                    else
                        // Turn toward the bottom
                        boid.HeadingAngle -= additionValue;

                }
                // Otherwise if we are close to the bottom edge and heading down
                else if(boid.Position.Y + offset >= ScreenHeight && headingDown)
                {
                    // If we are heading right
                    if(headingRight)
                        // Turn toward the right
                        boid.HeadingAngle -= additionValue;

                    // Otherwise...
                    else
                        // Turn toward the left
                        boid.HeadingAngle += additionValue;
                }
                // Otherwise if we are close to the top edge and heading up
                else if(boid.Position.Y - offset < 0 && headingUp)
                {
                    // If we are heading left
                    if(!headingRight)
                        // Turn toward the left
                        boid.HeadingAngle -= additionValue;
                    
                    // Otherwise...
                    else
                        // Turn to the right
                        boid.HeadingAngle += additionValue;
                }


                // Get new position after applying velocity
                var newPosition = new Vector2(
                    (float)(boid.Position.X + mBoidVelocity.X * Math.Cos(boid.HeadingAngle)),
                    (float)(boid.Position.Y + mBoidVelocity.Y * Math.Sin(boid.HeadingAngle)));


                // TODO: remove the debug breaks after you finish
                // Limit the boid within screen bounds
                if(newPosition.Y > ScreenHeight + 20) Debugger.Break();// newPosition.Y = 0;
                if(newPosition.Y < -20) Debugger.Break();//newPosition.Y = ScreenHeight;
                if(newPosition.X < -20) Debugger.Break();//newPosition.X = ScreenWidth;
                if(newPosition.X > ScreenWidth + 20) Debugger.Break();//newPosition.X = 0;

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
