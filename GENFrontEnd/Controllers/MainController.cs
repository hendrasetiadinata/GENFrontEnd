using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogic;
using GENFrontEnd.Models;
using Newtonsoft.Json;

namespace GENFrontEnd.Controllers
{
    public class MainController : Controller
    {
        // GET: Main
        BusinessLogic.Model.Message message = new BusinessLogic.Model.Message();
        BusinessLogic.Model.Email email = new BusinessLogic.Model.Email();
        Models.EntityManager.EmailManager emailManager = new Models.EntityManager.EmailManager();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Default()
        {
            return View();
        }
        public ActionResult Home()
        {
            ViewData["MAPS_API_KEY"] = Util.getProperty("MAPS_API_KEY");
            return View();
        }
        public ActionResult Program()
        {
            return View();
        }
        public ActionResult PartialSectionContact()
        {
            return View();
        }
        [HttpPost]
        public String SendQuestion(string SenderFullName, string SenderEmail, string SenderPhone, string SenderQuestion)
        {
            string Result = "";

            if (SenderFullName != null && SenderEmail != null && SenderPhone != null && SenderQuestion != null)
            {
                string SenderIP = System.Web.HttpContext.Current.Request.UserHostAddress;
                email = email.setSaveEmail(SenderFullName, SenderPhone, SenderEmail, SenderQuestion, SenderIP);
                message = emailManager.insert_Email(email);
                Result = JsonConvert.SerializeObject(message);
            }
            return Result;
        }
    }
}