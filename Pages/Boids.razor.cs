using Microsoft.JSInterop;
using System.Drawing;
using System.Numerics;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Recreate.Pages;

public partial class Boids
{
    #region Consts

    /// <summary>
    /// The width of the canvas in pixels
    /// </summary>
    const int CanvasWidth = 800;

    /// <summary>
    /// The height of the canvas in pixels
    /// </summary>
    const int CanvasHeight = 600;

    #endregion

    #region Private Members

    /// <summary>
    /// Javascript module that will have the function that will draw the boids into the canvas
    /// </summary>
    IJSObjectReference mJSModule = default!;

    /// <summary>
    /// The timer that will run a function every time an interval elapses
    /// </summary>
    private System.Timers.Timer? mTimer;

    /// <summary>
    /// The list of boids to work with
    /// </summary>
    private List<Boid> mBoids = new();

    #endregion

    #region Lifecycle Methods

    protected override async Task OnInitializedAsync()
    {
        // When the element gets initialized


        mBoids.Add(new Boid()
        {
            Color = Color.FromArgb(255, 255, 0, 255),
            XPosition = 50,
            YPosition = 50,
        });
        mBoids.Add(new Boid()
        {
            Color = Color.FromArgb(255, 255, 255, 0),
            XPosition = 150,
            YPosition = 150,
        });
        mBoids.Add(new Boid()
        {
            Color = Color.FromArgb(255, 255, 0, 0),
            XPosition = 350,
            YPosition = 350,
        });
        mBoids.Add(new Boid()
        {
            Color = Color.FromArgb(255, 0, 255, 0),
            XPosition = 450,
            YPosition = 450,
        });

        mBoids.AddRange(new Boid[]{
            new()
            {
                Color = Color.FromArgb(255, 155, 255, 0),
                XPosition = 250,
                YPosition = 450,
            },
            new()
            {
                Color = Color.FromArgb(255, 0, 55, 0),
                XPosition = 150,
                YPosition = 350,
            },
            new()
            {
                Color = Color.FromArgb(255, 255, 255, 0),
                XPosition = 150,
                YPosition = 200,
            },
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
            await Draw();
        };

        // Load the javascript module that will have the drawing method
        mJSModule = await jsRuntime.InvokeAsync<IJSObjectReference>("import", "./Pages/Boids.razor.js");

        // Start the timer
        mTimer.Start();
    }

    #endregion

    #region Private Methods


    private void Update()
    {
        // Rules for flocking

        // Separation
        //    Avoid crowding neighbors(short range repulsion)
        // Alignment
        //    Steer towards average heading of neighbors
        // Cohesion
        //    Steer towards average position of neighbors(long range attraction)


        foreach(var boid in mBoids)
        {
            var otherBoids = mBoids.Where(x => x != boid);
            var averageX= otherBoids.Average(x=>x.XPosition);
            var averageY= otherBoids.Average(x=>x.YPosition);

            boid.HeadingAngle = Math.Atan2(boid.YPosition - averageY, boid.XPosition - averageX);
        }

        foreach (var boid in mBoids)
        {
            boid.XPosition =(uint) (boid.XPosition + Math.Cos(-boid.HeadingAngle) * 2);
            boid.YPosition = (uint)(boid.YPosition + Math.Sin(-boid.HeadingAngle) * 2);
        }



    }

    private async Task Draw()
    {
        // Call the javascript method passing in the pixels array
        await mJSModule.InvokeVoidAsync("drawImage", mBoids);
    }

    #endregion

    #region IDisposable Implementation

    ValueTask IAsyncDisposable.DisposeAsync()
    {
        // Stop the timer
        mTimer?.Stop();

        // Clear it's resources
        mTimer?.Close();
        mTimer = null;

        // Clear js module resources
        return mJSModule.DisposeAsync();
    }

    #endregion

}