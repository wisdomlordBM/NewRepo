using Jobportalwebsite.Data;
using Jobportalwebsite.IHelper;
using Jobportalwebsite.Models;
using Jobportalwebsite.Viewmodel;
using Microsoft.AspNetCore.Identity;

namespace Jobportalwebsite.Helper
{
    public class UserHelper : IUserHelper
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserHelper(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Method 1: CreateUserByAsync with default behavior
        public async Task<ApplicationUser> CreateUserByAsync(RegistrationViewModel model)
        {
            if (model != null)
            {
                var user = new ApplicationUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address = model.Address,
                    Gender = model.Gender,
                    DateOfBirth = model.DateOfBirth,
                    DateCreated = DateTime.Now,
                    PhoneNumber = model.PhoneNumber,
                    State = model.State,
                    Country = model.Country,
                    Email = model.Email,
                    UserName = model.Email,
                    Role = model.Role
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Role);
                    return user;
                }
            }

            return null;
        }

        // Method 2: CreateUserByAsync with a specific role
        public async Task<ApplicationUser> CreateUserByAsync(RegistrationViewModel model, string role)
        {
            if (model != null)
            {
                var user = new ApplicationUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address = model.Address,
                    Gender = model.Gender,
                    DateOfBirth = model.DateOfBirth,
                    DateCreated = DateTime.Now,
                    PhoneNumber = model.PhoneNumber,
                    State = model.State,
                    Country = model.Country,
                    Email = model.Email,
                    UserName = model.Email,
                    Role = role
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Ensure the role exists before adding
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(role));
                    }

                    await _userManager.AddToRoleAsync(user, role);
                    return user;
                }
            }

            return null;
        }
    }
}






//using Jobportalwebsite.Data;
//using Jobportalwebsite.IHelper;
//using Jobportalwebsite.Models;
//using Jobportalwebsite.Viewmodel;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;

//namespace Jobportalwebsite.Helper
//{
//    public class UserHelper : IUserHelper

//    {
//        private readonly ApplicationDbContext _context;
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly SignInManager<ApplicationUser> _signInManager;
//        private readonly RoleManager<IdentityRole> _roleManager;

//        public UserHelper(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
//           SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
//        {
//            _context = context;
//            _userManager = userManager;
//            _signInManager = signInManager;
//            _roleManager = roleManager;
//        }
//        public async Task<ApplicationUser> CreateUserByAsync(RegistrationViewModel applicationUserViewModel)

//        {
//            if (applicationUserViewModel != null)
//            {
//                var user = new ApplicationUser()
//                {

//                    FirstName = applicationUserViewModel.FirstName,
//                    LastName = applicationUserViewModel.LastName,
//                    Address = applicationUserViewModel.Address,

//                    Gender = applicationUserViewModel.Gender,
//                    DateOfBirth = applicationUserViewModel.DateOfBirth,
//                    DateCreated = DateTime.Now,
//                    PhoneNumber = applicationUserViewModel.PhoneNumber,
//                    State = applicationUserViewModel.State,
//                    Country = applicationUserViewModel.Country,
//                    Email = applicationUserViewModel.Email,
//                    UserName = applicationUserViewModel.Email,
//                    Role = applicationUserViewModel.Role


//                };
//                var result = await _userManager.CreateAsync(user, applicationUserViewModel.Password);
//                if (result.Succeeded)
//                {
//                    await _userManager.AddToRoleAsync(user, applicationUserViewModel.Role);
//                    return user;
//                }
//            }
//            return null;
//        }

//    }
//}
