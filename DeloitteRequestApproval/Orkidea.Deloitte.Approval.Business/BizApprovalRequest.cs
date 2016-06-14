using Orkidea.Deloitte.Approval.DAL;
using Orkidea.Deloitte.Approval.Entities;
using Orkidea.Framework.Messaging;
using Orkidea.Framework.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Orkidea.Deloitte.Approval.Business
{
    public static class BizApprovalRequest
    {
        public static List<ApprovalRequest> GetList(string userId)
        {
            string requestUser = HexSerialization.StringToHex(Cryptography.Encrypt(userId));

            List<ApprovalRequest> requests = DbMngmt<ApprovalRequest>.GetList(x => x.requestUser == requestUser).ToList();

            foreach (ApprovalRequest request in requests)
                request.request = Cryptography.Decrypt(HexSerialization.HexToString(request.request));

            return requests;
        }

        public static List<ApprovalRequest> GetList(int distributionListId)
        {
            List<ApprovalRequest> requests = DbMngmt<ApprovalRequest>.GetList(x => x.idDistributionList.Equals(distributionListId) && x.approved == null).ToList();

            foreach (ApprovalRequest request in requests)
            {
                request.request = Cryptography.Decrypt(HexSerialization.HexToString(request.request));
                request.requestUser = Cryptography.Decrypt(HexSerialization.HexToString(request.requestUser));
                request.approvalUser = request.approvalUser == null ? "" : Cryptography.Decrypt(HexSerialization.HexToString(request.approvalUser));
            }
            return requests;
        }

        public static ApprovalRequest GetSingle(int id)
        {
            ApprovalRequest request = DbMngmt<ApprovalRequest>.GetSingle(c => c.id.Equals(id));

            request.request = Cryptography.Decrypt(HexSerialization.HexToString(request.request));
            request.requestUser = Cryptography.Decrypt(HexSerialization.HexToString(request.requestUser));
            request.approvalUser = request.approvalUser == null ? "" : Cryptography.Decrypt(HexSerialization.HexToString(request.approvalUser));

            return request;
        }

        public static List<vwRequestReport> GetReport(DateTime start, DateTime end, string user)
        {
            WebUser webUser = BizWebUser.GetSingle(user);
            string pais = webUser.idPais;
            end = end.AddDays(1);

            List<vwRequestReport> report = new List<vwRequestReport>();

            if (pais.ToUpper() == "LA")
                report = DbMngmt<vwRequestReport>.GetList(x => x.dateRequest >= start && x.dateRequest < end).OrderBy(x => x.id).ToList();
            else
                report = DbMngmt<vwRequestReport>.GetList(x => x.dateRequest >= start && x.dateRequest < end && x.idPais == pais).OrderBy(x => x.id).ToList();

            foreach (vwRequestReport request in report)
            {
                request.requestUser = Cryptography.Decrypt(HexSerialization.HexToString(request.requestUser));
                request.distributionListName = Cryptography.Decrypt(HexSerialization.HexToString(request.distributionListName));
                request.request = Cryptography.Decrypt(HexSerialization.HexToString(request.request));

                if (request.approved != null)
                {
                    request.approvalUser = Cryptography.Decrypt(HexSerialization.HexToString(request.approvalUser));
                    request.comments = Cryptography.Decrypt(HexSerialization.HexToString(request.comments));
                }
            }

            return report;
        }

        public static void Add(ApprovalRequest approvalRequest)
        {
            try
            {
                approvalRequest.requestUser = HexSerialization.StringToHex(Cryptography.Encrypt(approvalRequest.requestUser));
                approvalRequest.request = HexSerialization.StringToHex(Cryptography.Encrypt(approvalRequest.request));

                DbMngmt<ApprovalRequest>.Add(approvalRequest);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void AttendRequest(int id, string approvalUser, bool approval, string comments)
        {
            ApprovalRequest request = DbMngmt<ApprovalRequest>.GetSingle(c => c.id.Equals(id)); ;

            request.approved = approval;
            request.dateApproval = DateTime.Now;
            request.approvalUser = HexSerialization.StringToHex(Cryptography.Encrypt(approvalUser));

            if (!string.IsNullOrEmpty(comments))
                request.comments = HexSerialization.StringToHex(Cryptography.Encrypt(comments));

            Update(request);
        }

        public static void Update(ApprovalRequest approvalRequest)
        {
            try
            {
                DbMngmt<ApprovalRequest>.Update(approvalRequest);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void Remove(int id)
        {
            ApprovalRequest approvalRequest = DbMngmt<ApprovalRequest>.GetSingle(c => c.id.Equals(id));

            DbMngmt<ApprovalRequest>.Remove(approvalRequest);
        }

        public static void Notificate()
        {
            List<DistributionList> distributionLists = BizDistributionList.GetList().ToList();
            List<ApprovalRequest> approvalRequests = DbMngmt<ApprovalRequest>.GetList(x => x.approved == null).ToList();
            List<int> notificationDisList = approvalRequests.Distinct().Select(x => x.idDistributionList).ToList();
            string urlApp = ConfigurationManager.AppSettings["urlApp"];

            foreach (int disList in notificationDisList)
            {
                DistributionList distributionList = distributionLists.Where(x => x.id.Equals(disList)).First();
                StringBuilder plainMessage = new StringBuilder();
                StringBuilder htmlMessage = new StringBuilder();

                List<ApprovalRequest> requests = approvalRequests.Where(x => x.idDistributionList.Equals(disList)).ToList();

                foreach (ApprovalRequest item in requests)
                {
                    string cipherId = HexSerialization.StringToHex(Cryptography.Encrypt(item.id.ToString()));

                    plainMessage.AppendLine(string.Format("{0} - {1}... - {2}/Approval/Details/{3}", item.id.ToString(), item.request.Length > 25 ? item.request.Substring(0, 25) : item.request, urlApp, cipherId));

                    htmlMessage.AppendLine(string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td><a href='{4}/Approval/Details/{5}> Responder </a>'</td></tr>",
                        item.id.ToString(), item.requestUser, item.dateRequest.ToString("yyyy-MM-dd"), item.request.Length > 25 ? item.request.Substring(0, 25) : item.request, urlApp, cipherId));
                }



                string host = "";
                int port = 0;
                bool enableSSL = false;
                string mailUser = "";
                string mailPassword = "";
                string fromAlias = "";
                string fromMail = "";
                string mailLogo = "";

                try
                {
                    host = ConfigurationManager.AppSettings["host"];
                    port = int.Parse(ConfigurationManager.AppSettings["port"]);
                    enableSSL = bool.Parse(ConfigurationManager.AppSettings["enableSSL"]);
                    mailUser = ConfigurationManager.AppSettings["mailUser"];
                    mailPassword = ConfigurationManager.AppSettings["mailPassword"];
                    fromAlias = ConfigurationManager.AppSettings["fromAlias"];
                    fromMail = ConfigurationManager.AppSettings["fromMail"];
                    mailLogo = ConfigurationManager.AppSettings["mailLogo"];
                }
                catch (Exception)
                {
                    throw new Exception("No se pudo obtener los parametros de envio de correo");
                }

                Mailing mailing = new Mailing(host, port, enableSSL, mailUser, mailPassword);
                MailAddress from = new MailAddress(fromMail, fromAlias);
                List<MailAddress> to = new List<MailAddress>();

                foreach (string item in distributionList.email.Split(','))
                    to.Add(new MailAddress(item));

                List<LinkedResource> linkedResources = new List<LinkedResource>();
                linkedResources.Add(new LinkedResource(mailLogo, "image/png") { TransferEncoding = System.Net.Mime.TransferEncoding.Base64, ContentId = "LogoNotification" });

                Dictionary<string, string> dynamicValues = new Dictionary<string, string>();
                dynamicValues.Add("detalleAprobaciones", htmlMessage.ToString());
                dynamicValues.Add("aprobaciones", plainMessage.ToString());

                string subject = "Portal de aprobaciones .::. Solicitudes pendientes ";

                string htmlTemplate = ConfigurationManager.AppSettings["htmlTemplate"];
                string plainTextTemplate = ConfigurationManager.AppSettings["plainTextTemplate"];

                mailing.SendMail(from, to, subject, htmlTemplate, plainTextTemplate, linkedResources, dynamicValues);
            }
        }
    }
}
