using System.ComponentModel.DataAnnotations;

namespace NoteMinimalApi.ViewModels.UserViewModel;

public class UpdateUserCommand
{
    [Required, Range(0, int.MaxValue)]
    public int UserId { get; set; } 
    [Required, StringLength(100)]
    public required string FirstName { get; set; }
    [StringLength(100)]
    public string? LastName { get; set; }
    [Required, StringLength(100)]
    public required string Login {get;set;}
    [Required, StringLength(100)]
    public required string Password {get;set;}
}