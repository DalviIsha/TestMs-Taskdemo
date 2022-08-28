using System.Data.Common;

namespace TestMS
{
    public interface IDapperCommandRepository : IDisposable
    {
        DbConnection GetDbconnection();
        Task<int> Update<T>(string query, T data);
    }
}
