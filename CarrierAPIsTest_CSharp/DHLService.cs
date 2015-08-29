using System;
using System.Text;
using System.Web.Services.Protocols;
using System.Net;
using System.IO;
using System.Xml;
using CarrierAPIsTest_CSharp.XML;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace CarrierAPIsTest_CSharp
{
    class DHLService
    {
        // static variables
        private static string SITEID = "NovitexMcrsf";
        private static string PASSWORD = "VqD60OdGNf";
        private static string DHL_API_URL = "https://xmlpi-ea.dhl.com/XMLShippingServlet";
        private static string requestText =
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
            + "<req:KnownTrackingRequest xmlns:req=\"http://www.dhl.com\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.dhl.com TrackingRequestKnown.xsd\">"
                + "<Request>"
                    + "<ServiceHeader>"
                        + "<MessageTime>{0}</MessageTime>"
                        + "<MessageReference>1234567890123456789012345678</MessageReference>"
                        + "<SiteID>{1}</SiteID>"
                        + "<Password>{2}</Password>"
                    + "</ServiceHeader>"
                + "</Request>"
                + "<LanguageCode>en</LanguageCode>"
                + "<AWBNumber>{3}</AWBNumber>"
                + "<LevelOfDetails>ALL_CHECK_POINTS</LevelOfDetails>"
            + "</req:KnownTrackingRequest>";

        // class variable
        private WebRequest trackWebRequest;
        private WebResponse trackWebResponse;
        private TrackingResponse trackReply;

        public DHLService()
        {
            // constructor is not needed at this time
        }

        public void CreateTrackRequest(string trackingNumber)
        {
            trackWebRequest = HttpWebRequest.Create(DHL_API_URL);
            trackWebRequest.ContentType = "application/x-www-form-urlencoded";
            trackWebRequest.Method = "POST";

            // request stream
            using (var stream = trackWebRequest.GetRequestStream())
            {
                var arrBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(String.Format(requestText, DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss.fff-07:00"), SITEID, PASSWORD, trackingNumber));
                stream.Write(arrBytes, 0, arrBytes.Length);
                stream.Close();
            }

            trackWebResponse = trackWebRequest.GetResponse();

            // response stream
            using (var stream = trackWebResponse.GetResponseStream())
            {
                var reader = new StreamReader(stream, System.Text.Encoding.ASCII);
                XmlSerializer serializer = new XmlSerializer(typeof(TrackingResponse));
                this.trackReply = (TrackingResponse)serializer.Deserialize(reader);
                stream.Close();
            }
        }

        public string GetCurrentStatus()
        {
            try
            {
                string status = this.trackReply.AWBInfo.ShipmentInfo.ShipmentEvents.Count == 0 ? "Shipment information received" : this.trackReply.AWBInfo.ShipmentInfo.ShipmentEvents[this.trackReply.AWBInfo.ShipmentInfo.ShipmentEvents.Count - 1].ServiceEvent.Description;

                Match match = Regex.Match(status, @"Delivered.*");
                if (match.Success)
                {
                    status = "Delivered";
                }

                match = Regex.Match(status, @"Shipment on hold.*");
                if (match.Success)
                {
                    status += " - SERVICE AREA CODE: " + this.trackReply.AWBInfo.ShipmentInfo.ShipmentEvents[this.trackReply.AWBInfo.ShipmentInfo.ShipmentEvents.Count - 1].ServiceArea.ServiceAreaCode;
                }

                return status;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Boolean EstimatedDeliveryExists()
        {
            return this.trackReply.AWBInfo.ShipmentInfo.EstDlvyDate == null ? false : true;
        }

        public string GetEstimatedDeliveryDate()
        {
            try
            {
                var receivedEstimateDateTime = this.trackReply.AWBInfo.ShipmentInfo.EstDlvyDate;

                string month = receivedEstimateDateTime.Substring(5, 2);
                string day = receivedEstimateDateTime.Substring(8, 2);
                string year = receivedEstimateDateTime.Substring(0, 4);
                string hour = receivedEstimateDateTime.Substring(11, 2);
                string minute = receivedEstimateDateTime.Substring(14, 2);
                string second = receivedEstimateDateTime.Substring(17, 2);

                return String.Format("{0}/{1}/{2} {3}:{4}:{5}", month, day, year, hour, minute, second);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GetActualDeliveryDate()
        {
            try
            {
                var receivedDate = this.trackReply.AWBInfo.ShipmentInfo.ShipmentEvents[this.trackReply.AWBInfo.ShipmentInfo.ShipmentEvents.Count - 1].Date.ToString();
                var receivedTime = this.trackReply.AWBInfo.ShipmentInfo.ShipmentEvents[this.trackReply.AWBInfo.ShipmentInfo.ShipmentEvents.Count - 1].Time.ToString();

                string month = receivedDate.Substring(5, 2);
                string day = receivedDate.Substring(8, 2);
                string year = receivedDate.Substring(0, 4);
                string hour = receivedTime.Substring(0, 2);
                string minute = receivedTime.Substring(3, 2);
                string second = receivedTime.Substring(6, 2);

                return String.Format("{0}/{1}/{2} {3}:{4}:{5}", month, day, year, hour, minute, second);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
