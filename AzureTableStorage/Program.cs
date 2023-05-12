using AzureTableStorage;
using Serilog;
using Serilog.Exceptions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var serilogLogger = new LoggerConfiguration()
    .Enrich.WithProperty("Oid", Guid.NewGuid().ToString())
    .Enrich.With(new OperationEnricher())
    .Enrich.FromLogContext()
    .Enrich.WithExceptionDetails()
    .WriteTo.AzureTableStorage(
        connectionString:"ConnectionString",
        storageTableName:"storageTableName",
        propertyColumns: new [] {"Oid", "Rid", "SourceContext"})
    .CreateLogger();

builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddSerilog(serilogLogger, true);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();