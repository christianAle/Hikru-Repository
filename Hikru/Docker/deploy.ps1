# Hikru Assessment Management API - Docker Deployment Script
param(
    [Parameter(HelpMessage="Action to perform: build, start, stop, restart, logs, status, clean")]
    [ValidateSet("build", "start", "stop", "restart", "logs", "status", "clean", "help")]
    [string]$Action = "help"
)

$ErrorActionPreference = "Stop"

function Write-ColorOutput {
    param(
        [string]$Message,
        [string]$Color = "White"
    )
    
    switch ($Color) {
        "Green" { Write-Host $Message -ForegroundColor Green }
        "Red" { Write-Host $Message -ForegroundColor Red }
        "Yellow" { Write-Host $Message -ForegroundColor Yellow }
        "Blue" { Write-Host $Message -ForegroundColor Blue }
        "Cyan" { Write-Host $Message -ForegroundColor Cyan }
        default { Write-Host $Message }
    }
}

function Test-DockerRunning {
    try {
        docker info | Out-Null
        return $true
    }
    catch {
        return $false
    }
}

function Test-OracleRegistryLogin {
    try {
        docker pull container-registry.oracle.com/database/express:21.3.0-xe --disable-content-trust 2>$null | Out-Null
        return $true
    }
    catch {
        return $false
    }
}

function Show-Help {
    Write-ColorOutput "🚀 Hikru Assessment Management API - Docker Deployment Script" "Cyan"
    Write-ColorOutput ""
    Write-ColorOutput "USAGE:" "Yellow"
    Write-ColorOutput "  .\deploy.ps1 -Action <action>" 
    Write-ColorOutput ""
    Write-ColorOutput "ACTIONS:" "Yellow"
    Write-ColorOutput "  build    - Build all Docker images"
    Write-ColorOutput "  start    - Start all services (build if needed)"
    Write-ColorOutput "  stop     - Stop all services"
    Write-ColorOutput "  restart  - Restart all services"
    Write-ColorOutput "  logs     - Show logs from all services"
    Write-ColorOutput "  status   - Show status of all services"
    Write-ColorOutput "  clean    - Stop and remove all containers, images, and volumes"
    Write-ColorOutput "  help     - Show this help message"
    Write-ColorOutput ""
    Write-ColorOutput "EXAMPLES:" "Yellow"
    Write-ColorOutput "  .\deploy.ps1 -Action start    # Start the application"
    Write-ColorOutput "  .\deploy.ps1 -Action logs     # View logs"
    Write-ColorOutput "  .\deploy.ps1 -Action stop     # Stop the application"
    Write-ColorOutput ""
    Write-ColorOutput "ACCESS POINTS:" "Yellow"
    Write-ColorOutput "  API Swagger UI:   http://localhost:8080/swagger"
    Write-ColorOutput "  API Health:       http://localhost:8080/health"
    Write-ColorOutput "  Oracle EM:        https://localhost:5500/em"
    Write-ColorOutput "  API Key:          hikru-api-key-2025"
}

function Build-Images {
    Write-ColorOutput "🔨 Building Docker images..." "Blue"
    Set-Location $PSScriptRoot
    
    try {
        Write-ColorOutput "Building Hikru Assessment API image..." "Yellow"
        docker-compose build --no-cache
        Write-ColorOutput "✅ Images built successfully!" "Green"
    }
    catch {
        Write-ColorOutput "❌ Failed to build images: $($_.Exception.Message)" "Red"
        exit 1
    }
}

function Start-Services {
    Write-ColorOutput "🚀 Starting Hikru Assessment Management API..." "Blue"
    Set-Location $PSScriptRoot
    
    try {
        Write-ColorOutput "Starting services..." "Yellow"
        docker-compose up -d
        
        Write-ColorOutput "⏳ Waiting for services to be healthy..." "Yellow"
        $timeout = 300
        $elapsed = 0
        
        do {
            Start-Sleep -Seconds 10
            $elapsed += 10
            
            $dbHealth = docker inspect --format='{{.State.Health.Status}}' hikru-oracle-db 2>$null
            $apiHealth = docker inspect --format='{{.State.Health.Status}}' hikru-assessment-api 2>$null
            
            Write-ColorOutput "Database: $dbHealth, API: $apiHealth" "Cyan"
            
            if ($elapsed -ge $timeout) {
                Write-ColorOutput "⚠️ Timeout waiting for services to be healthy" "Yellow"
                break
            }
        } while ($dbHealth -ne "healthy" -or $apiHealth -ne "healthy")
        
        if ($dbHealth -eq "healthy" -and $apiHealth -eq "healthy") {
            Write-ColorOutput "✅ All services are healthy and ready!" "Green"
            Write-ColorOutput ""
            Write-ColorOutput "🌐 Access Points:" "Cyan"
            Write-ColorOutput "  📚 API Documentation: http://localhost:8080/swagger" "Green"
            Write-ColorOutput "  ❤️ Health Check:      http://localhost:8080/health" "Green"
            Write-ColorOutput "  🗄️ Oracle Enterprise: https://localhost:5500/em" "Green"
            Write-ColorOutput "  🔑 API Key:           hikru-api-key-2025" "Yellow"
        } else {
            Write-ColorOutput "⚠️ Some services may not be fully ready. Check logs with: .\deploy.ps1 -Action logs" "Yellow"
        }
    }catch {
        Write-ColorOutput "❌ Failed to start services: $($_.Exception.Message)" "Red"
        exit 1
    }
}

