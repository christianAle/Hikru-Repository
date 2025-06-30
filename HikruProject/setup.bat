@echo off
REM Hikru Assessment API - Quick Setup Script for Windows
REM This script sets up the Oracle Database and starts the API

echo 🚀 Starting Hikru Assessment Management API Setup...

REM Check if Docker is running
docker info >nul 2>&1
if errorlevel 1 (
    echo ❌ Docker is not running. Please start Docker Desktop and try again.
    pause
    exit /b 1
)

echo ✅ Docker is running

REM Start the database
echo 🗄️  Starting Oracle Database container...
docker-compose up -d oracle-db

echo ⏳ Waiting for database to initialize (this may take 2-5 minutes)...

REM Wait for database to be healthy
:wait_loop
for /f "tokens=*" %%i in ('docker inspect --format="{{.State.Health.Status}}" hikru-oracle-db 2^>nul') do set health_status=%%i
if not "%health_status%"=="healthy" (
    echo ⏳ Database still initializing...
    timeout /t 30 /nobreak >nul
    goto wait_loop
)

echo ✅ Database is healthy and ready!

REM Display connection information
echo.
echo 🎉 Setup completed successfully!
echo.
echo 📊 Database Information:
echo    Host: localhost
echo    Port: 1521
echo    Service: XE
echo    Username: hikru_user
echo    Password: HikruUser123
echo.
echo 🌐 Next steps:
echo    1. cd Hikru
echo    2. dotnet run
echo    3. Open https://localhost:5001 in your browser
echo.
echo 🔑 API Key for testing: hikru-api-key-2025
echo.
echo 📖 For more information, see Database/README-Docker.md
echo.
pause
