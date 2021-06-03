using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PlaytimeOT.Controllers
{
    public class LoginController : Controller
    {

        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
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
    }
}
