using System.Collections.Generic;

namespace PenguinSdk
{
    /// <summary>
    /// TaskQueue class for managing penguin tasks
    /// </summary>
    public class TaskQueue
    {
        /// <summary>
        /// The running tasks.
        /// </summary>
        private List<Task> runningTasks;

        /// <summary>
        /// Finished tasks
        /// </summary>
        private List<Task> finishedTasks;

        /// <summary>
        /// The queued tasks.
        /// </summary>
        private Queue<Task> queued;

        /// <summary>
        /// The main event queue.
        /// </summary>
        /// <param name="task">
        /// The task.
        /// </param>
        public void Queue(Task task, bool runAysnc)
        {
            queued.Enqueue(task);

            if (runningTasks.Count == 0 || runAysnc)
            {                
                RunNext();
            }
        }

        /// <summary>
        /// The on completion event. Fires when the task is completed and starts the next one.
        /// </summary>
        private void OnCompletion(Task task)
        {
            finishedTasks.Add(task);
            for (int i = 0; i < runningTasks.Count; i++)
            {
                if (runningTasks[i].Equals(task))
                {
                    runningTasks.RemoveAt(i);
                    break;
                }
            }

            RunNext();
        }

        /// <summary>
        /// Runs the next task in the queue.
        /// </summary>
        public void RunNext()
        {
            if (queued.Count > 0)
            {
                Task task = queued.Dequeue();
                runningTasks.Add(task);
                task.OnTaskCompleted += OnCompletion;
                task.Start();
            }
        }

        /// <summary>
        /// Gets the last running task
        /// </summary>
        public List<Task> GetRunning()
        {
            return runningTasks;
        }

        /// <summary>
        /// Gets the last finished task
        /// </summary>
        public List<Task> GetFinished()
        {
            return finishedTasks;
        }

        /// <summary>
        /// Creates a new task queue
        /// </summary>
        public TaskQueue()
        {
            queued = new Queue<Task>();
            runningTasks = new List<Task>();
            finishedTasks = new List<Task>();
        }
    }
}
