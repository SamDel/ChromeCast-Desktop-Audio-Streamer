using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChromeCast.Desktop.AudioStreamer.Application
{
    public class TaskToCancel
    {
        public Task Task { get; set; }
        public CancellationTokenSource TokenSource { get; set; }
    }

    public class TasksToCancel
    {
        private List<TaskToCancel> taskList = new List<TaskToCancel>();
        private bool IsDisposed = false;

        public void Add(Action action, CancellationTokenSource cancellationTokenSource = null)
        {
            if (action == null || taskList == null || IsDisposed)
                return;

            lock(taskList)
            {
                if (cancellationTokenSource == null)
                {
                    cancellationTokenSource = new CancellationTokenSource();
                }
                var task = Task.Factory.StartNew(action, cancellationTokenSource.Token);
                taskList.Add(new TaskToCancel { Task = task, TokenSource = cancellationTokenSource });

                taskList.RemoveAll(x => x?.Task == null || x.Task.IsCompleted);
            }
        }

        public void Dispose()
        {
            if (taskList == null)
                return;

            IsDisposed = true;
            lock(taskList)
            {
                foreach (var item in taskList)
                {
                    if (item.Task != null)
                    {
                        if (!item.Task.IsCompleted)
                        {
                            item.TokenSource.Cancel();
                            item.TokenSource.Dispose();
                            if (item.Task.Status == TaskStatus.RanToCompletion
                                || item.Task.Status == TaskStatus.Canceled
                                || item.Task.Status == TaskStatus.Faulted)
                            {
                                item.Task.Dispose();
                            }
                        }
                    }
                }
                taskList.RemoveAll(x => x?.Task == null || x.Task.IsCompleted);
            }
            Task.WaitAll(taskList.Select(x => x.Task).ToArray(), 10000);
            taskList.RemoveAll(x => x?.Task == null || x.Task.IsCompleted);
            taskList = null;
        }
    }
}