using System.Linq.Expressions;
using MedCore.Appoitment.Data.Entities;

namespace MedCore.Appoitment.Data.Specifications;

public class GetActiveEmployeesSpec : ISpecification<Employee>
{
    public Expression<Func<Employee, bool>> Criteria =>
        x => x.IsActive;
}
