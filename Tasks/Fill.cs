using PlayerIOClient;

namespace Penguin.Tasks
{
    public class Fill : ITask
    {
        public int BlockID { get; set; }

        public void Perform(Connection connection)
        {
            
        }

        public Fill(int blockID)
        {
            BlockID = blockID;
        }
    }
}