function Stop-Services {
    Write-ColorOutput "🛑 Stopping services..." "Blue"
    Set-Location $PSScriptRoot
    
    try {
        docker-compose down
        Write-ColorOutput "✅ Services stopped successfully!" "Green"
    }
    catch {
        Write-ColorOutput "❌ Failed to stop services: $($_.Exception.Message)" "Red"
        exit 1
    }
}

function Restart-Services {
    Write-ColorOutput "🔄 Restarting services..." "Blue"
    Stop-Services
    Start-Services
}

function Show-Logs {
    Write-ColorOutput "📋 Showing logs..." "Blue"
    Set-Location $PSScriptRoot
    
    try {
        docker-compose logs -f
    }
    catch {
        Write-ColorOutput "❌ Failed to show logs: $($_.Exception.Message)" "Red"
        exit 1
    }
}

function Show-Status {
    Write-ColorOutput "📊 Service Status:" "Blue"
    Set-Location $PSScriptRoot
    
    try {
        docker-compose ps
        
        Write-ColorOutput ""
        Write-ColorOutput "🔍 Health Status:" "Blue"
        
        $containers = @("hikru-oracle-db", "hikru-assessment-api")
        foreach ($container in $containers) {
            $status = docker inspect --format='{{.State.Status}}' $container 2>$null
            $health = docker inspect --format='{{.State.Health.Status}}' $container 2>$null
            
            if ($status) {
                $color = if ($status -eq "running" -and $health -eq "healthy") { "Green" } else { "Yellow" }
                Write-ColorOutput "  $container : $status ($health)" $color
            } else {
                Write-ColorOutput "  $container : not found" "Red"
            }
        }
    }
    catch {
        Write-ColorOutput "❌ Failed to get status: $($_.Exception.Message)" "Red"
        exit 1
    }
}

function Remove-AllResources {
    Write-ColorOutput "🧹 Cleaning up all Docker resources..." "Blue"
    
    $confirmation = Read-Host "This will remove ALL containers, images, and volumes. Are you sure? (y/N)"
    if ($confirmation -ne "y" -and $confirmation -ne "Y") {
        Write-ColorOutput "❌ Cleanup cancelled." "Yellow"
        return
    }
    
    Set-Location $PSScriptRoot
    
    try {
        Write-ColorOutput "Stopping and removing containers..." "Yellow"
        docker-compose down -v --rmi all --remove-orphans
        
        Write-ColorOutput "Removing any remaining Hikru containers..." "Yellow"
        docker ps -a --filter "name=hikru" --format "{{.ID}}" | ForEach-Object { docker rm -f $_ 2>$null }
        
        Write-ColorOutput "Removing any remaining Hikru images..." "Yellow"
        docker images --filter "reference=*hikru*" --format "{{.ID}}" | ForEach-Object { docker rmi -f $_ 2>$null }
        
        Write-ColorOutput "✅ Cleanup completed!" "Green"
    }
    catch {
        Write-ColorOutput "❌ Failed to clean up: $($_.Exception.Message)" "Red"
        exit 1
    }
}

# Main script logic
Write-ColorOutput "🐳 Hikru Assessment Management API - Docker Deployment" "Cyan"
Write-ColorOutput "================================================================" "Cyan"

# Check if Docker is running
if (-not (Test-DockerRunning)) {
    Write-ColorOutput "❌ Docker is not running. Please start Docker Desktop and try again." "Red"
    exit 1
}

Write-ColorOutput "✅ Docker is running" "Green"

# Check Oracle Container Registry login for actions that need it
if ($Action -in @("build", "start", "restart")) {
    Write-ColorOutput "🔐 Checking Oracle Container Registry access..." "Blue"
    if (-not (Test-OracleRegistryLogin)) {
        Write-ColorOutput "❌ Oracle Container Registry access failed." "Red"
        Write-ColorOutput "Please follow these steps:" "Yellow"
        Write-ColorOutput "1. Visit: https://container-registry.oracle.com/" "Yellow"
        Write-ColorOutput "2. Sign in and navigate to Database → express" "Yellow"
        Write-ColorOutput "3. Accept the license agreement" "Yellow"
        Write-ColorOutput "4. Run: docker login container-registry.oracle.com" "Yellow"
        exit 1
    }
    Write-ColorOutput "✅ Oracle Container Registry access confirmed" "Green"
}

# Execute the requested action
switch ($Action) {
    "build" { Build-Images }
    "start" { Start-Services }
    "stop" { Stop-Services }
    "restart" { Restart-Services }
    "logs" { Show-Logs }
    "status" { Show-Status }
    "clean" { Remove-AllResources }
    "help" { Show-Help }
    default { Show-Help }
}

Write-ColorOutput ""
Write-ColorOutput "🎉 Operation completed!" "Green"
