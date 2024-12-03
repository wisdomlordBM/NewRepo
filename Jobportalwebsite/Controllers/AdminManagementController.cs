using Jobportalwebsite.Data;
using Jobportalwebsite.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jobportalwebsite.Controllers
{
    public class AdminManagementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminManagementController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Admin/ManageJobseekers
        public async Task<IActionResult> ManageJobseekers()
        {
            // Fetch all jobseekers from the database
            var jobseekers = await _context.Jobseekers.ToListAsync();
            return View(jobseekers);  // Pass jobseekers to the view
        }


        // POST: Admin/ToggleJobseekerStatus (To toggle IsActive status)
        //[HttpPost]
        //public async Task<IActionResult> ToggleJobseekerStatus(int id, bool isActive)
        //{
        //    var jobseeker = await _context.Jobseekers.FindAsync(id);
        //    if (jobseeker != null)
        //    {
        //        jobseeker.IsActive = !isActive;  // Toggle the IsActive status
        //        _context.Update(jobseeker);
        //        await _context.SaveChangesAsync();
        //    }

        //    return Json(new { success = true });  // Respond with success status
        //}

        // POST: Admin/ToggleJobseekerStatus (To toggle IsActive status)
        //[HttpPost]
        //public async Task<IActionResult> ToggleJobseekerStatus(int id)
        //{
        //    var jobseeker = await _context.Jobseekers.FindAsync(id);
        //    if (jobseeker != null)
        //    {
        //        jobseeker.IsActive = !jobseeker.IsActive;  // Toggle the IsActive status
        //        _context.Update(jobseeker);
        //        await _context.SaveChangesAsync();
        //    }

        //    return RedirectToAction("ManageJobseekers");
        //}

        // POST: Admin/DeleteJobseeker
        [HttpPost]
        public async Task<IActionResult> DeleteJobseeker(int id)
        {
            var jobseeker = await _context.Jobseekers.FindAsync(id);
            if (jobseeker != null)
            {
                // Optionally delete related Identity user (if needed)
                var user = await _userManager.FindByIdAsync(jobseeker.UserId);
                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                }

                _context.Jobseekers.Remove(jobseeker);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ManageJobseekers");
        }

    }
}

