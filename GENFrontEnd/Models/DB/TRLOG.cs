using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GENFrontEnd.Models.DB
{
    public partial class TRLOG
    {
        public String EmployeeID { get; set; }
        public String SessionID { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}