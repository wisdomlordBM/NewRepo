using Jobportalwebsite.Data;
using Jobportalwebsite.Models;
using Jobportalwebsite.Services;  // Make sure NotificationService is correctly imported
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Jobportalwebsite.Viewmodel;
using Microsoft.AspNetCore.Identity;

namespace Jobportalwebsite.Controllers
{
    [Authorize]
    public class CompanyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly NotificationService _notificationService;  // Notification service field
        private readonly UserManager<ApplicationUser> _userManager; // Inject UserManager
        // Constructor to inject ApplicationDbContext and NotificationService
        public CompanyController(ApplicationDbContext context, NotificationService notificationService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _notificationService = notificationService;  // Assign the notification service
            _userManager = userManager;
        }

      
        // GET: Company/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Company company)
        {
            if (ModelState.IsValid)
            {
                company.Email = User.Identity.Name;

                if (_context.Companies.Any(c => c.Email == company.Email || c.Name == company.Name))
                {
                    ModelState.AddModelError("", "Company with the same email or name already exists.");
                    return View(company);
                }

                _context.Companies.Add(company);
                await _context.SaveChangesAsync();

                await _notificationService.NotifyAdminAsync(
                    $"A new company '{company.Name}' has registered.",
                    companyId: company.Id
                );

                return RedirectToAction("Index");
            }

            return View(company);
        }

        public IActionResult ViewApplications(int companyId)
        {
            var applications = _context.Applications
                .Where(a => a.Job != null && a.Job.Company != null && a.Job.Company.Id == companyId)
                .Include(a => a.Job)
                .ThenInclude(j => j.Company)
                .Include(a => a.User)
                .Select(a => new ApplicationViewModel
                {
                    Id = a.Id,
                    JobTitle = a.Job.JobTitle,
                    ImageUrl = a.Job.ImageUrl,
                    CompanyId = a.Job.Company.Id,
                    Company = a.Job.Company,
                    EmploymentType = a.Job.EmploymentType,
                    Location = a.Job.Location,
                    Salary = a.Job.Salary,
                    UserId = a.UserId,
                    Email = a.User.Email ?? "N/A",
                    FirstName = a.User.FirstName ?? "N/A",
                    LastName = a.User.LastName ?? "N/A",
                    PhoneNumber = a.User.PhoneNumber ?? "N/A",
                    EducationLevel = a.EducationLevel,
                    CVPath = a.CVPath,  // Ensure CVPath is populated
                    Contact = a.Contact,
                    Description = a.Description,
                    DateApplied = a.DateApplied,
                    City = a.City,
                })
                .ToList();

            return View(applications);
        }
        [HttpPost]
        public async Task<IActionResult> Check(int applicationId)
        {
            var application = await _context.Applications
                .Include(a => a.User)
                .Include(j => j.Job)
                .ThenInclude(c => c.Company)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application != null)
            {
                var userId = application.UserId;
                var companyName = application.Job?.Company?.Name ?? "the company";
                var companyId = application.Job?.Company?.Id;

                var message = $"Your application is under review by {companyName}.";

                await _notificationService.NotifyUserAsync(userId, message);

                TempData["Message"] = "User has been notified about the review status.";

                if (companyId.HasValue)
                {
                    return RedirectToAction("ViewApplications", "Company", new { companyId = companyId.Value });
                }
            }

