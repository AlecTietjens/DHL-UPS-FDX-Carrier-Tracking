------- DAILY UPDATE -----





---- ShipmentDate
--UPDATE SIM -- This checks against all rows that do no have a ShipmentDate.. if SendSuite now has a ShipmentDate for the SR, then we can update the table with shipping info and start tracking.
--SET	SIM.ShipmentDate =
--CASE
--	-- If tracking has started in SendSuite Tracking, do the following
--	WHEN SIM.Carrier Like 'INTERNAL'
--		THEN (SELECT TOP 1 RDATE FROM [SST].[dbo].[MAILS] WHERE TRACKNO Like '86' + '%' + CONVERT(varchar(10), SR.ID))
--	-- If shipment has been created, do the following..
--	WHEN SR.ShipmentID IS NOT NULL
--		THEN (SELECT S.ShipmentDate FROM [SendSuite Live].[dbo].[Shipments] AS S WHERE SR.ShipmentID = S.ID)
--	ELSE null
--END
--FROM [BadgeTrackingProject].[dbo].[ShippingInfoMailroom] AS SIM
--INNER JOIN [SendSuite Live].[dbo].[Shipment Requisitions] AS SR
--ON SIM.ShipRequestNumber = SR.ID
--WHERE SIM.ShipmentDate IS NULL 


--if(shipmentdate is null and sr.shipmentid is not null) {

--}



---- Carrier
--UPDATE SIM -- This checks against all rows that recently had a ShipmentDate set and are missing an EstDeliverDate.. if so, we update the Carrier to much the one chosen for the shipment. Only checks external shipments since internal update is not necessary for Carrier.
--SET	SIM.Carrier = (SELECT S.CarrierName FROM [SendSuite Live].[dbo].[Shipments] AS S WHERE SR.ShipmentID = S.ID)
--FROM [BadgeTrackingProject].[dbo].[ShippingInfoMailroom] AS SIM
--INNER JOIN [SendSuite Live].[dbo].[Shipment Requisitions] AS SR
--ON SIM.ShipRequestNumber = SR.ID
--WHERE SIM.EstDeliverDate IS NULL AND SIM.ShipmentDate IS NOT NULL


---- CarrierTrackingNumber
--UPDATE SIM -- This checks against all rows that recently had a ShipmentDate set and are missing an EstDeliverDate.. if so, we will update the CarrierTrackingNumber to have the one used in the shipment.. external and internal.
--SET	SIM.CarrierTrackingNumber =
--CASE
--	-- If tracking has started in SendSuite Tracking, update the CarrierTrackingNumber to the one used in SendSuite Tracking
--	WHEN SIM.Carrier Like 'INTERNAL'
--		THEN (SELECT TOP 1 TRACKNO COLLATE SQL_LATIN1_GENERAL_CP1_CI_AS FROM [SST].[dbo].[MAILS] WHERE TRACKNO COLLATE SQL_LATIN1_GENERAL_CP1_CI_AS Like '86' + '%' + CONVERT(varchar(10), SR.ID))
--	-- Pull the CarrierTrackingNumber from the Packages tables - some joins are necessary
--	ELSE (SELECT P.PackageReference FROM [SendSuite Live].[dbo].[Shipments] AS S LEFT OUTER JOIN [SendSuite Live].[dbo].[Packages] AS P ON S.ID = P.ShipmentID WHERE SR.ShipmentID = S.ID)
--END
--FROM [BadgeTrackingProject].[dbo].[ShippingInfoMailroom] AS SIM
--INNER JOIN [SendSuite Live].[dbo].[Shipment Requisitions] AS SR
--ON SIM.ShipRequestNumber = SR.ID
--WHERE SIM.EstDeliverDate IS NULL AND SIM.ShipmentDate IS NOT NULL


---- CarrierPlannedDeliverDate


---- ActDeliverDate


---- ServiceLevel


---- PackageStatus


---- EstDeliverDate

--select * from [BadgeTrackingProject].[dbo].[ShippingInfoMailroom] WHERE carrier like 'internal'
--select * from [BadgeTrackingProject].[dbo].shippinginfomailroom
--select top 1000 * from [sendsuite live].[dbo].[shipments] order by timelog desc

