using CoinBazaar.Infrastructure.EventBus;
using CoinBazaar.Infrastructure.Mongo.Data;
using EventStore.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CoinBazaar.Transfer.ESConsumer.gRPC
{
    public class ConsumerWorker : BackgroundService
    {
        private readonly ILogger<ConsumerWorker> _logger;
        private readonly EventStorePersistentSubscriptionsClient _eventStorePersistentSubscription;
        private readonly EventStoreOptions _eventStoreOptions;
        private readonly BPMContext _bpmContext;

        public ConsumerWorker(ILogger<ConsumerWorker> logger, EventStorePersistentSubscriptionsClient eventStorePersistentSubscription, EventStoreOptions eventStoreOptions, BPMContext bpmContext)
        {
            _logger = logger;
            _eventStorePersistentSubscription = eventStorePersistentSubscription;
            _eventStoreOptions = eventStoreOptions;
            _bpmContext = bpmContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await _eventStorePersistentSubscription.CreateAsync($"$ce-{_eventStoreOptions.AggregateStream}", _eventStoreOptions.PersistentSubscriptionGroup, new PersistentSubscriptionSettings(startFrom: StreamPosition.End));
            }
            catch (InvalidOperationException ex)
            {
            }
            catch (Exception ex)
            {

                throw;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    await Task.Delay(1000, stoppingToken);

                    await _eventStorePersistentSubscription.SubscribeAsync($"$ce-{_eventStoreOptions.AggregateStream}", _eventStoreOptions.PersistentSubscriptionGroup,
                        async (subscription, evt, retryCount, cancelToken) =>
                        {
                            await HandleEvent(evt);
                        });
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }

        private async Task HandleEvent(ResolvedEvent @event)
        {
            var metadata = JsonSerializer.Deserialize<ESMetadata>(Encoding.UTF8.GetString(@event.Event.Metadata.ToArray()));

            if (metadata.ProcessStarter)
            {
                //event pointed to process starter.
                var processStarterEvent = JsonSerializer.Deserialize<IProcessStarter>(Encoding.UTF8.GetString(@event.Event.Data.ToArray()));

                var filter = Builders<Process>.Filter.Eq(x => x.ProcessId, processStarterEvent.ProcessId);

                var process = (await _bpmContext.Processes.FindAsync(filter)).FirstOrDefault();

                if (process != null)
                {
                    _logger.LogWarning($"Idempotent Process Exception. Process started with same Id. Process Id: {processStarterEvent.ProcessId}");
                    return;
                }

                process = new Process()
                {
                    ProcessId = processStarterEvent.ProcessId,
                    ProcessName = processStarterEvent.ProcessName,
                    CreationDate = DateTime.UtcNow
                };

                await _bpmContext.Processes.InsertOneAsync(process);


            }
            else
            {
                //Redirect to aggregateRoot for apply all events.
            }

            //return Task.CompletedTask;
        }
    }
}
