using EEPhysics;

using System.Collections.Generic;

namespace PenguinSdk.Tasks
{
    using System.Threading;

    using PlayerIOClient;

    /// <summary>
    /// The cancel task.
    /// </summary>
    public class Cancel : Task
    {
        /// <summary>
        /// The perform.
        /// </summary>
        /// <param name="caller">
        /// The caller.
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <param name="speed">
        /// The speed.
        /// </param>
        /// <returns>
        /// The <see cref="ThreadStart"/>.
        /// </returns>
        public override ThreadStart Perform(PhysicsPlayer caller, Connection connection, int speed)
        {
            return delegate()
            {
                List<Task> running = this.tokenizer.GetRunning();

                for (int i = running.Count - 1; i >= 0; i--)
                {
                    //Concurrent modification check
                    if (i < running.Count)
                    {
                        if (!(running[i] is Cancel))
                        {
                            running[i].StopTask();
                            break;
                        }
                    }
                }

                this.OnTaskCompleted(this);
            };           
        }

        /// <summary>
        /// Gets the block list.
        /// </summary>
        /// <param name="caller">
        /// The caller.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        public override void GetBlockList(PhysicsPlayer caller, BlocklistCalculated callback)
        {
            callback(new List<PenguinBlock>());
        }

        /// <summary>
        /// Queues the cancel as an async task
        /// </summary>
        public override void Queue(PhysicsPlayer caller, Connection connection, int speed)
        {
            start = Perform(caller, connection, speed);
            tokenizer.QueueTask(this, true);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cancel"/> class.
        /// </summary>
        /// <param name="tokenizer">
        /// The tokenizer.
        /// </param>
        /// <param name="selection">
        /// The selection.
        /// </param>
        public Cancel(Tokenizer tokenizer, ISelection selection)
            : base(tokenizer, selection)
        {

        }
    }
}
