using System;
using System.Collections.Generic;
using System.Linq;
using GENFrontEnd.Models.DB;

namespace GENFrontEnd.Models.EntityManager
{
    public class BranchManager
    {
        public List<MBRANCH> GetListBranch(String BranchID)
        {
            List<MBRANCH> listBranch = new List<MBRANCH>();
            using (GENEntities db = new GENEntities())
            {
                if (!String.IsNullOrEmpty(BranchID))
                {
                    listBranch = db.MBRANCHes.Where(x => x.BranchID == BranchID && x.Active == 1).ToList();
                }
                else
                {
                    listBranch = db.MBRANCHes.Where(x => x.Active == 1).ToList();
                }
            }
            return listBranch;
        }
    }
}