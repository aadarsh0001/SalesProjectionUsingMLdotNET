
CREATE OR ALTER PROCEDURE [dbo].[GetProjectedGuestsPerHour]
AS
BEGIN
  SELECT
    CAST(CONVERT(VARCHAR, [BusinessDayDate], 101) + ' ' +
         RIGHT('0' + CAST((TimeSegmentID - 1) / 4 AS VARCHAR(2)), 2) + ':00:00.000' AS DATETIME) AS CombinedDateTime,
    SUM(TotalGuests) AS TotalProjectedGuestsPerHour
  FROM CO_CashPlanningTimeSegments
  WHERE [BusinessDayDate] > '2023-01-01'
    AND [BusinessDayDate] < '2023-04-10'
    AND TimeSegmentID NOT BETWEEN 5 AND 24
  GROUP BY [BusinessDayDate], (TimeSegmentID - 1) / 4
  ORDER BY [BusinessDayDate],  (TimeSegmentID - 1) / 4
END;


CREATE OR ALTER PROCEDURE [dbo].[GetActualGuestsPerHour]
AS
BEGIN
SELECT
		[Project1].[timedate],
        sum([Project1].[C3]) AS TotalActualGuestsPerHour
    FROM
        (SELECT
            [GroupBy1].[A1] AS [C1],
            [GroupBy1].[A2] AS [C2],
            [GroupBy1].[A3] AS [C3],
            [GroupBy1].[A4] AS [C4],
            [GroupBy1].[A5] AS [C5],
            [GroupBy1].[K1] AS [RetailTransactionTypeCode],
            [GroupBy1].[K2] AS [TransactionStatus],
            [GroupBy1].[K3] AS [Name],
			CONCAT(
			  RIGHT('0000' + CAST([GroupBy1].[K4] AS VARCHAR), 4), -- Year with leading zeros
			  '-', 
			  RIGHT('00' + CAST([GroupBy1].[K5] AS VARCHAR), 2),   -- Month with leading zeros
			  '-', 
			  RIGHT('00' + CAST([GroupBy1].[K6] AS VARCHAR), 2),   -- Day with leading zeros
			  ' ',                                                 -- Space separator
			  RIGHT('00' + CAST([GroupBy1].[K7] AS VARCHAR), 2),   -- Hour with leading zeros
			  ':00:00'                                           
  
				) as timedate,
							1 AS [C10]
						FROM
					(SELECT
                [Filter2].[K1] AS [K1],
                [Filter2].[K2] AS [K2],
                [Filter2].[K3] AS [K3],
                [Filter2].[K4] AS [K4],
                [Filter2].[K5] AS [K5],
                [Filter2].[K6] AS [K6],
                [Filter2].[K7] AS [K7],
                SUM([Filter2].[A1]) AS [A1],
                SUM([Filter2].[A2]) AS [A2],
                COUNT([Filter2].[A3]) AS [A3],
                SUM([Filter2].[A4]) AS [A4],
                SUM([Filter2].[A5]) AS [A5]
            FROM
                (SELECT
                    [Filter1].[RetailTransactionTypeCode] AS [K1],
                    [Filter1].[TransactionStatus] AS [K2],
                    [Extent3].[Name] AS [K3],
                    DATEPART(year, [Filter1].[BeginDateTimestamp]) AS [K4],
                    DATEPART(month, [Filter1].[BeginDateTimestamp]) AS [K5],
                    DATEPART(day, [Filter1].[BeginDateTimestamp]) AS [K6],
                    DATEPART(hour, [Filter1].[BeginDateTimestamp]) AS [K7],
                    [Filter1].[TotalTransactionAmount] AS [A1],
                    [Filter1].[TotalTax] AS [A2],
                    1 AS [A3],
                    [Filter1].[NonProductAmount] AS [A4],
                    [Filter1].[NonProductTax] AS [A5]
                FROM
                    (SELECT
                        [Extent1].[RetailTransactionTypeCode] AS [RetailTransactionTypeCode],
                        [Extent1].[TransactionStatus] AS [TransactionStatus],
                        [Extent1].[TotalTransactionAmount] AS [TotalTransactionAmount],
                        [Extent1].[TotalTax] AS [TotalTax],
                        [Extent1].[NonProductAmount] AS [NonProductAmount],
                        [Extent1].[NonProductTax] AS [NonProductTax],
                        [Extent1].[PODtypeID] AS [PODtypeID],
                        [Extent2].[BusinessDayDate] AS [BusinessDayDate],
                        [Extent2].[BeginDateTimestamp] AS [BeginDateTimestamp]
                    FROM
                        [dbo].[TR_RetailTransaction] AS [Extent1]
                        INNER JOIN [dbo].[TR_Transaction] AS [Extent2] ON [Extent1].[TransactionID] = [Extent2].[TransactionID]
                    WHERE
                        [Extent2].[SessionStartTransactionID] IS NOT NULL
                        AND [Extent2].[BusinessDayDate] < '2023-04-10' AND [Extent2].[BusinessDayDate] > '2023-01-01' -- Parameter used here
                    ) AS [Filter1]
                    INNER JOIN [dbo].[ID_PODtype] AS [Extent3] ON [Filter1].[PODtypeID] = [Extent3].[PODtypeID]
						) AS [Filter2]
						GROUP BY [K1], [K2], [K3], [K4], [K5], [K6], [K7]
					) AS [GroupBy1]
				) AS [Project1]
			WHERE [Project1].[TransactionStatus] = 'SOLD'
   
				group by timedate
				ORDER BY timedate ASC 
				end;


