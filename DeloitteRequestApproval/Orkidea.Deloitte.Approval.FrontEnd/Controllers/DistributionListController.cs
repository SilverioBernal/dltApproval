using Orkidea.Deloitte.Approval.Business;
using Orkidea.Deloitte.Approval.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Orkidea.Deloitte.Approval.FrontEnd.Controllers
{
    public class DistributionListController : Controller
    {
        // GET: DistributionList
        public ActionResult Index()
        {
            return View(BizDistributionList.GetList());
        }        

        // GET: DistributionList/Create
        public ActionResult Create()
        {
            DistributionList disList = new DistributionList();
            return View(disList);
        }

        // POST: DistributionList/Create
        [HttpPost]
        public ActionResult Create(DistributionList disList)
        {
            try
            {
                // TODO: Add insert logic here
                BizDistributionList.Add(disList);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: DistributionList/Edit/5
        public ActionResult Edit(int id)
        {
            DistributionList disList = BizDistributionList.GetSingle(id);
            return View(disList);
        }

        // POST: DistributionList/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, DistributionList disList)
        {
            try
            {
                // TODO: Add update logic here
                disList.id = id;
                BizDistributionList.Update(disList);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: DistributionList/Delete/5
        public ActionResult Delete(int id)
        {
            BizDistributionList.Remove(new DistributionList() { id = id });
            return RedirectToAction("Index");
        }

        public JsonResult GetAsyncDistributionList() 
        {
            List<DistributionList> distributionLists = BizDistributionList.GetList().ToList();

            return Json(distributionLists, JsonRequestBehavior.AllowGet);
        }
    }
}
