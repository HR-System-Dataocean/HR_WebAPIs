-- Mobile app settings per company
IF OBJECT_ID(N'sys_MobileConfig', N'U') IS NULL
BEGIN
    CREATE TABLE sys_MobileConfig (
        CompanyId              INT            NOT NULL PRIMARY KEY,
        EmpCanUploadProfilePic BIT            NOT NULL CONSTRAINT DF_sys_MobileConfig_EmpCanUploadProfilePic DEFAULT (0),
        Remarks                NVARCHAR(MAX)  NULL,
        RegUserID              INT            NULL,
        RegComputerID          INT            NULL,
        RegDate                DATETIME       NULL,
        CancelDate             DATETIME       NULL
    );
END
GO
