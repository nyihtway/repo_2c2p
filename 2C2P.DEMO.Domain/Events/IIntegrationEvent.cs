using System;

namespace _2C2P.DEMO.Domain.Events
{
    public interface IIntegrationEvent
    {
        public Guid Id { get; }
        public int EventVersion { get; }
        public DateTime OccurredOn { get; }
        public string Env { get; }
        public string TraceId { get; set; }
    }

    public abstract class IntegrationEventBase : IIntegrationEvent
    {
        /// <summary>
        /// Allow derived class to override Event Id if they want to send a collection of messages to same partition
        /// </summary>        
        public Guid Id { get; protected set; } = Guid.NewGuid();
        public int EventVersion { get; protected set; } = 1;
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        /// <summary>
        /// Kafka cluster are being shared among environments, so using this Env property to differentiate message of each environment
        /// </summary>
        public string Env { get; set; } = "dev";
        public string TraceId { get; set; }
    }
}
