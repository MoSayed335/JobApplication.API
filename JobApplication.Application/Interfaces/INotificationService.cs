using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Application.Interfaces
{
    public interface INotificationService
    {
      void NotifyRecruiter(int applicationId);
    }
}
