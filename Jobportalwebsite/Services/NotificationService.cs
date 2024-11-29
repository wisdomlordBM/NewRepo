using Jobportalwebsite.Data;
using Jobportalwebsite.Models;
using Microsoft.Extensions.Logging;  // For logging notifications

namespace Jobportalwebsite.Services
{
    public class NotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ApplicationDbContext context, ILogger<NotificationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task NotifyAdmin(string message)
        {
            // You can implement email notification, or log it to the console as a placeholder
            _logger.LogInformation($"Notification to Admin: {message}");

            // For example, log the message in the database if needed
            var notification = new Notification
            {
                Message = message,
                Date = DateTime.Now
            };

            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }
    }
}


