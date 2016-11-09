@echo off
REM - File: RestartESService.bat
REM - Description: Restart's All ES1 Services
REM - Author: Travis Liu

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