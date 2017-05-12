using Orkidea.Deloitte.Approval.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orkidea.Deloitte.Approval.FrontEnd.Models
{
    public class vmWebUser:WebUser
    {
        public string id { get; set; }
        public string status { get; set; }
    }
}