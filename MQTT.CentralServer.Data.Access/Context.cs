using Microsoft.EntityFrameworkCore;
using MQTT.CentralServer.Entities.Message;
using MQTT.CentralServer.Entities.Scheduler;
using System.Data;

namespace MQTT.CentralServer.Data.Access
{
    public class Context : DbContext
    {
        public DbSet<SchedulerStatusInfo> SchedulerStatus { get; set; }
        public DbSet<MqttMessage> Messages { get; set; }

        public Context() : base()
        {

        }

        public Context(DbContextOptions<Context> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SchedulerStatusInfo>();
            modelBuilder.Entity<MqttMessage>();
        }
    }
}