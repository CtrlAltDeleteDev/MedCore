using System.Linq.Expressions;

namespace MedCore.Appoitment.Data.Specifications;

public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
}