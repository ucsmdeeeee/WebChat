using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Register()
    {
        return View();
    }
    public IActionResult Logs()
    {
        return View();
    }

    public IActionResult Messages()
    {
        return View();
    }
}
