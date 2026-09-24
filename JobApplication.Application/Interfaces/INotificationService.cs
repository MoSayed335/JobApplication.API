using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Application.Interfaces
{
    using JobApplication.Domain.Enums;

    public interface INotificationService
    {
        void NotifyRecruiter(int applicationId);
        void NotifyApplicationCancelled(int applicationId);
        void NotifyCandidateStatusChanged(int applicationId, JobApplicationStatus newStatus);
        void SendInterviewReminder(int applicationId);
    }
}
