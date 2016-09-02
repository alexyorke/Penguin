// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PenguinText.cs" company="">
//   
// </copyright>
// <summary>
//   The penguin text.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using PlayerIOClient;
namespace PenguinSdk
{
    /// <summary>
    /// The penguin text.
    /// </summary>
    public class PenguinText : PenguinBlock
    {
        #region Constructors and Destructors

        public override void Build(PenguinMap map, Connection c)
        {
            c.Send(map.WorldKey,
                Z,
                X,
                Y,
                ID,
                Text
            );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PenguinText"/> class.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        public PenguinText(int x, int y, string text)
            : base(x, y, 0, PenguinIds.Action.Sign.Textsign, 0)
        {
            this.Text = text;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text { get; set; }

        #endregion
    }
}