using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using fitnessCenter.Models;
using fitnessCenter.Services;
using fitnessCenter.Attributes;
using Microsoft.EntityFrameworkCore;

namespace fitnessCenter.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly FitnessContext f_db;
    private readonly EmailSender _emailSender;

    public HomeController(ILogger<HomeController> logger, FitnessContext context, EmailSender emailSender)
    {
        _logger = logger;
        f_db = context;
        _emailSender = emailSender;
    }

    public IActionResult Index()
    {
        return View();
    }
    [Guest]
    public IActionResult Login()
    {
        // Auto-login check via Cookie
        if (HttpContext.Request.Cookies.TryGetValue("FitnessRememberMe", out string userIdStr))
        {
            if (int.TryParse(userIdStr, out int userId))
            {
                var man = f_db.men.FirstOrDefault(x => x.manId == userId);
                if (man != null)
                {
                    // Restore Session
                    HttpContext.Session.SetInt32("UserId", man.manId);
                    HttpContext.Session.SetString("UserName", man.userName);
                    HttpContext.Session.SetString("Role", man.whoIam.ToString());

                    if (man.whoIam == Roles.admin) return RedirectToAction("Index", "Admin");
                    else if (man.whoIam == Roles.cotch) return RedirectToAction("Index", "Cotch");
                    return RedirectToAction("Index", "User");
                }
            }
        }
        return View();
    }

    [HttpPost]
    public IActionResult LoginResult(Man m, bool rememberMe)
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
            if (man == null) // login with email
            {
                man = f_db.men.FirstOrDefault(x => x.email == m.userName);
            }
            
            if (man != null)
            {
                string hashed = PasswordHasher.HashPassword(m.password);
                string verify = man.passwordHash;

                if (hashed == verify)
                {
                    // save data at session
                    HttpContext.Session.SetInt32("UserId", man.manId);
                    HttpContext.Session.SetString("UserName", man.userName);
                    HttpContext.Session.SetString("Role", man.whoIam.ToString());

                    // Remember Me Cookie
                    if (rememberMe)
                    {
                        CookieOptions option = new CookieOptions
                        {
                            Expires = DateTime.Now.AddDays(7),
                            HttpOnly = true,
                            IsEssential = true
                        };
                        Response.Cookies.Append("FitnessRememberMe", man.manId.ToString(), option);
                    }

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
    [Guest]
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
            // check if username exists
            var usernameExists = f_db.users.Any(x => x.userName == m.userName);
            if (usernameExists)
            {
                TempData["err"] = "Username already exists!";
                return RedirectToAction("Register");
            }

            // check if email exists
            var emailExists = f_db.users.Any(x => x.email == m.email);
            if (emailExists)
            {
                TempData["err"] = "Email already exists!";
                return RedirectToAction("Register");
            }

            m.passwordHash = PasswordHasher.HashPassword(m.password);
            m.whoIam = Roles.user;

            User u = new User()
            {
                name = m.name,
                userName = m.userName,
                email = m.email,
                passwordHash = m.passwordHash,
                boy = m.boy,
                wight = m.wight,
                age = m.age,
                number = m.number,
                whoIam = Roles.user,
                subscribeStatus = "New"
            };

            f_db.users.Add(u);
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
    [Role] // Optional: Only logged-in users need to logout
    public IActionResult Logout()
    {
        // Clear all session data
        HttpContext.Session.Clear();
        
        // Delete Remember Me cookie
        Response.Cookies.Delete("FitnessRememberMe");

        return RedirectToAction("Login", "Home");
    }

    public IActionResult Notifications()
    {
        int? id = HttpContext.Session.GetInt32("UserId");
        if (id == null) return RedirectToAction("Login");

        var man = f_db.men
            .Include(m => m.notifications)
            .FirstOrDefault(u => u.manId == id);

        if (man == null) return RedirectToAction("Login");

        // Enforce Profile Completion (Weight & Height) for Active Users
        // Casting check to access wight and boy if man is a User
        if (man is User u)
        {
             if (u.subscribeStatus == "Active" && (u.wight == 0 || u.boy == 0))
             {
                 TempData["msg"] = "Please complete your profile (Weight & Height) first.";
                 return RedirectToAction("EditProfile", "User");
             }
        }

        // Sort notifications: Newest first
        if (man.notifications != null)
        {
            man.notifications = man.notifications.OrderByDescending(n => n.notId).ToList();
        }

        // Mark all as read
        var newlyReadIds = new HashSet<int>();
        if (man.notifications != null)
        {
            var unread = man.notifications.Where(n => !n.IsRead).ToList();
            if (unread.Any())
            {
                foreach (var n in unread)
                {
                    newlyReadIds.Add(n.notId);
                    n.IsRead = true;
                }
                f_db.SaveChanges();
            }
        }
        ViewBag.NewlyReadIds = newlyReadIds;

        return View(man);
    }

    public IActionResult Info()
    {
        return View();
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
