using Prometheus;
using MassTransit;
using FeedTracker.Shared.Observability;
using OpenTelemetry.Trace;
using FeedTracker.Aggregator;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddLogging()
    .AddSwagger()
    .AddServices()
    .AddInfrastructure();


var app = builder.Build();

app.UseCorrelationId();
app.UseMetricServer();

app.MapGet("/", (Tracer tracer) =>
{
    using var span = tracer.StartActiveSpan("Hello");
    return Results.Ok();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.Run();