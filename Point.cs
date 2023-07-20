namespace Recreate
{
    /// <summary>
    /// Represents a point that will be drawn on the screen
    /// </summary>
    public class Point
    {
        #region Public Properties

        /// <summary>
        /// The x position of the point
        /// </summary>
        public uint X { get; set; }

        /// <summary>
        /// The y position of the point
        /// </summary>
        public uint Y { get; set; }

        /// <summary>
        /// Red color value
        /// </summary>
        public byte R { get; set; }

        /// <summary>
        /// Green color value
        /// </summary>
        public byte G { get; set; }

        /// <summary>
        /// Blue color value
        /// </summary>
        public byte B { get; set; }

        /// <summary>
        /// Opacity value
        /// </summary>
        public byte A { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="x">The x position of the point</param>
        /// <param name="y">The y position of the point</param>
        /// <param name="r">Red color value</param>
        /// <param name="g">Green color value</param>
        /// <param name="b">Blue color value</param>
        /// <param name="a">Opacity value</param>
        public Point(uint x, uint y, byte r, byte g, byte b, byte a)
            => (X, Y, R, G, B, A) = (x, y, r, g, b, a);

        #endregion
    }
}