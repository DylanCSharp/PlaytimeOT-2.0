using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PlaytimeOT.Models;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace PlaytimeOT.Controllers
{
    public class RegisterController : Controller
    {
        private IConfiguration _configuration;

        public RegisterController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string email, string password)
        {
            using SqlConnection conn = new(_configuration.GetConnectionString("PlaytimeDB"));
            SqlCommand command = new("PTD.PROC_REGISTER", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            conn.Open();
            command.Parameters.AddWithValue("@email", email);
            command.Parameters.AddWithValue("@LastName", password);

            return View();
        }
    }
}

