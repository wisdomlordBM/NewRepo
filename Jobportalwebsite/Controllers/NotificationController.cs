using Jobportalwebsite.Data;
using Jobportalwebsite.Models;
using Jobportalwebsite.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jobportalwebsite.Controllers
{
    public class NotificationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly NotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager; // Inject UserManager

        public NotificationController(ApplicationDbContext context, NotificationService notificationService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _notificationService = notificationService;  // Initialize NotificationService
            _userManager = userManager;  // Initialize UserManager
        }

        // Action to create a notification for a new job posting
        public void NotifyNewJob(Job job)
        {
            var notification = new Notification
            {
                Message = "A new job has been posted.",
                IsRead = false,
                Date = DateTime.Now,
                Type = NotificationType.NewJob,
                JobId = job.Id,
                JobTitle = job.JobTitle,
                CompanyName = job.Company.Name
            };

            _context.Notifications.Add(notification);
            _context.SaveChanges();
        }

        // Action to create a notification for a new company registration
        public void NotifyNewCompany(Company company)
        {
            var notification = new Notification
            {
                Message = "A new company has registered.",
                IsRead = false,
                Date = DateTime.Now,
                Type = NotificationType.NewCompany,
                CompanyId = company.Id,
                CompanyName = company.Name,
            };

            _context.Notifications.Add(notification);
            _context.SaveChanges();
        }

        // Action to mark a notification as read
        [HttpPost]
        public IActionResult MarkAsRead([FromBody] NotificationReadRequest request)
        {
            var notification = _context.Notifications.FirstOrDefault(n => n.Id == request.Id);
            if (notification != null)
            {
                notification.IsRead = true;
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        public class NotificationReadRequest
        {
            public int Id { get; set; }
        }

        [HttpGet]
        public IActionResult GetNotifications()
        {
            var notifications = _context.Notifications
                .Where(n => !n.IsRead)
                .OrderByDescending(n => n.Date)
                .ToList();

            return Json(notifications);
        }
        //[HttpPost]
        //public async Task<IActionResult> Check(int applicationId)
        //{
        //    var application = await _context.Applications
        //        .Include(a => a.User)
        //        .Include(j => j.Job)
        //        .ThenInclude(c => c.Company)
        //        .FirstOrDefaultAsync(a => a.Id == applicationId);

        //    if (application != null)
        //    {
        //        var userId = application.UserId;
        //        var message = $"Your application is under review by {application.Job?.Company?.Name ?? "the company"}.";

        //        // Notify the user (job seeker) via SignalR
        //        await _notificationService.NotifyUserAsync(userId, message);

        //        TempData["Message"] = "User has been notified about the review status.";
        //    }

        //    return RedirectToAction("ViewApplications", "Company");
        //}

        //[HttpPost]
        //public async Task<IActionResult> Accept(int applicationId)
        //{
        //    var application = await _context.Applications
        //        .Include(a => a.User)
        //        .Include(j => j.Job)
        //        .ThenInclude(c => c.Company)
        //        .FirstOrDefaultAsync(a => a.Id == applicationId);

        //    if (application != null)
        //    {
        //        var userId = application.UserId;
        //        var message = $"Congratulations! {application.Job?.Company?.Name ?? "The company"} has accepted your application. They will contact you soon.";

        //        // Notify the user (job seeker) via SignalR
        //        await _notificationService.NotifyUserAsync(userId, message);

        //        TempData["Message"] = "User has been notified of acceptance.";
        //    }

        //    return RedirectToAction("ViewApplications", "Company");
        //}

        //[HttpPost]
        //public async Task<IActionResult> Decline(int applicationId)
        //{
        //    var application = await _context.Applications
        //        .Include(a => a.User)
        //        .Include(j => j.Job)
        //        .FirstOrDefaultAsync(a => a.Id == applicationId);

        //    if (application != null)
        //    {
        //        var userId = application.UserId;
        //        var message = $"We regret to inform you that your application for {application.Job?.JobTitle ?? "the job"} has been declined.";

        //        // Notify the user (job seeker) via SignalR
        //        await _notificationService.NotifyUserAsync(userId, message);

        //        // Optionally remove the application if desired
        //        _context.Applications.Remove(application);
        //        await _context.SaveChangesAsync();

        //        TempData["Message"] = "User has been notified of the decline.";
        //    }

        //    return RedirectToAction("Index", "Company");
        //}

        // Action to mark an application as under review
        //[HttpPost]
        //public async Task<IActionResult> Check(int applicationId)
        //{
        //    var application = await _context.Applications
        //        .Include(a => a.User)
        //        .Include(j => j.Job)
        //        .ThenInclude(c => c.Company)
        //        .FirstOrDefaultAsync(a => a.Id == applicationId);

        //    if (application != null)
        //    {
        //        var userId = application.UserId;
        //        var message = $"Your application is under review by {application.Job.Company.Name}.";

        //        Notify the user(job seeker) via SignalR
        //        await _notificationService.NotifyUserAsync(userId, message);

        //        TempData["Message"] = "User has been notified about the review status.";
        //    }

        //    return RedirectToAction("Index", "Company");
        //}

        //[HttpPost]
        //public async Task<IActionResult> Accept(int applicationId)
        //{
        //    var application = await _context.Applications
        //        .Include(a => a.User)
        //        .Include(j => j.Job)
        //        .ThenInclude(c => c.Company)
        //        .FirstOrDefaultAsync(a => a.Id == applicationId);

        //    if (application != null)
        //    {
        //        var userId = application.UserId;
        //        var message = $"Congratulations! {application.Job.Company.Name} has accepted your application. They will contact you soon.";

        //        Notify the user(job seeker) via SignalR
        //        await _notificationService.NotifyUserAsync(userId, message);

        //        TempData["Message"] = "User has been notified of acceptance.";
        //    }

        //    return RedirectToAction("Index", "Company");
        //}

        //[HttpPost]
        //public async Task<IActionResult> Decline(int applicationId)
        //{
        //    var application = await _context.Applications.Include(a => a.User).FirstOrDefaultAsync(a => a.Id == applicationId);

        //    if (application != null)
        //    {
        //        var userId = application.UserId;
        //        var message = $"We regret to inform you that your application for {application.Job.JobTitle} has been declined.";

        //        Notify the user(job seeker) via SignalR
        //        await _notificationService.NotifyUserAsync(userId, message);

        //        Optionally remove the application if desired
        //        _context.Applications.Remove(application);
        //        await _context.SaveChangesAsync();

        //        TempData["Message"] = "User has been notified of the decline.";
        //    }

        //    return RedirectToAction("Index", "Company");
        //}
        //[HttpGet]
        //public async Task<IActionResult> GetUserNotifications()
        //{
        //    var userId = _userManager.GetUserId(User); // Get logged-in user's ID
        //    var notifications = await _context.Notifications
        //        .Where(n => n.UserId == userId && !n.IsRead)
        //        .OrderByDescending(n => n.Date)
        //        .Take(5)
        //        .Select(n => new { n.Message, n.Date })
        //        .ToListAsync();

        //    return Json(notifications);
        //}
        //[HttpPost]
        //public async Task<IActionResult> MarkAllAsRead()
        //{
        //    var userId = _userManager.GetUserId(User);
        //    var notifications = await _context.Notifications
        //        .Where(n => n.UserId == userId && !n.IsRead)
        //        .ToListAsync();

        //    foreach (var notification in notifications)
        //    {
        //        notification.IsRead = true;
        //    }

        //    await _context.SaveChangesAsync();
        //    return Ok();
        //}


    }

    //public class NotificationController : Controller
    //{

    //        private readonly ApplicationDbContext _context;

    //        public NotificationController(ApplicationDbContext context)
    //        {
    //            _context = context;
    //        }

    //        // Action to create a notification for a new job posting
    //        public void NotifyNewJob(Job job)
    //        {
    //            var notification = new Notification
    //            {
    //                Message = "A new job has been posted.",
    //                IsRead = false,
    //                Date = DateTime.Now,
    //                Type = NotificationType.NewJob,
    //                JobId = job.Id,
    //                JobTitle = job.JobTitle,
    //                CompanyName = job.Company.Name
    //            };

    //            _context.Notifications.Add(notification);
    //            _context.SaveChanges();
    //        }

    //        // Action to create a notification for a new company registration
    //        public void NotifyNewCompany(Company company)
    //        {
    //            var notification = new Notification
    //            {
    //                Message = "A new company has registered.",
    //                IsRead = false,
    //                Date = DateTime.Now,
    //                Type = NotificationType.NewCompany,
    //                CompanyId = company.Id,
    //                CompanyName = company.Name
    //            };

    //            _context.Notifications.Add(notification);
    //            _context.SaveChanges();
    //        }

    //        // Action to mark a notification as read
    //        [HttpPost]
    //        public IActionResult MarkAsRead([FromBody] NotificationReadRequest request)
    //        {
    //            var notification = _context.Notifications.FirstOrDefault(n => n.Id == request.Id);
    //            if (notification != null)
    //            {
    //                notification.IsRead = true;
    //                _context.SaveChanges();
    //                return Json(new { success = true });
    //            }
    //            return Json(new { success = false });
    //        }


    //    public class NotificationReadRequest
    //    {
    //        public int Id { get; set; }
    //    }

    //    [HttpGet]
    //    public IActionResult GetNotifications()
    //    {
    //        var notifications = _context.Notifications
    //            .Where(n => !n.IsRead)
    //            .OrderByDescending(n => n.Date)
    //            .ToList();

    //        return Json(notifications);
    //    }


    //}
}
