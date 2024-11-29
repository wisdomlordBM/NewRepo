using Jobportalwebsite.Data;
using Jobportalwebsite.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;
using System.Diagnostics.Metrics;


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

            // Retrieve the job details and user details to populate the application form if necessary
            var job = _context.Jobs.Include(j => j.Company).FirstOrDefault(j => j.Id == jobId);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (job == null || user == null)
            {
                return NotFound(); // Return 404 if the job or user is not found
            }

            // Pass job and user data to the view if you want to pre-populate form fields
            ViewBag.Job = job;
            ViewBag.User = user;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Application application, int jobId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user's ID
            var user = _context.Users.FirstOrDefault(u => u.Id == userId); // Retrieve the current user

            if (ModelState.IsValid)
            {
                var job = _context.Jobs.Include(j => j.Company).FirstOrDefault(j => j.Id == jobId);

                if (job == null)
                {
                    return NotFound(); // If the job doesn't exist, return 404
                }

                // Create a new application with data from the form and user
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
                    UserId = userId, // Only store the UserId
                    DateApplied = DateTime.Now
                };

                // Save the new application to the database
                _context.Applications.Add(myApplication);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Application submitted successfully!"; // Temporary success message

                return RedirectToAction("Index", "Job");
            }

            return View();
        }











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

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Jobseekers jobseeker)
        {
            if (ModelState.IsValid)
            {
                _context.Update(jobseeker);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(jobseeker);
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


