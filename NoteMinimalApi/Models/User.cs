namespace NoteMinimalApi.Models;

public class User
{
    public int UserId { get; set; } 
    public required string FirstName { get; set; }
    public string? LastName { get; set; }
    public required string Login {get;set;}
    public required string Password {get;set;}
    public bool IsDeleted {get;set;}
    public required ICollection<Note> Notes { get; set; }
}