using PlayerIOClient;

namespace Penguin.Tasks
{
    using System.Collections.Generic;

    public class Undo : ITask
    {
        public ISelection Selection { get; private set; }

        public void Perform(Connection connection)
        {
            
        }

        public IEnumerable<PenguinBlock> GetBlockList()
        {
            throw new System.NotImplementedException();
        }
    }
}
