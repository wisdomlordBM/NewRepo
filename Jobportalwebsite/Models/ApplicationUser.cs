using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jobportalwebsite.Models
{
    public class ApplicationUser : IdentityUser
    {
        // You can add additional properties here as needed
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public string? DateOfBirth { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public DateTime? DateCreated { get; set; }
        [NotMapped]
        public string? Role { get; set; }

    }
}

