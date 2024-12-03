using Jobportalwebsite.Data;
using Jobportalwebsite.Models;
using Jobportalwebsite.Services;
using Microsoft.AspNetCore.Mvc;

namespace Jobportalwebsite.Controllers
{
    public class NotificationController : Controller
    {
        
            private readonly ApplicationDbContext _context;

            public NotificationController(ApplicationDbContext context)
            {
                _context = context;
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
                    CompanyName = company.Name
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


    }
}
