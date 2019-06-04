using System;

namespace ContestPark.Core.CosmosDb.Interfaces
{
    public interface IDocument
    {
        string Id { get; set; }
        DateTime ModifiedDate { get; set; }
        DateTime CreatedDate { get; set; }
    }
}