            return RedirectToAction("ViewApplications", "Company");
        }


        [HttpPost]
        public async Task<IActionResult> Accept(int applicationId)
        {
            var application = await _context.Applications
                .Include(a => a.User)
                .Include(j => j.Job)
                .ThenInclude(c => c.Company)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application != null)
            {
                var userId = application.UserId;
                var companyId = application.Job?.Company?.Id;
                var message = $"Congratulations! {application.Job?.Company?.Name ??      "The company"} has accepted your application. They will contact you soon.";

                // Notify the user (job seeker) via SignalR
                await _notificationService.NotifyUserAsync(userId, message);

                TempData["Message"] = "User has been notified of acceptance.";

                if (companyId.HasValue)
                {
                    return RedirectToAction("ViewApplications", "Company", new { companyId = companyId.Value });
                }
            }

            return RedirectToAction("ViewApplications", "Company");
        }

        [HttpPost]

        public async Task<IActionResult> Decline(int applicationId)
        {
            var application = await _context.Applications
                .Include(a => a.User)
                .Include(j => j.Job)
                .ThenInclude(c => c.Company)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application != null)
            {
                var userId = application.UserId;
                var companyId = application.Job?.Company?.Id;
                var message = $"We regret to inform you that your application for {application.Job?.JobTitle ?? "the job"} has been declined.";

                // Notify the user (job seeker) via SignalR
                await _notificationService.NotifyUserAsync(userId, message);

                // Optionally remove the application if desired
                _context.Applications.Remove(application);
                await _context.SaveChangesAsync();

                TempData["Message"] = "User has been notified of the decline.";

                if (companyId.HasValue)
                {
                    return RedirectToAction("ViewApplications", "Company", new { companyId = companyId.Value });
                }
            }

            return RedirectToAction("ViewApplications", "Company");
        }


        [HttpGet]
        public async Task<IActionResult> GetUserNotifications()
        {
            var userId = _userManager.GetUserId(User); // Get logged-in user's ID
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead && n.Type == NotificationType.UserAlert)
                .OrderByDescending(n => n.Date)
                .Take(5)
                .Select(n => new { n.Message, n.Date })
                .ToListAsync();

            ViewData["Notifications"] = notifications;
            ViewData["UnreadCount"] = notifications.Count;


            return Json(notifications);
        }
        [HttpPost]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = _userManager.GetUserId(User);
            //var notifications = await _context.Notifications
            //    .Where(n => n.UserId == userId && !n.IsRead)
            //    .ToListAsync();
            var notifications = _context.Notifications.Where(n => !n.IsRead && n.Type == NotificationType.UserAlert).ToList();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Job");
        }
        [HttpGet]
        public IActionResult GetUserNotificationCount()
        {
            var unreadCount = _context.Notifications.Count(n => !n.IsRead && n.Type == NotificationType.UserAlert);
            return Json(unreadCount);
        }
        ////    [HttpPost]
        ////public IActionResult MarkNotificationsAsRead()
        ////{
        ////    var notifications = _context.Notifications.Where(n => !n.IsRead).ToList();

        ////    foreach (var notification in notifications)
        ////    {
        ////        notification.IsRead = true;
        ////    }

        ////    _context.SaveChanges();
        ////    return RedirectToAction(nameof(Index));
        ////}







        // GET: Company/Index
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var userEmail = User.Identity.Name;

            var company = _context.Companies
                .FirstOrDefault(c => c.Email == userEmail);

            if (company == null)
            {
                return RedirectToAction("CompanyRegistration");
            }
            

        
            var jobs = _context.Jobs
                .Where(j => j.CompanyId == company.Id)
                .Select(j => new JobViewModel
                {
                    Id = j.Id,
                    JobTitle = j.JobTitle,
                    Description = j.Description,
                    Location = j.Location,
                    EmploymentType = j.EmploymentType,
                    Salary = j.Salary,
                    ImageUrl = j.ImageUrl,
                    JobPostStatus = j.PostStatus
                })
                .ToList();

            var viewModel = new CompanyDashboardViewModel
            {
                CompanyId = company.Id,
                Name = company.Name,
                Location = company.Location,
                Industry = company.Industry,
                WebsiteUrl = company.WebsiteUrl,
                Jobs = jobs
            };

            return View(viewModel);

           
        }






        // GET: Company/Edit/5
        public IActionResult Edit(int id)
        {
            var company = _context.Companies.Find(id);  // Find the company by ID
            if (company == null)
            {
                return NotFound();  // Return a 404 if the company is not found
            }
            return View(company);  // Return the company model to the Edit view
        }

        // POST: Company/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Company company)
        {
            if (id != company.Id)
            {
                return NotFound();  // Return a 404 if the company ID does not match
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(company);  // Update the company in the database
                    _context.SaveChanges();    // Save changes to the database
                }
                catch (System.Exception)
                {
                    if (!_context.Companies.Any(c => c.Id == company.Id))
                    {
                        return NotFound();  // Return a 404 if the company ID is not found
                    }
                    throw;  // Rethrow the exception for other errors
                }

                return RedirectToAction("Index", "Company");  // Redirect to the Index page if successful
            }

            return View(company);  // Return the company model back to the view if validation fails
        }

        // GET: Company/Delete/5
        public IActionResult Delete(int id)
        {
            var company = _context.Companies.Find(id);  // Find the company by ID
            if (company == null)
            {
                return NotFound();  // Return a 404 if the company is not found
            }
            return View(company);  // Return the company model to the Delete view
        }

        // POST: Company/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var company = _context.Companies.Find(id);  // Find the company by ID
            if (company != null)
            {
                _context.Companies.Remove(company);  // Remove the company from the database
                _context.SaveChanges();  // Save changes to the database
            }

            return RedirectToAction("Index");  // Redirect to the Index page after deletion
        }

        // GET: Company/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var company = await _context.Companies.FindAsync(id);  // Find the company by ID
            if (company == null)
            {
                return NotFound();  // Return a 404 if the company is not found
            }

            return View(company);  // Return the company model to the Details view
        }

        public IActionResult CompanyRegistration()
        {
            return View();
        }
    }
}


