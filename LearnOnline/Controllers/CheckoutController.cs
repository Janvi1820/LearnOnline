using LearnOnline.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Razorpay.Api;

namespace LearnOnline.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext db;

        public CheckoutController(ApplicationDbContext db)
        {
            this.db = db;
        }

        // Display the checkout page
        public IActionResult Index()
        {
            int? userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
            {
                return RedirectToAction("SignIn", "Auth");
            }

            var cartItems = db.Carts.Include(c => c.Course)
                                    .Where(c => c.UserId == userId && !c.IsPurchased)
                                    .Select(c => new
                                    {
                                        c.CourseId,
                                        c.Course.Name,
                                        c.Course.Price
                                    })
                                    .ToList();

            var totalPrice = cartItems.Sum(c => c.Price);

            ViewBag.TotalPrice = totalPrice;
            ViewBag.CartItems = cartItems;

            return View();
        }


        // Handle placing the order
        [HttpPost]

        public IActionResult PlaceOrder()
        {
            int? userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
            {
                return RedirectToAction("SignIn", "Auth");
            }

            var cartItems = db.Carts.Include(c => c.Course).Where(c => c.UserId == userId && !c.IsPurchased).ToList();
            if (!cartItems.Any())
            {
                TempData["error"] = "Your cart is empty!";
                return RedirectToAction("Index");
            }

            var totalPrice = cartItems.Sum(c => c.Course.Price) * 100; // Convert to paise

            // Create Razorpay order
            var client = new RazorpayClient("rzp_test_Kl7588Yie2yJTV", "6dN9Nqs7M6HPFMlL45AhaTgp");
            var options = new Dictionary<string, object>
    {
        { "amount", totalPrice },
        { "currency", "INR" },
        { "receipt", Guid.NewGuid().ToString() }
    };
            Order order = client.Order.Create(options);

            ViewBag.OrderId = order["id"].ToString();
            ViewBag.TotalPrice = totalPrice;

            return View("PaymentPage");
        }



        // Razorpay payment success callback
        public IActionResult PaymentSuccess(string razorpayPaymentId, string razorpayOrderId, string razorpaySignature)
        {
            // Verify payment signature and process the order
            // Here you can update your database to mark the courses as purchased

            int? userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
            {
                return RedirectToAction("SignIn", "Auth");
            }

            var cartItems = db.Carts.Where(c => c.UserId == userId && !c.IsPurchased).ToList();
            foreach (var item in cartItems)
            {
                item.IsPurchased = true;
            }
            db.SaveChanges();

            return RedirectToAction("MyCourses", "Userside");
        }

    }
}
//using LearnOnline.Data;
//using LearnOnline.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Razorpay.Api;
//using Rotativa.AspNetCore.Options;
//using System.Drawing.Imaging;
//using System.Drawing.Printing;
//using DinkToPdf;
//using DinkToPdf.Contracts;

//namespace LearnOnline.Controllers
//{
//    public class CheckoutController : Controller
//    {
//        private readonly ApplicationDbContext db;
//        private readonly IConverter _pdfConverter;
//        public CheckoutController(ApplicationDbContext db, IConverter pdfConverter)
//        {
//            this.db = db;
//            _pdfConverter = pdfConverter;
//        }

//        // Display the checkout page
//        public IActionResult Index()
//        {
//            int? userId = HttpContext.Session.GetInt32("userId");
//            if (userId == null)
//            {
//                return RedirectToAction("SignIn", "Auth");
//            }

//            var cartItems = db.Carts.Include(c => c.Course)
//                                    .Where(c => c.UserId == userId && !c.IsPurchased)
//                                    .Select(c => new
//                                    {
//                                        c.CourseId,
//                                        c.Course.Name,
//                                        c.Course.Price
//                                    })
//                                    .ToList();

//            var totalPrice = cartItems.Sum(c => c.Price);

//            ViewBag.TotalPrice = totalPrice;
//            ViewBag.CartItems = cartItems;

//            return View();
//        }

//        // Handle placing the order
//        [HttpPost]
//        public IActionResult PlaceOrder()
//        {
//            int? userId = HttpContext.Session.GetInt32("userId");
//            if (userId == null)
//            {
//                return RedirectToAction("SignIn", "Auth");
//            }

