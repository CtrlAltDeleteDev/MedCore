using MedCore.Appoitment.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Appoitment.Data
{
    public class AppoitmentDbContext : DbContext
    {
        public AppoitmentDbContext(DbContextOptions<AppoitmentDbContext> options)
        : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Meet> Meets { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("Appoitment");

            builder.Entity<Employee>().HasKey(x => x.Id);
            builder.Entity<Skill>().HasKey(x => x.Id);
            builder.Entity<Meet>().HasKey(x => x.Id);

            builder.Entity<Employee>()
                .HasMany(e => e.Skills)
                .WithMany(s => s.Employees)
                .UsingEntity(j => j.ToTable("EmployeeSkills"));

             builder.Entity<Meet>()
                .HasOne(m => m.Employee)
                .WithMany(e => e.Meets)
                .HasForeignKey(m => m.EmployeeId);

            builder.Entity<Employee>().Property(x => x.FullName).IsRequired().HasMaxLength(256);
            builder.Entity<Employee>().Property(x => x.Title).IsRequired().HasMaxLength(128);

            builder.Entity<Skill>().Property(x => x.Name).IsRequired().HasMaxLength(256);

            builder.Entity<Meet>().Property(x => x.Subject).IsRequired().HasMaxLength(128);
        }
    }
}
