# Deployment Guide

Complete guide for deploying the Blazor Component Library in various environments.

## Prerequisites

- .NET 10 SDK or runtime
- Access to target deployment environment
- Appropriate permissions and credentials

## Deployment Methods

### Method 1: Docker Deployment (Recommended)

The simplest deployment method using containerization.

#### Build and Run Locally

```bash
# Build Docker image
docker build -t blazor-component-library:latest .

# Run container
docker run -p 5000:5000 -p 5001:5001 blazor-component-library:latest

# Access at http://localhost:5000
```

#### Using Docker Compose

```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down
```

#### Push to Docker Registry

```bash
# Tag image
docker tag blazor-component-library:latest myregistry/blazor-component-library:1.2.0

# Login to registry
docker login myregistry

# Push image
docker push myregistry/blazor-component-library:1.2.0
```

### Method 2: Direct Deployment to Windows/Linux

#### Prerequisites
- .NET 10 runtime installed
- IIS (Windows) or nginx/systemd (Linux)

#### Steps

1. **Build the application**
   ```bash
   dotnet build --configuration Release
   dotnet publish --configuration Release --output ./publish
   ```

2. **Copy to server**
   ```bash
   # Windows
   xcopy publish C:\inetpub\wwwroot\BlazorComponentLibrary /E

   # Linux
   rsync -av publish/ user@server:/var/www/blazor-component-library/
   ```

3. **Configure IIS** (Windows)
   - Open IIS Manager
   - Create new Application Pool (.NET v10.0)
   - Create new Website pointing to published folder
   - Ensure app pool has proper permissions

4. **Configure nginx** (Linux)
   ```nginx
   server {
       listen 80;
       server_name example.com;

       location / {
           proxy_pass http://localhost:5000;
           proxy_http_version 1.1;
           proxy_set_header Upgrade $http_upgrade;
           proxy_set_header Connection keep-alive;
           proxy_set_header Host $host;
           proxy_cache_bypass $http_upgrade;
       }
   }
   ```

5. **Enable systemd service** (Linux)
   ```bash
   sudo cp blazor-component-library.service /etc/systemd/system/
   sudo systemctl daemon-reload
   sudo systemctl enable blazor-component-library
   sudo systemctl start blazor-component-library
   ```

### Method 3: Azure App Service

#### Prerequisites
- Azure subscription
- Azure CLI installed

#### Steps

1. **Create App Service**
   ```bash
   az appservice plan create \
     --name blazor-plan \
     --resource-group myResourceGroup \
     --sku B1 \
     --is-linux

   az webapp create \
     --resource-group myResourceGroup \
     --plan blazor-plan \
     --name blazor-component-library \
     --runtime "DOTNETCORE|10.0"
   ```

2. **Deploy code**
   ```bash
   # Publish to Azure
   dotnet publish --configuration Release
   az webapp deployment source config-zip \
     --resource-group myResourceGroup \
     --name blazor-component-library \
     --src publish.zip
   ```

3. **Configure environment variables**
   ```bash
   az webapp config appsettings set \
     --resource-group myResourceGroup \
     --name blazor-component-library \
     --settings ASPNETCORE_ENVIRONMENT=Production
   ```

### Method 4: AWS Deployment

#### Using Elastic Beanstalk

1. **Install AWS CLI and EB CLI**
   ```bash
   pip install awsebcli --upgrade --user
   ```

2. **Initialize application**
   ```bash
   cd blazor-component-library
   eb init -p "Docker running on 64bit Amazon Linux 2"
   ```

3. **Create environment and deploy**
   ```bash
   eb create production-env
   eb deploy
   ```

4. **View logs**
   ```bash
   eb logs
   ```

#### Using ECS

1. **Create Docker image and push to ECR**
   ```bash
   aws ecr create-repository --repository-name blazor-component-library
   docker tag blazor-component-library:latest \
     [account-id].dkr.ecr.[region].amazonaws.com/blazor-component-library:latest
   docker push [account-id].dkr.ecr.[region].amazonaws.com/blazor-component-library:latest
   ```

2. **Create ECS task definition and service**
   - Configure container: image, port mappings (5000, 5001)
   - Set CPU/memory allocation
   - Create service with load balancer

## Configuration for Production

### Environment Variables

Set these for production deployments:

```bash
# Core settings
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:80

# Security
ASPNETCORE_FORWARDEDHEADERS_ENABLED=true

# Logging
Logging__LogLevel__Default=Information
Logging__LogLevel__BlazorComponentLibrary=Information

# Library-specific
BlazorComponentLibrary__EnableCaching=true
BlazorComponentLibrary__CacheDurationMinutes=60
BlazorComponentLibrary__EnableLogging=true
```

