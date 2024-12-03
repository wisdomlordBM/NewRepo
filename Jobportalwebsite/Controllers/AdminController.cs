
using Jobportalwebsite.Data;
using Jobportalwebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Jobportalwebsite.Services;
using Microsoft.AspNetCore.Authorization;
namespace Jobportalwebsite.Controllers
{
    [Authorize(Roles = "Admin")]

    //public class AdminController : Controller
    //{
    //    private readonly ApplicationDbContext _context;
    //    private readonly NotificationService _notificationService;
    //    private NotificationService? notificationService;

    //    public AdminController(ApplicationDbContext context, NotificationService notificationService)
    //    {
    //        _context = context;
    //        _notificationService = notificationService;
    //    }

    //    //public IActionResult Index()
    //    //{
    //    //    return View();
    //    //}
    //    public IActionResult Index()
    //    {
    //        var notifications = _context.Notifications
    //            .Where(n => !n.IsRead)  // Filter unread notifications
    //            .OrderByDescending(n => n.Date)
    //            .Take(5)  // Optionally limit the number of notifications
    //            .ToList();

    //        var unreadCount = notifications.Count();

    //        ViewData["Notifications"] = notifications;  // Pass notifications to the view
    //        ViewData["UnreadCount"] = unreadCount;      // Pass unread count to the view

    //        return View();
    //    }


    //    // Action to get the count of unread notifications
    //    [HttpGet]
    //    public IActionResult GetNotificationCount()
    //    {
    //        var unreadCount = _context.Notifications.Count(n => n.IsRead == false);
    //        return Json(unreadCount);
    //    }

    //    // Action to get the notifications list
    //    [HttpGet]
    //    public IActionResult GetNotifications()
    //    {
    //        var notifications = _context.Notifications
    //            .Where(n => n.IsRead == false)
    //            .OrderByDescending(n => n.Date)
    //            .Take(5) // Limit to the latest 5 notifications, or adjust as needed
    //            .Select(n => new { n.Message, n.Date })
    //            .ToList();
    //        return Json(notifications);
    //    }

    //    // Post action to mark notifications as read
    //    [HttpPost]
    //    public IActionResult MarkNotificationsAsRead()
    //    {
    //        var notifications = _context.Notifications.Where(n => !n.IsRead).ToList();

    //        foreach (var notification in notifications)
    //        {
    //            notification.IsRead = true;
    //        }

    //        _context.SaveChanges();
    //        return RedirectToAction(nameof(Index)); // Redirect back to the notification list page
    //    }
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly NotificationService _notificationService;

        public AdminController(ApplicationDbContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public IActionResult Index()
        {
            var notifications = _context.Notifications
                .Where(n => !n.IsRead)
                .OrderByDescending(n => n.Date)
                .Take(5)
                .ToList();

            ViewData["Notifications"] = notifications;
            ViewData["UnreadCount"] = notifications.Count;

            return View();
        }

        [HttpGet]
        public IActionResult GetNotificationCount()
        {
            var unreadCount = _context.Notifications.Count(n => !n.IsRead);
            return Json(unreadCount);
        }

        [HttpGet]
        public IActionResult GetNotifications()
        {
            var notifications = _context.Notifications
                .Where(n => !n.IsRead)
                .OrderByDescending(n => n.Date)
                .Take(5)
                .Select(n => new { n.Message, n.Date })
                .ToList();

            return Json(notifications);
        }

        [HttpPost]
        public IActionResult MarkNotificationsAsRead()
        {
            var notifications = _context.Notifications.Where(n => !n.IsRead).ToList();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }



        public async Task<IActionResult> Jobsekers()
        {
            //var jobseekers = await _context.Jobseekers.ToListAsync();
            var jobseekers = await _context.Applications.Include(x => x.User)
                .Select(js => new ApplicationViewModel()
                {
                    FirstName = js.User.FirstName,
                    LastName = js.User.LastName,
                    Email = js.User.Email,
                    PhoneNumber = js.User.PhoneNumber,
                    Location = js.User.Address,
                    Description = js.Description,
                    City = js.City,
                    DateApplied = js.DateApplied,
                }).ToListAsync();
            return View(jobseekers);
        }
        public async Task<IActionResult> Jobseekers()
        {
            var applications = await _context.Applications
                .Include(x => x.User)
                .ToListAsync();

            //var jobseekers = applications.Select(js => new ApplicationViewModel
            //{
            //    FirstName = js.User.FirstName,
            //    LastName = js.User.LastName,
            //    Email = js.User.Email,
            //    PhoneNumber = js.User.PhoneNumber,
            //    Location = js.User.Address,
            //    Description = js.Description,
            //    City = js.City,
            //    DateApplied = js.DateApplied,
            //}).ToList();

            return View(applications);
        }
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
            var listedJobs = jobListings.OrderByDescending(y => y.DatePosted);
            return View(listedJobs);
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


        // GET: Admin/DeleteJobseeker/5
        public async Task<IActionResult> DeleteJobseeker(int id)
        {
            var application = await _context.Applications
                .Include(x => x.User)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }

        // POST: Admin/ConfirmDeleteJobseeker/5
        [HttpPost, ActionName("DeleteJobseeker")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDeleteJobseeker(int id)
        {
            var application = await _context.Applications.FindAsync(id);

            if (application != null)
            {
                _context.Applications.Remove(application);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Jobseekers));
        }


    }

}



