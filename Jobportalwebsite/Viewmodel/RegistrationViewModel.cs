//using System.ComponentModel.DataAnnotations;

//namespace Jobportalwebsite.Viewmodel
//{
//    public class RegistrationViewModel
//    {
//        public int Id { get; set; }
//        public string FirstName { get; set; } = string.Empty;
//        public string LastName { get; set; } = string.Empty;

//        [Required]
//        [EmailAddress]
//        public string Email { get; set; } = string.Empty;

//        [Required]
//        [DataType(DataType.Password)]
//        public string Password { get; set; } = string.Empty;

//        [Required]
//        [DataType(DataType.Password)]
//        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
//        public string ConfirmPassword { get; set; } = string.Empty;

//        public string PhoneNumber { get; set; } = string.Empty;
//        public string Address { get; set; } = string.Empty;
//        public string DateOfBirth { get; set; } = string.Empty;
//        public string Gender { get; set; } = string.Empty;
//        public string State { get; set; } = string.Empty;
//        public string Country { get; set; } = string.Empty;
//        public string WebsiteURL {  get; set; } = string.Empty;
//        public string CompanyName { get; set; } = string.Empty;
//        public string Location { get; set; } = string.Empty;
//        public string Industry { get; set; } = string.Empty;
//        public string Description { get; set; } = string.Empty;

//        public string Role { get; set; } = "Jobseeker"; // Default to "Jobseeker"
//    }
//}
using System.ComponentModel.DataAnnotations;

namespace Jobportalwebsite.Viewmodel
{
    public class RegistrationViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be 11 digits and it should be numeric")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]
        public string? DateOfBirth { get; set; } 

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }
        [Required(ErrorMessage = "State is required")]
        public string State { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; }
        public string WebsiteURL { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string Role { get; set; } = "Jobseeker"; // Default to "Jobseeker"
    }
}


