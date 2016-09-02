// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PenguinCoinGate.cs" company="">
//   
// </copyright>
// <summary>
//   The penguin coin gate.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using PlayerIOClient;
namespace PenguinSdk
{
    /// <summary>
    /// The penguin coin gate.
    /// </summary>
    public class PenguinCoinGate : PenguinBlock
    {
        #region Constructors and Destructors

        public override void Build(PenguinMap map, Connection c)
        {
            c.Send(map.WorldKey,
                Z,
                X,
                Y,
                ID,
                CoinsRequired
            );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PenguinCoinGate"/> class.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <param name="coinsRequired">
        /// The coins required.
        /// </param>
        public PenguinCoinGate(int x, int y, int coinsRequired)
            : base(x, y, 0, PenguinIds.Action.Gates.Coin, 0)
        {
            this.CoinsRequired = coinsRequired;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the coins required.
        /// </summary>
        public int CoinsRequired { get; set; }

        #endregion
    }
}