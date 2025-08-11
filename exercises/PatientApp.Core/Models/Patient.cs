namespace PatientApp.Core.Models;

public enum Gender
{
    Male,
    Female
}

public sealed class Patient
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public Gender Gender { get; set; }
    public string? Phone { get; set; }
    public string? Notes { get; set; }
}


