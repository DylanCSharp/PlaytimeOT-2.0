using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PlaytimeOT.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PlaytimeOT.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration _configuration;
        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet("login")]
        public IActionResult Login(string returnURL)
        {
            ViewData["ReturnURL"] = returnURL;
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Validate(string email, string password, string returnURL)
        {
            using SqlConnection conn = new(_configuration.GetConnectionString("PlaytimeDB"));
            SqlCommand command = new("PTD.PROC_LOGIN", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            conn.Open();

            command.Parameters.AddWithValue("@email", email);
            command.Parameters.AddWithValue("@password", password);

            int loginResult = Convert.ToInt32(command.ExecuteScalar());

            command.Dispose();
            conn.Close();

            ViewData["ReturnUrl"] = returnURL;

            if (loginResult == 1)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, email),
                    new Claim(ClaimTypes.Email, email)
                };  
               
                var claimsId = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var claimsPrincipal = new ClaimsPrincipal(claimsId); 
                await HttpContext.SignInAsync(claimsPrincipal);

                return Redirect(returnURL);   
            }
            else
            {
                TempData["Error"] = "Username or password is incorrect!";
                return View("login");
            }
        }

        [Authorize] 
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return Redirect("/"); 
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
