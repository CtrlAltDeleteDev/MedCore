namespace MedCore.Appoitment.Data.Entities
{
    public class Employee
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public ICollection<Skill> Skills { get; set; } = new List<Skill>();

        public ICollection<Meet> Meets { get; set; } = new List<Meet>();
    }
}