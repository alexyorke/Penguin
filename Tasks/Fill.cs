// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Fill.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Fill type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace PenguinSdk.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using EEPhysics;

    using PlayerIOClient;

    /// <summary>
    ///     The fill.
    /// </summary>
    public class Fill : Task
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Fill"/> class.
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
        public Fill(Tokenizer tokenizer, ISelection selection, int blockID, int originX, int originY)
            : base(tokenizer, selection)
        {
            this.BlockID = blockID;
            this.OriginX = originX;
            this.OriginY = originY;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the block id.
        /// </summary>
        public int BlockID { get; set; }

        /// <summary>
        /// Origin X point
        /// </summary>
        public int OriginX { get; set; }

        /// <summary>
        /// Origin Y point
        /// </summary>
        public int OriginY { get; set; }

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
                    int layer = this.BlockID >= 500 ? 1 : 0;

                    PenguinMap map = this.Selection.GetMap();

                    var pos = new PenguinVector(
                        OriginX,
                        OriginY,
                        layer);
                    int sourceID = map.GetBlockAt(pos).ID;

                    var total = new List<PenguinVector>();
                    var queue = new Queue<PenguinVector>();
                    queue.Enqueue(pos);
                    total.Add(pos);

                    while (queue.Count > 0)
                    {
                        PenguinVector cur = queue.Dequeue();
                        IEnumerator<PenguinVector> adj = this.GetAdjacent(cur).GetEnumerator();

                        while (adj.MoveNext())
                        {
                            PenguinBlock adjBlock = map.GetBlockAt(adj.Current);
                            if (adjBlock != null)
                            {
                                if (adjBlock.ID == sourceID)
                                {
                                    bool contains = false;
                                    for (int i = 0; i < total.Count; i++)
                                    {
                                        if (total[i].Equals(adj.Current))
                                        {
                                            contains = true;
                                            break;
                                        }
                                    }

                                    if (!contains)
                                    {
                                        queue.Enqueue(adj.Current);
                                        total.Add(adj.Current);
                                    }
                                }
                            }
                        }

                        connection.Send(map.WorldKey, cur.Z, cur.X, cur.Y, this.BlockID);
                        Thread.Sleep(speed);
                    }

                    this.OnTaskCompleted(this);
                };
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get adjacent.
        /// </summary>
        /// <param name="loc">
        /// The location.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        private IEnumerable<PenguinVector> GetAdjacent(PenguinVector loc)
        {
            yield return new PenguinVector(loc.X + 1, loc.Y, loc.Z);
            yield return new PenguinVector(loc.X - 1, loc.Y, loc.Z);
            yield return new PenguinVector(loc.X, loc.Y + 1, loc.Z);
            yield return new PenguinVector(loc.X, loc.Y - 1, loc.Z);
        }

        #endregion
    }
}