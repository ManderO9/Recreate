using System.Drawing;
using System.Numerics;

namespace Recreate;

public class Boid
{
    /// <summary>
    /// The color of this boid
    /// </summary>
    public Color Color { get; set; }

    /// <summary>
    /// The x position of the boid
    /// </summary>
    public uint XPosition { get; set; }

    /// <summary>
    /// The y position of the boid
    /// </summary>
    public uint YPosition { get; set; }

    /// <summary>
    /// The angle the boid is heading toward
    /// </summary>
    public double HeadingAngle{ get; set; }
}
