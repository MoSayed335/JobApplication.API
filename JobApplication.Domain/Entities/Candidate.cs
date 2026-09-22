using System;
using System.Collections.Generic;
using System.Text;

namespace JobApplication.Domain.Entities
{
    public class Candidate
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string? CvUrl { get; set; }
        public string Email { get; set; }
    }
}
