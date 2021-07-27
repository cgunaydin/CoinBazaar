using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinBazaar.Infrastructure.EventBus
{
    public interface IProcessStarter
    {
        public Guid ProcessId { get; set; }
        public string ProcessName { get; init; }
        public IList<KeyValuePair<string, object>> ProcessParameters { get; init; }
    }
}
