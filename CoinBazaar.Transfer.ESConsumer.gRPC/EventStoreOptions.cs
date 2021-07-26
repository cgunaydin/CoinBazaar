using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinBazaar.Transfer.ESConsumer.gRPC
{
    public class EventStoreOptions
    {
        public string ConnectionString { get; set; }
        public string AggregateStream { get; set; }
        public string PersistentSubscriptionGroup { get; set; }
    }
}
