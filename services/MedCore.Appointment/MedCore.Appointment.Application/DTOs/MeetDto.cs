namespace MedCore.Appointment.Application.DTOs;

public record MeetDto(int Id, string Subject, DateTime Start, DateTime? End, int EmployeeId, int PacientId, int[] SkillIds);