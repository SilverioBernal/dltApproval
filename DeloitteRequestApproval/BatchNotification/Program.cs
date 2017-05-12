using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace BatchNotification
{
    class Program
    {
        static void Main(string[] args)
        {
            string uri = string.Format("{0}/approval/DisListNotification", ConfigurationManager.AppSettings["urlApp"]);

            string xml = PostRequest(uri, string.Empty, MediaTypeNames.Text.Html);

            WriteLog(string.Format("{0} ::: {1} {2}", DateTime.Now.ToString("yyyy-MM-dd hh:mm"), xml.Replace("\"", ""), Environment.NewLine));
        }

        private static string PostRequest(string uri, string msgBody, string contentType)
        {
            HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(uri);
            webReq.Timeout = 60 * 1000;
            webReq.Method = "POST";
            webReq.ContentType = contentType;
            webReq.ContentLength = msgBody.Length;

            using (StreamWriter streamOut =
               new StreamWriter(webReq.GetRequestStream(), System.Text.Encoding.ASCII))
            {
                streamOut.Write(msgBody);
                streamOut.Flush();
                streamOut.Close();
            }

            string response = null;

            try
            {
                using (StreamReader streamIn =
                       new StreamReader(webReq.GetResponse().GetResponseStream()))
                {
                    response = streamIn.ReadToEnd();
                    streamIn.Close();
                }
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }

            return response;
        }

        private static void WriteLog(string message)
        {
            string logMainPath = ConfigurationManager.AppSettings["log"];

            if (!Directory.Exists(logMainPath))
            {
                DirectoryInfo di = Directory.CreateDirectory(logMainPath);
            }

            string logMonthPath = string.Format(@"{0}-{1}", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString().PadLeft(2, '0'));
            string logDayFile = string.Format(@"{0}-{1}-{2}", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString().PadLeft(2, '0'), DateTime.Now.Day.ToString().PadLeft(2, '0'));

            if (!Directory.Exists(string.Format(@"{0}\{1}", logMainPath, logMonthPath)))
            {
                DirectoryInfo di = Directory.CreateDirectory(string.Format(@"{0}\{1}", logMainPath, logMonthPath));
            }

            if (!File.Exists(string.Format(@"{0}\{1}\{2}.log", logMainPath, logMonthPath, logDayFile)))
            {
                using (StreamWriter sw = File.CreateText(string.Format(@"{0}\{1}\{2}.log", logMainPath, logMonthPath, logDayFile)))
                {
                    sw.WriteLine(string.Format("++++++++++ LOG QCA {0} ++++++++++++", DateTime.Now.ToString("yyyy-MM-dd")));
                }
            }

            File.AppendAllText(string.Format(@"{0}\{1}\{2}.log", logMainPath, logMonthPath, logDayFile), message);


        }
    }
}
