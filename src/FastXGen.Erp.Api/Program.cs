using FastXGen.Erp.Api.Setup;
using FastXGen.Erp.Application;
using FastXGen.Erp.Infrastructure;
using FastXGen.Erp.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

builder.Services
    .AddApi(builder.Configuration)
    .AddApplication()
    .AddInfrastructure()
    .AddPersistence(builder.Configuration);

var app = builder.Build();

app.UseApi();
app.Run();

/// <summary>Entry point, exposed for integration tests.</summary>
public partial class Program;
