using _2C2P.DEMO.Domain.Events;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _2C2P.DEMO.Infrastructure.Kafka
{
    public static class KafkaHelper
    {
        public static void CreateTopics(string bootstrapServers, string env, int numPartitions, short replicationFactor, short inSyncReplicas)
        {
            var logger = Log.ForContext(typeof(KafkaHelper));

            using var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = bootstrapServers }).Build();

            try
            {
                var metaData = adminClient.GetMetadata(TimeSpan.FromSeconds(20));
                if (metaData != null)
                {
                    var eventNames = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.GetName().Name.Contains("2C2P.DEMO."))
                                                                        .SelectMany(x => x.GetTypes())
                                                                        .Where(x => typeof(IIntegrationEvent).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                                                                        .Select(x => x.Name).ToList();
                    var topicSpecs = new List<TopicSpecification>();

                    foreach (var eventName in eventNames)
                    {
                        if (!metaData.Topics.Any(x => x.Topic == $"{eventName}_{env}"))
                        {
                            var topicSpec = new TopicSpecification
                            {
                                Name = $"{eventName}_{env}",
                                NumPartitions = numPartitions,
                                ReplicationFactor = replicationFactor,
                                Configs = new Dictionary<string, string>() { { "min.insync.replicas", inSyncReplicas.ToString() } }
                            };
                            topicSpecs.Add(topicSpec);
                        }
                    }
                    if (topicSpecs.Any())
                    {
                        _ = adminClient.CreateTopicsAsync(topicSpecs);
                    }
                }
            }
            catch (CreateTopicsException ex)
            {
                logger.Fatal(ex, "Failed creating topic");
            }
        }
    }
}
