using Jobportalwebsite.Data;
using Jobportalwebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Jobportalwebsite.Services;

namespace Jobportalwebsite.Controllers
{
    public class JobController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly NotificationService _notificationService;

        public JobController(ApplicationDbContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }





        public IActionResult Index()
        {
            IEnumerable<Job> objJobList = _context.Jobs.Where(x => x.PostStatus == JobPostStatus.Posted);
            var listedJobs = objJobList.OrderByDescending(y => y.DatePosted);

            // Fetch notifications for the logged-in user
            var notifications = _context.Notifications
                .Where(n => n.UserId == User.Identity.Name) // Ensure User.Identity.Name matches your user identifier
                .OrderByDescending(n => n.Date)
                .Take(10)
                .ToList();

            ViewData["Notifications"] = notifications;

            return View(listedJobs);
        }

        //public IActionResult Index()
        //{
        //    // Fetching jobs with the given condition
        //    IEnumerable<Job> objJobList = _context.Jobs.Where(x => x.PostStatus == JobPostStatus.Posted);
        //    var listedJobs = objJobList.OrderByDescending(y => y.DatePosted);

        //    // Fetching notifications (example)
        //    var notifications = _context.Notifications // Replace with your actual notifications retrieval logic
        //        .Where(n => n.UserId == User.Identity.Name) // Assuming User.Identity.Name holds the current user's identifier
        //        .OrderByDescending(n => n.DateCreated)
        //        .Take(10) // Limit to the latest 10 notifications
        //        .Select(n => n.Message) // Assuming `Message` contains the notification text
        //        .ToList();

        //    // Passing notifications to the view
        //    ViewData["Notifications"] = notifications;

        //    return View(listedJobs);
        //}

        //public IActionResult Index()
        //{
        //    IEnumerable<Job> objJobList = _context.Jobs.Where(x => x.PostStatus == JobPostStatus.Posted);
        //    var listedJobs = objJobList.OrderByDescending(y => y.DatePosted);
        //    return View(listedJobs);
        //    //return View(objJobList);
        //}



        // Admin approves a job
        public IActionResult ApproveJob(int id)
        {
            var job = _context.Jobs.FirstOrDefault(j => j.Id == id);
            if (job != null)
            {
                job.PostStatus = JobPostStatus.Posted; // Change the status to Post (approved)
                _context.SaveChanges();
            }
            return RedirectToAction("Index"); // Redirect to company index or another page
        }


        public IActionResult AllJobs()
        {
            var jobs = _context.Jobs.Where(j => j.PostStatus == JobPostStatus.Posted).ToList();
            return View(jobs);
        }

        // GET: Job/Create
        [HttpGet]
        public IActionResult Create()
        {
            // Assuming company is linked to the logged-in user
            var companyEmail = User.Identity?.Name;
            var company = _context.Companies.FirstOrDefault(c => c.Email == companyEmail);
            if (company == null)
            {
                return RedirectToAction("CompanyRegistration", "Company");
            }

            ViewBag.CompanyId = company.Id; // Pass CompanyId to the view
            return View();
        }
        //Post
     

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateJob(Job job)
        {
            if (ModelState.IsValid)
            {
                var company = _context.Companies.FirstOrDefault(c => c.Email == User.Identity.Name);

                if (company == null)
                {
                    return RedirectToAction("CompanyRegistration", "Company");
                }

                job.CompanyId = company.Id;
                job.DatePosted = DateTime.UtcNow;
                _context.Jobs.Add(job);
                await _context.SaveChangesAsync();

                await _notificationService.NotifyAdminAsync(
                    $"A new job '{job.JobTitle}' has been posted by {company.Name}.",
                    jobId: job.Id,
                    companyId: company.Id
                );

                return RedirectToAction("Index", "company");
            }

            return View(job);
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateJob(Job job)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        job.PostStatus = JobPostStatus.Pending;

        //        // Ensure the job is linked to the logged-in company
        //        var companyEmail = User.Identity?.Name;
        //        var company = _context.Companies.FirstOrDefault(c => c.Email == companyEmail);
        //        if (company == null)
        //        {
        //            return RedirectToAction("CompanyRegistration", "Company");
        //        }

        //        job.CompanyId = company.Id;
        //        job.DatePosted = DateTime.Now;
        //        job.IsDeleted = false;
        //        job.PostStatus = JobPostStatus.Pending;

        //        _context.Jobs.Add(job);
        //        await _context.SaveChangesAsync();
        //        var newJobNotification = new Notification
        //        {
        //            Message = "A new job has been posted.",
        //            IsRead = false,
        //            Date = DateTime.Now,
        //            Type = NotificationType.NewJob,
        //            JobId = job.Id,
        //            JobTitle = job.JobTitle,
        //            CompanyName = job.Company.Name
        //        };

        //        _context.Notifications.Add(newJobNotification);
        //        _context.SaveChanges();



        //        return RedirectToAction("Index", new { companyId = company.Id });
        //    }
        //    return View(job);
        //}

        public IActionResult ViewApplications(int jobId)
        {
            var applications = _context.Applications
                .Include(a => a.User)
                .Where(a => a.JobId == jobId)
                .Select(a => new
                {
                    a.User.FirstName,
                    a.User.LastName,
                    a.User.Email,
                    a.User.Address,
                    a.Contact,
                    a.Description,
                    a.DateApplied
                }).ToList();

            return View(applications);
        }

        //Get
        public IActionResult Edit(int? id)

        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var JobsFromDb = _context.Jobs.Find(id);

            if (JobsFromDb == null)
            {
                return NotFound();
            }


            return View(JobsFromDb);
        }
        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Edit(Job obj)
        {
            if (ModelState.IsValid)
            {
                var jobFromDb = _context.Jobs.AsNoTracking().FirstOrDefault(j => j.Id == obj.Id);
                if (jobFromDb != null)
                {
                    obj.CompanyId = jobFromDb.CompanyId;
                    _context.Jobs.Update(obj);
                    _context.SaveChanges();
                    return RedirectToAction("Index", "Company");
                }
                return NotFound();
            }
            return View(obj);
        }

