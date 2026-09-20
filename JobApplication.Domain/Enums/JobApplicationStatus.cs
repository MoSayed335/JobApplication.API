using System;
using System.Collections.Generic;
using System.Text;

namespace JobApplication.Domain.Enums
{
    public enum JobApplicationStatus
    {
        Applied = 1,
        UnderReview = 2,
        Interview = 3,
        Accepted = 4,
        Rejected = 5,
        Cancelled = 6
    }
}
