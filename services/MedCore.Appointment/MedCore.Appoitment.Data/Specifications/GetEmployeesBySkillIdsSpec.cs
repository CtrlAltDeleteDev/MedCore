using System.Linq.Expressions;
using MedCore.Appoitment.Data.Entities;

namespace MedCore.Appoitment.Data.Specifications;

public class GetEmployeesBySkillIdsSpec : ISpecification<Employee>
{
    private readonly int[] _skillIds;

    public GetEmployeesBySkillIdsSpec(int[] skillIds) => _skillIds = skillIds;

    public Expression<Func<Employee, bool>> Criteria =>
        x => x.Skills.Any(s => _skillIds.Contains(s.Id));
}