        //Get
        public IActionResult Delete(int? id)

        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var JobsFromDb = _context.Jobs.Find(id);
            // var JobsFromDbFirst = _context.Jobs.FirstOrDefault(u => u.Id == id);
            // var JobsFromDbSingle = _context.Jobs. SingleOrDefault(u => u.Id == id);

            if (JobsFromDb == null)
            {
                return NotFound();
            }


            return View(JobsFromDb);
        }
        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _context.Jobs.Find(id);
            if (obj == null)
            {
                return NotFound();
            }

            _context.Jobs.Remove(obj);
            _context.SaveChanges();
            return RedirectToAction("Index", "company");
        }


        [HttpPost]
        public IActionResult Index(string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
            {

                return RedirectToAction("Index");
            }
            var filteredJob = _context.Jobs

                .AsEnumerable()
                .Where(p => p.Location.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                || p.JobTitle.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                || p.EmploymentType.Contains(searchString, StringComparison.OrdinalIgnoreCase))


                .ToList();

            return View("Index", filteredJob);
        }


    }


}

//using Jobportalwebsite.Data;
//using Jobportalwebsite.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Security.Claims;
//using Jobportalwebsite.Services;

//namespace Jobportalwebsite.Controllers
//{
//    public class JobController : Controller
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly NotificationService _notificationService;

//        public JobController(ApplicationDbContext context, NotificationService notificationService)
//        {
//            _context = context;
//            _notificationService = notificationService;
//        }






//        public IActionResult Index()
//        {
//            IEnumerable<Job> objJobList = _context.Jobs.Where(x => x.PostStatus == JobPostStatus.Posted);
//            return View(objJobList);
//        }



//        // Admin approves a job
//        public IActionResult ApproveJob(int id)
//        {
//            var job = _context.Jobs.FirstOrDefault(j => j.Id == id);
//            if (job != null)
//            {
//                job.PostStatus = JobPostStatus.Posted; // Change the status to Post (approved)
//                _context.SaveChanges();
//            }
//            return RedirectToAction("Index"); // Redirect to company index or another page
//        }


//        public IActionResult AllJobs()
//        {
//            var jobs = _context.Jobs.Where(j => j.PostStatus == JobPostStatus.Posted).ToList();
//            return View(jobs);
//        }

//        // GET: Job/Create
//        [HttpGet]
//        public IActionResult Create()
//        {
//            // Assuming company is linked to the logged-in user
//            var companyEmail = User.Identity?.Name;
//            var company = _context.Companies.FirstOrDefault(c => c.Email == companyEmail);
//            if (company == null)
//            {
//                return RedirectToAction("CompanyRegistration", "Company");
//            }

//            ViewBag.CompanyId = company.Id; // Pass CompanyId to the view
//            return View();
//        }
//        //Post
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public IActionResult xCreates(Job obj)
//        {
//            if (ModelState.IsValid)
//            {

//                _context.Jobs.Add(obj);
//                _context.SaveChanges();

//                return RedirectToAction("Index");
//            }
//            return View(obj);
//        }
//        [HttpPost]
//        public async Task<IActionResult> CreateJob(Job job)
//        {
//            if (ModelState.IsValid)
//            {
//                job.PostStatus = JobPostStatus.Pending;

