---How to use System.DirectoryServices.AccountManagement 

1. Add the references into solution.
4. Add a Key 'DomainName' into web.config file in the app settings tag and set the correspondant Domain.

**********************web.config file ********************************
  <appSettings>
    <add key="DomainName" value=XXXXXXX/>
  </appSettings>
**********************************************************************

**********************.cs Code****************************************
public class Constants
    {
        public static string DomainName
        {
            get
            {
                string rtn = "";
                try
                {
                    rtn = System.Configuration.ConfigurationManager.AppSettings["DomainName"].ToString();
                }
                catch (Exception)
                {
                    rtn = "";

                }
                return rtn;
            }
        }

    }
**********************************************************************

2. Create a new PrincipalContext, and set the parameters (context type and DomainName).

**********************************************************************
private bool ValidateOnDomain(string username, string password)
        {
            bool rtn = false;
            try
            {
                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, Constants.DomainName))
                {
                    rtn = pc.ValidateCredentials(username, password);
                }
            }
            catch (Exception)
            {
                rtn = false;
            }

            return rtn;
        }
**********************************************************************

3. ValidateCredentials with username and password.