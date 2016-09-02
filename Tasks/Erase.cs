using PlayerIOClient;

namespace PenguinSdk.Tasks
{
    using EEPhysics;
    using System;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// The erase.
    /// </summary>
    public class Erase : Task
    {
        /// <summary>
        /// Gets or sets the block id.
        /// </summary>
        public int BlockID { get; set; }

        /// <summary>
        /// The get block list.
        /// </summary>
        /// <param name="caller">
        /// The caller.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public override void GetBlockList(PhysicsPlayer caller, BlocklistCalculated callback)
        {
            Run(delegate()
            {
                List<PenguinBlock> blockList = new List<PenguinBlock>();

                var list = Selection.GetSelectionArea();
                PenguinMap map = Selection.GetMap();

                for (int i = 0; i < list.Count; i++)
                {
                    PenguinBlock block = map.GetBlockAt(list[i]);
                    if (block.X != 0 && block.X != map.Width - 1 && block.Y != 0 && block.Y != map.Height - 1 && 
                        block.ID == this.BlockID)
                    {
                        blockList.Add(block);
                    }
                }

                callback(blockList);
            });
        }

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
                    var list = this.Selection.GetSelectionArea();
                    PenguinMap map = this.Selection.GetMap();

                    for (int i = 0; i < list.Count; i++)
                    {
                        if (map.GetBlockAt(list[i]).ID == this.BlockID)
                        {
                            connection.Send(map.WorldKey, list[i].Z, list[i].X, list[i].Y, 0); // zero for null block
                            Thread.Sleep(speed);
                        }
                    }

                    this.OnTaskCompleted(this);
                };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Erase"/> class.
        /// </summary>
        /// <param name="tokenizer">
        /// The tokenizer.
        /// </param>
        /// <param name="selection">
        /// The selection.
        /// </param>
        /// <param name="blockID">
        /// The block id.
        /// </param>
        public Erase(Tokenizer tokenizer, ISelection selection, int blockID)
            : base(tokenizer, selection)
        {
            this.BlockID = blockID;
        }
    }
}
