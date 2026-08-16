using System.Linq.Expressions;
using Warehouse.SharedLibrary.Responses;

namespace Warehouse.SharedLibrary.Interface
{
    public interface IGenericInterface<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> GetByAsync(Expression<Func<T, bool>> predicate);
        Task<Response> CreateAsync(T entity);
        Task<Response> UpdateAsync(T entity);
        Task<Response> DeleteAsync(int id);
    }
}
