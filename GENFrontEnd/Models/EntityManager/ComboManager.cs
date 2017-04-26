using System;
using System.Collections.Generic;
using System.Linq;
using GENFrontEnd.Models.DB;

namespace GENFrontEnd.Models.EntityManager
{
    public class ComboManager
    {
        public List<MCOMBO> GetListCombo(String ComboID)
        {
            List<MCOMBO> listCombo = new List<MCOMBO>();
            using (GENEntities db = new GENEntities())
            {
                if (!String.IsNullOrEmpty(ComboID))
                {
                    listCombo = db.MCOMBOes.Where(x => x.ComboId == ComboID && x.Active == 1).ToList();
                }
                else
                {
                    listCombo = db.MCOMBOes.Where(x => x.Active == 1).ToList();
                }
            }
            return listCombo;
        }

        public List<MCOMBO> GetListCombo(String ComboID, String Category)
        {
            List<MCOMBO> listCombo = new List<MCOMBO>();
            using (GENEntities db = new GENEntities())
            {
                if (!String.IsNullOrEmpty(ComboID) && !String.IsNullOrEmpty(Category))
                {
                    listCombo = db.MCOMBOes.Where(x => x.Category == Category && x.ComboId == ComboID && x.Active == 1).ToList();
                }
                else
                {
                    listCombo = db.MCOMBOes.Where(x => x.Category == Category && x.Active == 1).ToList();
                }
            }
            return listCombo;
        }
    }
}