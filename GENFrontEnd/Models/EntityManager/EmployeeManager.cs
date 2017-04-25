using BusinessLogic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GENFrontEnd.Models.DB;
using System.Reflection;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Web.Mvc;

namespace GENFrontEnd.Models.EntityManager
{
    public class EmployeeManager
    {
        Message msg = new Message();
        List<MEMPLOYEE> listEmployee = new List<MEMPLOYEE>();

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

        public Message UpdateEmployee(MEMPLOYEE Employee)
        {
            try
            {
                MEMPLOYEE employee = getEmployee(Employee.EmployeeId);
                employee.Password = Employee.Password;
                employee.Position = Employee.Position;
                employee.Supercoordinate = Employee.Supercoordinate;
                employee.Gender = Employee.Gender;
                employee.BranchID = Employee.BranchID;
                employee.PublicEmail = Employee.PublicEmail;
                employee.OfficeEmail = Employee.OfficeEmail;
                employee.UpdateDate = DateTime.Now;
                employee.UpdateBy = Employee.CreatedBy;
                employee.Active = Employee.Active;
                employee.Photo = Employee.Photo;
                employee.BirthDate = Employee.BirthDate;
                employee.Phone = Employee.Phone;
                employee.Phone2 = Employee.Phone2;
                employee.JoinDate = Employee.JoinDate;
                using (GENEntities db = new GENEntities())
                {
                    db.Entry(employee).State = EntityState.Modified;
                    if (db.SaveChanges() > 0)
                    {
                        msg.setMessage("MSG0001", "Data updated successfully", null);
                    }
                    else
                    {
                        msg.setMessage("MSG0002", "Data could not be saved", null);
                    }
                }
            }
            catch (Exception ex)
            {
                msg.setMessage("MSG0002", "Data could not be updated", null);
                throw ex;
            }
            return msg;
        }

        public Message UpdateEmployee (MEMPLOYEE entity, Expression<Func<MEMPLOYEE, object>>[] properties)
        {
            try
            {
                using (GENEntities dbContext = new GENEntities())
                {
                    dbContext.Entry(entity).State = EntityState.Unchanged;
                    foreach (var property in properties)
                    {
                        var propertyName = ExpressionHelper.GetExpressionText(property);
                        dbContext.Entry(entity).Property(propertyName).IsModified = true;
                    }
                    if (dbContext.SaveChanges() > 0)
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
                throw ex;
            }
            return msg;
        }

        public Message UpdateSession (MEMPLOYEE Employee)
        {
            try
            {
                using (GENEntities db = new GENEntities())
                {
                    MEMPLOYEE getEmp = getEmployee(Employee.EmployeeId);
                    getEmp.SessionID = Employee.SessionID;
                    db.Entry(getEmp).State = EntityState.Modified;
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
                throw ex;
            }
            return msg;
        }

        public Message UpdatePassword(MEMPLOYEE Employee)
        {
            try
            {
                using (GENEntities db = new GENEntities())
                {
                    MEMPLOYEE getEmp = getEmployee(Employee.EmployeeId);
                    getEmp.Password = Employee.Password;
                    db.Entry(getEmp).State = EntityState.Modified;
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
                throw ex;
            }
            return msg;
        }

        public List<MEMPLOYEE> ListEmployee (String Active)
        {
            listEmployee = new List<MEMPLOYEE>();
            try
            {
                using (GENEntities db = new GENEntities())
                {
                    if (Active == "1")
                    {
                        listEmployee = db.MEMPLOYEEs.Where(x => x.Active == 1).ToList();
                    }
                    else if (Active == "0")
                    {
                        listEmployee = db.MEMPLOYEEs.Where(x => x.Active == 0).ToList();
                    }
                    else
                    {
                        listEmployee = db.MEMPLOYEEs.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                msg.setMessage(null, ex.Message, "ERR0001");
                throw ex;
            }
            return listEmployee;
        }

        public MEMPLOYEE getEmployee(String EmployeeID)
        {
            MEMPLOYEE employee = new MEMPLOYEE();
            if (EmployeeID != null)
            {
                using (GENEntities db = new GENEntities())
                {
                    employee = db.MEMPLOYEEs.Where(x => x.Active == 1 && x.EmployeeId == EmployeeID).SingleOrDefault();
                }
            }
            return employee;
        }

        public MEMPLOYEE getEmployeeByUsername(String Username)
        {
            MEMPLOYEE employee = new MEMPLOYEE();
            if (Username != null)
            {
                using (GENEntities db = new GENEntities())
                {
                    employee = db.MEMPLOYEEs.Where(x => x.Active == 1 && x.UserId == Username).SingleOrDefault();
                }
            }
            return employee;
        }

        public String getEmployeeID()
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
            return (String.IsNullOrEmpty(emailID) ? "0" : emailID);
        }

    }
}