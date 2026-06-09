using System.Linq.Expressions;
using MedCore.Appoitment.Data.Entities;

namespace MedCore.Appoitment.Data.Specifications;

public class GetSkillsByIdsSpec : ISpecification<Skill>
{
    private readonly int[] _ids;

    public GetSkillsByIdsSpec(int[] ids) => _ids = ids;

    public Expression<Func<Skill, bool>> Criteria => s => _ids.Contains(s.Id);
}
