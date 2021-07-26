using System;

namespace CoinBazaar.Infrastructure.EventBus
{
    public class ESMetadata
    {
        public Guid StreamId { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; }

        public ESMetadata(Guid streamId, DateTime creationDate, string createdBy)
        {
            StreamId = streamId;
            CreationDate = creationDate;
            CreatedBy = createdBy;
        }
    }
}
