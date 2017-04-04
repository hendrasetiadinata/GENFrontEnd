using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessLogic;
using GENFrontEnd.Models.DB;
using System.Data.Linq.Mapping;
using Microsoft.Ajax.Utilities;
using BusinessLogic.Model;

namespace GENFrontEnd.Models.EntityManager
{
    public class EmailManager
    {
        Message msg = new Message();

        public Message insert_Email (BusinessLogic.Model.Email email)
        {
            try
            {
                TREMAIL trEmail = new TREMAIL();
                trEmail.EmailID = getEmailID();
                trEmail.SenderFullName = email.SenderFullName;
                trEmail.SenderEmail = email.SenderEmail;
                trEmail.SenderPhone = email.SenderPhone;
                trEmail.SenderQuestion = email.SenderQuestion;
                trEmail.SenderIP = email.SenderIP;
                trEmail.CreatedDate = DateTime.Now;
                GENEntities db = new GENEntities();
                db.TREMAILs.Add(trEmail);
                if (db.SaveChanges() > 0)
                {
                    msg.setMessage("MSG0001", "Data saved successfully", null);
                }
                else
                {
                    msg.setMessage("MSG0002", "Data could not be saved", null);
                }
            }
            catch (Exception ex)
            {
                msg.setMessage(null, ex.Message, "ERR0001");
            }
            return msg;
        }
        public Message update_Email(BusinessLogic.Model.Email email)
        {
            try
            {
                TREMAIL trEmail = new TREMAIL();
                trEmail.EmailID = 0;
                trEmail.IsReply = 1;
                trEmail.ReplyDate = DateTime.Now;
                trEmail.ReplyMessage = email.ReplyMessage;
                trEmail.CreatedBy = email.CreatedBy;
                trEmail.EmailCategory = email.EmailCategory;
                GENEntities db = new GENEntities();
                db.TREMAILs.Add(trEmail);
                if (db.SaveChanges() > 0)
                {
                    msg.setMessage("MSG0001", "Data updated successfully", null);
                }
                else
                {
                    msg.setMessage("MSG0002", "Data could not be updated", null);
                }
            }
            catch (Exception ex)
            {
                msg.setMessage(null, ex.Message, "ERR0001");
            }
            return msg;
        }
        public Int32 getEmailID()
        {
            var emailID = "";
            using (var db = new GENEntities())
            {
                try
                {
                    emailID = db.Database.SqlQuery<string>(String.Format(@"select dbo.GetEmailId()")).SingleOrDefault();
                }
                catch (Exception ex)
                {
                    msg.setMessage(null, ex.Message, "ERR0001");
                }
            }
            return Convert.ToInt32(emailID);
        }
        public List<TREMAIL> getEmailList(BusinessLogic.Model.Email email)
        {
            List<TREMAIL> list = new List<TREMAIL>();
            using (GENEntities db = new GENEntities())
            {
                list = db.Database.SqlQuery<TREMAIL>(@"EXEC SP_GETEMAIL '" + email.CreatedDate + "'").ToList();
            }
            return list;
        }
    }
}