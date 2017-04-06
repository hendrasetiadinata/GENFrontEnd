using BusinessLogic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GENFrontEnd.Models.DB;
using System.Reflection;
using System.ComponentModel;

namespace GENFrontEnd.Models.EntityManager
{
    public class EmployeeManager
    {
        Message msg = new Message();
        public Message InsertEmployee (MEMPLOYEE Employee)
        {
            try
            {
                MEMPLOYEE employee = new MEMPLOYEE();
                employee.EmployeeId = getEmployeeID();
                employee.UserId = Employee.UserId;
                employee.Password = Employee.Password;
                employee.Position = Employee.Position;
                employee.Supercoordinate = Employee.Supercoordinate;
                employee.Gender = Employee.Gender;
                employee.BranchID = Employee.BranchID;
                employee.PublicEmail = Employee.PublicEmail;
                employee.OfficeEmail = Employee.OfficeEmail;
                employee.CreatedDate = DateTime.Now;
                employee.CreatedBy = Employee.CreatedBy;
                employee.Active = Employee.Active;
                employee.Photo = Employee.Photo;
                employee.BirthDate = Employee.BirthDate;
                employee.Phone = Employee.Phone;
                employee.Phone2 = Employee.Phone2;
                employee.JoinDate = Employee.JoinDate;
                using (GENEntities db = new GENEntities())
                {
                    db.MEMPLOYEEs.Add(employee);
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
        public Message UpdateEmployee (MEMPLOYEE Employee)
        {
            try
            {
                using (GENEntities db = new GENEntities())
                {
                    var properties = Employee.GetType().GetProperties(BindingFlags.Public | BindingFlags.Static);
                    foreach (var data in properties)
                    {
                        var attribute = (DisplayNameAttribute)data.GetCustomAttribute(typeof(DisplayNameAttribute), true);
                        if (attribute != null)
                        {
                            
                        }
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
        public Int32 getEmployeeID()
        {
            var emailID = "";
            using (var db = new GENEntities())
            {
                try
                {
                    emailID = db.Database.SqlQuery<string>(String.Format(@"select dbo.GetEmployeeId()")).SingleOrDefault();
                }
                catch (Exception ex)
                {
                    msg.setMessage(null, ex.Message, "ERR0001");
                }
            }
            return Convert.ToInt32(String.IsNullOrEmpty(emailID) ? "0" : emailID);
        }
    }
}