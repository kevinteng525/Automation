
PATH=%PATH%;C:\Program Files\Microsoft SDKs\Windows\v7.0A\Bin\
PATH=%PATH%;C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\
PATH=%PATH%;%WINDIR%\Microsoft.NET\Framework\v4.0.30319\

REM al.exe

sn.exe -k keypair.snk
sn.exe -p keypair.snk publickey.pfx

mkdir Result

ildasm.exe .\ExJBSharePointArchiveUI.dll /out:.\Result\ExJBSharePointArchiveUI.il
ilasm.exe .\Result\ExJBSharePointArchiveUI.il /dll /key=.\keypair.snk /output=.\ExJBSharePointArchiveUI_signed.dll

copy /y ExJBSharePointArchiveUI.dll ExJBSharePointArchiveUI.dll.orig.dll
copy /y ExJBSharePointArchiveUI_signed.dll ExJBSharePointArchiveUI.dll