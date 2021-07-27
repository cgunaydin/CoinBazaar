using CoinBazaar.Infrastructure.EventBus;
using System;
using System.Collections.Generic;

namespace CoinBazaar.Transfer.Domain.Events
{
    public class TransferCreated : EventBase, IProcessStarter
    {
        public string FromWallet { get; private init; }
        public string ToWallet { get; private init; }
        public decimal Amount { get; private init; }
        public Guid ProcessId { get; set; }
        public string ProcessName { get; init; }
        public IList<KeyValuePair<string, object>> ProcessParameters { get; init; }

        public TransferCreated(string fromWallet, string toWallet, decimal amount, Guid processId, IList<KeyValuePair<string, object>> processParameters)
        {
            FromWallet = fromWallet;
            ToWallet = toWallet;
            Amount = amount;
            ProcessName = "TransferBPM";
            ProcessId = processId;
            ProcessParameters = processParameters;
        }
    }
}
