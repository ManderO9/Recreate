using Microsoft.JSInterop;
using Recreate.Core;
using System.Runtime.Intrinsics.X86;

namespace Recreate.Web;

public partial class Boids
{

    #region Private Members

    /// <summary>
    /// Javascript module that will have the function that will draw the boids into the canvas
    /// </summary>
    IJSObjectReference mJSModule = default!;

    /// <summary>
    /// The object that will contain all the logic related to boids and will manage game loop
    /// </summary>
    private Core.Boids mBoidsRuntime = new();

    #endregion

    #region Lifecycle Methods

    protected override async Task OnInitializedAsync()
    {
        // Initialize the game
        mBoidsRuntime.Initialize(Draw);

        // Load the javascript module that will have the drawing method
        mJSModule = await jsRuntime.InvokeAsync<IJSObjectReference>("import", "./Pages/Boids.razor.js");
    }

    #endregion

    #region Private Methosd

    private async Task Draw(List<Boid> boids)
    {
        // Call the javascript method passing in the pixels array
        await mJSModule.InvokeVoidAsync("drawImage",
            boids.Select(x =>
            new
            {
                x.Color,
                HeadPointX = x.HeadPoint.X,
                HeadPointY = x.HeadPoint.Y,
                LeftWingPointX = x.LeftWingPoint.X,
                LeftWingPointY = x.LeftWingPoint.Y,
                RightWingPointX = x.RightWingPoint.X,
                RightWingPointY = x.RightWingPoint.Y,
                StomachPointX = x.StomachPoint.X,
                StomachPointY = x.StomachPoint.Y,
                PositionX = x.X,
                PositionY = x.Y,
            }));
    }

    #endregion

    #region IDisposable Implementation

    ValueTask IAsyncDisposable.DisposeAsync()
    {
        // Dispose of resources in the game
        mBoidsRuntime.Dispose();

        // Clear js module resources
        return mJSModule.DisposeAsync();
    }

    #endregion

}