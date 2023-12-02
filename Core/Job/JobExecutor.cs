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

        public JobExecutor()
        {
            _jobQueue = new ConcurrentQueue<Action>();
            _scheduleJobQueue = new PriorityQueue<Action, DateTime>();
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
                });
        }

        internal virtual void Initialize()
        {
        }

        protected void FlushJob()
        {
            ExecuteJob();

            ExecuteScheduleJob();
        }

        private void ExecuteJob()
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
            var threshold = TimeSpan.FromMilliseconds(10);
            var curTime = DateTime.Now;

            while (true)
            {
                Action action;
                DateTime taskStartTime;

                if (!_scheduleJobQueue.TryPeek(out action, out taskStartTime))
                    return;

                var diffTime = taskStartTime - curTime;
                if (diffTime < threshold)
                {
                    var task = _scheduleJobQueue.Dequeue();
                    PushJob(task);
                }
                else
                {
                    return;
                }
            }
        }
    }
}
