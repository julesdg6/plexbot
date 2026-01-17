@echo off
setlocal enabledelayedexpansion

echo ===================================
echo PlexBot Installation Script
echo ===================================
echo.

REM Set paths relative to the script location
set "SCRIPT_DIR=%~dp0"
set "ROOT_DIR=%SCRIPT_DIR%.."
set "DOCKER_DIR=%SCRIPT_DIR%Docker"

REM Check for Docker
docker --version >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo Docker is not installed. Please install Docker Desktop for Windows first.
    echo Visit: https://www.docker.com/products/docker-desktop
    pause
    exit /b 1
)

REM Check for Docker Compose
docker-compose --version >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo Docker Compose is not installed. It should come with Docker Desktop.
    echo Please ensure Docker is properly installed.
    pause
    exit /b 1
)

REM Create extensions directory if it doesn't exist
if not exist "%DOCKER_DIR%\Extensions" mkdir "%DOCKER_DIR%\Extensions"

REM Create plugins directory if it doesn't exist
if not exist "%DOCKER_DIR%\plugins" mkdir "%DOCKER_DIR%\plugins"
    
REM Check if .env file exists and prompt if it does not
if not exist "%ROOT_DIR%\.env" (
    echo.
    echo Please update the .env file with your Discord token and Plex server details.
    echo You MUST rename to .env and update the file with your own credentials before continuing.
    echo.
    pause
    exit /b 1
)

REM Make sure data and logs directories exist
if not exist "%ROOT_DIR%\data" mkdir "%ROOT_DIR%\data"
if not exist "%ROOT_DIR%\logs" mkdir "%ROOT_DIR%\logs"

echo.
echo Building and starting Docker containers...
echo.

REM Navigate to the Docker directory and run docker-compose
cd "%DOCKER_DIR%"

REM Stop and remove existing containers, networks, and volumes
docker-compose down --volumes --remove-orphans

REM Remove any existing images
docker rmi -f plexbot:latest
docker rmi -f ghcr.io/lavalink-devs/lavalink:4

REM Clear build cache
docker builder prune -f

REM Build and start the containers
docker-compose -p plexbot up -d --build

echo.
echo PlexBot installation completed successfully!
echo The bot should now be running in the background.
echo.
echo You can check the logs with: docker-compose logs -f
echo.
pause