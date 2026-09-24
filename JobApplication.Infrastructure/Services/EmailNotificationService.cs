using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Infrastructure.Services
{
    public class EmailNotificationService : INotificationService
    {
         
        private readonly IApplicationRepository _jobCandidateApplicationRepository;
        private readonly ILogger<EmailNotificationService> _logger;

        public EmailNotificationService(IApplicationRepository jobCandidateApplicationRepository, ILogger<EmailNotificationService> logger)
        {
            _jobCandidateApplicationRepository = jobCandidateApplicationRepository;
            _logger = logger;
        }

        public void NotifyRecruiter(int applicationId)
        {
            var application = _jobCandidateApplicationRepository.Get().FirstOrDefault(a => a.Id == applicationId);

            if (application is null)
            {
                _logger.LogWarning("Application #{ApplicationId} not found for recruiter notification.", applicationId);
                return;
            }
            _logger.LogInformation("[Email Sent] New Application Notification: Candidate #{CandidateId} has applied to Job #{JobId} (Application #{ApplicationId}).",
                application.CandidateId, application.JobId, applicationId);
        }

        public void NotifyApplicationCancelled(int applicationId)
        {
            var application = _jobCandidateApplicationRepository.Get().FirstOrDefault(a => a.Id == applicationId);

            if (application is null)
            {
                _logger.LogWarning("Application #{ApplicationId} not found for cancellation notification.", applicationId);
                return;
            }
            _logger.LogInformation("[Email Sent] Application Cancellation: Candidate #{CandidateId} has cancelled Application #{ApplicationId} for Job #{JobId}.",
                application.CandidateId, applicationId, application.JobId);
        }

        public void NotifyCandidateStatusChanged(int applicationId, JobApplication.Domain.Enums.JobApplicationStatus newStatus)
        {
            var application = _jobCandidateApplicationRepository.Get().FirstOrDefault(a => a.Id == applicationId);

            if (application is null)
            {
                _logger.LogWarning("Application #{ApplicationId} not found for status update notification.", applicationId);
                return;
            }
            _logger.LogInformation("[Email Sent] Status Update: Candidate #{CandidateId}, your Application #{ApplicationId} for Job #{JobId} is now '{Status}'.",
                application.CandidateId, applicationId, application.JobId, newStatus);
        }

        public void SendInterviewReminder(int applicationId)
        {
            var application = _jobCandidateApplicationRepository.Get().FirstOrDefault(a => a.Id == applicationId);

            if (application is null)
            {
                _logger.LogWarning("Application #{ApplicationId} not found for interview reminder.", applicationId);
                return;
            }
            _logger.LogInformation("[Delayed Email Sent] Interview Reminder: Preparing Candidate #{CandidateId} for upcoming interview regarding Job #{JobId} (Application #{ApplicationId}).",
                application.CandidateId, application.JobId, applicationId);
        }
    }
}