//using Jobportalwebsite.Data;
//using Jobportalwebsite.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Jobportalwebsite.Services;
//namespace Jobportalwebsite.Controllers
//{
//    public class AdminController : Controller
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly NotificationService _notificationService;
//        private NotificationService? notificationService;

//        public AdminController(ApplicationDbContext context, NotificationService notificationService)
//        {
//            _context = context;
//            _notificationService = notificationService;
//        }

//        //public IActionResult Index()
//        //{
//        //    return View();
//        //}
//        public IActionResult Index()
//        {
//            // Fetch unread notifications for the admin
//           var notifications = _context.Notifications
//            .Where(n => n.IsRead == false) // Get notifications that are unread
//            .OrderByDescending(n => n.Date)
//            .Take(5)
//            .ToList();



//            // Count unread notifications
//            var unreadCount = notifications.Count();

//            // Pass notifications and unreadCount to the view
//            ViewData["Notifications"] = notifications;
//            ViewData["UnreadCount"] = unreadCount;

//            return View();
//        }
//        // Action to get the count of unread notifications
//        [HttpGet]
//        public IActionResult GetNotificationCount()
//        {
//            var unreadCount = _context.Notifications.Count(n => n.IsRead == false);
//            return Json(unreadCount);
//        }

//        // Action to get the notifications list
//        [HttpGet]
//        public IActionResult GetNotifications()
//        {
//            var notifications = _context.Notifications
//                .Where(n => n.IsRead == false)
//                .OrderByDescending(n => n.Date)
//                .Take(5) // Limit to the latest 5 notifications, or adjust as needed
//                .Select(n => new { n.Message, n.Date })
//                .ToList();
//            return Json(notifications);
//        }

//        // Post action to mark notifications as read
//        [HttpPost]
//        public async Task<IActionResult> MarkNotificationsAsRead()
//        {
//            var notifications = await _context.Notifications
//                .Where(n => n.IsRead == false)
//                .ToListAsync();

//            foreach (var notification in notifications)
//            {
//                notification.IsRead = true;
//            }

//            await _context.SaveChangesAsync();
//            return RedirectToAction("Index");
//        }
//        public async Task<IActionResult> Jobsekers()
//        {
//            //var jobseekers = await _context.Jobseekers.ToListAsync();
//            var jobseekers = await _context.Applications.Include(x => x.User)
//                .Select(js => new ApplicationViewModel()
//                {
//                    FirstName = js.User.FirstName,
//                    LastName = js.User.LastName,
//                    Email = js.User.Email,
//                    PhoneNumber = js.User.PhoneNumber,
//                    Location = js.User.Address,
//                    Description = js.Description,
//                    City = js.City,
//                    DateApplied = js.DateApplied,
//                }).ToListAsync();
//            return View(jobseekers);
//        }
//        public async Task<IActionResult> Jobseekers()
//        {
//            var applications = await _context.Applications
//                .Include(x => x.User)
//                .ToListAsync();

