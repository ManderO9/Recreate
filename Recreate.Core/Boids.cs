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

        // Add a couple of boids
        for(int i = 0; i < 10; i++)
        {
            mBoids.Add(new(new(
                Random.Shared.Next(ScreenWidth),
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

    /// <summary>
    /// Deletes all the boids and resets the canvas to clean state
    /// </summary>
    public void Reset()
    {
        // Lock the list
        lock(mBoids)
        {
            // Clear it
            mBoids.Clear();
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Updates the state of the boids
    /// </summary>
    private void Update()
    {
        // This code was taken from here:
        // https://vanhunteradams.com/Pico/Animal_Movement/Boids-algorithm.html
        // Check the website for more explanations of what it does

        // The minimum speed a boid can have
        const double minSpeed = 3;

        // The maximum speed a boid can have
        const double maxSpeed = 6;

        // The range at which boids can interact with each other
        const double visualRange = 40;

        // The range at which the boids are too close and should separate from each other
        const double protectedRangeSquared = 8 * 8;

        // The square of the visual range
        const double visualRangeSquared = visualRange * visualRange;

        // The factor at which boids will head toward the center of their neighbors
        const double centeringFactor = 0.0005;

        // The factor at which boids will match the heading direction of neighboring boids
        const double matchingFactor = 0.05;

        // The factor at which boids will avoid colliding with each other
        const double avoidFactor = 0.05;

        // The factor at which boids will move away from the edges of the screen
        const double turnFactor = 0.2;

        // The margin from the edges of the screen at which boids will start moving away from the edge
        const double margin = 150;

        // Lock the boids list
        lock(mBoids)
        {
            // For each boid
            foreach(var boid in mBoids)
            {
                // Zero all accumulator variables
                double xPositionAvg = 0,
                    yPositionAvg = 0,
                    xVelocityAvg = 0,
                    yVelocityAvg = 0,
                    neighboringBoids = 0,
                    closeDx = 0,
                    closeDy = 0;

                // For every other boid in the flock ...
                foreach(var otherBoid in mBoids)
                {
                    // If the boids is the same as the current one
                    if(otherBoid == boid)
                        // Skip it
                        continue;

                    // Compute differences in x and y coordinates between the two boids
                    var dx = boid.X - otherBoid.X;
                    var dy = boid.Y - otherBoid.Y;

                    // If both those differences are less than the visual range
                    if(Math.Abs(dx) < visualRange && Math.Abs(dy) < visualRange)
                    {

                        // Calculate the squared distance
                        var squaredDistance = dx * dx + dy * dy;

                        // If the squared distance is less than the protected range
                        if(squaredDistance < protectedRangeSquared)
                        {
                            // Add the close neighbor dx and dy to the accumulators
                            closeDx += dx;
                            closeDy += dy;
                        }
                        // Otherwise, is the boid in the visual range
                        else if(squaredDistance < visualRangeSquared)
                        {
                            // Add other x/y coordinates and velocity to accumulators
                            xPositionAvg += otherBoid.X;
                            yPositionAvg += otherBoid.Y;
                            xVelocityAvg += otherBoid.VelocityX;
                            yVelocityAvg += otherBoid.VelocityY;

                            // Increment number of boids within visual range
                            neighboringBoids += 1;
                        }

                    }
                }

                // If there were any boids in the visual range ...            
                if(neighboringBoids > 0)
                {
                    // Divide accumulator variables by number of boids in visual range
                    xPositionAvg /= neighboringBoids;
                    yPositionAvg /= neighboringBoids;
                    xVelocityAvg /= neighboringBoids;
                    yVelocityAvg /= neighboringBoids;

                    // Add the centering/matching contributions to velocity
                    boid.VelocityX = (boid.VelocityX +
                               (xPositionAvg - boid.X) * centeringFactor +
                               (xVelocityAvg - boid.VelocityX) * matchingFactor);

                    boid.VelocityY = (boid.VelocityY +
                               (yPositionAvg - boid.Y) * centeringFactor +
                               (yVelocityAvg - boid.VelocityY) * matchingFactor);
                }

                // Add the avoidance contribution to velocity
                boid.VelocityX = boid.VelocityX + (closeDx * avoidFactor);
                boid.VelocityY = boid.VelocityY + (closeDy * avoidFactor);


                // If the boid is near the top edge
                if(boid.Y < margin)
                {
                    // Add the turn factor to the y velocity
                    boid.VelocityY = boid.VelocityY + turnFactor;

                    // If the x velocity is null or negative
                    if(boid.VelocityX <= 0)
                        // Decrease it
                        boid.VelocityX -= 0.1;
                    // Otherwise...
                    else
                        // Increase it
                        boid.VelocityX += 0.1;

                }
                // If the boid is near the right edge
                if(boid.X > ScreenWidth - margin)
                {
                    // Add the turn factor to the x velocity
                    boid.VelocityX = boid.VelocityX - turnFactor;

                    // If the boid is not near the top or the bottom edge
                    if(!(boid.Y < margin || boid.Y > ScreenHeight - margin))
                        // If the y velocity is null or negative
                        if(boid.VelocityY <= 0)
                            // Decrease it
                            boid.VelocityY -= 0.1;
                        // Otherwise...
                        else
                            // Increase it
                            boid.VelocityY += 0.1;

                }

                // If the boid is near the left edge
                if(boid.X < margin)
                {
                    // Add the turn factor to the x velocity
                    boid.VelocityX = boid.VelocityX + turnFactor;

                    // If the boid is not near the top or the bottom edge
                    if(!(boid.Y < margin || boid.Y > ScreenHeight - margin))
                        // If the y velocity is null or negative
                        if(boid.VelocityY <= 0)
                            // Decrease it
                            boid.VelocityY -= 0.1;
                        // Otherwise...
                        else
                            // Increase it
                            boid.VelocityY += 0.1;

                }

                // If the boid is near the bottom edge
                if(boid.Y > ScreenHeight - margin)
                {
                    // Add the turn factor to the y velocity
                    boid.VelocityY = boid.VelocityY - turnFactor;

                    // If the x velocity is null or negative
                    if(boid.VelocityX <= 0)
                        // Decrease it
                        boid.VelocityX -= 0.1;
                    // Otherwise...
                    else
                        // Increase it
                        boid.VelocityX += 0.1;
                }

                // Calculate the boid's speed
                var speed = Math.Sqrt(boid.VelocityX * boid.VelocityX + boid.VelocityY * boid.VelocityY);

                // Enforce min and max speeds
                if(speed < minSpeed)
                {
                    boid.VelocityX = boid.VelocityX * (minSpeed / speed);
                    boid.VelocityY = (boid.VelocityY / speed) * minSpeed;
                }
                if(speed > maxSpeed)
                {
                    boid.VelocityX = (boid.VelocityX / speed) * maxSpeed;
                    boid.VelocityY = (boid.VelocityY / speed) * maxSpeed;
                }

                // Update boid's position
                boid.X = double.Clamp(boid.X + boid.VelocityX, 0, ScreenWidth);
                boid.Y = double.Clamp(boid.Y + boid.VelocityY, 0, ScreenHeight);

                // Rotate the boid to the correct direction so it will get displayed correctly on the screen
                boid.Rotate(Math.Atan2(boid.VelocityY, boid.VelocityX));
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
