namespace MedCore.Appoitment.Data.Entities
{
    public class Meet
    {
        public int Id { get; set; }

        public string Subject { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int EmployeeId { get; set; }

        public int PatientId { get; set; }

        public bool IsActive { get; set; } = true;

        public int[] SkillIds { get; set; } = [];

        public Employee Employee { get; set; }

        public Skill[] Skills { get; set; }
    }
}