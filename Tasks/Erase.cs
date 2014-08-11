using PlayerIOClient;

namespace Penguin.Tasks
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The erase.
    /// </summary>
    public class Erase : ITask
    {
        /// <summary>
        /// Gets or sets the block id.
        /// </summary>
        public int BlockID { get; set; }

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
            ISelection selection = new RectangularSelection();

            var list = new List<PenguinVector>(selection.GetSelectionArea());
            PenguinMap map = selection.GetMap();

            for (int i = 0; i < list.Count; i++)
            {
                if (map.GetBlockAt(map[i]).ID == this.BlockID)
                {
                    connection.Send(map.WorldKey, list[i].X, list[i].Y, 0); // zero for null block
                }
            }
        }

        /// <summary>
        /// The get block list.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public IEnumerable<PenguinBlock> GetBlockList()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Erase"/> class.
        /// </summary>
        /// <param name="blockID">
        /// The block id.
        /// </param>
        public Erase(int blockID)
        {
            this.BlockID = blockID;
        }
    }
}
