// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PenguinPiano.cs" company="">
//   
// </copyright>
// <summary>
//   The penguin piano.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using PlayerIOClient;
namespace PenguinSdk
{
    /// <summary>
    /// The penguin piano.
    /// </summary>
    public class PenguinPiano : PenguinBlock
    {
        #region Constructors and Destructors

        public override void Build(PenguinMap map, Connection c)
        {
            c.Send(map.WorldKey,
                Z,
                X,
                Y,
                ID,
                Note
            );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PenguinPiano"/> class.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <param name="note">
        /// The note.
        /// </param>
        public PenguinPiano(int x, int y, int note)
            : base(x, y, 0, PenguinIds.Action.Music.Piano, 0)
        {
            this.Note = note;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the note.
        /// </summary>
        public int Note { get; set; }

        #endregion
    }
}