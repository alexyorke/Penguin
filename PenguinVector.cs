// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PenguinVector.cs" company="">
//   
// </copyright>
// <summary>
//   The penguin vector.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PenguinSdk
{
    /// <summary>
    /// The penguin vector.
    /// </summary>
    public class PenguinVector
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PenguinVector"/> class.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        public PenguinVector(int x, int y)
            : this(x, y, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PenguinVector"/> class.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <param name="z">
        /// The z.
        /// </param>
        public PenguinVector(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets the z.
        /// </summary>
        public int Z { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Equals(PenguinVector vector)
        {
            return vector.X == this.X && vector.Y == this.Y && vector.Z == this.Z;
        }

        #endregion
    }
}