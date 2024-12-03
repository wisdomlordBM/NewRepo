using Jobportalwebsite.Data;
using Jobportalwebsite.Models;
using Jobportalwebsite.Services;
using Microsoft.EntityFrameworkCore;

public class NotificationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ApplicationDbContext context, ILogger<NotificationService> logger)
    {
        _context = context;
        _logger = logger;
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
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Notification created: {message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving notification");
            throw; // Re-throw to ensure calling code handles the error
        }
    }
}








