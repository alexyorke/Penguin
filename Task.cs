// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Task.cs" company="">
//   
// </copyright>
// <summary>
//   The blocklist calculated.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PenguinSdk
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using EEPhysics;

    using PlayerIOClient;

    /// <summary>
    /// The blocklist calculated.
    /// </summary>
    /// <param name="blocks">
    /// The blocks.
    /// </param>
    public delegate void BlocklistCalculated(List<PenguinBlock> blocks);

    /// <summary>
    /// The task completed.
    /// </summary>
    public delegate void TaskCompleted(Task task);

    /// <summary>
    ///     The task.
    /// </summary>
    public abstract class Task
    {
        #region Fields

        /// <summary>
        ///     When task is finished in ee
        /// </summary>
        public TaskCompleted OnTaskCompleted;

        /// <summary>
        ///     Map save point before task began
        /// </summary>
        public PenguinBlock[,,] SavePoint;

        /// <summary>
        /// The start.
        /// </summary>
        protected ThreadStart start;

        /// <summary>
        /// The thread.
        /// </summary>
        protected Thread thread;

        /// <summary>
        /// The tokenizer.
        /// </summary>
        protected Tokenizer tokenizer;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Task"/> class. 
        /// Initializes a new instance of the <see cref="Task"/>class.
        /// </summary>
        /// <param name="tokenizer">
        /// The tokenizer.
        /// </param>
        /// <param name="selection">
        /// The selection.
        /// </param>
        public Task(Tokenizer tokenizer, ISelection selection)
        {
            this.tokenizer = tokenizer;
            this.Selection = selection;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Is the task running
        /// </summary>
        public bool Running
        {
            get
            {
                return this.thread == null ? false : this.thread.IsAlive;
            }
        }

        /// <summary>
        ///     Gets the task's selection area
        /// </summary>
        public ISelection Selection { get; protected set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Queue a task in the thread pool
        /// </summary>
        /// <param name="action">
        /// Function to queue
        /// </param>
        public static void Run(Action action)
        {
            System.Threading.Tasks.Task.Factory.StartNew(action);
        }

        /// <summary>
        /// Gets a list of blocks to be changed
        /// </summary>
        /// <param name="caller">
        /// The caller.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        public abstract void GetBlockList(PhysicsPlayer caller, BlocklistCalculated callback);

        /// <summary>
        /// Performs the task using a ee connection
        /// </summary>
        /// <param name="caller">
        /// The caller.
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <param name="speed">
        /// Speed to perform in ms
        /// </param>
        /// <returns>
        /// The <see cref="ThreadStart"/>.
        /// </returns>
        public abstract ThreadStart Perform(PhysicsPlayer caller, Connection connection, int speed);

        /// <summary>
        /// Queues the task to be run
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
        public virtual void Queue(PhysicsPlayer caller, Connection connection, int speed)
        {
            this.start = this.Perform(caller, connection, speed);
            this.tokenizer.QueueTask(this, false);
        }

        /// <summary>
        ///     Runs the task
        /// </summary>
        public void Start()
        {
            if (this.start == null)
            {
                throw new PenguinException("Do not call Task.Start() directly, please use Task.Queue()");
            }

            this.SavePoint = this.Selection.GetMap().GetCopy();
            if (!this.Running)
            {
                this.thread = new Thread(this.start);
                this.thread.Start();
            }
            else
            {
                throw new PenguinException("Task already running.");
            }
        }

        /// <summary>
        ///     Stops the task
        /// </summary>
        public void StopTask()
        {
            if (this.Running)
            {
                try
                {
                    this.thread.Abort();
                }
                catch
                {
                }

                OnTaskCompleted(this);
            }
        }

        #endregion
    }
}