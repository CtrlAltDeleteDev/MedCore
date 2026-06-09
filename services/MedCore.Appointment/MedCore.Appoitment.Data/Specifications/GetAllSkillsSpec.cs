using System.Linq.Expressions;
using MedCore.Appoitment.Data.Entities;

namespace MedCore.Appoitment.Data.Specifications;

public class GetAllSkillsSpec : ISpecification<Skill>
{
    public Expression<Func<Skill, bool>> Criteria => _ => true;
}