--truncate table [BadgeTrackingProject].[dbo].[ShippingInfoMailroom]

--update [badgetrackingproject].[dbo].ShippingInfoMailroom set shipmentdate = null, estdeliverdate = null where carrier like 'internal'








-- DAILY UPDATE --

UPDATE SIM
SET		ShipmentDate = --* Try to update if shipment has been created for SR with Shipments info
			CASE
				WHEN SR.ShipmentID IS NOT NULL
					THEN S.ShipmentDate
				WHEN SIM.Carrier Like 'INTERNAL' -- Will pull a ShipmentDate if a Tracking item has been created with the SR#
					THEN (SELECT TOP 1 RDATE FROM [SST].[dbo].[MAILS] WHERE TRACKNO Like '86' + '%' + CONVERT(varchar(10), SR.ID))
				ELSE
					SIM.ShipmentDate
			END,

		Carrier = --* Try to update if shipment has been created for SR with Shipments info
			CASE
				WHEN SR.ShipmentID IS NOT NULL
					THEN S.CarrierName
				ELSE 
					SIM.Carrier
			END,

		CarrierTrackingNumber = --* Try to update if shipment has been created for SR with Shipments/Packages info
			CASE
				WHEN SR.ShipmentID IS NOT NULL
					THEN P.PackageReference
				WHEN SIM.Carrier Like 'INTERNAL' -- Will pull a ShipmentDate if a Tracking item has been created with the SR#
					THEN (SELECT TOP 1 TRACKNO COLLATE SQL_LATIN1_GENERAL_CP1_CI_AS FROM [SST].[dbo].[MAILS] WHERE TRACKNO COLLATE SQL_LATIN1_GENERAL_CP1_CI_AS Like '86' + '%' + CONVERT(varchar(10), SR.ID))
				ELSE
					SIM.CarrierTrackingNumber
			END,

		CarrierPlannedDeliverDate = --* INTERNAL is updated in DAILY UPDATE job / External is done through carrier APIs
			CASE
				WHEN SIM.Carrier Like 'INTERNAL'
					THEN DATEADD(day, 1, (SELECT TOP 1 RDATE FROM [SST].[dbo].[MAILS] WHERE TRACKNO Like '86' + '%' + CONVERT(varchar(10), SR.ID)))
				ELSE 
					SIM.CarrierPlannedDeliverDate
			END,  

		EstDeliverDate = --* Try to update if shipment has been created for SR with Shipments info
			CASE
				WHEN SR.ShipmentID IS NOT NULL
					THEN DATEADD(day, (SELECT MaxDeliverPeriod FROM [BadgeTrackingProject].[dbo].[MaxDeliverPeriodsForCC] WHERE CountryCode COLLATE SQL_LATIN1_GENERAL_CP1_CI_AS Like S.CountryCode COLLATE SQL_LATIN1_GENERAL_CP1_CI_AS), S.ShipmentDate)
				WHEN SIM.Carrier Like 'INTERNAL'
					THEN DATEADD(day, 1, (SELECT TOP 1 RDATE FROM [SST].[dbo].[MAILS] WHERE TRACKNO Like '86' + '%' + CONVERT(varchar(10), SR.ID)))
		
				ELSE
					SIM.EstDeliverDate
			END,

		ActDeliverDate = --* INTERNAL updates if ActDeliverDate shows as the same day as ShipmentDate... since this updates daily... otherwise we run another check later
			CASE
				WHEN SIM.Carrier Like 'INTERNAL'
					THEN (SELECT TOP 1 DLVRDATE FROM [SST].[dbo].[MAILS] WHERE TRACKNO Like '86' + '%' + CONVERT(varchar(10), SR.ID))
				ELSE
					SIM.ActDeliverDate
			END,

		ServiceLevel = --* Will check for updates in DAILY UPDATE job
			CASE
				WHEN SR.ShipmentID IS NOT NULL
					THEN C.Description
				ELSE
					SIM.ServiceLevel
			END,

		PackageStatus = --* Will check for updates on INTERNAL in DAILY UPDATE job
			CASE
				WHEN SIM.Carrier Like 'INTERNAL'
					THEN (SELECT TOP 1 PKGSTATUS COLLATE SQL_LATIN1_GENERAL_CP1_CI_AS FROM [SST].[dbo].[MAILS] WHERE TRACKNO COLLATE SQL_LATIN1_GENERAL_CP1_CI_AS Like '86' + '%' + CONVERT(varchar(10), SR.ID))
				ELSE 
					SIM.PackageStatus
			END,

		ShortBuildingName =
			CASE
				WHEN SR.ShipmentID IS NOT NULL
					THEN S.Address3
				ELSE
					SIM.ShortBuildingName
			END,

		ReceiverAddress1 =
			CASE
				WHEN SR.ShipmentID IS NOT NULL
					THEN ISNULL(S.Address1, '')
				ELSE
					SIM.ReceiverAddress1
			END,

		ReceiverAddress2 =
			CASE
				WHEN SR.ShipmentID IS NOT NULL
					THEN S.Address2
				ELSE
					SIM.ReceiverAddress2
			END,

		ReceiverCity = 
			CASE
				WHEN SR.ShipmentID IS NOT NULL
					THEN ISNULL(S.City, '')
				ELSE
					SIM.ReceiverCity
			END,

		ReceiverPostalCode =
			CASE
				WHEN SR.ShipmentID IS NOT NULL
					THEN S.PostalCode
				ELSE
					SIM.ReceiverPostalCode
			END,

		ReceiverCountryCode =
			CASE
				WHEN SR.ShipmentID IS NOT NULL
					THEN S.CountryCode
				ELSE
					SIM.ReceiverCountryCode
			END,

		MaxDeliverPeriod =
			CASE
				WHEN SR.ShipmentID IS NOT NULL
					THEN ISNULL((SELECT MaxDeliverPeriod FROM [BadgeTrackingProject].[dbo].[MaxDeliverPeriodsForCC] WHERE CountryCode COLLATE SQL_LATIN1_GENERAL_CP1_CI_AS Like S.CountryCode COLLATE SQL_LATIN1_GENERAL_CP1_CI_AS), 0)
				ELSE
					SIM.MaxDeliverPeriod
			END

