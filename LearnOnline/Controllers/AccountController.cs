
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authentication.Google;
using static System.Runtime.InteropServices.JavaScript.JSType;
using LearnOnline.Data;
using LearnOnline.Models;

namespace LearnOnline.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext db;

        public AccountController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(Auth a)
        {
            db.auths.Add(a);
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

            if (user == null)
            {
                TempData["Error"] = "Invalid email or password";
                return RedirectToAction("SignIn", "Account");
            }
            HttpContext.Session.SetString("myuser", user.Email);
            return RedirectToAction("Index", "Course");
        }
        public async Task Login()
        {

            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,

            new AuthenticationProperties

            {

                RedirectUri = Url.Action("GoogleResponse")


            });

        }

        public async Task<IActionResult> GoogleResponse()

        {

            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new

            {

                claim.Issuer,

                claim.OriginalIssuer,

                claim.Type,

                claim.Value

            });


            if (result.Succeeded)
            {


                return RedirectToAction("UploadVideo", "UploadVideos");
            }
            else
            {
                // Handle failed authentication
                return RedirectToAction("SignIn");
            }


        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("myuser");
            return RedirectToAction("SignIn", "Auth");
        }
    }
}
