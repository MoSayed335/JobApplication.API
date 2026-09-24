using JobApplication.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JobApplication.Domain.Entities
{
    public  class JobCandidateApplication
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        [ForeignKey(nameof(CandidateId))]
        public Candidate Candidate { get; set; } = null!;
        public int JobId { get; set; }
        [ForeignKey(nameof(JobId))]
        public Job Job { get; set; } = null!;
        public JobApplicationStatus JobApplicationStatus { get; set; }

        public DateTime? CancelledAt { get; set; }
        public DateTime AppliedAt { get; set; }
        public DateTime StatusUpdatedAt { get; set; }
    }
}