### appsettings.Production.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "BlazorComponentLibrary": "Information"
    }
  },
  "BlazorComponentLibrary": {
    "EnableCaching": true,
    "CacheDurationMinutes": 60,
    "DefaultPageSize": 50,
    "EnableRateLimiting": true,
    "RequestsPerMinute": 100,
    "EnableLogging": true
  }
}
```

## Security Best Practices

### HTTPS/TLS

Always use HTTPS in production:

```bash
# Azure
az webapp update --name blazor-component-library \
  --https-only true

# IIS
# Configure binding to use HTTPS with valid certificate
```

### Authentication & Authorization

1. **Enable ASP.NET Core Authentication**
   ```csharp
   services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
       .AddCookie();
   
   services.AddAuthorization();
   ```

2. **Require authentication for sensitive endpoints**
   ```csharp
   [Authorize]
   public class ProtectedController : ControllerBase { }
   ```

### Rate Limiting

Enable and configure rate limiting:

```csharp
services.AddBlazorComponentLibrary(options =>
{
    options.EnableRateLimiting = true;
    // Rate limiter configured per endpoint
});
```

### CORS Configuration

Configure CORS for allowed origins:

```csharp
services.AddCors(options =>
{
    options.AddPolicy("AllowedOrigins", builder =>
        builder
            .WithOrigins("https://yourdomain.com")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

app.UseCors("AllowedOrigins");
```

## Database Configuration

### Using SQL Server

1. **Update connection string**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=sqlserver.database.windows.net;Database=BlazorDB;User Id=sa;Password=YourPassword;"
     }
   }
   ```

2. **Implement custom repository**
   ```csharp
   public class SqlServerComponentRepository : IComponentRepository
   {
       private readonly ApplicationDbContext _context;

       public async Task<ComponentConfig> CreateAsync(ComponentConfig config)
       {
           _context.Components.Add(config);
           await _context.SaveChangesAsync();
           return config;
       }
       // ... implement other methods
   }
   ```

3. **Register custom repository**
   ```csharp
   services.AddScoped<IComponentRepository, SqlServerComponentRepository>();
   ```

### Using MongoDB

1. **Install MongoDB driver**
   ```bash
   dotnet add package MongoDB.Driver
   ```

2. **Configure connection**
   ```csharp
   var mongoClient = new MongoClient("mongodb://localhost:27017");
   var database = mongoClient.GetDatabase("BlazorDB");
   ```

3. **Implement MongoDB repository**
   ```csharp
   public class MongoComponentRepository : IComponentRepository
   {
       private readonly IMongoCollection<ComponentConfig> _collection;
       
       // Implementation
   }
   ```

## Monitoring & Logging

### Application Insights

```csharp
services.AddApplicationInsightsTelemetry(configuration);

// Log custom events
var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Application started");
```

### Structured Logging

Use Serilog for structured logging:

```bash
dotnet add package Serilog
dotnet add package Serilog.AspNetCore
```

```csharp
builder.Host.UseSerilog((context, configuration) =>
    configuration
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day));
```

### Health Checks

```csharp
services.AddHealthChecks();

app.MapHealthChecks("/health");
```

## Scaling

### Horizontal Scaling

1. **Load Balancing**
   - Configure multiple instances behind load balancer
   - Use sticky sessions if needed
   - Monitor health checks

2. **Caching Strategy**
   - Enable distributed cache (Redis)
   - Set appropriate TTLs
   - Monitor cache hit rates

### Vertical Scaling

- Increase CPU and memory allocation
- Monitor resource usage
- Adjust application pool settings

## Backup & Recovery

### Database Backups

```bash
# SQL Server
BACKUP DATABASE BlazorDB TO DISK = '/backups/BlazorDB.bak'

# MongoDB
mongodump --db BlazorDB --out /backups/
```

### Application Backup

```bash
# Archive application
tar -czf blazor-backup.tar.gz /var/www/blazor-component-library/

# Upload to backup storage
aws s3 cp blazor-backup.tar.gz s3://my-backup-bucket/
```

## Troubleshooting

### Application won't start

1. Check logs: `docker logs container-name`
2. Verify configuration files are valid
3. Ensure all dependencies are installed
4. Check environment variables

### High memory usage

1. Enable garbage collection: `DOTNET_GCHeapCount=4`
2. Reduce cache TTL
3. Monitor and optimize queries
4. Enable memory profiling

### Connection issues

1. Verify network connectivity
2. Check firewall rules
3. Validate connection strings
4. Review DNS resolution

---

For more help, see the [FAQ](faq.md) or visit https://sarmkadan.com
