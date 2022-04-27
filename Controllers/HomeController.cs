using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DemoWebApplication.Models;
using Newtonsoft.Json;

namespace DemoWebApplication.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        RoleViewModel roleViewModel = new RoleViewModel();
        long val = 0;
        string role = string.Empty;
        if (TempData.Count > 0)
        {
            //long.TryParse(TempData["RoleId"].ToString(), out val);
            //role = TempData["RoleName"] as string;
            roleViewModel= JsonConvert.DeserializeObject<RoleViewModel>((string)TempData["Role"]);
        }
        return View(roleViewModel);
    }

    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
