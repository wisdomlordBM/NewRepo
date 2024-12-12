//using Jobportalwebsite.Data;
//using Jobportalwebsite.Models;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.SignalR;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Threading.Tasks;
//using Jobportalwebsite.Hubs;
//using Jobportalwebsite.Services;

//public class NotificationService
//{
//    private readonly ApplicationDbContext _context;
//    private readonly ILogger<NotificationService> _logger;
//    private readonly IHubContext<NotificationHub> _hubContext; // Inject SignalR Hub context

//    public NotificationService(ApplicationDbContext context, ILogger<NotificationService> logger, IHubContext<NotificationHub> hubContext)
//    {
//        _context = context ?? throw new ArgumentNullException(nameof(context));
//        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
//    }

//    public async Task NotifyAdminAsync(string message, int? companyId = null, int? jobId = null)
//    {
//        if (string.IsNullOrWhiteSpace(message))
//        {
//            _logger.LogWarning("Notification message is empty. No notification will be created.");
//            return;
//        }

//        var notification = new Notification
//        {
//            Message = message,
//            Date = DateTime.UtcNow,  // Use UTC for consistency across time zones
//            CompanyId = companyId,
//            JobId = jobId,
//            IsRead = false  // Mark as unread by default
//        };

//        try
//        {
//            // Save notification to the database
//            await _context.Notifications.AddAsync(notification);
//            await _context.SaveChangesAsync();

//            _logger.LogInformation($"Notification created: {message}");

//            // Send real-time notification via SignalR
//            await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error while creating notification");
//            throw; // Re-throw to ensure calling code handles the error
//        }
//    }

//    public async Task NotifyUserAsync(string userId, string message)
//    {
//        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(message))
//        {
//            _logger.LogWarning("UserId or message is empty. Notification will not be sent.");
//            return;
//        }

//        try
//        {
//            // Send real-time notification to a specific user via SignalR
//            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", message);
//            _logger.LogInformation($"Notification sent to user {userId}: {message}");
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, $"Error while sending notification to user {userId}");
//            throw;
//        }
//    }
//}



using Jobportalwebsite.Data;
using Jobportalwebsite.Models;
using Jobportalwebsite.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Jobportalwebsite.Hubs;

public class NotificationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<NotificationService> _logger;
    private readonly IHubContext<NotificationHub> _hubContext; // Inject SignalR Hub context

    public NotificationService(ApplicationDbContext context, ILogger<NotificationService> logger, IHubContext<NotificationHub> hubContext)
    {
        _context = context;
        _logger = logger;
        _hubContext = hubContext; // Assign SignalR Hub context
    }

    public async Task NotifyAdminAsync(string message, int? companyId = null, int? jobId = null)

    {

        if (string.IsNullOrWhiteSpace(message))
        {
            _logger.LogWarning("Notification message is empty. No notification will be created.");
            return;
        }

        var notification = new Notification
        {
            Message = message,
            Date = DateTime.UtcNow,  // Use UTC for consistency across time zones
            CompanyId = companyId,
            JobId = jobId,
            IsRead = false  // Mark as unread by default
        };

        try
        {
            // Save notification to the database
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Notification created: {message}");

            // Send real-time notification via SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving notification");
            throw; // Re-throw to ensure calling code handles the error
        }
    }
    public async Task NotifyUserAsync(string userId, string message)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(message))
        {
            _logger.LogWarning("User ID or message is empty. Notification not sent.");
            return;
        }

        // Send real-time notification to the specific user
        await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", message);

        // Save the notification to the database
        var notification = new Notification
        {
            UserId = userId,
            Message = message,
            Date = DateTime.UtcNow,
            IsRead = false,
            Type = NotificationType.UserAlert // Categorize it as a user alert
        };

        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();
    }


}




//using Jobportalwebsite.Data;
//using Jobportalwebsite.Models;
//using Jobportalwebsite.Services;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.SignalR;


//public class NotificationService
//{
//    private readonly ApplicationDbContext _context;
//    private readonly ILogger<NotificationService> _logger;


//    public NotificationService(ApplicationDbContext context, ILogger<NotificationService> logger)
//    {
//        _context = context;
//        _logger = logger;
//    }

//    public async Task NotifyAdminAsync(string message, int? companyId = null, int? jobId = null)
//    {
//        if (string.IsNullOrWhiteSpace(message))
//        {
//            _logger.LogWarning("Notification message is empty. No notification will be created.");
//            return;
//        }

//        var notification = new Notification
//        {
//            Message = message,
//            Date = DateTime.UtcNow,  // Use UTC for consistency across time zones
//            CompanyId = companyId,
//            JobId = jobId,
//            IsRead = false  // Mark as unread by default
//        };

//        try
//        {
//            await _context.Notifications.AddAsync(notification);
//            await _context.SaveChangesAsync();
//            _logger.LogInformation($"Notification created: {message}");
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error saving notification");
//            throw; // Re-throw to ensure calling code handles the error
//        }
//    }
//}








