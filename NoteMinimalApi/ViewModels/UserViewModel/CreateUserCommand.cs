using System.ComponentModel.DataAnnotations;
using NoteMinimalApi.Models;

namespace NoteMinimalApi.ViewModels.UserViewModel;

public class CreateUserCommand
{
    [Required, StringLength(100)]
    public required string Login {get;set;} 
    [Required, StringLength(100)]
    public required string Password {get;set;}
    [Required, StringLength(100)]
    public required string FirstName {get;set;}
    [StringLength(100)]
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