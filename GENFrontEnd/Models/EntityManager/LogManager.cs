using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GENFrontEnd.Models.DB;
using BusinessLogic.Model;

namespace GENFrontEnd.Models.EntityManager
{
    public class LogManager
    {
        Message msg = new Message();

        public Message InsertLog(TRLOG Log)
        {
            try
            {
                TRLOG InsertLog = new TRLOG();
                InsertLog.EmployeeID = Log.EmployeeID;
                InsertLog.SessionID = Log.SessionID;
                InsertLog.CreatedDate = Log.CreatedDate;
                using (GENEntities db = new GENEntities())
                {
                    db.TRLOGs.Add(InsertLog);
                    if (db.SaveChanges() > 0)
                    {
                        msg.setMessage("MSG0001", "Data saved successfully", null);
                    }
                    else
                    {
                        msg.setMessage("MSG0002", "Data could not be saved", null);
                    }
                }
            }
            catch (Exception ex)
            {
                msg.setMessage(null, ex.Message, "ERR0001");
                throw ex;
            }
            return msg;
        }
        public Message UpdateLog(TRLOG Log)
        {
            try
            {
                TRLOG UpdateLog = getLog(Log.EmployeeID);
                UpdateLog.SessionID = Guid.NewGuid().ToString();
                UpdateLog.CreatedDate = DateTime.Now;
                using (GENEntities db = new GENEntities())
                {
                    db.Entry(UpdateLog).State = System.Data.Entity.EntityState.Modified;
                    if (db.SaveChanges() > 0)
                    {
                        msg.setMessage("MSG0001", "Data updated successfully", null);
                    }
                    else
                    {
                        msg.setMessage("MSG0002", "Data could not be updated", null);
                    }
                }
            }
            catch (Exception ex)
            {
                msg.setMessage(null, ex.Message, "ERR0001");
            }
            return msg;
        }
        public TRLOG getLog(string EmployeeID)
        {
            TRLOG Log = new TRLOG();
            try
            {
                using (GENEntities db = new GENEntities())
                {
                    Log = db.TRLOGs.Where(x => x.SessionID == EmployeeID).OrderByDescending(x => x.CreatedDate).LastOrDefault();
                }
            }
            catch (Exception ex)
            {
                msg.setMessage(null, ex.Message, "ERR0001");
            }
            return Log;
        }
    }
}