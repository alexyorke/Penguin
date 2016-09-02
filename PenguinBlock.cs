// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PenguinBlock.cs" company="">
//   
// </copyright>
// <summary>
//   The penguin block.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using PlayerIOClient;
namespace PenguinSdk
{
    /// <summary>
    /// The penguin block.
    /// </summary>
    public class PenguinBlock
    {
        #region Fields

        /// <summary>
        /// The location.
        /// </summary>
        private PenguinVector location;

        #endregion

        public virtual void Build(PenguinMap map, Connection c)
        {
            c.Send(map.WorldKey,
                Z,
                X,
                Y,
                ID,
                Rotation
            );
        }

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PenguinBlock"/> class.
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
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="rotation">
        /// The rotation.
        /// </param>
        public PenguinBlock(int x, int y, int z, int id, int rotation)
        {
            this.location = new PenguinVector(x, y, z);
            this.ID = id;
            this.Rotation = rotation;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        public PenguinVector Location
        {
            get
            {
                return this.location;
            }

            set
            {
                this.location = value;
                this.X = this.location.X;
                this.Y = this.location.Y;
                this.Z = this.location.Z;
            }
        }

        /// <summary>
        /// Gets or sets the rotation.
        /// </summary>
        public int Rotation { get; set; }

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        public int X
        {
            get
            {
                return this.location.X;
            }

            set
            {
                this.location.X = value;
            }
        }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        public int Y
        {
            get
            {
                return this.location.Y;
            }

            set
            {
                this.location.Y = value;
            }
        }

        /// <summary>
        /// Gets or sets the z.
        /// </summary>
        public int Z
        {
            get
            {
                return this.location.Z;
            }

            set
            {
                this.location.Z = value;
            }
        }

        #endregion
    }
}