using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DemoWebApplication.Models;
using Npgsql;
using Newtonsoft.Json;

namespace DemoWebApplication.Controllers;

public class AuthenticationController : Controller
{
    private readonly ILogger<AuthenticationController> _logger;
    private IConfiguration _configuration;
    public AuthenticationController(IConfiguration configuration, ILogger<AuthenticationController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

     public IActionResult Login()
    {
        return View(new AuthenticationViewModel(){username="Test",password = "password@123"});
    }
    
    [HttpPost]
    public IActionResult Login(string username,string password)
    {
        if (username.Equals("Test") && password.Equals("password@123"))
        {
            CustomRole roleViewModelTemp = new CustomRole();
            string cs = this._configuration.GetSection("ConnectionStrings").GetSection("Dev").Value;
            using var con = new NpgsqlConnection(cs);
            con.Open();

            var sql = "Select id from UM where username ='" + username + "' and userpassword = '" + password +"'";
            using var cmd = new NpgsqlCommand(sql, con);
            var userid = cmd.ExecuteScalar();
            if (userid == null)
            {
                throw new UnauthorizedAccessException("Invalid username or passwrod.");
                //return ActionResult(i => i.Error(HttpResponse.)
            }

            //sql = "select * from userrole where userId =" + userid.ToString();

            //var cs = "Host=localhost;Username=postgres;Password=Dhaval@007;Database=TestDB";

            //string roleId = "3";
            
            sql = "SELECT * from Roles where role_id in (select roleid from userrole where userid =" + userid.ToString() + ")";
           
            long roleId = 0;
            string roleName = string.Empty;
            cmd.CommandText = sql;
            NpgsqlDataReader npgsqlDataReader = cmd.ExecuteReader();

            if (npgsqlDataReader.HasRows)
            {
                while (npgsqlDataReader.Read())
                {

                     //TempData["RoleId"] = npgsqlDataReader.GetInt64(0); ;
                     //TempData["RoleName"] = npgsqlDataReader.GetString(1);

                    roleId = npgsqlDataReader.GetInt64(0);
                    roleName = npgsqlDataReader.GetString(1);
                }
            }
            else
            {
                Console.WriteLine("No rows found.");
            }
            npgsqlDataReader.Close();

            //TempData["RoleId"] = roleId;
            //TempData["RoleName"] = roleName;

            RoleViewModel customRole = new RoleViewModel()
            {
                RoleId = roleId,
                RoleName = roleName
            };

            TempData["Role"] = JsonConvert.SerializeObject(customRole);

            return RedirectToAction("Privacy", "Home");
        }
        else
        {
            return RedirectToAction("Error", "Home");
        }
    }
}