//using Jobportalwebsite.Data;
//using Jobportalwebsite.Models;
//using Jobportalwebsite.Services;  // Make sure NotificationService is correctly imported
//using Microsoft.AspNetCore.Mvc;
//using System.Threading.Tasks;
//using System.Linq;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.EntityFrameworkCore;
//using Jobportalwebsite.Viewmodel;

//namespace Jobportalwebsite.Controllers
//{
//    [Authorize]
//    public class CompanyController : Controller
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly NotificationService _notificationService;  // Notification service field

//        // Constructor to inject ApplicationDbContext and NotificationService
//        public CompanyController(ApplicationDbContext context, NotificationService notificationService)
//        {
//            _context = context;
//            _notificationService = notificationService;  // Assign the notification service
//        }

//        // GET: Company/Create
//        public IActionResult Create()
//        {
//            return View();
//        }

//        // POST: Company/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Createss(Company company)
//        {
//            if (ModelState.IsValid)
//            {
//                // Assume the user's email is stored in User.Identity.Name
//                company.Email = User.Identity.Name;  // Assign the logged-in user's email to the company

//                _context.Companies.Add(company);
//                await _context.SaveChangesAsync();

//                await _notificationService.NotifyAdmin("A new company has registered.", companyId: company.Id);
//                // Notify the admin about the new company registration
//                //await _notificationService.NotifyAdmin($"New company registered: {company.Name}");

//                return RedirectToAction("Index");  // Redirect to the Index action
//            }

//            return View(company); // Return the view with the company model if validation fails
//        }

//        // POST: Company/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(Company company)
//        {
//            if (ModelState.IsValid)
//            {
//                company.Email = User.Identity.Name;

//                var existingCompanyEmail = await _context.Companies
//                    .FirstOrDefaultAsync(c => c.Email == company.Email);

//                if (existingCompanyEmail != null)
//                {
//                    ModelState.AddModelError("Email", "The email address is already associated with another company.");
//                    return View(company); 
//                }

//                var existingCompanyName = await _context.Companies
//                    .FirstOrDefaultAsync(c => c.Name == company.Name);

//                if (existingCompanyName != null)
//                {
//                    ModelState.AddModelError("Name", "The name address is already associated with another company.");
//                    return View(company);
//                }

//                _context.Companies.Add(company);
//                await _context.SaveChangesAsync();

//                return RedirectToAction("Index", new { companyId = company.Id });
//            }


//            return View(company);
//        }
//        public IActionResult ViewApplications(int companyId)
//        {