CREATE OR ALTER PROCEDURE GetHourlySalesDataToBeProjected
AS
BEGIN
    SELECT
        [Project1].[timedate],
        SUM([Project1].[C3]) AS TotalActualGuestsPerHour
    FROM
        (SELECT
            [GroupBy1].[A1] AS [C1],
            [GroupBy1].[A2] AS [C2],
            [GroupBy1].[A3] AS [C3],
            [GroupBy1].[A4] AS [C4],
            [GroupBy1].[A5] AS [C5],
            [GroupBy1].[K1] AS [RetailTransactionTypeCode],
            [GroupBy1].[K2] AS [TransactionStatus],
            [GroupBy1].[K3] AS [Name],
            CONCAT(
                RIGHT('0000' + CAST([GroupBy1].[K4] AS VARCHAR), 4),
                '-',
                RIGHT('00' + CAST([GroupBy1].[K5] AS VARCHAR), 2),
                '-',
                RIGHT('00' + CAST([GroupBy1].[K6] AS VARCHAR), 2),
                ' ',
                RIGHT('00' + CAST([GroupBy1].[K7] AS VARCHAR), 2),
                ':00:00'
            ) AS timedate,
            1 AS [C10]
        FROM
            (SELECT
                [Filter2].[K1] AS [K1],
                [Filter2].[K2] AS [K2],
                [Filter2].[K3] AS [K3],
                [Filter2].[K4] AS [K4],
                [Filter2].[K5] AS [K5],
                [Filter2].[K6] AS [K6],
                [Filter2].[K7] AS [K7],
                SUM([Filter2].[A1]) AS [A1],
                SUM([Filter2].[A2]) AS [A2],
                COUNT([Filter2].[A3]) AS [A3],
                SUM([Filter2].[A4]) AS [A4],
                SUM([Filter2].[A5]) AS [A5]
            FROM
                (SELECT
                    [Filter1].[RetailTransactionTypeCode] AS [K1],
                    [Filter1].[TransactionStatus] AS [K2],
                    [Extent3].[Name] AS [K3],
                    DATEPART(year, [Filter1].[BeginDateTimestamp]) AS [K4],
                    DATEPART(month, [Filter1].[BeginDateTimestamp]) AS [K5],
                    DATEPART(day, [Filter1].[BeginDateTimestamp]) AS [K6],
                    DATEPART(hour, [Filter1].[BeginDateTimestamp]) AS [K7],
                    [Filter1].[TotalTransactionAmount] AS [A1],
                    [Filter1].[TotalTax] AS [A2],
                    1 AS [A3],
                    [Filter1].[NonProductAmount] AS [A4],
                    [Filter1].[NonProductTax] AS [A5]
                FROM
                    (SELECT
                        [Extent1].[RetailTransactionTypeCode] AS [RetailTransactionTypeCode],
                        [Extent1].[TransactionStatus] AS [TransactionStatus],
                        [Extent1].[TotalTransactionAmount] AS [TotalTransactionAmount],
                        [Extent1].[TotalTax] AS [TotalTax],
                        [Extent1].[NonProductAmount] AS [NonProductAmount],
                        [Extent1].[NonProductTax] AS [NonProductTax],
                        [Extent1].[PODtypeID] AS [PODtypeID],
                        [Extent2].[BusinessDayDate] AS [BusinessDayDate],
                        [Extent2].[BeginDateTimestamp] AS [BeginDateTimestamp]
                    FROM
                        [dbo].[TR_RetailTransaction] AS [Extent1]
                        INNER JOIN [dbo].[TR_Transaction] AS [Extent2] ON [Extent1].[TransactionID] = [Extent2].[TransactionID]
                    WHERE
                        [Extent2].[SessionStartTransactionID] IS NOT NULL
                        AND [Extent2].[BusinessDayDate] = '2023-04-10'
                    ) AS [Filter1]
                    INNER JOIN [dbo].[ID_PODtype] AS [Extent3] ON [Filter1].[PODtypeID] = [Extent3].[PODtypeID]
                ) AS [Filter2]
                GROUP BY [K1], [K2], [K3], [K4], [K5], [K6], [K7]
            ) AS [GroupBy1]
        ) AS [Project1]
    WHERE [Project1].[TransactionStatus] = 'SOLD'
    GROUP BY timedate
    ORDER BY timedate ASC
END;

CREATE OR ALTER PROCEDURE GetHourlySalesDataProjectedSDM
AS
BEGIN
    SELECT
		CAST(CONVERT(VARCHAR, [BusinessDayDate], 101) + ' ' +
			 RIGHT('0' + CAST((TimeSegmentID - 1) / 4 AS VARCHAR(2)), 2) + ':00:00.000' AS DATETIME) AS CombinedDateTime,
		SUM(TotalGuests) AS TotalProjectedGuestsPerHour
	  FROM CO_CashPlanningTimeSegments
	  WHERE  [BusinessDayDate] = '2023-04-10'
		AND TimeSegmentID NOT BETWEEN 5 AND 24
	  GROUP BY [BusinessDayDate], (TimeSegmentID - 1) / 4
	  ORDER BY [BusinessDayDate],  (TimeSegmentID - 1) / 4
END;

EXEC GetHourlySalesDataProjectedSDM;
EXEC GetHourlySalesDataToBeProjected
exec [GetActualGuestsPerHour]
exec [GetProjectedGuestsPerHour]