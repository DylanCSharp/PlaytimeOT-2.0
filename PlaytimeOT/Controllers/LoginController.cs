using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PlaytimeOT.Models;
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
        private static string URL;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("login")]
        public IActionResult Login(string returnURL)
        {
            ViewData["ReturnURL"] = returnURL;
            URL = returnURL;
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Validate(string email, string password)
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

            ViewData["ReturnUrl"] = URL;

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

                return Redirect(URL);
            }
            else
            {
                TempData["Error"] = "Username or password is incorrect!";
                return View("login");
            }
        }

        [HttpGet("register")]
        public IActionResult Register(string returnURL)
        {
            ViewData["ReturnURL"] = returnURL;
            return View();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(string firstName, string lastName, string email, string password,string confirmPassword)
        {
            RegisterUser registerUser = new()
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = password,
                ConfirmPassword = confirmPassword
            };
            if (registerUser.ConfirmPassword != registerUser.Password)
            {
                return View();
            }
            using SqlConnection conn = new(_configuration.GetConnectionString("PlaytimeDB"));
            SqlCommand command = new("PTD.PROC_REGISTER", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            conn.Open();
            command.Parameters.AddWithValue("@firstName", registerUser.FirstName);
            command.Parameters.AddWithValue("@lastName", registerUser.LastName);
            command.Parameters.AddWithValue("@email", registerUser.Email);
            command.Parameters.AddWithValue("@password", registerUser.Password);

            command.ExecuteNonQuery();

            await Validate(registerUser.Email, registerUser.Password);

            return RedirectToAction("Index", "Home");
        }
    }
}
