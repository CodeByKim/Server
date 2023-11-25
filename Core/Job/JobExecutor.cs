using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Core.Job
{
    public class JobExecutor
    {
        private ConcurrentQueue<Action> _jobQueue;

        public JobExecutor()
        {
            _jobQueue = new ConcurrentQueue<Action>();
        }

        public void PushJob(Action job)
        {
            _jobQueue.Enqueue(job);
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
    }
}
