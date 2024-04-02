namespace NoteMinimalApi.ViewModels.UserViewModel;

public class UpdateUserCommand
{
    public int UserId { get; set; } 
    public required string FirstName { get; set; }
    public string? LastName { get; set; }
    public required string Login {get;set;}
    public required string Password {get;set;}
}