//                // Ensure the job is linked to the logged-in company
//                var companyEmail = User.Identity?.Name;
//                var company = _context.Companies.FirstOrDefault(c => c.Email == companyEmail);
//                if (company == null)
//                {
//                    return RedirectToAction("CompanyRegistration", "Company");
//                }

//                job.CompanyId = company.Id;
//                job.DatePosted = DateTime.Now;
//                job.IsDeleted = false;
//                job.PostStatus = JobPostStatus.Pending;

//                _context.Jobs.Add(job);
//                await _context.SaveChangesAsync();

//                // Send notification to admin

//                await _notificationService.NotifyAdmin("A new job has been posted.", jobId: job.Id);


//                return RedirectToAction("Index", new { companyId = company.Id });
//            }
//            return View(job);
//        }

//        //public async Task<IActionResult> CreateJob(Job job)
//        //{
//        //    if (ModelState.IsValid)
//        //    {
//        //        job.PostStatus = JobPostStatus.Pending;
//        //        // Ensure the job is linked to the logged-in company
//        //        var companyEmail = User.Identity?.Name;
//        //        var company = _context.Companies.FirstOrDefault(c => c.Email == companyEmail);
//        //        if (company == null)
//        //        {
//        //            return RedirectToAction("CompanyRegistration", "Company");
//        //        }

//        //        job.CompanyId = company.Id;
//        //        job.DatePosted = DateTime.Now;
//        //        job.IsDeleted = false;
//        //        job.PostStatus = JobPostStatus.Pending;
//        //        _context.Jobs.Add(job);
//        //        await _context.SaveChangesAsync();



//        //        return RedirectToAction("Index", new { companyId = company.Id });
//        //    }
//        //    return View(job);
//        //}

//        public IActionResult ViewApplications(int jobId)
//        {
//            var applications = _context.Applications
//                .Include(a => a.User)
//                .Where(a => a.JobId == jobId)
//                .Select(a => new
//                {
//                    a.User.FirstName,
//                    a.User.LastName,
//                    a.User.Email,
//                    a.User.Address,
//                    a.Contact,
//                    a.Description,
//                    a.DateApplied
//                }).ToList();

//            return View(applications);
//        }

//        //Get
//        public IActionResult Edit(int? id)

//        {
//            if (id == null || id == 0)
//            {
//                return NotFound();
//            }
//            var JobsFromDb = _context.Jobs.Find(id);

//            if (JobsFromDb == null)
//            {
//                return NotFound();
//            }


//            return View(JobsFromDb);
//        }
//        //Post
//        [HttpPost]
//        [ValidateAntiForgeryToken]

//        public IActionResult Edit(Job obj)
//        {
//            if (ModelState.IsValid)
//            {
//                var jobFromDb = _context.Jobs.AsNoTracking().FirstOrDefault(j => j.Id == obj.Id);
//                if (jobFromDb != null)
//                {
//                    obj.CompanyId = jobFromDb.CompanyId;
//                    _context.Jobs.Update(obj);
//                    _context.SaveChanges();
//                    return RedirectToAction("Index", "Company");
//                }
//                return NotFound();
//            }
//            return View(obj);
//        }

//        //Get
//        public IActionResult Delete(int? id)

//        {
//            if (id == null || id == 0)
//            {
//                return NotFound();
//            }
//            var JobsFromDb = _context.Jobs.Find(id);
//            // var JobsFromDbFirst = _context.Jobs.FirstOrDefault(u => u.Id == id);
//            // var JobsFromDbSingle = _context.Jobs. SingleOrDefault(u => u.Id == id);

//            if (JobsFromDb == null)
//            {
//                return NotFound();
//            }


//            return View(JobsFromDb);
//        }
//        //Post
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public IActionResult DeletePOST(int? id)
//        {
//            var obj = _context.Jobs.Find(id);
//            if (obj == null)
//            {
//                return NotFound();
//            }

//            _context.Jobs.Remove(obj);
//            _context.SaveChanges();
//            return RedirectToAction("Index");
//        }


//        [HttpPost]
//        public IActionResult Index(string searchString)
//        {
//            if (string.IsNullOrWhiteSpace(searchString))
//            {

//                return RedirectToAction("Index");
//            }
//            var filteredJob = _context.Jobs

//                .AsEnumerable()
//                .Where(p => p.Location.Contains(searchString, StringComparison.OrdinalIgnoreCase)
//                || p.JobTitle.Contains(searchString, StringComparison.OrdinalIgnoreCase)
//                ||p.EmploymentType.Contains(searchString, StringComparison.OrdinalIgnoreCase))


//                .ToList();

//            return View("Index", filteredJob);
//        }


//    }


//}  




