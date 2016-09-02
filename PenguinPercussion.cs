// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PenguinPercussion.cs" company="">
//   
// </copyright>
// <summary>
//   The penguin percussion.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using PlayerIOClient;
namespace PenguinSdk
{
    /// <summary>
    /// The penguin percussion.
    /// </summary>
    public class PenguinPercussion : PenguinBlock
    {
        #region Constructors and Destructors

        public override void Build(PenguinMap map, Connection c)
        {
            c.Send(map.WorldKey,
                Z,
                X,
                Y,
                ID,
                PercussionID
            );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PenguinPercussion"/> class.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <param name="percussionId">
        /// The percussion id.
        /// </param>
        public PenguinPercussion(int x, int y, int percussionId)
            : base(x, y, 0, PenguinIds.Action.Music.Percussion, 0)
        {
            this.PercussionID = percussionId;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the percussion id.
        /// </summary>
        public int PercussionID { get; set; }

        #endregion
    }
}