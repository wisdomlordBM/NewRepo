

using Jobportalwebsite.Data;
using Jobportalwebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Jobportalwebsite.Controllers
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public BlogController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var blogs = await _context.Blogs
                .Where(b => !b.Deleted)
                .OrderByDescending(b => b.DateCreated)
                .ToListAsync();
            return View(blogs);
        }
        public async Task<IActionResult> Details(int id)
        {
            var blog = await _context.Blogs
                .Include(b => b.Comments)
                .FirstOrDefaultAsync(b => b.Id == id && !b.Deleted);

            if (blog == null)
            {
                return NotFound();
            }

            var blogUrl = Url.Action("Details", "Blog", new { id = blog.Id }, Request.Scheme);
            ViewData["BlogUrl"] = blogUrl;

            return View(blog);
        }


       


        [Authorize(Roles = "Company")]
        public IActionResult Create()
        {
            return View();
        }

 

        [HttpPost]
        [Authorize(Roles = "Company")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Blog blog, List<IFormFile> pictures)
        {
            if (ModelState.IsValid)
            {
                var picturePaths = new List<string>();

                if (pictures != null && pictures.Any())
                {
                    var uploadFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "Blog");

                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    foreach (var picture in pictures)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(picture.FileName);
                        var extension = Path.GetExtension(picture.FileName);
                        var uniqueFileName = fileName + "_" + System.Guid.NewGuid().ToString() + extension;

                        var filePath = Path.Combine(uploadFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await picture.CopyToAsync(stream);
                        }

                        picturePaths.Add("/uploads/Blog/" + uniqueFileName);
                    }
                }

                blog.PicturePaths = picturePaths; 
                blog.DateCreated = DateTime.Now;
                blog.CreatedBy = User.Identity.Name;

                _context.Blogs.Add(blog);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(blog);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int blogId, string commentText, bool isAnonymous)
        {
            var blog = await _context.Blogs.FindAsync(blogId);

            if (blog == null)
            {
                return NotFound();
            }

            var comment = new Comment
            {
                CommentText = commentText,
                DateCommented = DateTime.Now,
                IsAnonymous = isAnonymous,
                BlogId = blogId,
                CommentedBy = isAnonymous ? null : User.Identity.Name
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = blogId });
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs.FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

           
            if (blog.CreatedBy != User.Identity.Name)
            {
                return Forbid(); 
            }

            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);

            if (blog == null)
            {
                return NotFound();
            }

           
            if (blog.CreatedBy != User.Identity.Name)
            {
                return Forbid(); 
            }

            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


    }
}
