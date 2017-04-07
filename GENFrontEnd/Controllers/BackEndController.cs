using System;
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
using System.Configuration;

namespace GENFrontEnd.Controllers
{
    public class BackEndController : Controller
    {
        // GET: BackEnd
        Encryption enc = new Encryption();
        BusinessLogic.Model.Message message = new BusinessLogic.Model.Message();
        BusinessLogic.Model.Email email = new BusinessLogic.Model.Email();
        Models.EntityManager.EmailManager emailManager = new Models.EntityManager.EmailManager();
        Models.EntityManager.LogManager logManager = new Models.EntityManager.LogManager();
        Models.EntityManager.EmployeeManager employeeManager = new Models.EntityManager.EmployeeManager();
        TREMAIL trEmail = new TREMAIL();
        TRLOG trLog = new TRLOG();
        MEMPLOYEE employee = new MEMPLOYEE();

        public ActionResult Login()
        {
            string Message = "";
            bool Status = false;
            if (Request.Form != null && Request.Form.AllKeys.Count() > 0)
            {
                NameValueCollection nvc = Request.Form;
                String Username = Convert.ToString(nvc["Username"]);
                String Password = Convert.ToString(nvc["Password"]);
                employee = employeeManager.getEmployee(Username);
                if (employee != null)
                {
                    if ((enc.Decrypt(employee.Password, "istananegara") == Password)
                        && (Username.Trim() == employee.UserId))
                    {
                        Message = "Login berhasil";

                        trLog = logManager.getLog(Convert.ToString(employee.EmployeeId));
                        string SessionID = Guid.NewGuid().ToString();

                        TRLOG sendLog = new TRLOG();
                        sendLog.EmployeeID = Convert.ToString(employee.EmployeeId);
                        sendLog.CreatedDate = DateTime.Now;
                        sendLog.SessionID = SessionID;
                        Message msgLog = new Message();
                        if (trLog != null)
                        {
                            msgLog = logManager.UpdateLog(sendLog);
                        }
                        else
                        {
                            msgLog = logManager.InsertLog(sendLog);
                        }
                        Status = true;
                        Session.Add("SESSIONID", Session);
                        Session.Add("EMPLOYEEID", employee.EmployeeId);
                    }
                    else
                    {
                        Message = "Password tidak valid";
                    }
                }
                else
                {
                    Message = "Username belum terdaftar";
                }
            }
            if (!Status)
            {
                Session["SESSIONID"] = null;
                Session["EMPLOYEEID"] = null;
            }
            ViewData["MESSAGE"] = Message;
            ViewData["STATUS"] = Status;
            return View();
        }

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
        private Message sendEmail (BusinessLogic.Model.Email email)
        {
            Message msg = new Message();
            trEmail = emailManager.getEmail(email.EmailID);
              
            MailMessage message = new MailMessage();
            message.From = new MailAddress(ConfigurationManager.AppSettings["USERNAMEMAIL"]);
            message.To.Add(new MailAddress(trEmail.SenderEmail));
            message.Subject = "GEN - Reply Message <gencourse@zoho.com>";
            message.Body = trEmail.ReplyMessage;
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = true;  

            NetworkCredential basicCredential1 = new NetworkCredential();
            basicCredential1.UserName = ConfigurationManager.AppSettings["USERNAMEMAIL"];
            basicCredential1.Password = ConfigurationManager.AppSettings["PASSWORDMAIL"];

            SmtpClient client = new SmtpClient();
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = basicCredential1;
            client.Host = ConfigurationManager.AppSettings["PASSWORDSMTP"];
            client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["PASSWORDSMTP"]);
            try
            {
                client.Send(message);
                msg.Description = "Message send successfully";
                msg.MessageCode = "MSG0001";
            }
            catch (Exception ex)
            {
                msg.Description = "Send message is failed, " + ex.Message;
                msg.ErrorCode = "ERR0001";
                throw ex;
            }
            return msg;
        }
    }
}