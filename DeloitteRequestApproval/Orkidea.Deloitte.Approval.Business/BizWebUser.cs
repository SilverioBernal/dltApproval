using Orkidea.Deloitte.Approval.Business.Enums;
using Orkidea.Deloitte.Approval.DAL;
using Orkidea.Deloitte.Approval.Entities;
using Orkidea.Framework.Security;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orkidea.Deloitte.Approval.Business
{
    public class BizWebUser
    {
        public static bool ValidateOnDomain(string userName, string password)
        {
            bool rtn = false;
            try
            {
                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, Constants.DomainName))
                {
                    rtn = pc.ValidateCredentials(userName, password);
                }
            }
            catch (Exception)
            {
                rtn = false;
            }

            return rtn;
        }

        public static WebUser GetSingle(string name)
        {
            string cipherName = HexSerialization.StringToHex(Cryptography.Encrypt(name.ToLower()));
            WebUser user = DbMngmt<WebUser>.GetSingle(c => c.name.Equals(cipherName));

            user.name = name;

            return user;
        }

        public static bool UserExists(string name)
        {
            string cipherName = HexSerialization.StringToHex(Cryptography.Encrypt(name.ToLower()));
            
            if (DbMngmt<WebUser>.GetSingle(c => c.name.Equals(cipherName)) != null)
                return true;
            else
                return false;
        }

        public static IList<WebUser> GetList()
        {
            List<WebUser> users = DbMngmt<WebUser>.GetList().ToList();

            foreach (WebUser user in users)            
                user.name = Cryptography.Decrypt(HexSerialization.HexToString(user.name));

            return users;
        }

        public static void Add(WebUser webUser)
        {
            try
            {
                webUser.name = HexSerialization.StringToHex(Cryptography.Encrypt(webUser.name));
                webUser.idPais = webUser.idPais.ToUpper();
                DbMngmt<WebUser>.Add(webUser);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void Update(WebUserAction action, WebUser webUser)
        {
            try
            {
                string name = HexSerialization.StringToHex(Cryptography.Encrypt(webUser.name));
                WebUser user = DbMngmt<WebUser>.GetSingle(x => x.name == name);
                switch (action)
                {                              
                    case WebUserAction.AdminReset:
                        //item.pass = Cryptography.Encrypt(HexSerialization.StringToHex(PasswordHelper.Generate()));
                        break;
                    case WebUserAction.UserReset:
                        //item.pass = Cryptography.Encrypt(HexSerialization.StringToHex(item.pass));
                        break;
                    case WebUserAction.Disable:
                        user.active = false;
                        break;
                    case WebUserAction.Enable:
                        user.active = true;
                        break;
                    default:
                        user.idPais = webUser.idPais;
                        user.idRol = webUser.idRol;
                        break;
                }

                DbMngmt<WebUser>.Update(user);

                //SendWebUserNotification(item, action);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void Remove(WebUser webUser)
        {
            WebUser user = DbMngmt<WebUser>.GetSingle(x => x.name == webUser.name);
            DbMngmt<WebUser>.Remove(user);
        }
    }
}
