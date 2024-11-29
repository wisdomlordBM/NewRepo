using Jobportalwebsite.Migrations;




namespace Jobportalwebsite.Models
{
    public class ApplicationViewModel
    {
        public string? JobTitle { get; set; }
        public int? JobId { get; set; }
        public string? ImageUrl { get; set; }
        public int CompanyId { get; set; }
        public Company? Company { get; set; }
        public string? EmploymentType { get; set; }
        public string? Location { get; set; }
        public decimal? Salary { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? EducationLevel { get; set; }
        public string? Contact { get; set; }
        public string? Description { get; set; }
        public DateTime DateApplied { get; set; }
        public string? UserId { get; set; }
        
        public ApplicationUser? User { get; set; }
        //public ApplicationUser? Job { get; set; }
        public Job? Job { get; set; }


        public string? City { get; set; }
    }

}
