using Jobportalwebsite.Data;
using Jobportalwebsite.IHelper;
using Jobportalwebsite.Models; 
using Jobportalwebsite.Viewmodel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Jobportalwebsite.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserHelper _userHelper;

        public AccountController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IUserHelper userHelper)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _userHelper = userHelper;
        }

        // GET: Register
        [HttpGet]
        public IActionResult Register() => View();

        public IActionResult CompanyRegister() => View();

        public IActionResult AdminRegister() => View();

        // POST: Register
        //[HttpPost]
        //public async Task<IActionResult> Register(RegistrationViewModel model)
        //{
        //    if (!ModelState.IsValid) return View(model);

        //    if (model.Password != model.ConfirmPassword)
        //    {
        //        ModelState.AddModelError("ConfirmPassword", "The password and confirm password do not match.");
        //        return View(model);
        //    }

        //    var userEmail = await _userManager.FindByEmailAsync(model.Email);
        //    if (userEmail != null)
        //    {
        //        ModelState.AddModelError("Email", "Email already exists.");
        //        return View(model);
        //    }

        //    var addUser = await _userHelper.CreateUserByAsync(model);
        //    if (addUser != null)
        //    {
               
               

        //        TempData["Message"] = "Registered Successfully";
        //        await _signInManager.PasswordSignInAsync(addUser, model.Password, true, true);

        //        return RedirectToAction("Login", "Account");
        //    }

        //    return View(model);
        //}

        // GET: Login
        public IActionResult Login() => View();


               [HttpPost]
        public async Task<IActionResult> Register(RegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "The password and confirm password do not match.");
                    return View(model);
                }
                if (model != null)
                {
                    var userEmail = await _userManager.FindByEmailAsync(model.Email);
                    if (userEmail != null)
                    {
                        ModelState.AddModelError("Email", "Email already exists.");
                        return View(model);
                    }
                    var addUser = await _userHelper.CreateUserByAsync(model);
                    if (addUser != null)
                    {
                        TempData["Message"] = "Registered Successfully";
                        await _signInManager.PasswordSignInAsync(addUser, addUser.PasswordHash, true, true);

                        return RedirectToAction("Login", "Account");
                    }
                }
                return View(model);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CompanyRegister(RegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "The password and confirm password do not match.");
                    return View(model);
                }
                if (model != null)
                {
                    var userEmail = await _userManager.FindByEmailAsync(model.Email);
                    if (userEmail != null)
                    {
                        ModelState.AddModelError("Email", "Email already exists.");
                        return View(model);
                    }
                    var addUser = await _userHelper.CreateUserByAsync(model);
                    if (addUser != null)
                    {
                        TempData["Message"] = "Registered Successfully";
                        await _signInManager.PasswordSignInAsync(addUser, addUser.PasswordHash, true, true);

                        return RedirectToAction("Login", "Account");
                    }
                }
                return View(model);
            }
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Please register first.";
                return RedirectToAction("Register", "Account");
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

            if (result.Succeeded)
            {
                var role = await _userManager.GetRolesAsync(user);

                if (role.Contains("Admin"))
                {
                    return RedirectToAction("Index", "Admin");
                }

                if (role.Contains("Company"))
                {
                    var isEmailRegistered = _context.Companies.Any(c => c.Email == model.Email);
                    return isEmailRegistered ? RedirectToAction("Index", "Company") : RedirectToAction("CompanyRegistration", "Company");
                }

                if (role.Contains("Jobseeker"))
                {
                    return RedirectToAction("Index", "Job");
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        // GET: Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            Response.Cookies.Delete(".AspNetCore.Identity.Application");
            return RedirectToAction("Login", "Account");
        }
    }
}






//using Jobportalwebsite.Data;
//using Jobportalwebsite.IHelper;
//using Jobportalwebsite.Models;
//using Jobportalwebsite.Viewmodel;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using System.Threading.Tasks;

//namespace Jobportalwebsite.Controllers
//{
//    public class AccountController : Controller
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly SignInManager<ApplicationUser> _signInManager;
//        private readonly RoleManager<IdentityRole> _roleManager;
//        private readonly IUserHelper _userHelper;

//        public AccountController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
//            SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IUserHelper userHelper)
//        {
//            _context = context;
//            _userManager = userManager;
//            _signInManager = signInManager;
//            _roleManager = roleManager;
//            _userHelper = userHelper;
//        }

//       [HttpGet]
//        public IActionResult Register()
//        {
//            return View();
//        }
//         public IActionResult CompanyRegister()
//        {
//            return View();
//        }
//        public IActionResult AdminRegister()
//        {
//            return View();
//        }

//        [HttpPost]
//        public async Task<IActionResult> Register(RegistrationViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                if (model.Password != model.ConfirmPassword)
//                {
//                    ModelState.AddModelError("ConfirmPassword", "The password and confirm password do not match.");
//                    return View(model);
//                }
//                if (model != null)
//                {
//                    var userEmail = await _userManager.FindByEmailAsync(model.Email);
//                    if (userEmail != null)
//                    {
//                        ModelState.AddModelError("Email", "Email already exists.");
//                        return View(model);
//                    }
//                    var addUser = await _userHelper.CreateUserByAsync(model);
//                    if (addUser != null)
//                    {
//                        TempData["Message"] = "Registered Successfully";
//                        await _signInManager.PasswordSignInAsync(addUser, addUser.PasswordHash, true, true);

//                        return RedirectToAction("Login", "Account");
//                    }
//                }
//                return View(model);
//            }
//            return View(model);
//        }



//        // GET: Login
//        public IActionResult Login()
//        {
//            return View();
//        }
//        // POST: Login
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Login(LoginViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                var user = await _userManager.FindByEmailAsync(model.Email);


//                if (user == null)
//                {
//                    TempData["ErrorMessage"] = "Please register first.";
//                    return RedirectToAction("Register", "Account");
//                }

//                var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

//                if (result.Succeeded)
//                {
//                    var role = await _userManager.GetRolesAsync(user);

//                    if (role.Contains("Admin"))
//                    {
//                        return RedirectToAction("Index", "Admin");
//                    }

//                    if (role.Contains("Company"))
//                    {
//                        var isEmailRegistered = _context.Companies.Any(c => c.Email == model.Email);
//                        if (isEmailRegistered)
//                        {
//                            return RedirectToAction("Index", "Company");
//                        }
//                        else
//                        {
//                            return RedirectToAction("CompanyRegistration", "Company");
//                        }
//                    }

//                    if (role.Contains("Jobseeker"))
//                    {
//                        return RedirectToAction("Index", "Job");
//                    }
//                }

//                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
//            }

//            return View(model);
//        }





//        [HttpGet]
//        public async Task<IActionResult> Logout()
//        {
//            await HttpContext.SignOutAsync();
//            Response.Cookies.Delete(".AspNetCore.Identity.Application");
//            return RedirectToAction("Login", "Account");
//        }

//        //[HttpPost]
//        //[ValidateAntiForgeryToken]
//        //public async Task<IActionResult> Logout()
//        //{
//        //    await HttpContext.SignOutAsync();
//        //    Response.Cookies.Delete(".AspNetCore.Identity.Application");

//        //    // Debug user state after logout
//        //    var isAuthenticated = User.Identity.IsAuthenticated;
//        //    Console.WriteLine($"User authenticated after logout: {isAuthenticated}");

//        //    return RedirectToAction("Login", "Account");
//        //}




//    }
//}



