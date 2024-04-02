using NoteMinimalApi.Models;

namespace NoteMinimalApi.ViewModels.UserViewModel;

public class CreateUserCommand
{
    public required string Login {get;set;} 
    public required string Password {get;set;}
    public required string FirstName {get;set;}
    public string? LastName {get;set;}

    public User ToUser()
    {
        return new User
        {
            Login = Login,
            Password = Password,
            FirstName = FirstName,
            LastName = LastName,
            Notes = new List<Note>(),
        };
    }
}