//            var cartItems = db.Carts.Include(c => c.Course).Where(c => c.UserId == userId && !c.IsPurchased).ToList();
//            if (!cartItems.Any())
//            {
//                TempData["error"] = "Your cart is empty!";
//                return RedirectToAction("Index");
//            }

//            var totalPrice = cartItems.Sum(c => c.Course.Price) * 100; // Convert to paise

//            // Create Razorpay order
//            var client = new RazorpayClient("rzp_test_Kl7588Yie2yJTV", "6dN9Nqs7M6HPFMlL45AhaTgp");
//            var options = new Dictionary<string, object>
//            {
//                { "amount", totalPrice },
//                { "currency", "INR" },
//                { "receipt", Guid.NewGuid().ToString() }
//            };
//            Order order = client.Order.Create(options);

//            ViewBag.OrderId = order["id"].ToString();
//            ViewBag.TotalPrice = totalPrice;

//            return View("PaymentPage");
//        }

//        // Razorpay payment success callback
//        public IActionResult PaymentSuccess(string razorpayPaymentId, string razorpayOrderId, string razorpaySignature)
//        {
//            int? userId = HttpContext.Session.GetInt32("userId");
//            if (userId == null)
//            {
//                return RedirectToAction("SignIn", "Auth");
//            }

//            var cartItems = db.Carts.Where(c => c.UserId == userId && !c.IsPurchased).ToList();
//            foreach (var item in cartItems)
//            {
//                item.IsPurchased = true;
//            }
//            db.SaveChanges();

//            return RedirectToAction("DownloadInvoice", new { userId = userId.Value });
//        }

//        // Action to generate invoice
//        public IActionResult DownloadInvoice(int userId)
//        {
//            var model = GetInvoiceViewModel(userId);

//            var htmlContent = GenerateInvoiceHtml(model); // Generate HTML content for the invoice

//            var doc = new DinkToPdf.HtmlToPdfDocument()
//            {
//                GlobalSettings = new DinkToPdf.GlobalSettings
//                {
//                    ColorMode = DinkToPdf.ColorMode.Color,
//                    Orientation = DinkToPdf.Orientation.Portrait,
//                    PaperSize = DinkToPdf.PaperKind.A4
//                },
//                Objects = {
//        new DinkToPdf.ObjectSettings
//        {
//            HtmlContent = htmlContent,
//            WebSettings = { DefaultEncoding = "utf-8" }
//        }
//    }
//            };

//            var pdfBytes = _pdfConverter.Convert(doc); // Use _pdfConverter to convert HTML to PDF

//            return File(pdfBytes, "application/pdf", "Invoice.pdf");
//        }

//        private string GenerateInvoiceHtml(InvoiceViewModel model)
//        {
//            // Generate HTML content for the invoice using Razor syntax or manually construct it
//            // Example:
//            var htmlContent = $@"
//                <html>
//                <head><title>Invoice</title></head>
//                <body>
//                    <h1>Invoice for {model.UserName}</h1>
//                    <p>Email: {model.UserEmail}</p>
//                    <table border='1'>
//                        <thead>
//                            <tr>
//                                <th>Course Name</th>
//                                <th>Price</th>
//                            </tr>
//                        </thead>
//                        <tbody>";

//            foreach (var item in model.Items)
//            {
//                htmlContent += $@"
//                    <tr>
//                        <td>{item.Course.Name}</td>
//                        <td>{item.Course.Price}</td>
//                    </tr>";
//            }

//            htmlContent += $@"
//                        </tbody>
//                    </table>
//                    <p>Total Price: {model.TotalPrice}</p>
//                </body>
//                </html>";

//            return htmlContent;
//        }

//        private InvoiceViewModel GetInvoiceViewModel(int userId)
//        {
//            var user = db.auths.Find(userId);
//            if (user == null)
//            {
//                throw new Exception($"User with ID {userId} not found.");
//            }

//            var cartItems = db.Carts.Include(c => c.Course)
//                                    .Where(c => c.UserId == userId && c.IsPurchased)
//                                    .ToList();

//            var totalPrice = cartItems.Sum(c => c.Course.Price);

//            return new InvoiceViewModel
//            {
//                UserName = user.Username,
//                UserEmail = user.Email,
//                Items = cartItems,
//                TotalPrice = totalPrice,
//                PaymentMethod = "Razorpay",
//                UserId = userId
//            };
//        }
//    }

//}


