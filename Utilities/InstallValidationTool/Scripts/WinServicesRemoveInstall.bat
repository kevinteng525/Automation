::===========================================================================================
:: Remote Install Windows Service
:: Author: Tony Tang
:: Date: 2013-07-17
::===========================================================================================

@echo off

:: Define Parameters
SET /p ServerName=%1
SET SourceFolder=%2
SET DestinationFolder=%3

@ECHO Remote Install Windows Service
@ECHO ServerName = %ServerName%
@ECHO SourceFolder = %SourceFolder%
@ECHO DestinationFolder = %DestinationFolder%

:: Check Parameters
IF "%ServerName%" == "" 
@ECHO %ServerName% IS EMPTY

IF NOT EXIST "%SourceFolder%"
(
	@ECHO %SourceFolder% NOT EXIST
	EXIT
)
IF NOT EXIST "%DestinationFolder%"
(
	@ECHO %DestinationFolder% NOT EXIST
	EXIT
)


@ECHO Start copy from %SourceFolder% to %DestinationFolder%...
