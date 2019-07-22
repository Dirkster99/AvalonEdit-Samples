@ECHO OFF
pushd "%~dp0"
ECHO.
ECHO.
ECHO.
ECHO This script deletes all temporary build files in their
ECHO corresponding BIN and OBJ Folder contained in the following projects
ECHO.
ECHO Apps\AEDemo
ECHO TextEditLib
ECHO.
REM Ask the user if hes really sure to continue beyond this point XXXXXXXX
set /p choice=Are you sure to continue (Y/N)?
if not '%choice%'=='Y' Goto EndOfBatch
REM Script does not continue unless user types 'Y' in upper case letter
ECHO.
ECHO XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
ECHO.
ECHO XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
RMDIR /S /Q .\.vs

ECHO.
ECHO Deleting BIN and OBJ Folders in TextEditLib
ECHO.
RMDIR /S /Q .\TextEditLib\bin
RMDIR /S /Q .\TextEditLib\obj
ECHO.

ECHO.
ECHO Deleting BIN and OBJ Folders in Apps\AEDemo
ECHO.
RMDIR /S /Q .\Apps\AEDemo\bin
RMDIR /S /Q .\Apps\AEDemo\obj
ECHO.

PAUSE

:EndOfBatch
