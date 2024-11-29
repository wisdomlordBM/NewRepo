using Jobportalwebsite.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Jobportalwebsite.Viewmodel
{
    public class CompanyDashBoard
    {
        public int Id { get; set; }
        public string? Name { get; set; } 
        public string? Email { get; set; } 
        public string? Description { get; set; } 
        public string? Location { get; set; } 
        public string? Industry { get; set; } 
        public string? WebsiteUrl { get; set; } 
        public int? EmployerId { get; set; }
        public virtual User? Employer { get; set; }
        public List<Job> Jobs { get; set; }
        public int? CompanyId { get; set; }
    }
}
