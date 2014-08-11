// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Fill.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Fill type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



namespace Penguin.Tasks
{
    using System.Collections.Generic;

    using PlayerIOClient;

    /// <summary>
    /// The fill.
    /// </summary>
    public class Fill : ITask
    {
        /// <summary>
        /// Gets or sets the block id.
        /// </summary>
        public int BlockID { get; set; }

        /// <summary>
        /// The get adjacent.
        /// </summary>
        /// <param name="loc">
        /// The loc.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        private IEnumerable<PenguinBlock> GetAdjacent(PenguinVector loc)
        {
            return null;
        }

        /// <summary>
        /// Gets the selection.
        /// </summary>
        public ISelection Selection { get; private set; }

        /// <summary>
        /// The perform.
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        public void Perform(Connection connection)
        {
            
        }

        /// <summary>
        /// The get block list.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<PenguinBlock> GetBlockList()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Fill"/> class.
        /// </summary>
        /// <param name="selection">
        /// The selection.
        /// </param>
        /// <param name="blockID">
        /// The block id.
        /// </param>
        public Fill(ISelection selection, int blockID)
        {
            BlockID = blockID;
        }
    }
}
