namespace JobApplication.Domain.Entities
{
    public class Recruiter
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public ICollection<Job> Jobs { get; set; } = new List<Job>();

    }
}