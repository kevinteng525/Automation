-- ===========================================================================================
--
-- Copyright 2013 EMC Corporation.  All rights reserved.  This software 
-- (including documentation) is subject to the terms and conditions set forth 
-- in the end user license agreement or other applicable agreement, and you 
-- may use this software only if you accept all the terms and conditions of 
-- the license agreement.  This software comprises proprietary and confidential 
-- information of EMC.  Unauthorized use, disclosure, and distribution are 
-- strictly prohibited.  Use, duplication, or disclosure of the software and 
-- documentation by the U.S. Government are subject to restrictions set forth 
-- in subparagraph (c)(1)(ii) of the Rights in Technical Data and Computer 
-- Software clause at DFARS 252.227-7013 or subparagraphs (c)(1) and (2) of the 
-- Commercial Computer Software - Restricted Rights at 48 CFR 52.227-19, as 
-- applicable. Manufacturer is EMC Corporation, 176 South St., Hopkinton, MA  01748.
-- 
-- FILE
-- 	GrantDMPermission.sql
--
-- CREATED
--   	12.10.2012
--
-- AUTHOR
--    	Simon Shi
--
-- DESCRIPTION
--   	Silently add login and user into the Database.
--
-- LAST UPDATED 
--   	12.10.2012
--
-- VERSION	
--	 7.0.0.1
--
-- USAGE
-- 	SILENT MODE : Command Prompt> SQLCMD -S [Server] -i [FolderPath]\GrantPermission.sql -b -v AdminGroup=[AdminGroup] SecurityGroup=[SecurityGroup]
--	
--	Arguments can be specified via command line arguments or entered directly
--	within the batch script below.
--
-- ===========================================================================================

DECLARE @UserRoleReader nvarchar(255),@UserRoleWriter nvarchar(255)
DECLARE @AdminGroup nvarchar(255), @SecurityGroup nvarchar(255)

SET @UserRoleReader = 'db_datareader'
SET @UserRoleWriter = 'db_datawriter'
SET @AdminGroup = $(AdminGroup)
SET @SecurityGroup = $(SecurityGroup)

-- Add new login for AdminGroup
IF NOT EXISTS(SELECT NAME FROM master.sys.server_principals WHERE NAME = @AdminGroup)
BEGIN
	EXEC (N'CREATE LOGIN ['+ @AdminGroup +'] FROM WINDOWS WITH DEFAULT_DATABASE = ES1Activity')
END

-- Add new login for SecurityGroup
IF NOT EXISTS(SELECT NAME FROM master.sys.server_principals WHERE NAME = @SecurityGroup)
BEGIN
	EXEC (N'CREATE LOGIN ['+ @SecurityGroup +'] FROM WINDOWS WITH DEFAULT_DATABASE = ES1Activity')
END

-- Add users for Activity DB
USE ES1Activity; 
IF NOT EXISTS(SELECT NAME FROM sys.database_principals WHERE NAME = @AdminGroup)
BEGIN
	EXEC (N'CREATE USER ['+ @AdminGroup +'] FOR LOGIN ['+ @AdminGroup +']')
	EXEC sp_addrolemember @UserRoleReader, @AdminGroup
	EXEC sp_addrolemember @UserRoleWriter, @AdminGroup
	EXEC (N'GRANT EXECUTE TO ['+ @AdminGroup +']')
	EXEC (N'GRANT ALTER TO ['+ @AdminGroup +']')
	EXEC (N'GRANT CREATE TABLE TO ['+ @AdminGroup +']')
END

IF NOT EXISTS(SELECT NAME FROM sys.database_principals WHERE NAME = @SecurityGroup)
BEGIN
	EXEC (N'CREATE USER ['+ @SecurityGroup +'] FOR LOGIN ['+ @SecurityGroup +']')
	EXEC sp_addrolemember @UserRoleReader, @SecurityGroup
	EXEC sp_addrolemember @UserRoleWriter, @SecurityGroup
	EXEC (N'GRANT EXECUTE TO ['+ @SecurityGroup +']')
	EXEC (N'GRANT ALTER TO ['+ @SecurityGroup +']')
	EXEC (N'GRANT CREATE TABLE TO ['+ @SecurityGroup +']')
END

-- Add users for Archive DB
USE ES1Archive;
IF NOT EXISTS(SELECT NAME FROM sys.database_principals WHERE NAME = @AdminGroup)
BEGIN
	EXEC (N'CREATE USER ['+ @AdminGroup +'] FOR LOGIN ['+ @AdminGroup +']')
	EXEC sp_addrolemember @UserRoleReader, @AdminGroup
	EXEC sp_addrolemember @UserRoleWriter, @AdminGroup
	EXEC (N'GRANT EXECUTE TO ['+ @AdminGroup +']')
	EXEC (N'GRANT ALTER TO ['+ @AdminGroup +']')
	EXEC (N'GRANT CREATE TABLE TO ['+ @AdminGroup +']')
END

IF NOT EXISTS(SELECT NAME FROM sys.database_principals WHERE NAME = @SecurityGroup)
BEGIN
	EXEC (N'CREATE USER ['+ @SecurityGroup +'] FOR LOGIN ['+ @SecurityGroup +']')
	EXEC sp_addrolemember @UserRoleReader, @SecurityGroup
	EXEC sp_addrolemember @UserRoleWriter, @SecurityGroup
	EXEC (N'GRANT EXECUTE TO ['+ @SecurityGroup +']')
	EXEC (N'GRANT ALTER TO ['+ @SecurityGroup +']')
	EXEC (N'GRANT CREATE TABLE TO ['+ @SecurityGroup +']')
END

-- Add users for Search DB
USE ES1Search;
IF NOT EXISTS(SELECT NAME FROM sys.database_principals WHERE NAME = @AdminGroup)
BEGIN
	EXEC (N'CREATE USER ['+ @AdminGroup +'] FOR LOGIN ['+ @AdminGroup +']')
	EXEC sp_addrolemember @UserRoleReader, @AdminGroup
	EXEC sp_addrolemember @UserRoleWriter, @AdminGroup
	EXEC (N'GRANT EXECUTE TO ['+ @AdminGroup +']')
	EXEC (N'GRANT ALTER TO ['+ @AdminGroup +']')
	EXEC (N'GRANT CREATE TABLE TO ['+ @AdminGroup +']')
END

IF NOT EXISTS(SELECT NAME FROM sys.database_principals WHERE NAME = @SecurityGroup)
BEGIN
	EXEC (N'CREATE USER ['+ @SecurityGroup +'] FOR LOGIN ['+ @SecurityGroup +']')
	EXEC sp_addrolemember @UserRoleReader, @SecurityGroup
	EXEC sp_addrolemember @UserRoleWriter, @SecurityGroup
	EXEC (N'GRANT EXECUTE TO ['+ @SecurityGroup +']')
	EXEC (N'GRANT ALTER TO ['+ @SecurityGroup +']')
	EXEC (N'GRANT CREATE TABLE TO ['+ @SecurityGroup +']')
END
