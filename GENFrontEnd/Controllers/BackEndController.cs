﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogic;
using GENFrontEnd.Models;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Net;
using System.Net.Mail;
using System.Text;
using GENFrontEnd.Models.DB;
using BusinessLogic.Model;

namespace GENFrontEnd.Controllers
{
    public class BackEndController : Controller
    {
        // GET: BackEnd
        BusinessLogic.Model.Message message = new BusinessLogic.Model.Message();
        BusinessLogic.Model.Email email = new BusinessLogic.Model.Email();
        Models.EntityManager.EmailManager emailManager = new Models.EntityManager.EmailManager();
        TREMAIL trEmail = new TREMAIL();

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
        [HttpPost]
        public String ReplyMessage (String EmailID, String ReplyMessage, String EmailCategory)
        {
            String Result = "";
            if (!String.IsNullOrEmpty(EmailID) && !String.IsNullOrEmpty(ReplyMessage) && !String.IsNullOrEmpty(EmailCategory))
            {
                email.EmailID = Convert.ToInt32(EmailID);
                email.ReplyMessage = ReplyMessage;
                email.EmailCategory = EmailCategory;
                message = emailManager.update_Email(email);
                Message getSendEmailStatus = sendEmail(email);
                if (message.ErrorCode != null || getSendEmailStatus.ErrorCode != null)
                {
                    message = emailManager.update_Email(email.setReplyEmail(Convert.ToInt32(EmailID), null, DateTime.Now, null, null));
                }
                Result = JsonConvert.SerializeObject(message);
            }
            return Result;
        }
        private void sendEmail2 ()
        {
            //CDO.Message
        }
        private Message sendEmail (BusinessLogic.Model.Email email)
        {
            Message msg = new Message();
            trEmail = emailManager.getEmail(email.EmailID);

            string to = trEmail.SenderEmail; //To address    
            string from = "gencourse@zoho.com"; //From address    
            MailMessage message = new MailMessage(from, to);

            string mailbody = trEmail.ReplyMessage;
            message.Subject = "GEN - Reply Message <gencourse@zoho.com>";
            message.Body = mailbody;
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = true;
            SmtpClient client = new SmtpClient("smtp.zoho.com", 465); //Zoho smtp    
            System.Net.NetworkCredential basicCredential1 = new System.Net.NetworkCredential("hendrasetiadinata18@gmail.com", "bozzy010077");
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = basicCredential1;
            try
            {
                client.Send(message);
                msg.Description = "Message send successfully";
                msg.MessageCode = "MSG0001";
            }
            catch (Exception ex)
            {
                msg.Description = "Send message is failed";
                msg.ErrorCode = "ERR0001";
                throw ex;
            }
            return msg;
        }
    }
}