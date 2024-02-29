using FeedTracker.Shared.Logging;
using FeedTracker.Shared.Observability;
using FeedTracker.Subscribers.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddSerilog();

builder.Logging.UseOpenTelemetry("subscribers");
builder.Services.AddOpenTelemerty("subscribers");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ISubscriberService, SubscriberService>();
builder.Services.AddGrpc();

var app = builder.Build();

app.UseCorrelationId();

app.MapGrpcService<SubscriberGrpcService>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