//                var applications = _context.Applications
//                .Where(a => a.Job != null && a.Job.Company != null && a.Job.Company.Id == companyId)
//                .Include(a => a.Job)
//                .ThenInclude(j => j.Company)
//                .Include(a => a.User)
//                .Select(a => new ApplicationViewModel
//                {
//                    JobTitle = a.Job.JobTitle,
//                    ImageUrl = a.Job.ImageUrl,
//                    CompanyId = a.Job.Company.Id,
//                    Company = a.Job.Company,
//                    EmploymentType = a.Job.EmploymentType,
//                    Location = a.Job.Location,
//                    Salary = a.Job.Salary,
//                    UserId = a.UserId,
//                    Email = a.User.Email ?? "N/A", 
//                    FirstName = a.User.FirstName ?? "N/A", 
//                    LastName = a.User.LastName ?? "N/A",
//                    PhoneNumber = a.User.PhoneNumber ?? "N/A",
//                    EducationLevel = a.EducationLevel,
//                    Contact = a.Contact,
//                    Description = a.Description,
//                    DateApplied = a.DateApplied,
//                    City = a.City,
//                })
//                .ToList();

//            return View(applications);
//        }




//        //public IActionResult ViewApplications(int companyId)
//        //{
//        //    var applications = _context.Applications
//        //        .Include(a => a.Job)
//        //        .ThenInclude(j => j.Company)
//        //        .Include(a => a.User)
//        //        .Where(a => a.Job.Company.Id == companyId)
//        //        .Select(a => new ApplicationViewModel
//        //        {
//        //            JobTitle = a.Job.JobTitle,
//        //            ImageUrl = a.Job.ImageUrl,
//        //            CompanyId = a.Job.Company.Id,
//        //            Company = a.Job.Company,
//        //            EmploymentType = a.Job.EmploymentType,
//        //            Location = a.Job.Location,
//        //            Salary = a.Job.Salary,
//        //            UserId = a.UserId,
//        //            Email = a.User != null ? a.User.Email : "N/A",
//        //            FirstName = a.User != null ? a.User.FirstName : "N/A",
//        //            LastName = a.User != null ? a.User.LastName : "N/A",
//        //            PhoneNumber = a.User != null ? a.User.PhoneNumber : "N/A",
//        //            EducationLevel = a.EducationLevel,
//        //            Contact = a.Contact,
//        //            Description = a.Description,
//        //            DateApplied = a.DateApplied,
//        //            City = a.City,
//        //        }).ToList();

//        //    return View(applications);
//        //}

//        //public IActionResult ViewApplications(int companyId)
//        //{
//        //    var applications = _context.Applications
//        //    .Include(a => a.Job)
//        //    .Include(a => a.User)
//        //    .Where(a => a.Job.Company.Id == companyId)
//        //    .Select(a => new ApplicationViewModel
//        //    {
//        //        JobTitle = a.Job.JobTitle,
//        //        ImageUrl = a.Job.ImageUrl,
//        //        CompanyId = a.Job.Company.Id,
//        //        Company = a.Job.Company,
//        //        EmploymentType = a.Job.EmploymentType,
//        //        Location = a.Job.Location,
//        //        Salary = a.Job.Salary,
//        //        UserId = a.UserId,
//        //        Email = a.User != null ? a.User.Email : "N/A",
//        //        FirstName = a.User != null ? a.User.FirstName : "N/A",
//        //        LastName = a.User != null ? a.User.LastName : "N/A",
//        //        PhoneNumber = a.User != null ? a.User.PhoneNumber : "N/A",
//        //        EducationLevel = a.EducationLevel,
//        //        Contact = a.Contact,
//        //        Description = a.Description,
//        //        DateApplied = a.DateApplied,
//        //        City = a.City,
//        //    }).ToList();


//        //    return View(applications);
//        //}





//        // GET: Company/Index
//        //public IActionResult Index()
//        //{
//        //    // Ensure the user is authenticated before proceeding
//        //    if (!User.Identity.IsAuthenticated)
//        //    {
//        //        return RedirectToAction("Login", "Account");  // Redirect to login if not authenticated
//        //    }

//        //    // Get the logged-in user's email
//        //    var userEmail = User.Identity.Name;

//        //    // Retrieve the company associated with the logged-in user's email
//        //    var company = _context.Companies
//        //        .FirstOrDefault(c => c.Email == userEmail); // Assuming company is linked with the user's email

