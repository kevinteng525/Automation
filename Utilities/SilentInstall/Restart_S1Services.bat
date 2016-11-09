::===========================================================================================
::
:: Copyright 2013 EMC Corporation.  All rights reserved.  This software 
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
:: 	Restart_S1Services.bat
::
:: CREATED
::   	06.08.2012
::
:: AUTHOR
::    	Travis Liu
::
:: DESCRIPTION
::   	Restart's All ES1 Services
::
:: LAST UPDATED 
::   	12.10.2012 Simon Shi
::
:: VERSION	
::	 7.0.0.1
::
:: USAGE
:: 	SILENT MODE : Command Prompt> Restart_S1Services.bat
::
:: ===========================================================================================

@echo off

echo Restarting Services...
echo ======================================================
net stop "EMC SourceOne Archive"
net stop "EMC SourceOne Indexer"
net stop "EMC SourceOne Query"
net stop "EMC SourceOne Administrator"
net stop "EMC SourceOne Address Cache"
net stop "EMC SourceOne Address Resolution"
net stop "EMC SourceOne Document Management Service"
net stop "EMC SourceOne Job Dispatcher"
net stop "EMC SourceOne Job Scheduler"
net stop "EMC SourceOne Offline Access Retrieval Service"
net stop "EMC SourceOne Search Service"

pause

net start "EMC SourceOne Archive"
net start "EMC SourceOne Indexer"
net start "EMC SourceOne Query"
net start "EMC SourceOne Administrator"
net start "EMC SourceOne Address Cache"
net start "EMC SourceOne Address Resolution"
net start "EMC SourceOne Document Management Service"
net start "EMC SourceOne Job Dispatcher"
net start "EMC SourceOne Job Scheduler"
net start "EMC SourceOne Offline Access Retrieval Service"
net start "EMC SourceOne Search Service"

echo ======================================================
echo All ES1 Services Restarted

pause