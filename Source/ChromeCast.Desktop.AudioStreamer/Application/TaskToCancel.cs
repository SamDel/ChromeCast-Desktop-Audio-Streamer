using System;
using System.Collections.Generic;
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

        public void Add(Action action)
        {
            if (action == null || taskList == null)
                return;

            lock(taskList)
            {
                var ctsListener = new CancellationTokenSource();
                var task = Task.Factory.StartNew(action, ctsListener.Token);
                taskList.Add(new TaskToCancel { Task = task, TokenSource = ctsListener });

                taskList.RemoveAll(x => x?.Task == null || x.Task.IsCompleted);
            }
        }

        public void Dispose()
        {
            if (taskList == null)
                return;

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
            }
            taskList = null;
        }
    }
}