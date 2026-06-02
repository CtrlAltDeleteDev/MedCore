using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Appoitment.Data.Entities
{
    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