//        //    // If no company is found, redirect to registration page
//        //    if (company == null)
//        //    {
//        //        return RedirectToAction("CompanyRegistration");
//        //    }

//        //    // Retrieve related jobs for the company
//        //    company.Jobs = _context.Jobs.Where(j => j.CompanyId == company.Id).ToList();

//        //    // Pass the company model to the view
//        //    return View(company);  // This view will show only the current company's profile and related jobs
//        //}

//        // GET: Company/Index
//        public IActionResult Index()
//        {
//            if (!User.Identity.IsAuthenticated)
//            {
//                return RedirectToAction("Login", "Account"); 
//            }

//            var userEmail = User.Identity.Name;

//            var company = _context.Companies
//                .FirstOrDefault(c => c.Email == userEmail);

//            if (company == null)
//            {
//                return RedirectToAction("CompanyRegistration");
//            }

//            var jobs = _context.Jobs
//                .Where(j => j.CompanyId == company.Id)
//                .Select(j => new JobViewModel
//                {
//                    Id = j.Id,
//                    JobTitle = j.JobTitle,
//                    Description = j.Description,
//                    Location = j.Location,
//                    EmploymentType = j.EmploymentType,
//                    Salary = j.Salary,
//                    ImageUrl = j.ImageUrl,
//                    JobPostStatus = j.PostStatus
//                })
//                .ToList();

//            var viewModel = new CompanyDashboardViewModel
//            {
//                CompanyId = company.Id,
//                Name = company.Name,
//                Location = company.Location,
//                Industry = company.Industry,
//                WebsiteUrl = company.WebsiteUrl,
//                Jobs = jobs
//            };

//            return View(viewModel);
//        }






//        // GET: Company/Edit/5
//        public IActionResult Edit(int id)
//        {
//            var company = _context.Companies.Find(id);  // Find the company by ID
//            if (company == null)
//            {
//                return NotFound();  // Return a 404 if the company is not found
//            }
//            return View(company);  // Return the company model to the Edit view
//        }

//        // POST: Company/Edit/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public IActionResult Edit(int id, Company company)
//        {
//            if (id != company.Id)
//            {
//                return NotFound();  // Return a 404 if the company ID does not match
//            }

//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    _context.Update(company);  // Update the company in the database
//                    _context.SaveChanges();    // Save changes to the database
//                }
//                catch (System.Exception)
//                {
//                    if (!_context.Companies.Any(c => c.Id == company.Id))
//                    {
//                        return NotFound();  // Return a 404 if the company ID is not found
//                    }
//                    throw;  // Rethrow the exception for other errors
//                }

//                return RedirectToAction("Index", "Company");  // Redirect to the Index page if successful
//            }

//            return View(company);  // Return the company model back to the view if validation fails
//        }

//        // GET: Company/Delete/5
//        public IActionResult Delete(int id)
//        {
//            var company = _context.Companies.Find(id);  // Find the company by ID
//            if (company == null)
//            {
//                return NotFound();  // Return a 404 if the company is not found
//            }
//            return View(company);  // Return the company model to the Delete view
//        }

//        // POST: Company/Delete/5
//        [HttpPost, ActionName("DeleteConfirmed")]
//        [ValidateAntiForgeryToken]
//        public IActionResult DeleteConfirmed(int id)
//        {
//            var company = _context.Companies.Find(id);  // Find the company by ID
//            if (company != null)
//            {
//                _context.Companies.Remove(company);  // Remove the company from the database
//                _context.SaveChanges();  // Save changes to the database
//            }

//            return RedirectToAction("Index");  // Redirect to the Index page after deletion
//        }

//        // GET: Company/Details/5
//        public async Task<IActionResult> Details(int id)
//        {
//            var company = await _context.Companies.FindAsync(id);  // Find the company by ID
//            if (company == null)
//            {
//                return NotFound();  // Return a 404 if the company is not found
//            }

//            return View(company);  // Return the company model to the Details view
//        }

//        public IActionResult CompanyRegistration()
//        {
//            return View();
//        }
//    }
//}


