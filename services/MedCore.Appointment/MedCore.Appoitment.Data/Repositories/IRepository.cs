using MedCore.Appoitment.Data.Entities;
using MedCore.Appoitment.Data.Specifications;

namespace MedCore.Appoitment.Data.Repositories;

public interface IRepository<T>
{ 
    Task<IEnumerable<T>> GetItemsAsync(ISpecification<T> spec);
}