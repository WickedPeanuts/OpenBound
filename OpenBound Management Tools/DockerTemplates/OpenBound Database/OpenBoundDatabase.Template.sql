USE [master];
GO
CREATE LOGIN __db_sql_admin_login__ 
    WITH PASSWORD    = N'__db_sql_admin_password__',
    CHECK_POLICY     = OFF,
    CHECK_EXPIRATION = OFF;
GO

EXEC sp_addsrvrolemember 
    @loginame = N'__db_sql_admin_login__', 
    @rolename = N'sysadmin';