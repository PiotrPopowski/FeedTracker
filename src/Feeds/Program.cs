using FeedTracker.Feeds;
using FeedTracker.Feeds.Weather;
using FeedTracker.Shared.Observability;
using Microsoft.AspNetCore.Mvc;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddLogging()
    .AddSwagger()
    .AddServices()
    .AddInfrastructure();


var app = builder.Build();

app.UseCorrelationId();

app.MapGet("/Stop", async ([FromServices]ISchedulerFactory schedulerFactory) => 
{
    var scheduler = await schedulerFactory.GetScheduler();
    await scheduler.PauseJob(JobKey.Create(nameof(WeatherBackgroundService)));
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.Run();
