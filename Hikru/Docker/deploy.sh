#!/bin/bash

# Hikru Assessment Management API - Docker Deployment Script (Bash)
# This script builds and runs the complete application stack using Docker

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Function to print colored output
print_color() {
    local color=$1
    local message=$2
    echo -e "${color}${message}${NC}"
}

# Function to check if Docker is running
check_docker() {
    if ! docker info > /dev/null 2>&1; then
        print_color $RED "❌ Docker is not running. Please start Docker and try again."
        exit 1
    fi
    print_color $GREEN "✅ Docker is running"
}

# Function to check Oracle Container Registry login
check_oracle_registry() {
    if ! docker pull container-registry.oracle.com/database/express:21.3.0-xe --disable-content-trust > /dev/null 2>&1; then
        print_color $RED "❌ Oracle Container Registry access failed."
        print_color $YELLOW "Please follow these steps:"
        print_color $YELLOW "1. Visit: https://container-registry.oracle.com/"
        print_color $YELLOW "2. Sign in and navigate to Database → express"
        print_color $YELLOW "3. Accept the license agreement"
        print_color $YELLOW "4. Run: docker login container-registry.oracle.com"
        exit 1
    fi
    print_color $GREEN "✅ Oracle Container Registry access confirmed"
}

# Function to display help
show_help() {
    print_color $CYAN "🚀 Hikru Assessment Management API - Docker Deployment Script"
    echo ""
    print_color $YELLOW "USAGE:"
    echo "  ./deploy.sh <action>"
    echo ""
    print_color $YELLOW "ACTIONS:"
    echo "  build    - Build all Docker images"
    echo "  start    - Start all services (build if needed)"
    echo "  stop     - Stop all services"
    echo "  restart  - Restart all services"
    echo "  logs     - Show logs from all services"
    echo "  status   - Show status of all services"
    echo "  clean    - Stop and remove all containers, images, and volumes"
    echo "  help     - Show this help message"
    echo ""
    print_color $YELLOW "EXAMPLES:"
    echo "  ./deploy.sh start    # Start the application"
    echo "  ./deploy.sh logs     # View logs"
    echo "  ./deploy.sh stop     # Stop the application"
    echo ""
    print_color $YELLOW "ACCESS POINTS:"
    echo "  API Swagger UI:   http://localhost:8080/swagger"
    echo "  API Health:       http://localhost:8080/health"
    echo "  Oracle EM:        https://localhost:5500/em"
    echo "  API Key:          hikru-api-key-2025"
}

# Function to build images
build_images() {
    print_color $BLUE "🔨 Building Docker images..."
    
    cd "$(dirname "$0")"
    
    print_color $YELLOW "Building Hikru Assessment API image..."
    docker-compose build --no-cache
    print_color $GREEN "✅ Images built successfully!"
}

# Function to start services
start_services() {
    print_color $BLUE "🚀 Starting Hikru Assessment Management API..."
    
    cd "$(dirname "$0")"
    
    print_color $YELLOW "Starting services..."
    docker-compose up -d
    
    print_color $YELLOW "⏳ Waiting for services to be healthy..."
    timeout=300 # 5 minutes
    elapsed=0
    
    while [ $elapsed -lt $timeout ]; do
        sleep 10
        elapsed=$((elapsed + 10))
        
        db_health=$(docker inspect --format='{{.State.Health.Status}}' hikru-oracle-db 2>/dev/null || echo "not found")
        api_health=$(docker inspect --format='{{.State.Health.Status}}' hikru-assessment-api 2>/dev/null || echo "not found")
        
        print_color $CYAN "Database: $db_health, API: $api_health"
        
        if [ "$db_health" = "healthy" ] && [ "$api_health" = "healthy" ]; then
            print_color $GREEN "✅ All services are healthy and ready!"
            echo ""
            print_color $CYAN "🌐 Access Points:"
            print_color $GREEN "  📚 API Documentation: http://localhost:8080/swagger"
            print_color $GREEN "  ❤️ Health Check:      http://localhost:8080/health"
            print_color $GREEN "  🗄️ Oracle Enterprise: https://localhost:5500/em"
            print_color $YELLOW "  🔑 API Key:           hikru-api-key-2025"
            return
        fi
    done
    
    print_color $YELLOW "⚠️ Timeout waiting for services to be healthy. Check logs with: ./deploy.sh logs"
}

# Function to stop services
stop_services() {
    print_color $BLUE "🛑 Stopping services..."
    
    cd "$(dirname "$0")"
    
    docker-compose down
    print_color $GREEN "✅ Services stopped successfully!"
}

# Function to restart services
restart_services() {
    print_color $BLUE "🔄 Restarting services..."
    stop_services
    start_services
}

# Function to show logs
show_logs() {
    print_color $BLUE "📋 Showing logs..."
    
    cd "$(dirname "$0")"
    
    docker-compose logs -f
}

# Function to show status
show_status() {
    print_color $BLUE "📊 Service Status:"
    
    cd "$(dirname "$0")"
    
    docker-compose ps
    
    echo ""
    print_color $BLUE "🔍 Health Status:"
    
    containers=("hikru-oracle-db" "hikru-assessment-api")
    for container in "${containers[@]}"; do
        status=$(docker inspect --format='{{.State.Status}}' "$container" 2>/dev/null || echo "not found")
        health=$(docker inspect --format='{{.State.Health.Status}}' "$container" 2>/dev/null || echo "no health check")
        
        if [ "$status" != "not found" ]; then
            if [ "$status" = "running" ] && [ "$health" = "healthy" ]; then
                print_color $GREEN "  $container : $status ($health)"
            else
                print_color $YELLOW "  $container : $status ($health)"
            fi
        else
            print_color $RED "  $container : not found"
        fi
    done
}

# Function to clean everything
clean_all() {
    print_color $BLUE "🧹 Cleaning up all Docker resources..."
    
    read -p "This will remove ALL containers, images, and volumes. Are you sure? (y/N): " confirmation
    if [ "$confirmation" != "y" ] && [ "$confirmation" != "Y" ]; then
        print_color $YELLOW "❌ Cleanup cancelled."
        return
    fi
    
    cd "$(dirname "$0")"
    
    print_color $YELLOW "Stopping and removing containers..."
    docker-compose down -v --rmi all --remove-orphans
    
    print_color $YELLOW "Removing any remaining Hikru containers..."
    docker ps -a --filter "name=hikru" --format "{{.ID}}" | xargs -r docker rm -f
    
    print_color $YELLOW "Removing any remaining Hikru images..."
    docker images --filter "reference=*hikru*" --format "{{.ID}}" | xargs -r docker rmi -f
    
    print_color $GREEN "✅ Cleanup completed!"
}

# Main script logic
print_color $CYAN "🐳 Hikru Assessment Management API - Docker Deployment"
print_color $CYAN "================================================================"

# Check if Docker is running
check_docker

ACTION=${1:-help}

# Check Oracle Container Registry login for actions that need it
if [[ "$ACTION" =~ ^(build|start|restart)$ ]]; then
    print_color $BLUE "🔐 Checking Oracle Container Registry access..."
    check_oracle_registry
fi

# Execute the requested action
case $ACTION in
    "build")
        build_images
        ;;
    "start")
        start_services
        ;;
    "stop")
        stop_services
        ;;
    "restart")
        restart_services
        ;;
    "logs")
        show_logs
        ;;
    "status")
        show_status
        ;;
    "clean")
        clean_all
        ;;
    "help"|*)
        show_help
        ;;
esac

echo ""
print_color $GREEN "🎉 Operation completed!"
