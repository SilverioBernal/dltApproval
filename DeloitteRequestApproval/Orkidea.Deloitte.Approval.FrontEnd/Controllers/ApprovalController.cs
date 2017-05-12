using Orkidea.Deloitte.Approval.Business;
using Orkidea.Deloitte.Approval.Entities;
using Orkidea.Deloitte.Approval.FrontEnd.Models;
using Orkidea.Framework.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace Orkidea.Deloitte.Approval.FrontEnd.Controllers
{
    public class ApprovalController : Controller
    {
        // GET: Approval
        [Authorize]
        public ActionResult Index()
        {
            IIdentity context = HttpContext.User.Identity;

            List<ApprovalRequest> appReqList = BizApprovalRequest.GetList(context.Name).OrderByDescending(x => x.dateRequest).ToList();
            List<DistributionList> disList = BizDistributionList.GetList().ToList();
            List<vmApprovalRequest> approvalRequestList = new List<vmApprovalRequest>();

            foreach (ApprovalRequest request in appReqList)
            {
                approvalRequestList.Add(new vmApprovalRequest()
                {
                    id = request.id,
                    cipherId = HexSerialization.StringToHex(Cryptography.Encrypt(request.id.ToString())),
                    dateRequest = request.dateRequest,
                    requestUser = request.requestUser,
                    request = request.request,
                    idDistributionList = request.idDistributionList,
                    distributionListName = disList.Where(x => x.id.Equals(request.idDistributionList)).Select(x => x.name).First(),
                    approved = request.approved,
                    approvalStatus = request.approved == null ? "Pendiente" : ((bool)request.approved ? "Aprobado" : "Rechazado"),
                    dateApproval = request.dateApproval,
                    dateApprovalString = request.dateApproval == null ? "" : (((DateTime)request.dateApproval).ToString("yyyy-MM-dd"))
                });
            }

            return View(approvalRequestList);
        }

        [Authorize]
        public ActionResult pendingRequest()
        {
            #region User identification
            IIdentity context = HttpContext.User.Identity;

            string idRol = "";

            if (context.IsAuthenticated)
            {
                System.Web.Security.FormsIdentity ci = (System.Web.Security.FormsIdentity)HttpContext.User.Identity;
                string[] userRole = ci.Ticket.UserData.Split('|');

                idRol = userRole[1];
            }
            #endregion

            #region Distribution list identification
            List<DistributionList> disList = new List<DistributionList>();

            foreach (var item in BizDistributionList.GetList().ToList())
            {
                if (item.email.Contains(context.Name))
                    disList.Add(item);
            }
            #endregion

            #region Requests identification
            List<ApprovalRequest> appReqList = new List<ApprovalRequest>();

            foreach (DistributionList item in disList)
                appReqList.AddRange(BizApprovalRequest.GetList(item.id).ToList());

            #endregion

            List<vmApprovalRequest> approvalRequestList = new List<vmApprovalRequest>();

            foreach (ApprovalRequest request in appReqList)
            {
                approvalRequestList.Add(new vmApprovalRequest()
                {
                    id = request.id,
                    cipherId = HexSerialization.StringToHex(Cryptography.Encrypt(request.id.ToString())),
                    dateRequest = request.dateRequest,
                    requestUser = request.requestUser,
                    request = request.request.Length > 50 ? string.Format("{0}...", request.request.Substring(0, 50)) : request.request,
                    idDistributionList = request.idDistributionList,
                    distributionListName = disList.Where(x => x.id.Equals(request.idDistributionList)).Select(x => x.name).First(),
                    approved = request.approved,
                    approvalStatus = request.approved == null ? "Pendiente" : ((bool)request.approved ? "Aprobado" : "Rechazado"),
                    dateApproval = request.dateApproval,
                    dateApprovalString = request.dateApproval == null ? "" : (((DateTime)request.dateApproval).ToString("yyyy-MM-dd"))
                });
            }

            return View(approvalRequestList);
        }

        // GET: Approval/Details/5
        [Authorize]
        public ActionResult Details(string id)
        {
            int realId = int.Parse(Cryptography.Decrypt(HexSerialization.HexToString(id)));
            ViewBag.cipherId = id;
            ApprovalRequest request = BizApprovalRequest.GetSingle(realId);

            return View(request);
        }

        [Authorize]
        public ActionResult Approve(string id)
        {
            int realId = int.Parse(Cryptography.Decrypt(HexSerialization.HexToString(id)));
            ViewBag.cipherId = id;
            ApprovalRequest request = BizApprovalRequest.GetSingle(realId);

            return View(request);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Approve(string id, ApprovalRequest approval)
        {
            IIdentity context = HttpContext.User.Identity;

            int realId = int.Parse(Cryptography.Decrypt(HexSerialization.HexToString(id)));

            BizApprovalRequest.AttendRequest(realId, context.Name, true, approval.comments);

            return RedirectToAction("index", "home");
        }

        [Authorize]
        public ActionResult Reject(string id)
        {
            int realId = int.Parse(Cryptography.Decrypt(HexSerialization.HexToString(id)));
            ViewBag.cipherId = id;
            ApprovalRequest request = BizApprovalRequest.GetSingle(realId);

            return View(request);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Reject(string id, ApprovalRequest approval)
        {
            IIdentity context = HttpContext.User.Identity;

            int realId = int.Parse(Cryptography.Decrypt(HexSerialization.HexToString(id)));

            BizApprovalRequest.AttendRequest(realId, context.Name, false, approval.comments);

            return RedirectToAction("index", "home");
        }

        // GET: Approval/Create
        [Authorize]
        public ActionResult Create()
        {
            ApprovalRequest approval = new ApprovalRequest();
            return View(approval);
        }

        // POST: Approval/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(ApprovalRequest approval)
        {
            try
            {
                IIdentity context = HttpContext.User.Identity;

                if (context.IsAuthenticated)
                {
                    approval.requestUser = context.Name;
                    approval.dateRequest = DateTime.Now;

                    BizApprovalRequest.Add(approval);
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [Authorize]
        public ActionResult RequestReport()
        {
            return View();
        }

        public JsonResult AsyncRequestReport(string id)
        {

            IIdentity context = HttpContext.User.Identity;

            DateTime start = DateTime.Parse(id.Split('|')[0]);
            DateTime end = DateTime.Parse(id.Split('|')[1]);

            List<vwRequestReport> report = BizApprovalRequest.GetReport(start, end, context.Name);

            var data = from c in report
                       select new[] { c.id.ToString(), c.dateRequest.ToString("yyyy-MM-dd"), c.requestUser, 
                           c.distributionListName, c.request.Length > 20? string.Format("{0}...", c.request.Substring(0,20)): c.request, c.approved==null?"Pendiente":((bool)c.approved?"Aprobado":"Rechazado")};
            //c.dateApproval == null ? "": ((DateTime)c.dateApproval).ToString("yyyy-MM-dd"), 
            //c.approvalUser == null?"": c.approvalUser, c.comments == null?"": c.comments};
            return Json(new
            {
                data = data
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public FileResult AsyncRequestReportXls(string id)
        {
            IIdentity context = HttpContext.User.Identity;

            DateTime start = DateTime.Parse(id.Split('|')[0]);
            DateTime end = DateTime.Parse(id.Split('|')[1]);

            MemoryStream stream = new MemoryStream(); // cleaned up automatically by MVC
            List<vwRequestReport> report = BizApprovalRequest.GetReport(start, end, context.Name); // simple entity framework-based data retrieval
            List<vmReport> reportData = new List<vmReport>();

            foreach (vwRequestReport item in report)
            {
                reportData.Add(new vmReport()
                {
                    id = item.id,
                    approvalUser = item.approvalUser,
                    approved = item.approved == null ? "Pendiente" : ((bool)item.approved ? "Aprobado" : "Rechazado"),
                    comments = item.comments,
                    dateApproval = item.dateApproval == null ? "" : ((DateTime)item.dateApproval).ToString("yyyy-MM-dd hh-mm"),
                    dateRequest = item.dateRequest.ToString("yyyy-MM-dd hh-mm"),
                    distributionListName = item.distributionListName,
                    request = item.request,
                    requestUser = item.requestUser
                });
            }


            var serialicer = new XmlSerializer(typeof(List<vmReport>));

            serialicer.Serialize(stream, reportData);
            stream.Position = 0;

            //ExportHelper.GetWinnersAsExcelMemoryStream(stream, winnerList, drawingId);

            string suggestedFilename = string.Format("Solicitudes_{0}_{1}.xls", id.Split('|')[0], id.Split('|')[1]);
            return File(stream, "application/vnd.ms-excel", suggestedFilename);

            //return File(stream, "application/vnd.ms-excel", "Pedidos.xls");
        }

        public JsonResult DisListNotification()
        {
            string res = "";

            try
            {
                BizApprovalRequest.Notificate();
                res = "Las notificaciones fueron enviada exitosamente";
            }
            catch (Exception ex)
            {
                res =string.Format("Error al procesar las notificaciones: {0}", ex.Message);
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }        
    }
}
