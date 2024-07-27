using LearnOnline.Data;
using LearnOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace LearnOnline.Controllers
{
    public class UsersideController : Controller
    {
        private readonly ApplicationDbContext db;

        public UsersideController(ApplicationDbContext db)
        {
            this.db = db;
        }

        // Action to display all courses
        [Route("courses/all")]

        public IActionResult AllCourses()
        {
            var courses = db.Courses.ToList();
            return View(courses);
        }

        // Action to display user's purchased and cart courses
        // Action to display user's purchased and cart courses
        [Route("courses/mycourses")]
        public IActionResult MyCourses()
        {
            int? userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
            {
                return RedirectToAction("SignIn", "Auth");
            }

            var cartCourses = db.Carts.Where(c => c.UserId == userId)
                                      .Select(c => c.CourseId).ToList();
            var purchasedCourses = db.Carts.Where(c => c.UserId == userId && c.IsPurchased)
                                            .Select(c => c.CourseId).ToList();
            var courses = db.Courses.Where(c => cartCourses.Contains(c.Id)).ToList();

            ViewBag.PurchasedCourses = purchasedCourses;
            ViewBag.CartCourses = cartCourses;

            return View(courses);
        }


        // Action to add a course to the cart
        // Action to add a course to the cart
        [HttpPost]
        public IActionResult AddToCart(int courseId)
        {
            int? userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
            {
                return RedirectToAction("SignIn", "Auth");
            }

            // Check if the course is already in the cart
            var existingCartItem = db.Carts.FirstOrDefault(c => c.UserId == userId && c.CourseId == courseId && !c.IsPurchased);
            if (existingCartItem != null)
            {
                TempData["message"] = "The course is already in your cart.";
                return RedirectToAction("AllCourses");
            }

            var cartItem = new Cart
            {
                UserId = userId.Value,
                CourseId = courseId,
                IsPurchased = false // Initially set to false
            };
            db.Carts.Add(cartItem);
            db.SaveChanges();

            TempData["message"] = "Course added to cart successfully.";
            return RedirectToAction("MyCourses"); // Redirect to AllCourses after adding to cart
        }


        // Action to handle the purchase of all courses in the cart
        [HttpPost]
        // Action to handle the purchase of all courses in the cart
       
        public IActionResult BuyNow()
        {
            int? userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
            {
                return RedirectToAction("SignIn", "Auth");
            }

            return RedirectToAction("Index", "Checkout");
        }


        // Action to show videos of a purchased course
        [Route("courses/showvideos")]
        public IActionResult ShowVideos(int courseId)
        {
            int? userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
            {
                return RedirectToAction("SignIn", "Auth");
            }

            var selectedCourse = db.Courses.FirstOrDefault(c => c.Id == courseId);
            if (selectedCourse == null)
            {
                return RedirectToAction("MyCourses");
            }

            var purchasedCourses = db.Carts.Where(c => c.UserId == userId && c.IsPurchased)
                                            .Select(c => c.CourseId).ToList();
            if (!purchasedCourses.Contains(courseId))
            {
                return RedirectToAction("MyCourses"); // Redirect if user has not purchased the course
            }

            var uploads = db.Uploads.Where(u => u.Courses == selectedCourse.Name).ToList();
            ViewBag.Course = selectedCourse;
            return View(uploads);
        }

        // Action to remove a course from the cart
        [HttpPost]
        public IActionResult RemoveFromCart(int courseId)
        {
            int? userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
            {
                return RedirectToAction("SignIn", "Auth");
            }

            var cartItem = db.Carts.FirstOrDefault(c => c.UserId == userId && c.CourseId == courseId && !c.IsPurchased);
            if (cartItem != null)
            {
                db.Carts.Remove(cartItem);
                db.SaveChanges();

                TempData["message"] = "Course removed from cart successfully.";
            }
            else
            {
                TempData["message"] = "Course not found in your cart.";
            }

            return RedirectToAction("MyCourses");
        }

    }
}