FROM [BadgeTrackingProject].[dbo].[ShippingInfoMailroom] AS SIM
LEFT OUTER JOIN [SendSuite Live].[dbo].[Shipment Requisitions] AS SR
ON SIM.ShipRequestNumber = SR.ID
LEFT OUTER JOIN [SendSuite Live].[dbo].[Shipments] AS S
ON SR.ShipmentID = S.ID
LEFT OUTER JOIN [SendSuite Live].[dbo].[Packages] AS P
ON S.ID = P.ShipmentID
LEFT OUTER JOIN [SendSuite Live].[dbo].[Carrier Services] AS C
ON S.CarrierServicesID = C.ID

WHERE SIM.ShipmentDate IS NULL



-- CHECK FOR ACTDELIVERDATE ON INTERNALS --

UPDATE SIM
SET ActDeliverDate = --* INTERNAL updates if ActDeliverDate shows as the same day as ShipmentDate... since this updates daily... otherwise we run another check later
			CASE
				WHEN SIM.Carrier Like 'INTERNAL' AND (SELECT TOP 1 DLVRDATE FROM [SST].[dbo].[MAILS] WHERE TRACKNO Like '86' + '%' + CONVERT(varchar(10), SR.ID)) >= '07/07/1990'
					THEN (SELECT TOP 1 DLVRDATE FROM [SST].[dbo].[MAILS] WHERE TRACKNO Like '86' + '%' + CONVERT(varchar(10), SR.ID))
				ELSE
					SIM.ActDeliverDate
			END
FROM [BadgeTrackingProject].[dbo].[ShippingInfoMailroom] AS SIM
LEFT OUTER JOIN [SendSuite Live].[dbo].[Shipment Requisitions] AS SR
ON SIM.ShipRequestNumber = SR.ID

WHERE SIM.ActDeliverDate IS NULL