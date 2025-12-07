using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using fitnessCenter.Models;
using Microsoft.Extensions.Primitives;

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
                    // if user redirect to user control 
                    // if cotch redirect to cotch control
                    if (man.whoIam == Roles.admin)
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (man.whoIam == Roles.cotch)
                    {
                        return RedirectToAction("Index", "Cotch");
                    }
                    else 
                    {
                        return RedirectToAction("Index", "User");
                    }
                }
                else
                {
                    TempData["err"] = "Wrong Password!!";
                    return RedirectToAction("Login");
                }
            }
            else 
            {
                TempData["err"] = "User not fount!!";
                return RedirectToAction("Login");
            }
        }
        TempData["err"] = "please fill all sections";
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
            m.passwordHash = PasswordHasher.HashPassword(m.password);
            m.whoIam = Roles.user;
            f_db.Add(m);
            f_db.SaveChanges();
            return RedirectToAction("Index", "User");
        }
        TempData["err"] = "please fill all sections right!!";
        return RedirectToAction("Register");
    }
    public IActionResult Info()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    //Forgot Password (GET)
    public IActionResult ForgotPassword()
    {
        return View();
    }

    //Forgot Password (POST)
    [HttpPost]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            ViewBag.Success = false;
            ViewBag.Message = "Please enter your email address.";
            return View();
        }

        string token = Guid.NewGuid().ToString();

        string resetLink = Url.Action(
            "ResetPassword",
            "Home",
            new { token = token },
            Request.Scheme
        );

        string subject = "Password Reset Link";
        string body =
            $"<h3>Password Reset</h3>" +
            $"<p>You can reset your password by clicking the link below:</p>" +
            $"<a href='{resetLink}'>Reset Password</a>";

        await _emailSender.SendEmailAsync(email, subject, body);

        ViewBag.Success = true;
        ViewBag.Message = @"
        <strong>The reset link has been sent to your email!</strong><br>
        Please check your inbox<br>
        <a href='https://mail.google.com' target='_blank' style='color:#4da3ff;font-weight:bold;'>
            Open Gmail
        </a>
    ";

        return View();
    }

    //Reset Password (GET)
    public IActionResult ResetPassword(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return BadRequest("Geçersiz token.");
        }

        ViewBag.Token = token;
        return View();
    }

    //Reset Password (POST)
    [HttpPost]
    public IActionResult ResetPassword(string newPassword, string confirmPassword, string token)
    {
        if (newPassword != confirmPassword)
        {
            ViewBag.Message = "Şifreler eşleşmiyor!";
            return View();
        }

        ViewBag.Success = true;
        return View();
    }

    // Default Error Handler
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
