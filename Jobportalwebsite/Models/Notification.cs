﻿using Jobportalwebsite.Models;

namespace Jobportalwebsite.Services
{
    public class Notification
    {
        public int Id { get; set; }
        public string? Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime Date { get; set; }
        public NotificationType Type { get; set; }
        public int? JobId { get; set; }
        public int? CompanyId { get; set; }
        public string? JobTitle { get; set; }
        public string? CompanyName { get; set; }
    }

    public enum NotificationType
    {
        NewJob,
        NewCompany
    }


}



