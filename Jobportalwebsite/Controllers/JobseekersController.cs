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




namespace Jobportalwebsite.Controllers
{
    public class JobseekersController : Controller
    {
        private readonly ApplicationDbContext _context;
       

        public JobseekersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Jobseekers
        public IActionResult Index()
        {
            var jobseekers = _context.Jobseekers.ToList(); // Fetch all jobseekers
            return View(jobseekers); // Pass jobseekers to the view
        }

        // DELETE: Jobseeker
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var jobseeker = _context.Jobseekers.Find(id);
            if (jobseeker != null)
            {
                _context.Jobseekers.Remove(jobseeker); // Remove the jobseeker
                _context.SaveChanges(); // Save changes
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
            // Get the current logged-in user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieve the job details
            var job = _context.Jobs.Include(j => j.Company).FirstOrDefault(j => j.Id == jobId);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (job == null || user == null)
            {
                TempData["ErrorMessage"] = "You must register as a jobseeker to apply for a job.";
                return RedirectToAction("Register", "Account"); // Return 404 if the job or user is not found
            }

            // Check if the user has already applied for the job
            var existingApplication = _context.Applications
                .FirstOrDefault(a => a.JobId == jobId && a.UserId == userId);

            if (existingApplication != null)
            {
                TempData["ErrorMessage"] = "You have already applied for this job.";
                return RedirectToAction("Index", "Job"); // Redirect to the job listing page with the error message
            }

            // Pass job and user data to the view if you want to pre-populate form fields
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user's ID

            // Check if the user has already applied for the job
            var existingApplication = _context.Applications
                .FirstOrDefault(a => a.JobId == jobId && a.UserId == userId);

            if (existingApplication != null)
            {
                TempData["ErrorMessage"] = "You cannot apply for the same job more than once.";
                return RedirectToAction("Index", "Job");
            }

            // Validate the file size (10MB max)
            if (CV != null && CV.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError("CV", "File size exceeds the 5MB limit.");
            }

            // Validate file extension (only PDF, DOC, DOCX)
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

                // If a CV is uploaded, save it
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

                    // Update the application with the CV file path
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

            // Ensure the upload directory exists
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "cvs");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // Generate a unique file name
            var uniqueFileName = Guid.NewGuid() + Path.GetExtension(model.CV.FileName);
            var filePath = Path.Combine(uploadPath, uniqueFileName);

            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.CV.CopyToAsync(stream);
            }

            // Update the application record with the file path
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

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> UploadCV(UploadCVViewModel model)
        //{
        //    if (model.CV == null || model.CV.Length == 0)
        //    {
        //        ModelState.AddModelError("CV", "Please upload a valid CV file.");
        //        return View(model);
        //    }

        //    var application = _context.Applications.FirstOrDefault(a => a.Id == model.ApplicationId);

        //    if (application == null || application.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
        //    {
        //        return NotFound();
        //    }

        //    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "cvs");
        //    if (!Directory.Exists(uploadPath))
        //    {
        //        Directory.CreateDirectory(uploadPath);
        //    }

        //    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.CV.FileName);
        //    var filePath = Path.Combine(uploadPath, uniqueFileName);

        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        await model.CV.CopyToAsync(stream);
        //    }

        //    application.CVPath = "/uploads/cvs/" + uniqueFileName;
        //    _context.Update(application);
        //    await _context.SaveChangesAsync();

        //    TempData["SuccessMessage"] = "CV uploaded successfully!";
        //    return RedirectToAction("Index", "Job");
        //}

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
        //        TempData["ErrorMessage"] = "Please wait for the company's response. You cannot apply for the same job more than once.";
        //        return RedirectToAction("Index", "Job"); // Redirect to job listings or another appropriate page
        //    }

        //    // Handle CV file upload
        //    if (ModelState.IsValid)
        //    {
        //        if (application.CV != null)
        //        {
        //            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "cvs");

        //            if (!Directory.Exists(uploadPath))
        //            {
        //                Directory.CreateDirectory(uploadPath); // Create the directory if it doesn't exist
        //            }

        //            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(application.CV.FileName);
        //            var filePath = Path.Combine(uploadPath, uniqueFileName);

        //            using (var stream = new FileStream(filePath, FileMode.Create))
        //            {
        //                await application.CV.CopyToAsync(stream);
        //            }

        //            application.CVPath = "/uploads/cvs/" + uniqueFileName;
        //        }

        //        var job = _context.Jobs.Include(j => j.Company).FirstOrDefault(j => j.Id == jobId);
        //        if (job == null)
        //        {
        //            return NotFound(); // Job not found
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
        //            UserId = userId, // Store the current user's ID
        //            DateApplied = DateTime.Now
        //        };

        //        _context.Applications.Add(myApplication);
        //        await _context.SaveChangesAsync();

        //        TempData["SuccessMessage"] = "Application submitted successfully!";
        //        return RedirectToAction("Index", "Job");
        //    }

        //    return View(); // Return the view with validation errors
        //}




        //////[HttpPost]
        //////[ValidateAntiForgeryToken]
        //////public IActionResult Create(Application application, int jobId)
        //////{
        //////    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user's ID
        //////    var user = _context.Users.FirstOrDefault(u => u.Id == userId); // Retrieve the current user

        //////    if (ModelState.IsValid)
        //////    {
        //////        if (application.CV != null)
        //////        {
        //////            // Ensure a valid file is uploaded
        //////            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", application.CV.FileName);

        //////            using (var stream = new FileStream(filePath, FileMode.Create))
        //////            {
        //////                await application.CV.CopyToAsync(stream);
        //////            }

        //////            // Store the file path in the model or database as needed
        //////            application.CVPath = "/uploads/" + application.CV.FileName; // Save this path to the database if necessary
        //////        }
        //////        var job = _context.Jobs.Include(j => j.Company).FirstOrDefault(j => j.Id == jobId);

        //////        if (job == null)
        //////        {
        //////            return NotFound(); // If the job doesn't exist, return 404
        //////        }

        //////        // Create a new application with data from the form and user
        //////        var myApplication = new Application
        //////        {
        //////            JobId = jobId,
        //////            Description = application.Description,
        //////            Contact = application.Contact,
        //////            EducationLevel = application.EducationLevel,
        //////            FieldOfStudy = application.FieldOfStudy,
        //////            SchoolName = application.SchoolName,
        //////            City = application.City,
        //////            State = application.State,
        //////            JobTitle = job.JobTitle,
        //////            CompanyName = job.Company.Name,
        //////            Country = application.Country,
        //////            EmploymentType = application.EmploymentType,
        //////            UserId = userId, // Only store the UserId
        //////            DateApplied = DateTime.Now
        //////        };

        //////        // Save the new application to the database
        //////        _context.Applications.Add(myApplication);
        //////        _context.SaveChanges();

        //////        TempData["SuccessMessage"] = "Application submitted successfully!"; // Temporary success message

        //////        return RedirectToAction("Index", "Job");
        //////    }

        //////    return View();
        //////}











        [HttpGet]
        public IActionResult Edit(int id)
        {
            var jobseeker = _context.Jobseekers.Find(id);
            if (jobseeker == null)
            {
                return NotFound();
            }
            return View(jobseeker);
        }

        
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Edit(Jobseekers jobseeker)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Update(jobseeker);
        //        _context.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(jobseeker);
        //}

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


        public IActionResult Details()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            var jobseeker = _context.Jobseekers.FirstOrDefault(j => j.UserId == userId);
            if (jobseeker == null)
            {
                return NotFound();
            }

            return View(jobseeker);
        }

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


