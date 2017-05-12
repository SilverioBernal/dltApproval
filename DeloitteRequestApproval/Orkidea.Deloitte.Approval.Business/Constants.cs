using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orkidea.Deloitte.Approval.Business
{
    public class Constants
    {
        public static string DomainName
        {
            get
            {
                string rtn = "";
                try
                {
                    rtn = ConfigurationManager.AppSettings["DomainName"].ToString();
                }
                catch (Exception)
                {
                    rtn = "";

                }
                return rtn;
            }
        }

    }
}
