using PlayerIOClient;

namespace Penguin.Tasks
{
    public class Replace : ITask
    {
        public int SourceID { get; set; }

        public int ReplaceID { get; set; }

        public void Perform(Connection connection)
        {
            
        }

        public Replace(int sourceID, int replaceID)
        {
            SourceID = sourceID;
            ReplaceID = replaceID;
        }
    }
}
