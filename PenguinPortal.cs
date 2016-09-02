// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PenguinPortal.cs" company="">
//   
// </copyright>
// <summary>
//   The penguin portal.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using PlayerIOClient;namespace PenguinSdk
{
    /// <summary>
    /// The penguin portal.
    /// </summary>
    public class PenguinPortal : PenguinBlock
    {
        #region Constructors and Destructors

        public override void Build(PenguinMap map, Connection c)
        {
            c.Send(map.WorldKey,
                Z,
                X,
                Y,
                ID,
                Rotation,
                PortalID,
                DestinationID
            );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PenguinPortal"/> class.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <param name="rotation">
        /// The rotation.
        /// </param>
        /// <param name="portalId">
        /// The portal id.
        /// </param>
        /// <param name="destinationId">
        /// The destination id.
        /// </param>
        /// <param name="isVisible">
        /// The is visible.
        /// </param>
        public PenguinPortal(int x, int y, int rotation, int portalId, int destinationId, bool isVisible)
            : base(x, y, 0, isVisible ? 242 : 381, rotation)
        {
            this.PortalID = portalId;
            this.DestinationID = destinationId;
            this.IsVisible = isVisible;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the destination id.
        /// </summary>
        public int DestinationID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is visible.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Gets or sets the portal id.
        /// </summary>
        public int PortalID { get; set; }

        #endregion
    }
}