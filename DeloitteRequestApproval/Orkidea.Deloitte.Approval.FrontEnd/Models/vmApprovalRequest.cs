using Orkidea.Deloitte.Approval.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orkidea.Deloitte.Approval.FrontEnd.Models
{
    public class vmApprovalRequest : ApprovalRequest
    {
        public string distributionListName { get; set; }
        public string approvalStatus { get; set; }
        public string dateApprovalString { get; set; }
        public string cipherId { get; set; }
    }
}