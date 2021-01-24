using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace _2C2P.DEMO.Domain.Events
{
    public interface IBusClient
    {
        /// <summary>
        /// For high performance
        /// </summary>        
        void Publish<TEvent>(IReadOnlyList<TEvent> @lstEvent) where TEvent : IIntegrationEvent;
        /// <summary>
        /// Every server round trip is 3ms at a minimum, actual latency depends on many factors. If you need something with higher performance, you can use Publish method for instead.
        /// </summary>        
        Task PublishAsync<TEvent>(TEvent @event) where TEvent : IIntegrationEvent;
        Task SubscribeAsync<TEvent>(Func<TEvent, Task> func, string topic, CancellationToken cancellationToken, int commitPeriod = 10) where TEvent : IIntegrationEvent;
    }
}
