@echo off
setlocal

cd /d "%~dp0"

echo.
echo ========================================
echo        TaxOra Docker Deployment
echo ========================================
echo.

echo Stopping existing containers...
docker compose down

if errorlevel 1 (
    echo ERROR: Could not stop existing containers.
    pause
    exit /b 1
)

echo.
echo Building and starting TaxOra...
docker compose up -d --build

if errorlevel 1 (
    echo ERROR: Docker deployment failed.
    pause
    exit /b 1
)

echo.
echo Running containers:
docker ps

echo.
echo ========================================
echo     TaxOra deployment successful
echo ========================================
echo.
echo Frontend:
echo http://localhost:8081
echo.
echo Backend:
echo https://localhost:7131
echo.

pause