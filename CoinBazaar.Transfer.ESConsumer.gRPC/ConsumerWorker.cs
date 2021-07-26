using EventStore.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoinBazaar.Transfer.ESConsumer.gRPC
{
    public class ConsumerWorker : BackgroundService
    {
        private readonly ILogger<ConsumerWorker> _logger;
        private readonly EventStorePersistentSubscriptionsClient _eventStorePersistentSubscription;
        private readonly EventStoreOptions _eventStoreOptions;

        public ConsumerWorker(ILogger<ConsumerWorker> logger, EventStorePersistentSubscriptionsClient eventStorePersistentSubscription, EventStoreOptions eventStoreOptions)
        {
            _logger = logger;
            _eventStorePersistentSubscription = eventStorePersistentSubscription;
            _eventStoreOptions = eventStoreOptions;
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

        private Task HandleEvent(ResolvedEvent @event)
        {
            //@event.Event.Data
            return Task.CompletedTask;
        }
    }
}
