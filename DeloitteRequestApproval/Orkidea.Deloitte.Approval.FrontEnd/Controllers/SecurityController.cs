using Orkidea.Deloitte.Approval.Business;
using Orkidea.Deloitte.Approval.Entities;
using Orkidea.Deloitte.Approval.FrontEnd.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Orkidea.Deloitte.Approval.FrontEnd.Controllers
{
    public class SecurityController : Controller
    {
        //
        // GET: /Security/
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            LoginViewModel model = new LoginViewModel();

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            #region user exists
            if (!BizWebUser.UserExists(model.Email))
            {
                ViewBag.ErrorMessage = "Usuario no existe";
                return View(model);
            }
            #endregion

            #region Authentication
            string userName = model.Email.ToLower();
            string password = model.Password;

            if (!BizWebUser.ValidateOnDomain(userName, password))
            {
                ViewBag.ErrorMessage = "Usuario o contraseña errados";
                return View(model);
            }
            else
            {
                #region SESSION OBJECTS
                FormsAuthentication.SetAuthCookie(model.Email, false);

                WebUser userTarget = BizWebUser.GetSingle(model.Email.ToLower());

                string id = userTarget.name;
                string rol = userTarget.idRol;

                string userData = string.Format("{0}|{1}", id.Trim(), rol.Trim());

                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, model.Email, DateTime.Now, DateTime.Now.AddMinutes(30), false, userData);

                string encTicket = FormsAuthentication.Encrypt(ticket);
                HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                HttpContext.Response.Cookies.Add(faCookie);

                HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

                if (authCookie != null)
                {
                    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                    CustomIdentity identity = new CustomIdentity(authTicket.Name, userData);
                    GenericPrincipal newUser = new GenericPrincipal(identity, new string[] { });
                    HttpContext.User = newUser;
                }
                #endregion

                if (!string.IsNullOrEmpty(returnUrl))
                    return RedirectToLocal(returnUrl);
                else
                    return RedirectToAction("index", "Home");
            }
            #endregion
        }

        private bool validateCredentials(LoginViewModel model)
        {
            //autenticacion Deloitte
            bool res = true;

            // Code Here

            return res;
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                //return RedirectToAction("Index", "Home");
                return RedirectToAction
                ("Login");
            }
        }

        [Authorize]
        public ActionResult Logout()
        {
            Session.Abandon();
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }
    }
}