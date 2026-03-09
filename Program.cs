using GenericCalcLogViewer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Register application services
// Singleton services (stateless, shared across all requests)
builder.Services.AddSingleton<IEnvironmentService, EnvironmentService>();
builder.Services.AddSingleton<ILogParser, LogParser>();
builder.Services.AddSingleton<ILogMergeService, LogMergeService>();

// Scoped services (per request)
builder.Services.AddScoped<IDatabaseLogService, DatabaseLogService>();
builder.Services.AddScoped<IFileLogService, FileLogService>();
builder.Services.AddScoped<ILogSearchService, LogSearchService>();

// Add logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
    logging.SetMinimumLevel(LogLevel.Information);
});

// Add CORS if needed
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowInternalNetwork", policy =>
    {
        policy.WithOrigins("http://internal-app.company.com")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseCors("AllowInternalNetwork");
app.UseAuthorization();
app.MapControllers();

app.Run();
