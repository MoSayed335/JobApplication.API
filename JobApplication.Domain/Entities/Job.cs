using System;
using System.Collections.Generic;
using System.Text;

namespace JobApplication.Domain.Entities
{
    public class Job
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int RecruiterId { get; set; }
        public Recruiter Recruiter { get; set; } = null!;   // if you have a Recruiter entity

        public ICollection<JobCandidateApplication> Applications { get; set; } = new List<JobCandidateApplication>();
    }
}
