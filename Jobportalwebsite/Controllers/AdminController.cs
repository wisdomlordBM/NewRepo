using Jobportalwebsite.Data;
using Jobportalwebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jobportalwebsite.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        //View all jobseekers
        //public async Task<IActionResult> Jobseekers()
        //{
        //    var jobseekers = await _context.Jobseekers.ToListAsync();
        //    return View();
        //}
        public async Task<IActionResult> ManageJobseekers()
        {
            var jobseekers = await _context.Jobseekers.ToListAsync();
            return View(jobseekers); // Pass jobseekers to the view
        }
        // View all companies
        public async Task<IActionResult> Companies()
        {
            var companies = await _context.Companies.ToListAsync();
            return View(companies);
        }

        // View all job listings
        public async Task<IActionResult> JobListings()
        {
            var jobListings = await _context.Jobs.ToListAsync();
            return View(jobListings);
        }

        // AdminController
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id, bool isActive)
        {
            var jobseeker = await _context.Jobseekers.FindAsync(id);
            if (jobseeker == null)
            {
                return NotFound();
            }

            // Toggle the jobseeker's status
            jobseeker.IsActive = !isActive;
            _context.Update(jobseeker);
            await _context.SaveChangesAsync();

            return Ok();  // Return a success status for the AJAX call
        }

        // Approve Job (Post)
        [HttpPost]
        public async Task<IActionResult> ApproveJob(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null) return NotFound();

            job.PostStatus = JobPostStatus.Posted;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(JobListings));
        }

        // Decline Job
        [HttpPost]
        public async Task<IActionResult> DeclineJob(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null) return NotFound();

            job.PostStatus = JobPostStatus.Declined;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(JobListings));
        }


        // Other admin actions like delete, edit, etc.


        //// GET: Company/Edit/5
        //public IActionResult Edit(int id)
        //{
        //    var company = _context.Companies.Find(id);  // Find the company by ID
        //    if (company == null)
        //    {
        //        return NotFound();  // Return a 404 if the company is not found
        //    }
        //    return View(company);  // Return the company model to the Edit view
        //}

        //// POST: Company/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Edit(int id, Company company)
        //{
        //    if (id != company.Id)
        //    {
        //        return NotFound();  // Return a 404 if the company ID does not match
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(company);  // Update the company in the database
        //            _context.SaveChanges();    // Save changes to the database
        //        }
        //        catch (System.Exception)
        //        {
        //            if (!_context.Companies.Any(c => c.Id == company.Id))
        //            {
        //                return NotFound();  // Return a 404 if the company ID is not found
        //            }
        //            throw;  // Rethrow the exception for other errors
        //        }

        //        return RedirectToAction("Index");  // Redirect to the Index page if successful
        //    }

        //    return View(company);  // Return the company model back to the view if validation fails
        //}
        // GET: Company/Delete/5
        public IActionResult DeleteCompany(int id)
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
            return RedirectToAction("companies");  // Redirect to the Index page after deletion
        }

    }

}
