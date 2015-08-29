using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CarrierAPIsTest_CSharp.XML
{
    [XmlRoot(Namespace = "http://www.dhl.com")]
    public partial class TrackingResponse
    {
        [XmlElement(Namespace = "")]
        public Response Response { get; set; }

        [XmlElement(Namespace = "")]
        public AWBInfo AWBInfo { get; set; }

        [XmlElement(Namespace = "")]
        public string LanguageCode { get; set; }
    }

    public partial class Response
    {
        [XmlElement(Namespace = "")]
        public ServiceHeader ServiceHeader { get; set; }
    }

    public partial class ServiceHeader
    {
        [XmlElement(Namespace = "")]
        public string MessageTime { get; set; }

        [XmlElement(Namespace = "")]
        public string MessageReference { get; set; }

        [XmlElement(Namespace = "")]
        public string SiteID { get; set; }
    }

    public partial class AWBInfo
    {
        [XmlElement(Namespace = "")]
        public string AWBNumber { get; set; }

        [XmlElement(Namespace = "")]
        public Status Status { get; set; }

        [XmlElement(Namespace = "")]
        public ShipmentInfo ShipmentInfo { get; set; }
    }

    public partial class Status
    {
        [XmlElement(Namespace = "")]
        public string ActionStatus { get; set; }
    }

    public partial class ShipmentInfo
    {
        [XmlElement(Namespace = "")]
        public OriginOfService OriginOfService { get; set; }

        [XmlElement(Namespace = "")]
        public DestinationServiceArea DestinationServiceArea { get; set; }

        [XmlElement(Namespace = "")]
        public string ShipperName { get; set; }

        [XmlElement(Namespace = "")]
        public string ShipperAccountNumber { get; set; }

        [XmlElement(Namespace = "")]
        public string ConsigneeName { get; set; }

        [XmlElement(Namespace = "")]
        public string ShipmentDate { get; set; }

        [XmlElement(Namespace = "")]
        public string Pieces { get; set; }

        [XmlElement(Namespace = "")]
        public string Weight { get; set; }

        [XmlElement(Namespace = "")]
        public string WeighUnit { get; set; }

        [XmlElement(Namespace = "")]
        public string EstDlvyDate { get; set; }

        [XmlElement(Namespace = "")]
        public string EstDlvyDateUTC { get; set; }

        [XmlElement(Namespace = "")]
        public string GlobalProductCode { get; set; }

        [XmlElement(Namespace = "")]
        public string ShipmentDesc { get; set; }

        [XmlElement(Namespace = "")]
        public string DlvyNotificationFlag { get; set; }

        [XmlElement(Namespace = "")]
        public Shipper Shipper { get; set; }

        [XmlElement(Namespace = "")]
        public Consignee Consignee { get; set; }

        [XmlElement(Namespace = "")]
        public ShipperReference ShipperReference { get; set; }

        [XmlElement("ShipmentEvent", Namespace = "")]
        public List<ShipmentEvent> ShipmentEvents { get; set; }
    }

    public partial class OriginOfService
    {
        [XmlElement(Namespace = "")]
        public string ServiceAreaCode { get; set; }

        [XmlElement(Namespace = "")]
        public string Description { get; set; }
    }

    public partial class DestinationServiceArea
    {
        [XmlElement(Namespace = "")]
        public string ServiceAreaCode { get; set; }

        [XmlElement(Namespace = "")]
        public string Description { get; set; }
    }

    public partial class Shipper
    {
        [XmlElement(Namespace = "")]
        public string City { get; set; }

        [XmlElement(Namespace = "")]
        public string DivisionCode { get; set; }

        [XmlElement(Namespace = "")]
        public string PostalCode { get; set; }

        [XmlElement(Namespace = "")]
        public string CountryCode { get; set; }
    }

    public partial class Consignee
    {
        [XmlElement(Namespace = "")]
        public string City { get; set; }

        [XmlElement(Namespace = "")]
        public string DivisionCode { get; set; }

        [XmlElement(Namespace = "")]
        public string PostalCode { get; set; }

        [XmlElement(Namespace = "")]
        public string CountryCode { get; set; }
    }

    public partial class ShipperReference
    {
        [XmlElement(Namespace = "")]
        public string ReferenceID { get; set; }
    }

    public partial class ShipmentEvent
    {
        [XmlElement(Namespace = "")]
        public string Date { get; set; }

        [XmlElement(Namespace = "")]
        public string Time { get; set; }

        [XmlElement(Namespace = "")]
        public ServiceEvent ServiceEvent { get; set; }

        [XmlElement(Namespace = "")]
        public string Signatory { get; set; }

        [XmlElement(Namespace = "")]
        public ServiceArea ServiceArea { get; set; }
    }

    public partial class ServiceEvent
    {
        [XmlElement(Namespace = "")]
        public string EventCode { get; set; }

        [XmlElement(Namespace = "")]
        public string Description { get; set; }
    }

    public partial class ServiceArea
    {
        [XmlElement(Namespace = "")]
        public string ServiceAreaCode { get; set; }

        [XmlElement(Namespace = "")]
        public string Description { get; set; }
    }


}
