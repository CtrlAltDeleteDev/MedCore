using System.Linq.Expressions;
using MedCore.Appoitment.Data.Entities;

namespace MedCore.Appoitment.Data.Specifications;

public class GetActiveMeetsByEmployeeIdSpec : ISpecification<Meet>
{
    private readonly int _employeeId;

    public GetActiveMeetsByEmployeeIdSpec(int employeeId) => _employeeId = employeeId;

    public Expression<Func<Meet, bool>> Criteria =>
        m => m.EmployeeId == _employeeId && m.IsActive;
}
