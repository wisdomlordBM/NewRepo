//using Jobportalwebsite.Migrations;
//using System;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace Jobportalwebsite.Models
//{
//    public class Application
//    {
//        public int Id { get; set; }
//        public DateTime DateApplied { get; set; } = DateTime.Now;
//        public string? Description { get; set; }
//        public string? Contact { get; set; }
//        public string? EducationLevel { get; set; }
//        public string? FieldOfStudy { get; set; }
//        public string? SchoolName { get; set; }
//        public string? City { get; set; }
//        public string? State { get; set; }
//        public string? JobTitle { get; set; }
//        public string? CompanyName { get; set; }
//        public string? Country { get; set; }
//        public string? EmploymentType { get; set; }

//        public int JobId { get; set; }
//        [ForeignKey(nameof(JobId))]
//        public virtual Job? Job { get; set; }

//        public string? JobseekerId { get; set; } // Replaced UserId with JobseekerId
//        [ForeignKey(nameof(JobseekerId))]
//        public virtual Jobseeker? Jobseeker { get; set; } // Replaced ApplicationUser with Jobseeker
//    }
//}


using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jobportalwebsite.Models
{
    public class Application
    {
        public int Id { get; set; }
        public DateTime DateApplied { get; set; } = DateTime.Now;
        public string? Description { get; set; }
        public string? Contact { get; set; }
        public string? EducationLevel { get; set; }
        public string? FieldOfStudy { get; set; }
        public string? SchoolName { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? JobTitle { get; set; }
        public string? CompanyName { get; set; }
        public string? Country { get; set; }
        public string? EmploymentType { get; set; }


        public int JobId { get; set; }
        [ForeignKey(nameof(JobId))]
        public virtual Job? Job { get; set; }

        public string? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser? User { get; set; } // Replace ApplicationUser with your user model
    }
}






