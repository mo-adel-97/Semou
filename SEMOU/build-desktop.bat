@echo off
REM ===== SEMOU Release build -> Desktop\SEMOU_Build =====
setlocal
set "PROJ=%~dp0SSTLI\SEMOU.vbproj"
set "REFS=%~dp0SSTLI\bin\Debug"
set "OUT=%USERPROFILE%\Desktop\SEMOU_Build"

for /f "usebackq tokens=*" %%i in (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`) do set "MSBUILD=%%i"
if not defined MSBUILD set "MSBUILD=C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe"

echo MSBuild: %MSBUILD%
echo Output : %OUT%
"%MSBUILD%" "%PROJ%" /t:Rebuild /p:Configuration=Release /p:OutputPath="%OUT%\\" /p:ReferencePath="%REFS%" /v:m /nologo
if errorlevel 1 (
    echo.
    echo ***** BUILD FAILED - see errors above *****
    if not "%1"=="/nopause" pause
    exit /b 1
)
echo.
echo ***** BUILD OK: %OUT%\SEMOU.exe *****
if not "%1"=="/nopause" (
    start "" "%OUT%"
    pause
)
