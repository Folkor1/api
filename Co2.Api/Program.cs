var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHealthChecks();
var app = builder.Build();
app.MapGet("/healthz", () => Results.Ok(new { ok = true, ts = DateTimeOffset.UtcNow }));
app.MapPost("/v1/calculations/page", () => Results.Ok(new {
  runId = Guid.NewGuid(), co2e_g = 0, range = new { min = 0, max = 0 },
  factors = Array.Empty<object>(), methodologyVersion = "ghg-protocol:1.0"
}));
app.Run();
