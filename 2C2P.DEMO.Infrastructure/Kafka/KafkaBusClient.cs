using _2C2P.DEMO.Domain.Events;
using Confluent.Kafka;
using Newtonsoft.Json;
using OpenTracing;
using OpenTracing.Propagation;
using OpenTracing.Tag;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace _2C2P.DEMO.Infrastructure.Kafka
{
    public class KafkaBusClient : IBusClient
    {
        private const int COMMITPERIOD = 10;
        private readonly ProducerConfig _producerConfig;
        private readonly ConsumerConfig _consumerConfig;
        private readonly ILogger _logger;
        private readonly ITracer _tracer;
        public KafkaBusClient(ProducerConfig producerConfig, ConsumerConfig consumerConfig, ITracer tracer)
        {
            _producerConfig = producerConfig;
            _consumerConfig = consumerConfig;
            _tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));
            _logger = Log.ForContext<KafkaBusClient>();
        }

        public void Publish<TEvent>(IReadOnlyList<TEvent> @lstEvent) where TEvent : IIntegrationEvent
        {
            var spanBuilder = _tracer.BuildSpan($"Kafka Publish: {typeof(TEvent).Name}").WithTag(Tags.SpanKind.Key, Tags.SpanKindProducer);

            using var scope = spanBuilder.StartActive(true);

            void DeliveryReportHandler(DeliveryReport<string, string> r)
            {
                if (r.Error.IsError)
                {
                    _logger.Error("Delivery Error: {ErrorReason}", r.Error.Reason);
                }
                else
                {
                    _logger.Information("Delivered message to {TopicPartitionOffset}", r.TopicPartitionOffset);
                }
            }

            using var producer = new ProducerBuilder<string, string>(_producerConfig).Build();
            try
            {
                var eventName = "";
                var headers = new Dictionary<string, string>();
                _tracer.Inject(scope.Span.Context, BuiltinFormats.TextMap, new TextMapInjectAdapter(headers));
                var traceId = headers.FirstOrDefault(x => x.Key.Equals("uber-trace-id", StringComparison.InvariantCultureIgnoreCase)).Value;
                foreach (var @event in @lstEvent)
                {
                    @event.TraceId = traceId;
                    eventName = $"{typeof(TEvent).Name}_{@event.Env}";
                    producer.Produce(eventName, new Message<string, string>
                    {
                        Key = @event.Id.ToString(),
                        Value = JsonConvert.SerializeObject(@event)
                    }, DeliveryReportHandler);
                }

                producer.Flush(TimeSpan.FromSeconds(10));
                scope.Span.SetTag("kafka.event_name", eventName);
                scope.Span.SetTag("kafka.number_of_messages", lstEvent.Count);
            }
            catch (ProduceException<string, string> ex)
            {
                _logger.Error(ex, "Failed delivering kafka message because of error");
            }
        }

        public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : IIntegrationEvent
        {
            var spanBuilder = _tracer.BuildSpan($"Kafka Publish Async: {typeof(TEvent).Name}").WithTag(Tags.SpanKind.Key, Tags.SpanKindProducer);

            using var scope = spanBuilder.StartActive(true);

            using var producer = new ProducerBuilder<string, string>(_producerConfig).Build();
            try
            {
                var headers = new Dictionary<string, string>();
                _tracer.Inject(scope.Span.Context, BuiltinFormats.TextMap, new TextMapInjectAdapter(headers));
                @event.TraceId = headers.FirstOrDefault(x => x.Key.Equals("uber-trace-id", StringComparison.InvariantCultureIgnoreCase)).Value;

                var deliveryReport = await producer.ProduceAsync($"{typeof(TEvent).Name}_{@event.Env}", new Message<string, string>
                {
                    Key = @event.Id.ToString(),
                    Value = JsonConvert.SerializeObject(@event)
                });
                _logger.Information("Delivered message to {Partition}", deliveryReport.TopicPartitionOffset);
                scope.Span.SetTag("kafka.event_name", $"{typeof(TEvent).Name}_{@event.Env}");
                scope.Span.SetTag("kafka.partition", deliveryReport.TopicPartitionOffset.Partition.Value);
                scope.Span.SetTag("kafka.offset", deliveryReport.TopicPartitionOffset.Offset.Value);
            }
            catch (ProduceException<string, string> ex)
            {
                _logger.Error(ex, "Failed delivering kafka message because of error");
                //TODO: what should we do here???
            }
        }

        public async Task SubscribeAsync<TEvent>(Func<TEvent, Task> func, string topic, CancellationToken cancellationToken, int commitPeriod = COMMITPERIOD) where TEvent : IIntegrationEvent
        {
            using var consumer = new ConsumerBuilder<string, string>(_consumerConfig).SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
                .SetPartitionsAssignedHandler((c, partitions) =>
                {
                    _logger.Information("Assigned partitions: [{Partition}]", string.Join(", ", partitions));
                })
                .SetPartitionsRevokedHandler((c, partitions) =>
                {
                    _logger.Information("Revoking assignment: [{Partition}]", string.Join(", ", partitions));
                })
                .Build();

            try
            {
                consumer.Subscribe(topic);
                var previousOffset = new ConsumeResult<string, string>();

                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(cancellationToken);

                        if (consumeResult.IsPartitionEOF)
                        {
                            //_logger.Information("Reached end of topic {Topic}, partition {Partition}, offset {Offset}.", consumeResult.Topic, consumeResult.Partition, consumeResult.Offset);
                            if (!string.IsNullOrWhiteSpace(previousOffset.Topic) && previousOffset.Message != null)
                            {
                                try
                                {
                                    consumer.Commit(previousOffset);
                                }
                                catch (KafkaException ex)
                                {
                                    _logger.Error(ex, "Failed committing kafka offset because of error");
                                }
                            }
                            continue;
                        }

                        previousOffset = consumeResult;

                        //using mediator to notify client                            
                        var data = JsonConvert.DeserializeObject<TEvent>(consumeResult.Message.Value);
                        _logger.Information("Received message at {TopicPartitionOffset}: {Message}", consumeResult.TopicPartitionOffset, consumeResult.Message.Value);
                        var spanBuilder = _tracer.BuildSpan($"Kafka Subscribe Async: {typeof(TEvent).Name}").WithTag(Tags.SpanKind.Key, Tags.SpanKindConsumer);
                        var parentMetadata = new Dictionary<string, string>
                        {
                            { "uber-trace-id", data.TraceId }
                        };
                        var parentSpan = _tracer.Extract(BuiltinFormats.TextMap, new TextMapExtractAdapter(parentMetadata));
                        spanBuilder.AddReference(References.FollowsFrom, parentSpan);
                        using var scope = spanBuilder.StartActive(true);

                        scope.Span.SetTag("kafka.event_name", topic);
                        scope.Span.SetTag("kafka.partition", consumeResult.TopicPartitionOffset.Partition.Value);
                        scope.Span.SetTag("kafka.offset", consumeResult.TopicPartitionOffset.Offset.Value);

                        await func.Invoke(data);

                        //Commit the last offset of partition
                        if (consumeResult.Offset % commitPeriod == 0)
                        {
                            try
                            {
                                consumer.Commit(consumeResult);
                            }
                            catch (KafkaException ex)
                            {
                                _logger.Error(ex, "Failed committing kafka offset because of error");
                            }
                        }
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.Error(ex, "Failed consuming kafka message because of error");
                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.Fatal(ex, "Closing consumer because of error");
                consumer.Close();
            }
        }
    }
}
