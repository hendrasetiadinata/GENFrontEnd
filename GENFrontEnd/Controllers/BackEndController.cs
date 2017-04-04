using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogic;
using GENFrontEnd.Models;
using Newtonsoft.Json;
using System.Collections.Specialized;

namespace GENFrontEnd.Controllers
{
    public class BackEndController : Controller
    {
        // GET: BackEnd
        BusinessLogic.Model.Message message = new BusinessLogic.Model.Message();
        BusinessLogic.Model.Email email = new BusinessLogic.Model.Email();
        Models.EntityManager.EmailManager emailManager = new Models.EntityManager.EmailManager();

        public ActionResult MasterEmail()
        {
            return View();
        }

        public String getQuestion()
        {
            var queryString = HttpContext.Request.QueryString;
            NameValueCollection nvc = HttpUtility.ParseQueryString(Convert.ToString(queryString));
            string EmailID = Convert.ToString(nvc["EmailID"]), 
                Result = "";
            if (EmailID != null)
            {
                email.EmailID = Convert.ToInt32(string.IsNullOrEmpty(EmailID)? "0" : EmailID);
                Result = JsonConvert.SerializeObject(emailManager.getEmailList(email));
            }
            return Result;
        }
    }
}