using System;
using System.ComponentModel.DataAnnotations;


namespace Jobportalwebsite.Models
{
    public class Company
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
        
    }
}


