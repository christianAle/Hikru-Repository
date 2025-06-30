#!/bin/bash

# Hikru Assessment API - Quick Setup Script
# This script sets up the Oracle Database and starts the API

echo "🚀 Starting Hikru Assessment Management API Setup..."

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker is not running. Please start Docker Desktop and try again."
    exit 1
fi

echo "✅ Docker is running"

# Check if logged into Oracle Container Registry
echo "🔐 Checking Oracle Container Registry login..."
if ! docker pull container-registry.oracle.com/database/express:21.3.0-xe > /dev/null 2>&1; then
    echo "❌ Not logged into Oracle Container Registry or license not accepted."
    echo "Please visit: https://container-registry.oracle.com/"
    echo "1. Sign in with your Oracle account"
    echo "2. Navigate to Database → express"
    echo "3. Accept the license agreement"
    echo "4. Run: docker login container-registry.oracle.com"
    exit 1
fi

echo "✅ Oracle Container Registry access confirmed"

# Start the database
echo "🗄️  Starting Oracle Database container..."
docker-compose up -d oracle-db

echo "⏳ Waiting for database to initialize (this may take 2-5 minutes)..."

# Wait for database to be healthy
while [ "$(docker inspect --format='{{.State.Health.Status}}' hikru-oracle-db 2>/dev/null)" != "healthy" ]; do
    echo "⏳ Database still initializing..."
    sleep 30
done

echo "✅ Database is healthy and ready!"

# Display connection information
echo ""
echo "🎉 Setup completed successfully!"
echo ""
echo "📊 Database Information:"
echo "   Host: localhost"
echo "   Port: 1521"
echo "   Service: XE"
echo "   Username: hikru_user"
echo "   Password: HikruUser123"
echo ""
echo "🌐 Next steps:"
echo "   1. cd Hikru"
echo "   2. dotnet run"
echo "   3. Open https://localhost:5001 in your browser"
echo ""
echo "🔑 API Key for testing: hikru-api-key-2025"
echo ""
echo "📖 For more information, see Database/README-Docker.md"
