﻿using MQTT.CentralServer.WorkerService.Schedule;
using MQTT.CentralServer.WorkerService.Services.Interfaces;
using Quartz.Impl;
using Quartz.Spi;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using MQTT.CentralServer.Services.Interfaces;
using MQTT.CentralServer.WorkerService.Jobs.Constants;

namespace MQTT.CentralServer.WorkerService.Services
{
    public class MqttJobService : IHostedService, IMqttJobService
    {
        private readonly ILogger<MqttJobService> _logger;
        private StdSchedulerFactory _factory;
        public IScheduler? Scheduler { get; set; }
        private readonly IJobFactory _jobFactory;
        private readonly IEnumerable<JobSchedule> _jobSchedules;
        private readonly ISchedulerStatusService _schedulerStatusService;

        public MqttJobService(ILogger<MqttJobService> logger, IJobFactory jobFactory, IEnumerable<JobSchedule> jobSchedules, ISchedulerStatusService schedulerStatusService)
        {
            _logger = logger;
            _factory = new StdSchedulerFactory(GetQuartzConfigurationProperties());
            _jobFactory = jobFactory;
            _jobSchedules = jobSchedules;
            _schedulerStatusService = schedulerStatusService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Scheduler = await _factory.GetScheduler(cancellationToken);
            Scheduler.JobFactory = _jobFactory;

            foreach (var jobSchedule in _jobSchedules)
            {
                var job = CreateJob(jobSchedule);
                var trigger = CreateTrigger(jobSchedule);

                if (!await Scheduler.CheckExists(job.Key, cancellationToken))
                {
                    await Scheduler.ScheduleJob(job, trigger, cancellationToken);
                }
            }

            await Scheduler.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Scheduler = await _factory.GetScheduler(cancellationToken);

            await Scheduler!.Shutdown(cancellationToken);
          
            _logger.LogInformation("===== Job shut down requested. =====");
        }

        private static IJobDetail CreateJob(JobSchedule schedule)
        {
            var jobType = schedule.JobType;
            return JobBuilder
                .Create(jobType)
                .WithIdentity(jobType.FullName!)
                .WithDescription(jobType.Name)
                .StoreDurably(durability: true)
                .Build();
        }

        private static ITrigger CreateTrigger(JobSchedule schedule)
        {
            return TriggerBuilder
                .Create()
                .WithIdentity($"{schedule.JobType.FullName}.trigger")
                .WithCronSchedule(schedule.CronExpression)
                .WithDescription(schedule.CronExpression)
                .StartNow()
                .WithSimpleSchedule(x => x
                     .WithIntervalInSeconds(JobValueConstants.TimeInterval)
                     .RepeatForever())
                .Build();
        }

        private static NameValueCollection GetQuartzConfigurationProperties() 
        {
            return new NameValueCollection
            {

                { "quartz.serializer.type", "json" },
                { "quartz.jobStore.useProperties", "true" },
                { "quartz.jobStore.type", "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz" },
                { "quartz.jobStore.dataSource", "default" },
                { "quartz.dataSource.default.provider", "SqlServer" },
                { "quartz.jobStore.tablePrefix", "QRTZ_" },
                { "quartz.jobStore.performSchemaValidation", "false" },
                // TODO: move the connection string elsewhere
                //{ "quartz.dataSource.default.connectionString", "Data Source=.\\SQLEXPRESS;Initial Catalog=Quartz;Integrated Security=False;User Id=sa;Password=Fabio1980; TrustServerCertificate=True" },
                { "quartz.dataSource.default.connectionString", "Data Source=.\\SQLEXPRESS;Initial Catalog=Quartz_Migration_1;Integrated Security=False;User Id=sa;Password=Fabio1980; TrustServerCertificate=True" },
                { "quartz.jobStore.clustered", "false" },
                { "quartz.jobStore.driverDelegateType", "Quartz.Impl.AdoJobStore.SqlServerDelegate, Quartz" }
            };
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
