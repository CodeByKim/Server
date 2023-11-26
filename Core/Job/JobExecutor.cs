using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Core.Job
{
    public class JobExecutor
    {
        private ConcurrentQueue<Action> _jobQueue;
        private PriorityQueue<Action, DateTime> _scheduleJobQueue;
        private AutoResetEvent _autoResetEvent;
        private Thread _thread;     //스레드를 개별로 가지고 있는건 안좋은데

        public JobExecutor()
        {
            _jobQueue = new ConcurrentQueue<Action>();
            _scheduleJobQueue = new PriorityQueue<Action, DateTime>();
            _autoResetEvent = new AutoResetEvent(false);
        }

        public void PushJob(Action job)
        {
            _jobQueue.Enqueue(job);
        }

        public void PushJob(TimeSpan startTime, Action job)
        {
            PushJob(
                () =>
                {
                    _scheduleJobQueue.Enqueue(job, DateTime.Now + startTime);

                    _autoResetEvent.Set();
                });
        }

        internal virtual void Initialize()
        {
            _thread = new Thread(ExecuteScheduleJob);
            _thread.IsBackground = true;

            _thread.Start();
        }

        protected void ExecuteJob()
        {
            var jobCount = _jobQueue.Count;

            for (var i = 0; i < jobCount; i++)
            {
                Action job;
                if (!_jobQueue.TryDequeue(out job))
                    return;

                job();
            }
        }

        private void ExecuteScheduleJob()
        {
            TimeSpan tolerance = TimeSpan.FromMilliseconds(10);

            while (true)
            {
                Action? action;
                DateTime taskStartTime;

                if (!_scheduleJobQueue.TryPeek(out action, out taskStartTime))
                {
                    _autoResetEvent.WaitOne();
                    continue;
                }

                var diff = taskStartTime - DateTime.Now;
                if (diff < tolerance)
                {
                    var task = _scheduleJobQueue.Dequeue();
                    PushJob(task);
                }
                else
                {
                    _autoResetEvent.WaitOne(diff);
                }
            }
        }
    }
}
