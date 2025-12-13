using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using fitnessCenter.Models;
using fitnessCenter.Services;

namespace fitnessCenter.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private FitnessContext f_db = new FitnessContext();
    private readonly EmailSender _emailSender;

    public HomeController(ILogger<HomeController> logger, EmailSender emailSender)
    {
        _logger = logger;
        _emailSender = emailSender;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult LoginResult(Man m)
    {
        // remove unused validations
        ModelState.Remove("passwordHash");
        ModelState.Remove("name");
        ModelState.Remove("age");
        ModelState.Remove("email");
        ModelState.Remove("number");
        ModelState.Remove("compare_password");

        if (ModelState.IsValid)
        {
            // if form right
            var man = f_db.men.FirstOrDefault(x => x.userName == m.userName);

            if (man != null)
            {
                string hashed = PasswordHasher.HashPassword(m.password);
                string verify = man.passwordHash;

                //bool rs = PasswordHasher.VerifyPassword(hashed, verify);
                if (hashed == verify)
                {
                    // verify is done you can login
                    // if user redirect to admin / coach / user control
                    if (man.whoIam == Roles.admin)
                        return RedirectToAction("Index", "Admin");
                    else if (man.whoIam == Roles.cotch)
                        return RedirectToAction("Index", "Cotch");
                    else
                        return RedirectToAction("Index", "User");
                }
                else
                {
                    TempData["err"] = "Wrong Password!!";
                    return RedirectToAction("Login");
                }
            }
            else
            {
                TempData["err"] = "User not found!!";
                return RedirectToAction("Login");
            }
        }

        TempData["err"] = "Please fill all sections!";
        return RedirectToAction("Login");
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult RegisterResult(Man m)
    {
        ModelState.Remove("passwordHash");

        if (ModelState.IsValid)
        {
            // if everything is okay
            m.passwordHash = PasswordHasher.HashPassword(m.password);
            m.whoIam = Roles.user;

            f_db.men.Add(m);
            f_db.SaveChanges();

            return RedirectToAction("Login");
        }

        TempData["err"] = "Please fill all sections correctly!!";
        return RedirectToAction("Register");
    }

    // Forgot Password (GET)
    public IActionResult ForgotPassword()
    {
        return View();
    }

    // send email function
    private async void SendResetPasswordEmail(string email)
    {
        // build reset password link
        string resetLink = Url.Action(
            "ResetPassword",
            "Home",
            new { email = email },
            Request.Scheme
        );

        string subject = "Password Reset Link";
        string body =
            "<h3>Password Reset</h3>" +
            "<p>You can reset your password by clicking the link below:</p>" +
            $"<a href='{resetLink}'>Reset Password</a>";

        // send email
        await _emailSender.SendEmailAsync(email, subject, body);
    }

    // Forgot Password (POST)
    [HttpPost]
    public IActionResult ForgotPasswordResult(Man m)
    {
        ModelState.Remove("passwordHash");
        ModelState.Remove("name");
        ModelState.Remove("age");
        ModelState.Remove("userName");
        ModelState.Remove("number");
        ModelState.Remove("compare_password");
        ModelState.Remove("password");

        if (ModelState.IsValid)
        {
            // check if email exists
            var man = f_db.men.FirstOrDefault(x => x.email == m.email);

            if (man == null)
            {
                TempData["err"] = "User Not Found!!";
                return RedirectToAction("ForgotPassword");
            }

            // send reset email
            SendResetPasswordEmail(man.email);
            return RedirectToAction("SendingEmail");
        }

        TempData["err"] = "Please enter a valid email!";
        return RedirectToAction("ForgotPassword");
    }

    public IActionResult SendingEmail()
    {
        return View();
    }

    // Reset Password (GET)
    public IActionResult ResetPassword(string email)
    {
        if (string.IsNullOrEmpty(email))
            return BadRequest("Invalid request");

        ViewBag.Email = email;
        return View();
    }

    // Reset Password (POST)
    [HttpPost]
    public IActionResult ResetPassword(string email, string newPassword, string confirmPassword)
    {
        // check passwords match
        if (newPassword != confirmPassword)
        {
            ViewBag.Message = "Passwords do not match!";
            ViewBag.Email = email;
            return View();
        }

        // find user by email
        var user = f_db.men.FirstOrDefault(x => x.email == email);

        if (user == null)
        {
            ViewBag.Message = "User not found!";
            return View();
        }

        // update password
        user.passwordHash = PasswordHasher.HashPassword(newPassword);
        f_db.SaveChanges();

        TempData["Success"] = "Password changed successfully. Please login.";
        return RedirectToAction("Login");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
