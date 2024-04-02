using Microsoft.AspNetCore.Identity.Data;

namespace NoteMinimalApi.ViewModels.UserViewModel;

public class UserDetailViewModel
{
    public int UserId { get; set; } 
    public required string FirstName { get; set; }
    public string? LastName { get; set; }
    public required string Login { get; set; }
    public required string Password {get;set;}
}