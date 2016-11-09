SET FILETOWAIT=%~1%
:LoopStart
IF NOT EXIST %FILETOWAIT% (
echo Wait the file's presence %~1%
ping localhost -n 20>nil
GOTO :LoopStart
)