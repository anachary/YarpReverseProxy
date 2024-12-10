
var builder = WebApplication.CreateBuilder(args);

// Load the configuration from appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var config = builder.Configuration.GetSection("ReverseProxy");

// Add YARP services and configure them from appsettings.json
builder.Services.AddReverseProxy()
    .LoadFromConfig(config);

var app = builder.Build();

// Use YARP Middleware
app.MapReverseProxy();

app.Run();
