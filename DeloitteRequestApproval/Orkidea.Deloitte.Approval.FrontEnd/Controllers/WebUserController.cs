using Orkidea.Deloitte.Approval.Business;
using Orkidea.Deloitte.Approval.Entities;
using Orkidea.Deloitte.Approval.FrontEnd.Models;
using Orkidea.Framework.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Orkidea.Deloitte.Approval.FrontEnd.Controllers
{
    public class WebUserController : Controller
    {
        // GET: WebUser
        public ActionResult Index()
        {
            List<WebUser> users = BizWebUser.GetList().ToList();
            List<vmWebUser> userList = new List<vmWebUser>();
            
            foreach (WebUser item in users)
            {
                userList.Add(new vmWebUser()
                {
                    id = HexSerialization.StringToHex(Cryptography.Encrypt(item.name)),
                    name = item.name,
                    idRol = item.idRol,
                    idPais = item.idPais,
                    status = item.active ? "Activo" : "Inactivo"
                });
            }
            return View(userList);
        }

        // GET: WebUser/Create
        public ActionResult Create()
        {
            return View(new WebUser());
        }

        // POST: WebUser/Create
        [HttpPost]
        public ActionResult Create(WebUser webUser)
        {
            try
            {
                // TODO: Add insert logic here
                BizWebUser.Add(webUser);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: WebUser/Edit/5
        public ActionResult Edit(string id)
        {
            string name = Cryptography.Decrypt(HexSerialization.HexToString(id));

            return View(BizWebUser.GetSingle(name));
        }

        // POST: WebUser/Edit/5
        [HttpPost]
        public ActionResult Edit(string id, WebUser webUser)
        {
            try
            {
                // TODO: Add update logic here
                string name = Cryptography.Decrypt(HexSerialization.HexToString(id));
                webUser.name = name;
                BizWebUser.Update(Business.Enums.WebUserAction.New, webUser);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: WebUser/Delete/5
        public ActionResult Delete(string id)
        {
            string name = Cryptography.Decrypt(HexSerialization.HexToString(id));           
            BizWebUser.Remove(BizWebUser.GetSingle(name));

            return RedirectToAction("Index");
        }        

        public JsonResult GetRoles()
        {
            Dictionary<string, string> roles = new Dictionary<string, string>();
            string[] strRoles = ConfigurationManager.AppSettings["roles"].Split('|');

            foreach (string item in strRoles)
                roles.Add(item.Substring(0, 1), item);

            return Json(roles, JsonRequestBehavior.AllowGet);
        }
    }
}
