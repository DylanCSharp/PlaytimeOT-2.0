using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PlaytimeOT.Models;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace PlaytimeOT.Controllers
{
    public class RegController
         public string value = "";

    [HttpGet]
    public ActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Index(Reg r)
    {
        if (Request.HttpMethod == "POST")
        {
            Enroll er = new Enroll();
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("PlaytimeDB")))
            {
                using (SqlCommand cmd = new SqlCommand("PTD.PROC_Reg", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FirstName", r.FirstName);
                    cmd.Parameters.AddWithValue("@LastName",r.LastName);
                    cmd.Parameters.AddWithValue("@Password", e.Password);
                   // cmd.Parameters.AddWithValue("@Gender", e.Gender);
                    cmd.Parameters.AddWithValue("@Email", r.Email);
                    //cmd.Parameters.AddWithValue("@Phone", e.PhoneNumber);
                    //cmd.Parameters.AddWithValue("@SecurityAnwser", e.SecurityAnwser);
                    //cmd.Parameters.AddWithValue("@status", "INSERT");
                    con.Open();
                    ViewData["result"] = cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        }
        return View();
    }  
          
    
    }

