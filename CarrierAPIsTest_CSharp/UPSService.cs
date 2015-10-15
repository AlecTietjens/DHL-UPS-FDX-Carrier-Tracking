using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarrierAPIsTest_CSharp.UPSTrackServiceWebReference;
using System.Web.Services.Protocols;

namespace CarrierAPIsTest_CSharp
{
    class UPSService
    {
        // private static variables
        private static string USERNAME = "username here";
        private static string PASSWORD = "password here";
        private static string ACCESS_TOKEN = "access token here";

        // class variables
        private TrackService service;
        private TrackRequest request;
        private TrackResponse reply;

        public UPSService()
        {
            this.service = new TrackService();

            // Authentication information
            service.UPSSecurityValue = new UPSSecurity();
            service.UPSSecurityValue.UsernameToken = new UPSSecurityUsernameToken();
            service.UPSSecurityValue.UsernameToken.Username = USERNAME;
            service.UPSSecurityValue.UsernameToken.Password = PASSWORD;

            // Access code information
            service.UPSSecurityValue.ServiceAccessToken = new UPSSecurityServiceAccessToken();
            service.UPSSecurityValue.ServiceAccessToken.AccessLicenseNumber = ACCESS_TOKEN;
        }

        public void CreateTrackRequest(string trackingNumber)
        {
            // Tracking information
            this.request = new TrackRequest();
            request.InquiryNumber = trackingNumber;

            try
            {
                this.reply = this.service.ProcessTrack(request);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GetCurrentStatus()
        {
            try
            {
                return this.reply.Shipment[0].Package[0].Activity[0].Status.Description;
            }
            catch (Exception)
            {
                throw; 
            }
        }

        public string GetEstimatedDeliveryDate()
        {
            try
            {
                var receivedDate = this.reply.Shipment[0].DeliveryDetail[0].Date;
                var receivedTime = this.reply.Shipment[0].DeliveryDetail[0].Time;

                string month = receivedDate.Substring(4, 2);
                string day = receivedDate.Substring(6, 2);
                string year = receivedDate.Substring(0, 4);
                string hour = "00";
                string minute = "00";
                string second = "00";

                // sometimes this is null
                if (receivedTime != null)
                {
                    hour = receivedTime.Substring(0, 2);
                    minute = receivedTime.Substring(2, 2);
                    second = receivedTime.Substring(4, 2);
                }

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
                var receivedDate = this.reply.Shipment[0].Package[0].Activity[0].Date;
                var receivedTime = this.reply.Shipment[0].Package[0].Activity[0].Time;

                string month = receivedDate.Substring(4,2);
                string day = receivedDate.Substring(6,2);
                string year = receivedDate.Substring(0,4);
                string hour = receivedTime.Substring(0,2);
                string minute = receivedTime.Substring(2,2);
                string second = receivedTime.Substring(4,2);

                return String.Format("{0}/{1}/{2} {3}:{4}:{5}", month, day, year, hour, minute, second);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
