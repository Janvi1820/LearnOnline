using LearnOnline.Data;
using LearnOnline.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LearnOnline.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext db;
        public AuthController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(Auth auth)
        {
            var userExists = db.auths.Any(u => u.Username == auth.Username || u.Email == auth.Email);

            if (userExists)
            {
                TempData["error"] = "Username or Email Id Already Exist";
                return RedirectToAction("SignUp");
            }

            auth.Role = "User"; // Assign default role as "User"
            db.auths.Add(auth);
            db.SaveChanges();

            return RedirectToAction("SignIn");
        }

        public IActionResult SignIn()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult SignIn(Login log)
        {
            var user = db.auths.SingleOrDefault(t => t.Email == log.Email && t.Password == log.Password);

            if (user != null)
            {
                HttpContext.Session.SetString("Role", user.Role);
                HttpContext.Session.SetInt32("userId", user.Id); // Store user ID in session
                HttpContext.Session.SetString("myuser", user.Email);

                if (user.Role == "Admin")
                {
                    return RedirectToAction("UploadVideo", "UploadVideos");
                }
                else if (user.Role == "User")
                {
                    return RedirectToAction("AllCourses", "Userside");
                }
            }
            else
            {
                TempData["error"] = "Invalid Email or Password!!";
                return RedirectToAction("SignIn");
            }

            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("userId");
            HttpContext.Session.Remove("myuser");
            return RedirectToAction("SignIn");
        }

        public async Task<IActionResult> GoogleLogin()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") });
            return new EmptyResult();
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                TempData["error"] = "Google login failed!";
                return RedirectToAction("SignIn");
            }

            var claims = result.Principal.Identities.FirstOrDefault().Claims;
            var emailClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            var email = emailClaim?.Value;

            var user = db.auths.SingleOrDefault(u => u.Email == email);
            if (user == null)
            {
                user = new Auth
                {
                    Email = email,
                    Username = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
                    Password = GenerateRandomPassword(), // Generate a random password
                    Role = "User" // Assign default role as "User"
                };
                db.auths.Add(user);
                db.SaveChanges();
            }

            HttpContext.Session.SetString("Role", user.Role);
            HttpContext.Session.SetInt32("userId", user.Id); // Store user ID in session
            HttpContext.Session.SetString("myuser", user.Email);

            if (user.Role == "Admin")
            {
                return RedirectToAction("UploadVideo", "UploadVideos");
            }
            else if (user.Role == "User")
            {
                return RedirectToAction("AllCourses", "Userside");
            }

            return RedirectToAction("SignIn");
        }

        //public IActionResult Logout()
        //{
        //    HttpContext.Session.Remove("userId");
        //    HttpContext.Session.Remove("myuser");
        //    return RedirectToAction("SignIn");
        //}

        private string GenerateRandomPassword(int length = 12)
        {
            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
            StringBuilder res = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];
                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(validChars[(int)(num % (uint)validChars.Length)]);
                }
            }
            return res.ToString();
        }
    }
}
