
using System;

namespace Jobportalwebsite.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public string AdminId { get; set; }  // Foreign key for Admin
        public Admin Admin { get; set; }
        public int CompanyId { get; set; }
        public int JobId { get; set; }
    }
}



