@echo off
setlocal

REM Always start from the folder where build.bat is located
cd /d "%~dp0"

echo.
echo ========================================
echo        TaxOra Production Build
echo ========================================
echo.

echo Project folder:
echo %CD%
echo.

REM ========================================
REM BACKEND
REM ========================================

echo [1/4] Restoring backend dependencies...
cd /d "%~dp0backend\TaxBankingApi"

dotnet restore

if errorlevel 1 (
    echo.
    echo ERROR: Backend restore failed.
    pause
    exit /b 1
)

echo.
echo [2/4] Publishing backend...

dotnet publish -c Release -o publish

if errorlevel 1 (
    echo.
    echo ERROR: Backend publish failed.
    echo Make sure the backend is not currently running.
    pause
    exit /b 1
)

REM ========================================
REM FRONTEND
REM ========================================

echo.
echo [3/4] Installing frontend dependencies...
cd /d "%~dp0frontend"

call npm install

if errorlevel 1 (
    echo.
    echo ERROR: Frontend dependency installation failed.
    pause
    exit /b 1
)

echo.
echo [4/4] Building frontend...

call npm run build

if errorlevel 1 (
    echo.
    echo ERROR: Frontend build failed.
    pause
    exit /b 1
)

REM ========================================
REM FINISHED
REM ========================================

cd /d "%~dp0"

echo.
echo ========================================
echo     TaxOra build completed successfully
echo ========================================
echo.
echo Backend output:
echo backend\TaxBankingApi\publish
echo.
echo Frontend output:
echo frontend\dist
echo.
pause