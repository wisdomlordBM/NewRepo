using Jobportalwebsite.Data;
using Jobportalwebsite.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;
using System.Diagnostics.Metrics;
using Jobportalwebsite.Migrations;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System.Linq;

using System;
using System.Threading.Tasks;
using UploadCVViewModel = Jobportalwebsite.Models.UploadCVViewModel;
using Microsoft.Extensions.Hosting;




namespace Jobportalwebsite.Controllers
{
    public class JobseekersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public JobseekersController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Jobseekers






        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieve users who belong to the "Jobseeker" role

            var UserID = _context.UserRoles
                .Where(ur => ur.RoleId == _context.Roles.FirstOrDefault(r => r.Name == "Jobseeker").Id)
                .Select(ur => ur.UserId);

            var user = _context.Users
                .Where(u => userId.Contains(u.Id))
                .ToList();
            // Pass the profile picture path (or default) to the view
          
            return View(user);
        }

        //public IActionResult Index()
        //{
        //    // Filter users with the role of "Jobseeker"
        //    var jobseekers = _context.Users
        //        .Where(u => u.Role == "Jobseeker") // Adjust "Role" to match your actual property for roles
        //        .ToList();

        //    return View(jobseekers);
        //}

        //public IActionResult Index()
        //{
        //    var jobseekers = _context.Jobseekers.ToList(); 
        //    return View(jobseekers); 
        //}

        // DELETE: Jobseeker
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var jobseeker = _context.Jobseekers.Find(id);
            if (jobseeker != null)
            {
                _context.Jobseekers.Remove(jobseeker); 
                _context.SaveChanges(); 
            }
            return RedirectToAction(nameof(Index));
        }



        //public IActionResult Index()
        //{
        //    var jobseekersList = _context.Jobseekers.ToList();
        //    ViewBag.Message = TempData["SuccessMessage"];
        //    return View(jobseekersList);
        //}
        public IActionResult Create(int jobId)
        {
          
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            
            var job = _context.Jobs.Include(j => j.Company).FirstOrDefault(j => j.Id == jobId);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (job == null || user == null)
            {
                TempData["ErrorMessage"] = "You must register as a jobseeker to apply for a job.";
                return RedirectToAction("Register", "Account"); 
            }

            var existingApplication = _context.Applications
                .FirstOrDefault(a => a.JobId == jobId && a.UserId == userId);

            if (existingApplication != null)
            {
                TempData["ErrorMessage"] = "You have already applied for this job.";
                return RedirectToAction("Index", "Job"); 
            }

            ViewBag.Job = job;
            ViewBag.User = user;

            return View();
        }

        //public IActionResult Create(int jobId)
        //{
        //    // Get the current logged-in user's ID
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    // Retrieve the job details and user details to populate the application form if necessary
        //    var job = _context.Jobs.Include(j => j.Company).FirstOrDefault(j => j.Id == jobId);
        //    var user = _context.Users.FirstOrDefault(u => u.Id == userId);

        //    if (job == null || user == null)
        //    {
        //        TempData["ErrorMessage"] = "You must register as a jobseeker to apply for a job.";
        //        return RedirectToAction("Register", "Account"); // Return 404 if the job or user is not found
        //    }

        //    // Pass job and user data to the view if you want to pre-populate form fields
        //    ViewBag.Job = job;
        //    ViewBag.User = user;

        //    return View();
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Application application, int jobId, IFormFile CV)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 

            var existingApplication = _context.Applications
                .FirstOrDefault(a => a.JobId == jobId && a.UserId == userId);

            if (existingApplication != null)
            {
                TempData["ErrorMessage"] = "You cannot apply for the same job more than once.";
                return RedirectToAction("Index", "Job");
            }
            if (CV != null && CV.Length > 5 * 1024 * 1024) 
            {
                ModelState.AddModelError("CV", "File size exceeds the 5MB limit.");
                ViewBag.CurrentSection = "jobExperience"; 
                return View(application);
            }





            var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
            var fileExtension = Path.GetExtension(CV?.FileName).ToLower();
            if (CV != null && !allowedExtensions.Contains(fileExtension))
            {
                ModelState.AddModelError("CV", "Only PDF, DOC, and DOCX files are allowed.");
            }

            if (ModelState.IsValid)
            {
                var job = _context.Jobs.Include(j => j.Company).FirstOrDefault(j => j.Id == jobId);
                if (job == null)
                {
                    return NotFound();
                }

                var myApplication = new Application
                {
                    JobId = jobId,
                    Description = application.Description,
                    Contact = application.Contact,
                    EducationLevel = application.EducationLevel,
                    FieldOfStudy = application.FieldOfStudy,
                    SchoolName = application.SchoolName,
                    City = application.City,
                    State = application.State,
                    JobTitle = job.JobTitle,
                    CompanyName = job.Company.Name,
                    Country = application.Country,
                    EmploymentType = application.EmploymentType,
                    UserId = userId,
                    DateApplied = DateTime.Now
                };

                _context.Applications.Add(myApplication);
                await _context.SaveChangesAsync();

               
                if (CV != null)
                {
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "cvs");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    var uniqueFileName = Guid.NewGuid() + Path.GetExtension(CV.FileName);
                    var filePath = Path.Combine(uploadPath, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await CV.CopyToAsync(stream);
                    }

                    myApplication.CVPath = "/uploads/cvs/" + uniqueFileName;
                    _context.Update(myApplication);
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = "Application submitted successfully!";
                return RedirectToAction("Index", "Job");
            }

            return View(application);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(Application application, int jobId)
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user's ID

        //    // Check if the user has already applied for the job
        //    var existingApplication = _context.Applications
        //        .FirstOrDefault(a => a.JobId == jobId && a.UserId == userId);

        //    if (existingApplication != null)
        //    {
        //        TempData["ErrorMessage"] = "You cannot apply for the same job more than once.";
        //        return RedirectToAction("Index", "Job");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        var job = _context.Jobs.Include(j => j.Company).FirstOrDefault(j => j.Id == jobId);
        //        if (job == null)
        //        {
        //            return NotFound();
        //        }

        //        var myApplication = new Application
        //        {
        //            JobId = jobId,
        //            Description = application.Description,
        //            Contact = application.Contact,
        //            EducationLevel = application.EducationLevel,
        //            FieldOfStudy = application.FieldOfStudy,
        //            SchoolName = application.SchoolName,
        //            City = application.City,
        //            State = application.State,
        //            JobTitle = job.JobTitle,
        //            CompanyName = job.Company.Name,
        //            Country = application.Country,
        //            EmploymentType = application.EmploymentType,
        //            UserId = userId,
        //            DateApplied = DateTime.Now
        //        };

        //        _context.Applications.Add(myApplication);
        //        await _context.SaveChangesAsync();

        //        TempData["SuccessMessage"] = "Application submitted successfully! Please upload your CV.";
        //        return RedirectToAction("UploadCV", new { applicationId = myApplication.Id });
        //    }

        //    return View(application);
        //}

        [HttpGet]
        public IActionResult UploadCV(int applicationId)
        {
            var application = _context.Applications.FirstOrDefault(a => a.Id == applicationId);

            if (application == null || application.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();
            }

            return View(new UploadCVViewModel { ApplicationId = applicationId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadCV(UploadCVViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var application = _context.Applications.FirstOrDefault(a => a.Id == model.ApplicationId);

            if (application == null || application.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();
            }

            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "cvs");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var uniqueFileName = Guid.NewGuid() + Path.GetExtension(model.CV.FileName);
            var filePath = Path.Combine(uploadPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.CV.CopyToAsync(stream);
            }

            application.CVPath = "/uploads/cvs/" + uniqueFileName;
            _context.Update(application);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "CV uploaded successfully!";
            return RedirectToAction("Index", "Job");
        }

        //[HttpGet]
        //public IActionResult UploadCV(int applicationId)
        //{
        //    var application = _context.Applications.FirstOrDefault(a => a.Id == applicationId);

        //    if (application == null || application.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
        //    {
        //        return NotFound();
        //    }

        //    return View(new UploadCVViewModel { ApplicationId = applicationId });
        //}







        [HttpGet]
        public IActionResult Edit(string id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationUser model, IFormFile? profilePicture)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the user from the database
                var user = await _context.Users.FindAsync(model.Id);
                if (user == null)
                {
                    return NotFound();
                }

                // Update user details
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Address = model.Address;
                user.PhoneNumber = model.PhoneNumber;
                user.Country = model.Country;

                // Handle profile picture upload
                if (profilePicture != null && profilePicture.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder);

                    var fileName = Guid.NewGuid() + Path.GetExtension(profilePicture.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await profilePicture.CopyToAsync(stream);
                    }

                    user.ProfilePicturePath = "/uploads/" + fileName;
                }

                // Save changes to the database
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", new { id = user.Id });
            }

            return View(model);
        }



        //[HttpGet]
        //public IActionResult Delete(int id)
        //{
        //    var jobseeker = _context.Jobseekers.Find(id);
        //    if (jobseeker == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(jobseeker);
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var jobseeker = _context.Jobseekers.Find(id);
            if (jobseeker != null)
            {
                _context.Jobseekers.Remove(jobseeker);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");

        }

        [Route("Jobseekers/Details/{email}")]
        public IActionResult Details(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest();
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return NotFound();
            }

            var currentUserEmail = User.Identity?.Name; 
            bool isOwnProfile = string.Equals(currentUserEmail, user.Email, StringComparison.OrdinalIgnoreCase);

            ViewBag.IsOwnProfile = isOwnProfile;

            return View(user);
        }



        //[Route("Jobseekers/Details/{email}")]
        //public IActionResult Details(string email)
        //{
        //    if (string.IsNullOrEmpty(email))
        //    {
        //        return BadRequest();
        //    }

        //    var user = _context.Users.FirstOrDefault(u => u.Email == email);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(user);
        //}



        ////public IActionResult Details()
        ////{
        ////    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        ////    if (userId == null)
        ////    {
        ////        return Unauthorized();
        ////    }

        ////    var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        ////    if (user == null)
        ////    {
        ////        return NotFound();
        ////    }

        ////    return View(user);
        ////}


        public IActionResult Detailsaaa(int id)
        {
            var jobseeker = _context.Jobseekers.Find(id);
            if (jobseeker == null)
            {
                return NotFound();
            }
            return View(jobseeker);
        }
    }
}

