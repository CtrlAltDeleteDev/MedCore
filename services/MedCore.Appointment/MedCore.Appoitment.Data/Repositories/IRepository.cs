using MedCore.Appoitment.Data.Entities;
using MedCore.Appoitment.Data.Specifications;

namespace MedCore.Appoitment.Data.Repositories;

public interface IRepository<TEntity> where TEntity : class
{ 
    Task<TEntity> GetItemAsync(ISpecification<TEntity> spec, bool useAsNoTracking = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetItemsAsync(ISpecification<TEntity> spec, bool useAsNoTracking = false, CancellationToken cancellationToken = default);
    Task Add(TEntity entity, CancellationToken cancellationToken = default);
    Task Update(TEntity entity, CancellationToken cancellationToken = default);
}