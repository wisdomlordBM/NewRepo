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
        [NotMapped]
        public IFormFile? CV { get; set; }

        // Add CVPath property
        public string? CVPath { get; set; } // Store the path to the uploaded CV file

        public int JobId { get; set; }
        [ForeignKey(nameof(JobId))]
        public virtual Job? Job { get; set; }

        public string? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser? User { get; set; } // Assuming ApplicationUser is your custom user model
    }
}


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
//        public IFormFile? CV { get; set; }

//        public int JobId { get; set; }
//        [ForeignKey(nameof(JobId))]
//        public virtual Job? Job { get; set; }

//        public string? UserId { get; set; }
//        [ForeignKey(nameof(UserId))]
//        public virtual ApplicationUser? User { get; set; } // Replace ApplicationUser with your user model
//    }
//}






