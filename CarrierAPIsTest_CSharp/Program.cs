using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Protocols;

namespace CarrierAPIsTest_CSharp
{
    class Program
    {

        static void Main(string[] args)
        {
            FedExService fedex = new FedExService();
            DHLService dhl = new DHLService();
            UPSService ups = new UPSService();

            using (var db = new BadgeTrackingProjectEntities())
            {
                var shipments = db.ShippingInfoMailrooms.ToList();

                foreach (var shipment in shipments)
                {
                    //
                    string status = "";

                    //
                    var carrier = shipment.Carrier;
                    switch (carrier)
                    {
                        // FedEx
                        case "FedEx":
                            if (shipment.PackageStatus != "DELIVERED")
                            {

                                try
                                {
                                    fedex.CreateTrackRequest(shipment.CarrierTrackingNumber);
                                    status = fedex.GetCurrentStatus().ToUpper();
                                    if (status != "DELIVERED")
                                    {
                                        var date = Convert.ToDateTime(fedex.GetEstimatedDeliveryDate());
                                        if (date >= Convert.ToDateTime("07/07/1900"))
                                            shipment.CarrierPlannedDeliverDate = date;
                                    }
                                    else if (status == "DELIVERED")
                                    {
                                        shipment.ActDeliverDate = Convert.ToDateTime(fedex.GetActualDeliveryDate());
                                    }
                                    shipment.PackageStatus = status;
                                }
                                catch (Exception)
                                { }
                            }
                            break;
                        // UPS
                        case "UPS API":
                            if (shipment.PackageStatus != "DELIVERED")
                            {
                                try
                                {
                                    ups.CreateTrackRequest(shipment.CarrierTrackingNumber);
                                    status = ups.GetCurrentStatus().ToUpper();
                                    if (status != "DELIVERED")
                                    {
                                        var date = Convert.ToDateTime(ups.GetEstimatedDeliveryDate());
                                        if (date >= Convert.ToDateTime("07/07/1900"))
                                            shipment.CarrierPlannedDeliverDate = date;
                                    }
                                    else if (status == "DELIVERED")
                                    {
                                        shipment.ActDeliverDate = Convert.ToDateTime(ups.GetActualDeliveryDate());
                                    }
                                    shipment.PackageStatus = status;
                                }
                                catch (Exception)
                                { }
                            }
                            break;
                        // DHL
                        case "DHL":
                            if (shipment.PackageStatus != "DELIVERED")
                            {
                                try
                                {
                                    dhl.CreateTrackRequest(shipment.CarrierTrackingNumber);
                                    status = dhl.GetCurrentStatus().ToUpper();
                                    if (status != "DELIVERED" && dhl.EstimatedDeliveryExists())
                                    {
                                        var date = Convert.ToDateTime(dhl.GetEstimatedDeliveryDate());
                                        if (date >= Convert.ToDateTime("07/07/1900"))
                                            shipment.CarrierPlannedDeliverDate = date;
                                    }
                                    else if (status == "DELIVERED")
                                    {
                                        shipment.ActDeliverDate = Convert.ToDateTime(dhl.GetActualDeliveryDate());
                                    }
                                    shipment.PackageStatus = status;
                                }
                                catch (Exception)
                                { }
                            }
                            break;

                        default:
                            break;
                    }
                }

                db.SaveChanges();
            }
        }
    }
}
