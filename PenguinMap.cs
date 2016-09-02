// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PenguinMap.cs" company="">
//   
// </copyright>
// <summary>
//   The penguin map.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PenguinSdk
{
    /// <summary>
    /// The penguin map.
    /// </summary>
    public class PenguinMap
    {
        #region Fields

        /// <summary>
        /// The map data.
        /// </summary>
        private readonly PenguinBlock[,,] mapData;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PenguinMap"/> class. 
        /// Creates a new map
        /// </summary>
        /// <param name="width">
        /// Width of map
        /// </param>
        /// <param name="height">
        /// Height of map
        /// </param>
        public PenguinMap(int width, int height)
        {
            this.Width = width;
            this.Height = height;

            this.mapData = new PenguinBlock[width, height, 2];
            for (int z = 0; z < 2; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        this.mapData[x, y, z] = new PenguinBlock(x, y, z, 0, 0);
                    }
                }
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Total height of the world
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        ///     Total width of the world
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        ///     World key used as a token to build blocks in the world
        /// </summary>
        public string WorldKey { get; set; }

        #endregion

        #region Public Indexers

        /// <summary>
        /// The this.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <exception cref="PenguinException">
        /// </exception>
        /// <returns>
        /// The <see cref="PenguinBlock"/>.
        /// </returns>
        public PenguinBlock this[int i]
        {
            get
            {
                int x = i % this.Width;
                int y = i / this.Width;
                int z = i / (this.Width * this.Height);

                if (!this.IsValid(x, y, z))
                {
                    throw new PenguinException("Index out of bounds.");
                }

                return this.mapData[x, y, z];
            }

            set
            {
                int x = i % this.Width;
                int y = i / this.Width;
                int z = i / (this.Width * this.Height);

                if (!this.IsValid(x, y, z))
                {
                    throw new PenguinException("Index out of bounds.");
                }

                this.mapData[x, y, z] = value;
            }
        }

        /// <summary>
        /// The this.
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
        /// <exception cref="PenguinException">
        /// </exception>
        /// <returns>
        /// The <see cref="PenguinBlock"/>.
        /// </returns>
        public PenguinBlock this[int x, int y, int z]
        {
            get
            {
                if (!this.IsValid(x, y, z))
                {
                    throw new PenguinException("Index out of bounds.");
                }

                return this.mapData[x, y, z];
            }

            set
            {
                if (!this.IsValid(x, y, z))
                {
                    throw new PenguinException("Index out of bounds.");
                }

                this.mapData[x, y, z] = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Adds the block to the map
        /// </summary>
        /// <param name="block">
        /// Block to add
        /// </param>
        public void AddBlock(PenguinBlock block)
        {
            this.mapData[block.X, block.Y, block.Z] = block;
        }

        /// <summary>
        /// Gets a block at a specified location
        /// </summary>
        /// <param name="loc">
        /// </param>
        /// <returns>
        /// The <see cref="PenguinBlock"/>.
        /// </returns>
        public PenguinBlock GetBlockAt(PenguinVector loc)
        {
            return this.GetBlockAt(loc.X, loc.Y, loc.Z);
        }

        /// <summary>
        /// The get block at.
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
        /// <returns>
        /// The <see cref="PenguinBlock"/>.
        /// </returns>
        public PenguinBlock GetBlockAt(int x, int y, int z)
        {
            if (!this.IsValid(x, y, z))
            {
                return null;
            }

            return this.mapData[x, y, z];
        }

        /// <summary>
        /// Copies map block data
        /// </summary>
        /// <returns>
        /// The <see cref="PenguinBlock[,,]"/>.
        /// </returns>
        public PenguinBlock[,,] GetCopy()
        {
            return (PenguinBlock[,,])this.mapData.Clone();
        }

        /// <summary>
        /// Calculates if block if within conditional bounds
        /// </summary>
        /// <param name="loc">
        /// Location to test
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsValid(PenguinVector loc)
        {
            return this.IsValid(loc.X, loc.Y, loc.Z);
        }

        /// <summary>
        /// Calculates if
        /// </summary>
        /// <param name="x">
        /// </param>
        /// <param name="y">
        /// </param>
        /// <param name="z">
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsValid(int x, int y, int z)
        {
            return x >= 0 && x < this.Width && y >= 0 && y < this.Height && z >= 0 && z < 2;
        }

        #endregion
    }
}