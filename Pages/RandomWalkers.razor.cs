using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Recreate.Pages
{
    public partial class RandomWalkers
    {
        #region Consts

        /// <summary>
        /// The width of the canvas in pixels
        /// </summary>
        const int CanvasWidth = 350;

        /// <summary>
        /// The height of the canvas in pixels
        /// </summary>
        const int CanvasHeight = 250;

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

        /// <summary>
        /// Whether the game loop is running or not
        /// </summary>
        private bool mRunning = false;

        /// <summary>
        /// The current mouse position
        /// </summary>
        private (uint X, uint Y) mMousePosition;

        /// <summary>
        /// Whether we are adding points to the list of points or not
        /// </summary>
        private bool mAddingEnabled = false;

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

        #region LifeCycle Methods

        protected override async Task OnInitializedAsync()
        {
            // When the element gets initialized

            // Initiate the timer
            mTimer = new();

            // Set the interval to run 30 times per second
            mTimer.Interval = 1000 / 30;

            // Set default screen pixels
            ClearScreen();

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

            // Set running to true
            mRunning = true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sets the pixels in the screen to the default value
        /// </summary>
        private void ClearScreen()
        {
            // Set the initial value of the pixels to be all dark gray
            for(var i = 0; i < mScreen.Length; i += 4)
            {
                // Set each pixel value
                mScreen[i + 0] = 0X22; // R value
                mScreen[i + 1] = 0X22; // G value
                mScreen[i + 2] = 0X22; // B value
                mScreen[i + 3] = 255; // A value
            }

        }

        /// <summary>
        /// Runs every frame to update the state of the game
        /// </summary>
        private void Update()
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

            // If we are adding points to the list
            if(mAddingEnabled)
            {
                // Add a new one at the current mouse position
                AddPoint(mMousePosition.X, mMousePosition.Y);
            }

        }

        /// <summary>
        /// Draws the pixels array into the canvas
        /// </summary>
        /// <returns></returns>
        private async Task Draw()
        {
            // Call the javascript method passing in the pixels array
            await mJSModule.InvokeVoidAsync("drawImage", mScreen);
        }

        /// <summary>
        /// Stops the running game loop or resumes it accordingly
        /// </summary>
        private void PauseResume()
        {
            // If the game is running
            if(mRunning)
            {
                // Set running to false
                mRunning = false;

                // Stop the timer callbacks
                mTimer?.Stop();
            }
            // Otherwise...
            else
            {
                // Set running to true
                mRunning = true;

                // Start the timer callbacks again
                mTimer?.Start();
            }
        }

        /// <summary>
        /// Starts adding points to the current mouse position
        /// </summary>
        /// <param name="x">The x coordinate of the mouse position</param>
        /// <param name="y">The y coordinate of the mouse position</param>
        private void StartAdding(uint x, uint y)
        {
            // Set flag for adding points to true
            mAddingEnabled = true;

            // Set the current mouse position
            mMousePosition = (x, y);
        }

        /// <summary>
        /// Stops adding points to the screen 
        /// </summary>
        private void StopAdding()
        {
            // Set adding points flag to false
            mAddingEnabled = false;
        }

        /// <summary>
        /// Changes the position of the mouse
        /// </summary>
        /// <param name="x">The x coordinate of the mouse position</param>
        /// <param name="y">The y coordinate of the mouse position</param>
        private void ChangedPosition(uint x, uint y)
        {
            // Change mouse position
            mMousePosition = (x, y);
        }

        /// <summary>
        /// Adds a point to the list of points with a specified positino and random color
        /// </summary>
        /// <param name="x">The x position of the point</param>
        /// <param name="y">The y position of the point</param>
        private void AddPoint(uint x, uint y)
        {
            // Add a point to the list of points
            mPoints.Add(new Point(x, y,
                (byte)Random.Shared.Next(255),
                (byte)Random.Shared.Next(255),
                (byte)Random.Shared.Next(255), 255));
        }

        /// <summary>
        /// Deletes all the points from the list of points and resets the screen into default pixels
        /// </summary>
        /// <returns>A Task representing the asynchronous operation</returns>
        private async Task ClearPoints()
        {
            // Clear the list of points
            mPoints.Clear();

            // Reset screen color
            ClearScreen();

            // If the game is not running
            if(!mRunning)
                // Draw a single frame to reset the canvas
                await Draw();
        }

        #endregion
    }
}