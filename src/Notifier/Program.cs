using MassTransit;
using FeedTracker.Shared.Observability;
using FeedTracker.Notifier;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddLogging()
    .AddSwagger()
    .RegisterServices()
    .AddInfrastructure();

var app = builder.Build();

app.UseCorrelationId();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();