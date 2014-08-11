using PlayerIOClient;
using System.Collections.Generic;

namespace Penguin
{
    public interface ITask
    {
        /// <summary>
        /// Gets the task's selection area
        /// </summary>
        ISelection Selection { get; }

        /// <summary>
        /// Performs the task using a ee connection
        /// </summary>
        void Perform(Connection connection);

        /// <summary>
        /// Gets a list of blocks to be changed
        /// </summary>
        /// <returns></returns>
        IEnumerable<PenguinBlock> GetBlockList();
    }
}
