using MedCore.Appoitment.Data.Entities;
using MedCore.Appoitment.Data.Specifications;

namespace MedCore.Appoitment.Data.Repositories;

public interface IRepository<TEntity, TDto> where TEntity : class
{ 
    Task<IEnumerable<TDto>> GetItemsAsync(ISpecification<TEntity> spec, bool useAsNoTracking=true);
}