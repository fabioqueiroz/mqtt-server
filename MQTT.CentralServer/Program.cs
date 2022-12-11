using MQTT.CentralServer.Api.Services;
using MQTT.CentralServer.Api.Services.Interfaces;
using MQTT.CentralServer.WorkerService.Jobs;
using MQTT.CentralServer.WorkerService.Schedule;
using MQTT.CentralServer.WorkerService.Services;
using MQTT.CentralServer.WorkerService.Services.Interfaces;
using Quartz.Spi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services
    .AddScoped<IActivateMqttService, ActivateMqttService>()
    .AddScoped<IMqttJobService, MqttJobService>();

builder.Services
    .AddSingleton<IJobFactory, JobFactory>()
    .AddSingleton<ActivateMqttJob>();

builder.Services.AddSingleton(new JobSchedule(
    jobType: typeof(ActivateMqttJob),
    cronExpression: "0/5 * * * * ?"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
