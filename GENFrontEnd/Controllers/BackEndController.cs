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
using GENFrontEnd.Models.EntityManager;

namespace GENFrontEnd.Controllers
{
    public class BackEndController : Controller
    {
        // GET: BackEnd
        Encryption enc = new Encryption();
        Message message = new Message();
        Email email = new Email();
        EmailManager emailManager = new EmailManager();
        EmployeeManager employeeManager = new EmployeeManager();
        TREMAIL trEmail = new TREMAIL();
        MEMPLOYEE employee = new MEMPLOYEE();
        BranchManager branchManager = new BranchManager();
        PositionManager positionManager = new PositionManager();
        ComboManager comboManager = new ComboManager();

        public ActionResult Login()
        {
            string Message = "";
            bool Status = false;
            if (Request.Form != null && Request.Form.AllKeys.Count() > 0)
            {
                NameValueCollection nvc = Request.Form;
                String Username = Convert.ToString(nvc["Username"]);
                String Password = Convert.ToString(nvc["Password"]);
                String Md5Password = enc.getMd5Hash(Password);
                employee = employeeManager.getEmployeeByUsername(Username);
                if (employee != null)
                {
                    if ((employee.Password == Md5Password) && (Username.Trim() == employee.UserId))
                    {
                        Message = "Login berhasil";
                        
                        string SessionID = Guid.NewGuid().ToString();
                        employee.SessionID = SessionID;
                        Message updateSession = employeeManager.UpdateSession(employee);

                        Status = true;
                        Session.Add("SESSIONID", Session);
                        Session.Add("EMPLOYEEID", employee.EmployeeId);
                        Session.Add("COMPLETENAME", employee.EmployeeName);
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
            if (Status)
            {
                Response.Redirect("/BackEnd/Home");
            }
            else
            {
                Session["SESSIONID"] = null;
                Session["EMPLOYEEID"] = null;
                Session["COMPLETENAME"] = null;
            }
            ViewData["MESSAGE"] = Message;
            return View();
        }

        public ActionResult _LayoutPageLumino()
        {
            employee = employeeManager.getEmployee(Convert.ToString(Session["EMPLOYEEID"]));
            String EmployeeName = "";
            if (employee != null)
            {
                EmployeeName = employee.EmployeeName;
            }
            ViewData["EMPLOYEENAME"] = EmployeeName;
            return View();
        }

        public ActionResult Home()
        {
            return View();
        }

        public ActionResult MasterEmail()
        {
            return View();
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        public ActionResult MasterEmployee()
        {
            List<MBRANCH> ListBranch = branchManager.GetListBranch(null);
            List<MPOSITION> ListPosition = positionManager.GetListPosition(null);
            List<MCOMBO> ListCombo = comboManager.GetListCombo(null);

            var itemBranch = ListBranch.Select(x => new SelectListItem
            {
                Text = Convert.ToString(x.BranchName),
                Value = Convert.ToString(x.BranchID)
            });

            var itemPosition = ListPosition.Select(x => new SelectListItem
            {
                Text = Convert.ToString(x.PositionName),
                Value = Convert.ToString(x.IdPosition)
            });

            var itemCombo = ListCombo.Select(x => new SelectListItem
            {
                Text = Convert.ToString(x.Title),
                Value = Convert.ToString(x.ComboId)
            });

            ViewBag.ListBranch = itemBranch;
            ViewBag.ListPosition = itemPosition;
            ViewBag.ListCombo = itemCombo;

            return View();
        }

        [HttpPost]
        public String SaveChangePassword(String _CurrentPassword, String _NewPassword, String _ConfirmPassword)
        {
            if (!String.IsNullOrEmpty(_CurrentPassword) && !String.IsNullOrEmpty(_NewPassword) && !String.IsNullOrEmpty(_ConfirmPassword))
            {
                if (Request.Form != null && Request.Form.AllKeys.Count() > 0)
                {
                    employee = employeeManager.getEmployee(Convert.ToString(Session["EMPLOYEEID"]));
                    if (employee != null)
                    {
                        String CurrentPassword = enc.getMd5Hash(_CurrentPassword);
                        String NewPassword = _NewPassword;
                        String ConfirmPassword = _ConfirmPassword;
                        if (CurrentPassword == Convert.ToString(employee.Password))
                        {
                            if (NewPassword == ConfirmPassword)
                            {
                                MEMPLOYEE updatePass = new MEMPLOYEE();
                                updatePass.EmployeeId = employee.EmployeeId;
                                updatePass.Password = enc.getMd5Hash(NewPassword);
                                Message updTrans = employeeManager.UpdatePassword(updatePass);
                                if (updTrans != null && updTrans.ErrorCode == null)
                                {
                                    message = updTrans;
                                }
                                else
                                {
                                    message = message.getMessage(null, "Update password is failed, call administrator", "ERR001");
                                }
                            }
                            else
                            {
                                message = message.getMessage(null, "New password tidak sesuai dengan confirm password", "ERR001");
                            }
                        }
                        else
                        {
                            message = message.getMessage(null, "Confirm password tidak valid", "ERR001");
                        }
                    }
                }
            }
            return JsonConvert.SerializeObject(message);
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
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = basicCredential1;
            client.Host = ConfigurationManager.AppSettings["HOSTMAIL"];
            client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["PORT"]);
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

        public String getListEmployee()
        {
            var queryString = HttpContext.Request.QueryString;
            NameValueCollection nvc = HttpUtility.ParseQueryString(Convert.ToString(queryString));
            string EmployeeID = Convert.ToString(nvc["EmployeeID"]),
                Result = "";
            employee = employeeManager.getEmployee(Convert.ToString(Session["EMPLOYEEID"]));
            if (employee != null)
            {
                List<MEMPLOYEE> listEmployee = employeeManager.ListEmployee(null);
                Result = JsonConvert.SerializeObject(listEmployee);
            }
            return Result;
        }

        [HttpPost]
        public String InsertNewEmployee(String UserID, String Password, String EmployeeName, String Position,
            String Supercoordinate, String Gender, String BranchID, String PublicEmail, String OfficeEmail,
            String BirthDate, String Phone, String Phone2)
        {
            String Result = "";
            employee = employeeManager.getEmployee(Convert.ToString(Session["EMPLOYEEID"]));
            if (employee != null)
            {
                MEMPLOYEE insertEmployee = new MEMPLOYEE();
                insertEmployee.Active = 1;
                insertEmployee.BirthDate = Convert.ToDateTime(BirthDate);
                insertEmployee.BranchID = Convert.ToString(BranchID);
                insertEmployee.CreatedBy = employee.EmployeeId;
                insertEmployee.CreatedDate = DateTime.Now;
                insertEmployee.EmployeeName = EmployeeName;
                insertEmployee.Gender = Gender;
                insertEmployee.JoinDate = DateTime.Now;
                insertEmployee.OfficeEmail = OfficeEmail;
                insertEmployee.Password = Password;
                insertEmployee.Phone = Phone;
                insertEmployee.Phone2 = Phone2;
                insertEmployee.Position = Position;
                insertEmployee.PublicEmail = PublicEmail;
                insertEmployee.Supercoordinate = Supercoordinate;
                insertEmployee.UserId = UserID;
                message = employeeManager.InsertEmployee(insertEmployee);
                Result = JsonConvert.SerializeObject(message);
            }
            return Result;
        }

        [HttpPost]
        public String UpdateEmployee(String EmployeeID,String UserID, String Password, String EmployeeName, String Position,
            String Supercoordinate, String Gender, String BranchID, String PublicEmail, String OfficeEmail,
            String BirthDate, String Phone, String Phone2)
        {
            String Result = "";
            employee = employeeManager.getEmployee(Convert.ToString(Session["EMPLOYEEID"]));
            if (employee != null)
            {
                MEMPLOYEE updateEmployee = new MEMPLOYEE();
                updateEmployee.EmployeeId = EmployeeID;
                updateEmployee.Active = 1;
                updateEmployee.BirthDate = Convert.ToDateTime(BirthDate);
                updateEmployee.BranchID = Convert.ToString(BranchID);
                updateEmployee.CreatedBy = employee.EmployeeId;
                updateEmployee.CreatedDate = DateTime.Now;
                updateEmployee.EmployeeName = EmployeeName;
                updateEmployee.Gender = Gender;
                updateEmployee.JoinDate = DateTime.Now;
                updateEmployee.OfficeEmail = OfficeEmail;
                updateEmployee.Password = Password;
                updateEmployee.Phone = Phone;
                updateEmployee.Phone2 = Phone2;
                updateEmployee.Position = Position;
                updateEmployee.PublicEmail = PublicEmail;
                updateEmployee.Supercoordinate = Supercoordinate;
                updateEmployee.UserId = UserID;
                message = employeeManager.UpdateEmployee(updateEmployee);
                Result = JsonConvert.SerializeObject(message);
            }
            return Result;
        }
    }
}