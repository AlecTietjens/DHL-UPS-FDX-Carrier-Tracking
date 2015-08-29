using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarrierAPIsTest_CSharp.FedExTrackServiceWebReference;
using System.Web.Services.Protocols;

namespace CarrierAPIsTest_CSharp
{
    class FedExService
    {
        // Static variables
        private static string KEY = "RGj2yyzW0WEVI51M";
        private static string PASSWORD = "lhmccbI7rzDC6KgI5nBzS3Vfk";
        private static string ACCOUNT_NUMBER = "489262622";
        private static string METER_NUMBER = "108250743";

        // Class variables
        private TrackService service;
        private TrackRequest request;
        private TrackReply reply;

        // Constructor
        public FedExService()
        {
            this.service = new TrackService();
        }


        public void CreateTrackRequest(string trackingNumber)
        {
            // Initialize
            this.request = new TrackRequest();

            // Authentication information
            request.WebAuthenticationDetail = new WebAuthenticationDetail();
            request.WebAuthenticationDetail.UserCredential = new WebAuthenticationCredential();
            request.WebAuthenticationDetail.UserCredential.Key = KEY; // Replace "XXX" with the Key
            request.WebAuthenticationDetail.UserCredential.Password = PASSWORD; // Replace "XXX" with the Password

            // Account & meter numbers
            request.ClientDetail = new ClientDetail();
            request.ClientDetail.AccountNumber = ACCOUNT_NUMBER; // Replace "XXX" with the client's account number
            request.ClientDetail.MeterNumber = METER_NUMBER; // Replace "XXX" with the client's meter number

            //
            request.TransactionDetail = new TransactionDetail();
            request.TransactionDetail.CustomerTransactionId = "***Track Request using VC#***";  //This is a reference field for the customer.  Any value can be used and will be provided in the response.

            //
            request.Version = new VersionId();
            //

            // Tracking information
            request.SelectionDetails = new TrackSelectionDetail[1] { new TrackSelectionDetail() };
            request.SelectionDetails[0].PackageIdentifier = new TrackPackageIdentifier();
            request.SelectionDetails[0].PackageIdentifier.Value = trackingNumber; // Replace "XXX" with tracking number or door tag
            request.SelectionDetails[0].PackageIdentifier.Type = TrackIdentifierType.TRACKING_NUMBER_OR_DOORTAG;

            // Date range is optional.
            // If omitted, set to false
            request.SelectionDetails[0].ShipDateRangeBegin = DateTime.Parse("06/18/2012"); //MM/DD/YYYY
            request.SelectionDetails[0].ShipDateRangeEnd = request.SelectionDetails[0].ShipDateRangeBegin.AddDays(0);
            request.SelectionDetails[0].ShipDateRangeBeginSpecified = false;
            request.SelectionDetails[0].ShipDateRangeEndSpecified = false;

            // Include detailed scans is optional.
            // If omitted, set to false
            request.ProcessingOptions = new TrackRequestProcessingOptionType[1];
            request.ProcessingOptions[0] = TrackRequestProcessingOptionType.INCLUDE_DETAILED_SCANS;

            // Run the track request and get reply
            try
            {
                this.reply = this.service.track(request);
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
                return this.reply.CompletedTrackDetails[0].TrackDetails[0].StatusDetail.Description;
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
                return this.reply.CompletedTrackDetails[0].TrackDetails[0].EstimatedDeliveryTimestamp.ToString();
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
                return this.reply.CompletedTrackDetails[0].TrackDetails[0].ActualDeliveryTimestamp.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
