using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using fitnessCenter.Models;
using Microsoft.Extensions.Primitives;

namespace fitnessCenter.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private FitnessContext f_db = new FitnessContext();

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
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
            // if every thing is okay
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
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
