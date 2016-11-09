::===========================================================================================
::
:: Copyright 2015 EMC Corporation.  All rights reserved.  This software 
:: (including documentation) is subject to the terms and conditions set forth 
:: in the end user license agreement or other applicable agreement, and you 
:: may use this software only if you accept all the terms and conditions of 
:: the license agreement.  This software comprises proprietary and confidential 
:: information of EMC.  Unauthorized use, disclosure, and distribution are 
:: strictly prohibited.  Use, duplication, or disclosure of the software and 
:: documentation by the U.S. Government are subject to restrictions set forth 
:: in subparagraph (c)(1)(ii) of the Rights in Technical Data and Computer 
:: Software clause at DFARS 252.227-7013 or subparagraphs (c)(1) and (2) of the 
:: Commercial Computer Software - Restricted Rights at 48 CFR 52.227-19, as 
:: applicable. Manufacturer is EMC Corporation, 176 South St., Hopkinton, MA  01748.
:: 
:: FILE
:: 	BuildAll.bat
::
:: CREATED
::   	02.07.2015
::
:: AUTHOR
::    	James Zhang

:: 	Build All released AutoIt3 scripts.
::	

::@ECHO OFF
SET CURRENT_PATH=%~dp0

:: Make sure AutoIt3Wrapper.exe is installed in the default place.

"C:\Program Files (x86)\AutoIt3\SciTE\AutoIt3Wrapper\AutoIt3Wrapper.exe" /NoStatus /prod /in "%CURRENT_PATH%\Master.au3"
"C:\Program Files (x86)\AutoIt3\SciTE\AutoIt3Wrapper\AutoIt3Wrapper.exe" /NoStatus /prod /in "%CURRENT_PATH%\Archive.au3"
"C:\Program Files (x86)\AutoIt3\SciTE\AutoIt3Wrapper\AutoIt3Wrapper.exe" /NoStatus /prod /in "%CURRENT_PATH%\Console.au3"
"C:\Program Files (x86)\AutoIt3\SciTE\AutoIt3Wrapper\AutoIt3Wrapper.exe" /NoStatus /prod /in "%CURRENT_PATH%\Worker.au3"
"C:\Program Files (x86)\AutoIt3\SciTE\AutoIt3Wrapper\AutoIt3Wrapper.exe" /NoStatus /prod /in "%CURRENT_PATH%\WebService.au3"
"C:\Program Files (x86)\AutoIt3\SciTE\AutoIt3Wrapper\AutoIt3Wrapper.exe" /NoStatus /prod /in "%CURRENT_PATH%\Search.au3"
"C:\Program Files (x86)\AutoIt3\SciTE\AutoIt3Wrapper\AutoIt3Wrapper.exe" /NoStatus /prod /in "%CURRENT_PATH%\Mobile.au3"
"C:\Program Files (x86)\AutoIt3\SciTE\AutoIt3Wrapper\AutoIt3Wrapper.exe" /NoStatus /prod /in "%CURRENT_PATH%\DMServer.au3"
"C:\Program Files (x86)\AutoIt3\SciTE\AutoIt3Wrapper\AutoIt3Wrapper.exe" /NoStatus /prod /in "%CURRENT_PATH%\DMClient.au3"
