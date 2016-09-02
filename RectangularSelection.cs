// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RectangularSelection.cs" company="">
//   
// </copyright>
// <summary>
//   The rectangular selection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PenguinSdk
{
    using System.Collections.Generic;

    /// <summary>
    /// The rectangular selection.
    /// </summary>
    public class RectangularSelection : ISelection
    {
        #region Fields

        /// <summary>
        /// The map.
        /// </summary>
        private readonly PenguinMap map;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RectangularSelection"/> class.
        /// </summary>
        /// <param name="map">
        /// The map.
        /// </param>
        public RectangularSelection(PenguinMap map)
            : this(map, 0, 0, map.Width, map.Height)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RectangularSelection"/> class.
        /// </summary>
        /// <param name="map">
        /// The map.
        /// </param>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        public RectangularSelection(PenguinMap map, int x, int y, int width, int height)
        {
            this.map = map;
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Rectangle's height
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        ///     Rectangle's width
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        ///     Rectangle's x coordinate
        /// </summary>
        public int X { get; set; }

        /// <summary>
        ///     Rectangle's y coordinate
        /// </summary>
        public int Y { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The contains.
        /// </summary>
        /// <param name="loc">
        /// The loc.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Contains(PenguinVector loc)
        {
            return loc.X >= this.X && loc.X < this.X + this.Width && loc.Y >= this.Y && loc.Y < this.Y + this.Height;
        }

        /// <summary>
        /// The get map.
        /// </summary>
        /// <returns>
        /// The <see cref="PenguinMap"/>.
        /// </returns>
        public PenguinMap GetMap()
        {
            return this.map;
        }

        /// <summary>
        /// The get selection area.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<PenguinVector> GetSelectionArea()
        {
            var selection = new List<PenguinVector>();
            for (int z = 0; z < 2; z++)
            {
                for (int y = this.Y; y < this.Y + this.Height; y++)
                {
                    for (int x = this.X; x < this.X + this.Width; x++)
                    {
                        selection.Add(new PenguinVector(x, y, z));
                    }
                }
            }

            return selection;
        }

        #endregion
    }
}