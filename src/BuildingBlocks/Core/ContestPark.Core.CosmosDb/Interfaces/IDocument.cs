using System;

namespace ContestPark.Core.CosmosDb.Interfaces
{
    public interface IDocument
    {
        DateTime ModifiedDate { get; set; }
        DateTime CreatedDate { get; set; }
    }
}