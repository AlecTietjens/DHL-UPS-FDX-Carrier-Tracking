-- DAILY INSERT --

INSERT INTO [BadgeTrackingProject].[dbo].[ShippingInfoMailroom] (ShipmentDate, Carrier, CarrierTrackingNumber, CarrierPlannedDeliverDate, EstDeliverDate, ActDeliverDate, ServiceLevel, ShipRequestCreateDate, 
														PackageStatus, ShortBuildingName, ReceiverAddress1, ReceiverAddress2, ReceiverCity, ReceiverPostalCode, ReceiverCountryCode, ShipRequestNumber, MaxDeliverPeriod)
SELECT	ShipmentDate = --* Will check for updates in DAILY UPDATE job
			CASE
				WHEN SR.ReceiverAddress3 Like 'USWA%' -- Track on Redmond's campus **NOTE: USWA% will be the check and flag for Redmond shipments.. INTERNAL info is pulled from SendSuite Tracking
					THEN (SELECT TOP 1 RDATE FROM [SST].[dbo].[MAILS] WHERE TRACKNO Like '86' + '%' + CONVERT(varchar(10), SR.ID))
				ELSE 
					S.ShipmentDate
			END,

		Carrier = --* Will check for updates  in DAILY UPDATE job
			CASE
				WHEN SR.ReceiverAddress3 Like 'USWA%' -- Track on Redmond's campus **NOTE: USWA% will be the check and flag for Redmond shipments.. INTERNAL info is pulled from SendSuite Tracking
					THEN 'INTERNAL'
				ELSE 
					CASE
						WHEN SR.ShipmentID IS NOT NULL
							THEN S.CarrierName
						ELSE 
							ISNULL((SELECT CarrierName FROM [SendSuite Live].[dbo].[Carriers] AS C WHERE C.ID = SR.CarrierID), '')
					END
			END,

		CarrierTrackingNumber = --* Will check for updates in DAILY UPDATE job
			CASE
				WHEN SR.ReceiverAddress3 Like 'USWA%'
					THEN (SELECT TOP 1 TRACKNO COLLATE SQL_LATIN1_GENERAL_CP1_CI_AS FROM [SST].[dbo].[MAILS] WHERE TRACKNO COLLATE SQL_LATIN1_GENERAL_CP1_CI_AS Like '86' + '%' + CONVERT(varchar(10), SR.ID))
				ELSE 
					P.PackageReference -- Pull carrier tracking # for off campus shipment
			END,

		CarrierPlannedDeliverDate = --* INTERNAL is updated in DAILY UPDATE job / External is done through carrier APIs
			CASE
				WHEN SR.ReceiverAddress3 Like 'USWA%'
					THEN DATEADD(day, 1, (SELECT TOP 1 RDATE FROM [SST].[dbo].[MAILS] WHERE TRACKNO Like '86' + '%' + CONVERT(varchar(10), SR.ID)))
				ELSE 
					NULL
			END,  

		EstDeliverDate = --* Will check for updates in DAILY UPDATE job
			CASE
				WHEN SR.ReceiverAddress3 Like 'USWA%'
					THEN DATEADD(day, 1, (SELECT TOP 1 RDATE FROM [SST].[dbo].[MAILS] WHERE TRACKNO Like '86' + '%' + CONVERT(varchar(10), SR.ID)))
				ELSE 
					DATEADD(day, (SELECT MaxDeliverPeriod FROM [BadgeTrackingProject].[dbo].[MaxDeliverPeriodsForCC] WHERE CountryCode COLLATE SQL_LATIN1_GENERAL_CP1_CI_AS Like SR.ReceiverCountryCode COLLATE SQL_LATIN1_GENERAL_CP1_CI_AS), S.ShipmentDate)
			END,

		NULL AS ActDeliverDate, -- ActDeliverDate .. pulled from carrier's API / Internal info is gathered in DAILY UPDATE job

		ServiceLevel = --* Will check for updates in DAILY UPDATE job
			CASE
				WHEN SR.ReceiverAddress3 Like 'USWA%' -- INTERNAL IDENTIFIER
					THEN 'INTERNAL'
				ELSE 
					CASE
						WHEN SR.ShipmentID IS NOT NULL
							THEN C.Description
						ELSE 
							(SELECT Description FROM [SendSuite Live].[dbo].[Carrier Services] AS C WHERE C.ID = SR.CarrierServiceID)
					END
			END,

		SR.TimeLog AS ShipRequestCreateDate,

		PackageStatus = --* Will check for updates on INTERNAL in DAILY UPDATE job
			CASE
				WHEN SR.ReceiverAddress3 Like 'USWA%' -- INTERNAL IDENTIFIER
					THEN (SELECT TOP 1 PKGSTATUS COLLATE SQL_LATIN1_GENERAL_CP1_CI_AS FROM [SST].[dbo].[MAILS] WHERE TRACKNO COLLATE SQL_LATIN1_GENERAL_CP1_CI_AS Like '86' + '%' + CONVERT(varchar(10), SR.ID))
				ELSE 
					''
			END,

		ShortBuildingName =
			CASE
				WHEN SR.ShipmentID IS NOT NULL
					THEN S.Address3
				ELSE
					SR.ReceiverAddress3
			END,

		ReceiverAddress1 =
			CASE
				WHEN SR.ShipmentID IS NOT NULL
					THEN ISNULL(S.Address1, '')
				ELSE
					ISNULL(SR.ReceiverAddress1, '')
			END,

		ReceiverAddress2 =
			CASE
				WHEN SR.ShipmentID IS NOT NULL
					THEN S.Address2
				ELSE
					SR.ReceiverAddress2
			END,

		ReceiverCity = 
			CASE
				WHEN SR.ShipmentID IS NOT NULL
					THEN ISNULL(S.City, '')
				ELSE
					ISNULL(SR.ReceiverCity, '')
			END,

		ReceiverPostalCode =
			CASE
				WHEN SR.ShipmentID IS NOT NULL
					THEN S.PostalCode
				ELSE
					SR.ReceiverPostalCode
			END,

		ReceiverCountryCode =
			CASE
				WHEN SR.ShipmentID IS NOT NULL
					THEN S.CountryCode
				ELSE
					SR.ReceiverCountryCode
			END,

		SR.ID AS ShipRequestNumber,
	    ISNULL((SELECT MaxDeliverPeriod FROM [BadgeTrackingProject].[dbo].[MaxDeliverPeriodsForCC] WHERE CountryCode COLLATE SQL_LATIN1_GENERAL_CP1_CI_AS Like SR.ReceiverCountryCode COLLATE SQL_LATIN1_GENERAL_CP1_CI_AS), 0) AS MaxDeliverPeriod

FROM [SendSuite Live].[dbo].[Shipment Requisitions] AS SR
LEFT OUTER JOIN [SendSuite Live].[dbo].[Shipments] AS S
ON SR.ShipmentID = S.ID
LEFT OUTER JOIN [SendSuite Live].[dbo].[Packages] AS P
ON S.ID = P.ShipmentID
LEFT OUTER JOIN [SendSuite Live].[dbo].[Carrier Services] AS C
ON S.CarrierServicesID = C.ID

-- Filter.. pulls where ship requests were made previous day and cost center is 
WHERE SR.TimeLog >= DATEADD(day, -1, CONVERT(date, GETDATE())) AND SR.CostCenterCode = 1571045