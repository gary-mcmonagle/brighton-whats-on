using System;
using Azure;
using Azure.Data.Tables;
using Domain;

namespace Functions.Models;

public record EventTableModel : EventModel, ITableEntity
{
    public EventTableModel(EventModel original) : base(original)
    {
    }

    public EventTableModel() : base()
    {
    }

    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    public string EventId => $"{Venue}-{Name}";
}
