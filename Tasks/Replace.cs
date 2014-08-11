using PlayerIOClient;
using System.Collections.Generic;

namespace Penguin.Tasks
{
    using System.Threading;

    public class Replace : ITask
    {
        public int SourceID { get; set; }

        public int ReplaceID { get; set; }

        private ISelection selection;

        public ISelection Selection
        {
            get { return selection; }
        }

        public IEnumerable<PenguinBlock> GetBlockList()
        {
            throw new System.NotImplementedException();
        }

        public void Perform(Connection connection)
        {
            List<PenguinVector> list = new List<PenguinVector>(selection.GetSelectionArea());
            PenguinMap map = selection.GetMap();

            for (int i = 0; i < list.Count; i++)
            {
                // i think that's right but not sure though
                if (map.GetBlockAt(list[i]).ID == SourceID)
                {
                    connection.Send(map.WorldKey, list[i].X, list[i].Y, ReplaceID);
                    Thread.Sleep(50);
                }
            }
        }

        public Replace(ISelection selection, int sourceID, int replaceID)
        {
            this.selection = selection;
            SourceID = sourceID;
            ReplaceID = replaceID;
        }
    }
}
