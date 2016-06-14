using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Orkidea.Deloitte.Approval.FrontEnd.Models
{
    [Serializable]
    public class vmReport
    {
        [XmlAttribute("id-solicitud")]
        public int id { get; set; }
        [XmlAttribute("fecha-requerimiento")]
        public string dateRequest { get; set; }
        [XmlAttribute("usuario-solicitante")]
        public string requestUser { get; set; }
        [XmlAttribute("asignado-a")]
        public string distributionListName { get; set; }
        [XmlAttribute("requerimiento")]
        public string request { get; set; }
        [XmlAttribute("estado")]
        public string approved { get; set; }
        [XmlAttribute("fecha-respuesta")]
        public string dateApproval { get; set; }
        [XmlAttribute("respondido-ppor")]
        public string approvalUser { get; set; }
        [XmlAttribute("comentarios")]
        public string comments { get; set; }
    }
}