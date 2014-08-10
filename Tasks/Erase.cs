using PlayerIOClient;

namespace Penguin.Tasks
{
    public class Erase : ITask
    {
        public int BlockID { get; set; }

        public void Perform(Connection connection)
        {
            
        }

        public Erase(int blockID)
        {
            this.BlockID = blockID;
        }
    }
}
