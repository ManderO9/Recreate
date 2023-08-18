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

                //ScreenWidth / 2,
                //ScreenHeight / 2),
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

    /// <summary>
    /// Adds a boid to the list of boids
    /// </summary>
    /// <param name="x">The x position of the boid</param>
    /// <param name="y">The y position of the boid</param>
    /// <param name="direction">The angle at which the boid is heading</param>
    /// <param name="color">The color of the boid</param>
    /// <returns></returns>
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

    #region Public Properties

    public bool AlignmentEnabled { get; set; } = true;
    public bool LongDistanceAttractionEnabled { get; set; } = true;
    public bool ShortDistanceSeparationEnabled { get; set; } = true;
    public bool EdgeAvoidanceEnabled { get; set; } = true;
    public bool MovementRandomnessEnabled { get; set; } = true;

    #endregion

    #region Private Methods

    /// <summary>
    /// Updates the state of the boids
    /// </summary>
    private void Update()
    {
        double minspeed = 3;
        double maxspeed = 6;
        double visual_range = 40;
        double protected_range_squared = 8*8;
        double visual_range_squared = 40*40;
        double centering_factor = 0.0005;
        double matching_factor = 0.05;
        double avoidfactor = 0.05;
        double turnfactor = 0.2;

        var margin = 200;

        lock(mBoids)

            foreach(var boid in mBoids)
            {

                // Zero all accumulator variables (can't do this in one line in C)
                double xpos_avg = 0,
                    ypos_avg = 0,
                    xvel_avg = 0,
                    yvel_avg = 0,
                    neighboring_boids = 0,
                    close_dx = 0,
                    close_dy = 0;

                // For every other boid in the flock . . .
                foreach(var otherboid in mBoids)
                {
                    if(otherboid == boid)
                        continue;


                    // Compute differences in x and y coordinates
                    var dx = boid.X - otherboid.X;
                    var dy = boid.Y - otherboid.Y;

                    // Are both those differences less than the visual range?
                    if(Math.Abs(dx) < visual_range && Math.Abs(dy) < visual_range)
                    {

                        // If so, calculate the squared distance
                        var squared_distance = dx * dx + dy * dy;

                        // Is squared distance less than the protected range?
                        if(squared_distance < protected_range_squared)
                        {

                            // If so, calculate difference in x/y-coordinates to nearfield boid
                            close_dx += boid.X - otherboid.X;
                            close_dy += boid.Y - otherboid.Y;
                        }
                        // If not in protected range, is the boid in the visual range?
                        else if(squared_distance < visual_range_squared)
                        {

                            // Add other boid's x/y-coord and x/y vel to accumulator variables
                            xpos_avg += otherboid.X;
                            ypos_avg += otherboid.Y;
                            xvel_avg += otherboid.VelocityX;
                            yvel_avg += otherboid.VelocityY;

                            // Increment number of boids within visual range
                            neighboring_boids += 1;
                        }

                    }
                }




                // If there were any boids in the visual range . . .            
                if(neighboring_boids > 0)
                {

                    // Divide accumulator variables by number of boids in visual range
                    xpos_avg = xpos_avg / neighboring_boids;
                    ypos_avg = ypos_avg / neighboring_boids;
                    xvel_avg = xvel_avg / neighboring_boids;
                    yvel_avg = yvel_avg / neighboring_boids;

                    // Add the centering/matching contributions to velocity
                    boid.VelocityX = (boid.VelocityX +
                               (xpos_avg - boid.X) * centering_factor +
                               (xvel_avg - boid.VelocityX) * matching_factor);

                    boid.VelocityY = (boid.VelocityY +
                               (ypos_avg - boid.Y) * centering_factor +
                               (yvel_avg - boid.VelocityY) * matching_factor);
                }

                // Add the avoidance contribution to velocity
                boid.VelocityX = boid.VelocityX + (close_dx * avoidfactor);
                boid.VelocityY = boid.VelocityY + (close_dy * avoidfactor);


                // If the boid is near an edge, make it turn by turnfactor
                // (this describes a box, will vary based on boundary conditions)
                if(boid.Y < margin)
                    boid.VelocityY = boid.VelocityY + turnfactor;
                if(boid.X > ScreenWidth - margin)
                    boid.VelocityX = boid.VelocityX - turnfactor;
                if(boid.X < margin)
                    boid.VelocityX = boid.VelocityX + turnfactor;
                if(boid.Y > ScreenHeight - margin)
                    boid.VelocityY = boid.VelocityY - turnfactor;


                // Calculate the boid's speed
                // Slow step! Lookup the "alpha max plus beta min" algorithm
                var speed = Math.Sqrt(boid.VelocityX * boid.VelocityX + boid.VelocityY * boid.VelocityY);

                // Enforce min and max speeds
                if(speed < minspeed)
                {
                    if(speed == 0)
                    {
                        boid.VelocityX = 1;
                        boid.VelocityY = 1;
                    }
                    else
                    {
                        boid.VelocityX = boid.VelocityX * (minspeed / speed);
                        boid.VelocityY = (boid.VelocityY / speed) * minspeed;
                    }
                }

                if(speed > maxspeed)
                {
                    boid.VelocityX = (boid.VelocityX / speed) * maxspeed;
                    boid.VelocityY = (boid.VelocityY / speed) * maxspeed;
                }

                // Update boid's position
                boid.X = boid.X + boid.VelocityX;
                boid.Y = boid.Y + boid.VelocityY;

                // Rotate the boid to the correct direction
                boid.Rotate(Math.Atan2(boid.VelocityY, boid.VelocityX));


                
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
