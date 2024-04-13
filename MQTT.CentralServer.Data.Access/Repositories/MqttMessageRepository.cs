using MQTT.CentralServer.Data.Access.Interfaces;
using MQTT.CentralServer.Entities.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT.CentralServer.Data.Access.Repositories
{
    public class MqttMessageRepository : BaseRepository<MqttMessage>, IMqttMessageRepository
    {
        public MqttMessageRepository(Context context) : base(context)
        {           
        }

        public async Task AddAsync(MqttMessage message, CancellationToken cancellationToken = default)
        {
            await _context.AddAsync(message, cancellationToken);
            await CommitAsync(cancellationToken);
        }
    }
}
