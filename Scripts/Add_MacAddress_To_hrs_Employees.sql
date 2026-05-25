-- Add MacAddress column to hrs_Employees for single-device attendance restriction
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'hrs_Employees') AND name = N'MacAddress'
)
BEGIN
    ALTER TABLE hrs_Employees ADD MacAddress NVARCHAR(50) NULL;
END
GO
