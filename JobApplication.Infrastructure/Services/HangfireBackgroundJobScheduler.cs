using Hangfire;
using JobApplication.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Infrastructure.Services
{
    public class HangfireBackgroundJobScheduler : IBackgroundJobScheduler
    {
        private readonly IBackgroundJobClient _backgroundJobClient;

        public HangfireBackgroundJobScheduler(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }

        public void Enqueue<T>(Expression<Action<T>> methodCall)
        {
            _backgroundJobClient.Enqueue<T>(methodCall);
        }
        public void Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay)
        {
            _backgroundJobClient.Schedule<T>(methodCall, delay);
        }
    }
}
