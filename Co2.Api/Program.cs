using Co2.Api.Contracts;
using Co2.Api.Services;
using Co2Calc.Engine;

var builder = WebApplication.CreateBuilder(args);

// CORS для локальных фронтов
const string AllowLocal = "AllowLocal";
builder.Services.AddCors(o => o.AddPolicy(AllowLocal, p =>
    p.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
     .AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddHealthChecks();

// DI: движок + факторы + наш сервис
builder.Services.AddSingleton<IMethodology, GhgProtocolV1>();
builder.Services.AddSingleton<IFactorProvider, BasicFactorProvider>();
builder.Services.AddScoped<ICalculationService, CalculationService>();

var app = builder.Build();
app.UseCors(AllowLocal);

app.MapGet("/healthz", () => Results.Ok(new { ok = true, ts = DateTimeOffset.UtcNow }));

app.MapPost("/v1/calculations/page", (PageCalcRequest req, ICalculationService svc) =>
{
    try
    {
        return Results.Ok(svc.CalculatePage(req));
    }
    catch (ArgumentException ex) when (ex is ArgumentOutOfRangeException)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.Run();
