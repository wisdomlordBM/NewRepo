
namespace Jobportalwebsite.Models
{
    public class Jobseekers
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Location { get; set; }
        public string? Contact { get; set; }
        public string? Description { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public bool IsActive { get; set; }
    }
}

