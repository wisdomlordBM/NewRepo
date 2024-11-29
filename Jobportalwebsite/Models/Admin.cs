using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Jobportalwebsite.Models
{
    public class Admin : IdentityUser
    {
        // You can add extra properties specific to the admin
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; } // You could specify roles (e.g., SuperAdmin, Manager)

        // A collection of notifications for the admin
        public ICollection<Notification> Notifications { get; set; }

        // Methods to manage companies, jobseekers, and job listings (logic in the controller)
        public void NotifyAdmin(string message)
        {
            // This can be used to send notifications (logic for sending notifications)
            Notifications.Add(new Notification { Message = message, Date = DateTime.Now });
        }
    }
}

