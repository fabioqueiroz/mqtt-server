using MQTT.CentralServer.WorkerService.Schedule;
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
using MQTT.CentralServer.Entities.Enums;
using Microsoft.Extensions.DependencyInjection;
using MQTT.CentralServer.Data.Access.Repositories;
using MQTT.CentralServer.Data.Access;
using MQTT.CentralServer.Services.SchedulerStatus;

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
                    //await _schedulerStatusService.RecordSchedulerStatusAsync(job.Key.Name, cancellationToken, ServiceStatus.Initializing);
                }
            }

            await Scheduler.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Scheduler = await _factory.GetScheduler(cancellationToken);

            foreach (var jobSchedule in _jobSchedules)
            {
                // replace this with something else not creating a new job
                var job = CreateJob(jobSchedule);

                if (await Scheduler.CheckExists(job.Key, cancellationToken))
                {
                    await _schedulerStatusService.RecordSchedulerStatusAsync(job.Key.Name, cancellationToken, ServiceStatus.Closing);
                }
            }

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
                     .WithIntervalInSeconds(30)
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
                { "quartz.dataSource.default.connectionString", "Data Source=ZAISO-5079\\SQLEXPRESS;Initial Catalog=Quartz;Integrated Security=False;User Id=sa;Password=Fabio1980; TrustServerCertificate=True" },
                { "quartz.jobStore.clustered", "false" },
                { "quartz.jobStore.driverDelegateType", "Quartz.Impl.AdoJobStore.SqlServerDelegate, Quartz" }
            };
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        //private void Test(string jobName, CancellationToken cancellationToken)
        //{
        //    using (var serviceScope = _serviceProvider.GetService<IServiceScopeFactory>()!.CreateScope())
        //    {
        //        var _dbcontext = serviceScope.ServiceProvider.GetRequiredService<Context>();

        //        var repository = new SchedulerStatusRepository(_dbcontext);

        //        var service = new SchedulerStatusService(repository);

        //        await service.RecordSchedulerStatusAsync(jobName, cancellationToken, ServiceStatus.Started);
        //    }
        //}
    }
}
