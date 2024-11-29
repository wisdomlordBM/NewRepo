using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jobportalwebsite.Models
{
    public class Job
    {
        [Key]
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public string? JobTitle { get; set; }
        public string? Description { get; set; }
        public int? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public virtual Company? Company { get; set; }
        public string? RequiredSkills { get; set; } 
        public string? Location { get; set; } 
        public string? EmploymentType { get; set; } 
        public decimal Salary { get; set; }
        public DateTime DatePosted { get; set; } = DateTime.Now;
        public JobPostStatus PostStatus { get; set; } = JobPostStatus.Pending;

        public string? ImageUrl { get; set; } 
    }

    public enum JobPostStatus
    {
        Pending = 1,
        Posted = 2,
        Declined  = 3,
    }
}





