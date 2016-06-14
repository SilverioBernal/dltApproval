using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace Orkidea.Deloitte.Approval.FrontEnd.Models
{
    public class CustomIdentity : IIdentity
    {
        public CustomIdentity(string name, string id)
        {
            AuthenticationType = "Forms";
            IsAuthenticated = true;

            string[] info = id.Split('|');
            Name = name;
            Id = info[0];
            Rol = info[1];            
            
        }

        public string AuthenticationType { get; private set; }
        public bool IsAuthenticated { get; private set; }
        public string Name { get; private set; }
        public string Id { get; private set; }
        public string Rol { get; private set; }        
        
    }
}