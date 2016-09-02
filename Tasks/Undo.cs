using PlayerIOClient;

namespace PenguinSdk.Tasks
{
    using System.Collections.Generic;

    using EEPhysics;

    using System.Threading;

    public class Undo : Task
    {
        private void SendChanges(Connection connection, PenguinBlock[,,] old, int speed)
        {
            PenguinMap map = Selection.GetMap();

            for (int z = 0; z < 2; z++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    for (int x = 0; x < map.Width; x++)
                    {
                        PenguinBlock cur = map.GetBlockAt(x, y, z);
                        if (old[x, y, z].ID != cur.ID)
                        {
                            connection.Send(map.WorldKey, z, x, y, old[x, y, z].ID);
                            Thread.Sleep(speed);
                        }
                    }
                }
            }
        }

        public override ThreadStart Perform(PhysicsPlayer caller, Connection connection, int speed)
        {
            return delegate()
            {
                List<Task> finished = this.tokenizer.GetFinished();
                if (finished.Count > 0)
                {
                    for (int i = finished.Count - 1; i >= 0; i--)
                    {
                        //Ignore cancel save points
                        if (!(finished[i] is Cancel))
                        {
                            Task last = finished[i];
                            SendChanges(connection, last.SavePoint, speed);
                            break;
                        }
                    }
                }

                this.OnTaskCompleted(this);
            };  
        }

        public override void GetBlockList(PhysicsPlayer caller, BlocklistCalculated callback)
        {

        }

        public Undo(Tokenizer tokenizer, ISelection selection)
            : base(tokenizer, selection)
        {

        }
    }
}
