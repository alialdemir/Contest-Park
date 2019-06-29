using System.Data;

namespace ContestPark.Core.Dapper.Interfaces
{
    public interface IDatabaseConnection
    {
        IDbConnection Connection { get; }
    }
}
