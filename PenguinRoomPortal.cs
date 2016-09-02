// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PenguinRoomPortal.cs" company="">
//   
// </copyright>
// <summary>
//   The penguin room portal.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using PlayerIOClient;
namespace PenguinSdk
{
    /// <summary>
    /// The penguin room portal.
    /// </summary>
    public class PenguinRoomPortal : PenguinBlock
    {
        #region Constructors and Destructors

        public override void Build(PenguinMap map, Connection c)
        {
            c.Send(map.WorldKey,
                Z,
                X,
                Y,
                ID,
                DestinationID
            );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PenguinRoomPortal"/> class.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <param name="destinationID">
        /// The destination id.
        /// </param>
        public PenguinRoomPortal(int x, int y, string destinationID)
            : base(x, y, 0, 374, 0)
        {
            this.DestinationID = destinationID;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the destination id.
        /// </summary>
        public string DestinationID { get; set; }

        #endregion
    }
}