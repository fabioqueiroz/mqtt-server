using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using MQTT.CentralServer.Api.Services;
using MQTT.CentralServer.Api.Services.Interfaces;
using MQTT.CentralServer.Api.SignalR;
using MQTT.CentralServer.Data.Access;
using MQTT.CentralServer.Data.Access.Interfaces;
using MQTT.CentralServer.Data.Access.Repositories;
using MQTT.CentralServer.Entities.Options;
using MQTT.CentralServer.Services.Interfaces;
using MQTT.CentralServer.Services.Messages;
using MQTT.CentralServer.Services.SchedulerStatus;
using MQTT.CentralServer.WorkerService.Jobs;
using MQTT.CentralServer.WorkerService.Schedule;
using MQTT.CentralServer.WorkerService.Services;
using MQTT.CentralServer.WorkerService.Services.Interfaces;
using Quartz.Spi;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services
    .AddScoped<IActivateMqttService, ActivateMqttService>()
    .AddScoped<IMqttJobService, MqttJobService>()
    .AddScoped<ISchedulerStatusRepository, SchedulerStatusRepository>()
    .AddScoped<ISchedulerStatusService, SchedulerStatusService>()
    .AddScoped<IIdentityServerService, IdentityServerService>()
    .AddScoped<IMqttMessageRepository, MqttMessageRepository>()
    .AddScoped<IMqttMessageService, MqttMessageService>();

builder.Services.AddSignalR();

builder.Services.AddHttpClient<IIdentityServerService, IdentityServerService>();

builder.Services.Configure<ApiConfigs>(builder.Configuration.GetSection(nameof(ApiConfigs)));

builder.Services
    .AddSingleton<IJobFactory, JobFactory>()
    .AddSingleton<ActivateMqttJob>();

builder.Services.AddSingleton(new JobSchedule(
    jobType: typeof(ActivateMqttJob),
    cronExpression: "0/5 * * * * ?"));

builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), 
    opts =>
    {
        opts.EnableRetryOnFailure((int)TimeSpan.FromSeconds(5).TotalSeconds);
        opts.CommandTimeout((int)TimeSpan.FromMinutes(2).TotalSeconds);
    }));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHub<MQTTMessageHub>("/messagehub");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
