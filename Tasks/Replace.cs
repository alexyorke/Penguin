// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Replace.cs" company="">
//   
// </copyright>
// <summary>
//   The replace.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PenguinSdk.Tasks
{
    using System.Collections.Generic;
    using System.Threading;

    using EEPhysics;

    using PlayerIOClient;

    /// <summary>
    /// The replace.
    /// </summary>
    public class Replace : Task
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Replace"/> class.
        /// </summary>
        /// <param name="tokenizer">
        /// The tokenizer.
        /// </param>
        /// <param name="selection">
        /// The selection.
        /// </param>
        /// <param name="sourceID">
        /// The source id.
        /// </param>
        /// <param name="replaceID">
        /// The replace id.
        /// </param>
        public Replace(Tokenizer tokenizer, ISelection selection, int sourceID, int replaceID)
            : base(tokenizer, selection)
        {
            this.SourceID = sourceID;
            this.ReplaceID = replaceID;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the replace id.
        /// </summary>
        public int ReplaceID { get; set; }

        /// <summary>
        /// Gets or sets the source id.
        /// </summary>
        public int SourceID { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get block list.
        /// </summary>
        /// <param name="caller">
        /// The caller.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        public override void GetBlockList(PhysicsPlayer caller, BlocklistCalculated callback)
        {
            Run(
                delegate
                    {
                        var blockList = new List<PenguinBlock>();
                        List<PenguinVector> list = this.Selection.GetSelectionArea();
                        PenguinMap map = this.Selection.GetMap();

                        for (int i = 0; i < list.Count; i++)
                        {
                            PenguinBlock block = map.GetBlockAt(list[i]);

                            if (block.ID == this.SourceID)
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
            return delegate
                {
                    List<PenguinVector> list = this.Selection.GetSelectionArea();
                    PenguinMap map = this.Selection.GetMap();

                    for (int i = 0; i < list.Count; i++)
                    {
                        // i think that's right but not sure though
                        if (map.GetBlockAt(list[i]).ID == this.SourceID)
                        {
                            connection.Send(map.WorldKey, list[i].Z, list[i].X, list[i].Y, this.ReplaceID);
                            Thread.Sleep(speed);
                        }
                    }

                    this.OnTaskCompleted(this);
                };
        }

        #endregion
    }
}