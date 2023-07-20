using Microsoft.JSInterop;
using System.Numerics;

namespace Recreate.Pages
{
    public partial class RandomWalkers
    {
        #region Consts

        /// <summary>
        /// The width of the canvas in pixels
        /// </summary>
        const int CanvasWidth = 700;

        /// <summary>
        /// The height of the canvas in pixels
        /// </summary>
        const int CanvasHeight = 500;

        /// <summary>
        /// The length in bytes of the pixels arrays we display on the canvas
        /// </summary>
        const int ScreenSize = CanvasWidth * CanvasHeight * 4;

        #endregion

        #region Private Members

        /// <summary>
        /// The pixels to display on the canvas, each pixels is represented in RGBA format
        /// </summary>
        private byte[] mScreen = new byte[ScreenSize];

        /// <summary>
        /// The timer that will run a function every time an interval elapses
        /// </summary>
        private System.Timers.Timer? mTimer;

        /// <summary>
        /// Javascript module that will have the function that will draw the pixels into the canvas
        /// </summary>
        IJSObjectReference mJSModule = default!;

        /// <summary>
        /// Create the list of points to display in the screen
        /// </summary>
        private List<Point> mPoints = new List<Point>();

        #endregion

        protected override async Task OnInitializedAsync()
        {
            // When the element gets initialized

            // Initiate the timer
            mTimer = new();

            // Set the interval to run 30 times per second
            mTimer.Interval = 1000 / 30;

            // Set the initial value of the pixels to be all black
            for(var i = 0; i < mScreen.Length; i += 4)
            {
                // Set each pixel value
                mScreen[i + 0] = 0X22; // R value
                mScreen[i + 1] = 0X22; // G value
                mScreen[i + 2] = 0X22; // B value
                mScreen[i + 3] = 255; // A value
            }

            // Each time the set interval elapses
            mTimer.Elapsed += async (s, e) =>
            {
                // Update game state
                Update();

                // Draw the new pixels to the screen
                await Draw();
            };

            // Load the javascript module that will have the drawing method
            mJSModule = await jsRuntime.InvokeAsync<IJSObjectReference>("import", "./Pages/RandomWalkers.razor.js");

            // Start the timer
            mTimer.Start();


            mPoints.Add(new Point(0, 0, 234, 233, 12, 255));
            mPoints.Add(new Point(220, 220, 2, 233, 12, 255));
            mPoints.Add(new Point(032, 023, 4, 233, 12, 255));
            mPoints.Add(new Point(430, 420, 234, 3, 12, 255));
            mPoints.Add(new Point(230, 120, 2, 233, 233, 255));
            mPoints.Add(new Point(230, 0322, 2, 3, 233, 255));



        }

        #region Private Methods

        /// <summary>
        /// Runs every frame to update the state of the game
        /// </summary>
        void Update()
        {
            // For each point in the list of points we display
            for(var i = 0; i < mPoints.Count; i++)
            {
                // TODO: fix this code, it contains bugs


                // Get the point
                var point = mPoints[i];

                // Add a random offset to it's position
                point.X = (uint)((point.X + Random.Shared.Next(-1, 2)) % CanvasWidth);
                point.Y = (uint)((point.Y + Random.Shared.Next(-1, 2)) % CanvasHeight);

                // Calculate the offset in the pixels array to display the pixel in
                var offset = ((point.Y * CanvasWidth + point.X) * 4) % ScreenSize;

                // Display it on the screen
                mScreen[offset] = point.R; // R value
                mScreen[offset + 1] = point.G; // G value
                mScreen[offset + 2] = point.B; // B value
                mScreen[offset + 3] = point.A; // A value
            }

        }

        /// <summary>
        /// Draws the pixels array into the canvas
        /// </summary>
        /// <returns></returns>
        async Task Draw()
        {
            // Call the javascript method passing in the pixels array
            await mJSModule.InvokeVoidAsync("drawImage", mScreen);
        }

        #endregion
    }
}