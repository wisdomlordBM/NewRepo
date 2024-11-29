using Jobportalwebsite.Models;

namespace Jobportalwebsite.Viewmodel
{
    public class CompanyDashboardViewModel
    {
        public int CompanyId { get; set; }
        public string? Name { get; set; }
        public string? Location { get; set; }
        public string? Industry { get; set; }
        public string? WebsiteUrl { get; set; }
        public List<JobViewModel>? Jobs { get; set; }
        public int? JobId { get; set; }
    }

    public class JobViewModel
    {
        public int Id { get; set; }
        public string? JobTitle { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? EmploymentType { get; set; }
        public decimal? Salary { get; set; }
        public string? ImageUrl { get; set; }
        public JobPostStatus? JobPostStatus { get; set; }
        //public int? JobId { get; set; }
    }

}
