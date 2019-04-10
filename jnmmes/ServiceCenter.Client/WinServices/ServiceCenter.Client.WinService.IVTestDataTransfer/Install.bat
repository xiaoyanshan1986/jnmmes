@ECHO OFF
REM The following directory is for .NET4.0
set DOTNETFX=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319;%SystemRoot%\Microsoft.NET\Framework\v2.0.50727;
set PATH=%PATH%;%DOTNETFX%

echo 正在安装服务
echo ---------------------------------------------------

InstallUtil /i ServiceCenter.Client.WinService.IVTestDataTransfer.exe

echo ---------------------------------------------------

echo Done.
pause
exit