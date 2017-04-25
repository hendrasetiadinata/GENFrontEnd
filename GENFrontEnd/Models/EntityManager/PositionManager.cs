using GENFrontEnd.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GENFrontEnd.Models.EntityManager
{
    public class PositionManager
    {
        public List <MPOSITION> GetListPosition(String PositionID)
        {
            List<MPOSITION> listPosition = new List<MPOSITION>();
            using (GENEntities db = new GENEntities())
            {
                if (!String.IsNullOrEmpty(PositionID))
                {
                    listPosition = db.MPOSITIONs.Where(x => x.IdPosition == PositionID && x.Active == 1).ToList();
                }
                else
                {
                    listPosition = db.MPOSITIONs.Where(x => x.Active == 1).ToList();
                }
            }
            return listPosition;
        }
        
    }
}