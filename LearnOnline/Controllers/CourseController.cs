using Microsoft.AspNetCore.Mvc;
using LearnOnline.Data;
using LearnOnline.Models;
using System.IO;

namespace LearnOnline.Controllers
{
    public class CourseController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment env;

        public CourseController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            this.db = db;
            this.env = env;
        }

        public IActionResult AddCourse()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddCourse(Coursethumb courseThumb)
        {
            if (ModelState.IsValid)
            {
                var path = env.WebRootPath;
                var filePath = "Content/Images/" + courseThumb.Thumbnail.FileName;
                var fullPath = Path.Combine(path, filePath);
                UploadFile(courseThumb.Thumbnail, fullPath);

                var course = new Course
                {
                    Name = courseThumb.Name,
                    Thumbnail = filePath, // Save the file path
                    Price = courseThumb.Price,
                    Description = courseThumb.Description
                };

                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("AddCourse");
            }
            return View(courseThumb);
        }

        public void UploadFile(IFormFile file, string path)
        {
            using (var stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }
        }

       



        [HttpGet]
        public IActionResult GetAllCourses()
        {
            var courses = db.Courses.Select(c => new { c.Id, c.Name }).ToList();
            return Json(courses);
        }

        [HttpGet]
        public IActionResult GetCourseDetails(int id)
        {
            var course = db.Courses.FirstOrDefault(c => c.Id == id);
            if (course == null)
            {
                return NotFound();
            }
            return PartialView("_CourseDetails", course);
        }

    }
}