//            //var jobseekers = applications.Select(js => new ApplicationViewModel
//            //{
//            //    FirstName = js.User.FirstName,
//            //    LastName = js.User.LastName,
//            //    Email = js.User.Email,
//            //    PhoneNumber = js.User.PhoneNumber,
//            //    Location = js.User.Address,
//            //    Description = js.Description,
//            //    City = js.City,
//            //    DateApplied = js.DateApplied,
//            //}).ToList();

//            return View(applications);
//        }
//        public async Task<IActionResult> ManageJobseekers()
//        {
//            var jobseekers = await _context.Jobseekers.ToListAsync();
//            return View(jobseekers); // Pass jobseekers to the view
//        }
//        // View all companies
//        public async Task<IActionResult> Companies()
//        {
//            var companies = await _context.Companies.ToListAsync();
//            return View(companies);
//        }

//        // View all job listings
//        public async Task<IActionResult> JobListings()
//        {
//            var jobListings = await _context.Jobs.ToListAsync();
//            return View(jobListings);
//        }



//        // Approve Job (Post)
//        [HttpPost]
//        public async Task<IActionResult> ApproveJob(int id)
//        {
//            var job = await _context.Jobs.FindAsync(id);
//            if (job == null) return NotFound();

//            job.PostStatus = JobPostStatus.Posted;
//            await _context.SaveChangesAsync();

//            return RedirectToAction(nameof(JobListings));
//        }

//        // Decline Job
//        [HttpPost]
//        public async Task<IActionResult> DeclineJob(int id)
//        {
//            var job = await _context.Jobs.FindAsync(id);
//            if (job == null) return NotFound();

//            job.PostStatus = JobPostStatus.Declined;
//            await _context.SaveChangesAsync();

//            return RedirectToAction(nameof(JobListings));
//        }


//        // Other admin actions like delete, edit, etc.


//        //// GET: Company/Edit/5
//        //public IActionResult Edit(int id)
//        //{
//        //    var company = _context.Companies.Find(id);  // Find the company by ID
//        //    if (company == null)
//        //    {
//        //        return NotFound();  // Return a 404 if the company is not found
//        //    }
//        //    return View(company);  // Return the company model to the Edit view
//        //}

//        //// POST: Company/Edit/5
//        //[HttpPost]
//        //[ValidateAntiForgeryToken]
//        //public IActionResult Edit(int id, Company company)
//        //{
//        //    if (id != company.Id)
//        //    {
//        //        return NotFound();  // Return a 404 if the company ID does not match
//        //    }

//        //    if (ModelState.IsValid)
//        //    {
//        //        try
//        //        {
//        //            _context.Update(company);  // Update the company in the database
//        //            _context.SaveChanges();    // Save changes to the database
//        //        }
//        //        catch (System.Exception)
//        //        {
//        //            if (!_context.Companies.Any(c => c.Id == company.Id))
//        //            {
//        //                return NotFound();  // Return a 404 if the company ID is not found
//        //            }
//        //            throw;  // Rethrow the exception for other errors
//        //        }

//        //        return RedirectToAction("Index");  // Redirect to the Index page if successful
//        //    }

//        //    return View(company);  // Return the company model back to the view if validation fails
//        //}
//        // GET: Company/Delete/5
//        public IActionResult DeleteCompany(int id)
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
//            return RedirectToAction("companies");  // Redirect to the Index page after deletion
//        }


//        // GET: Admin/DeleteJobseeker/5
//        public async Task<IActionResult> DeleteJobseeker(int id)
//        {
//            var application = await _context.Applications
//                .Include(x => x.User)
//                .FirstOrDefaultAsync(a => a.Id == id);

//            if (application == null)
//            {
//                return NotFound();
//            }

//            return View(application);
//        }

//        // POST: Admin/ConfirmDeleteJobseeker/5
//        [HttpPost, ActionName("DeleteJobseeker")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> ConfirmDeleteJobseeker(int id)
//        {
//            var application = await _context.Applications.FindAsync(id);

//            if (application != null)
//            {
//                _context.Applications.Remove(application);
//                await _context.SaveChangesAsync();
//            }

//            return RedirectToAction(nameof(Jobseekers));
//        }


//    }

//}
