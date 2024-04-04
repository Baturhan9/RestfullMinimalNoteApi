using System.ComponentModel.DataAnnotations;
using NoteMinimalApi.Models;

namespace NoteMinimalApi.ViewModels.NoteViewModel;

public class CreateNoteCommand
{
    [Required(ErrorMessage = "User id is required")]
    [Range(0, int.MaxValue, ErrorMessage = "User id must be positive number")]
    public int UserId { get; set; }
    [Required(ErrorMessage = "Title is required")]
    [StringLength(100, ErrorMessage ="Title must be less then 100 characters")]
    public required string Title {get;set;} 
    [StringLength(100, ErrorMessage ="Description must be less then 100 characters")]
    public string? Description {get;set;}

    public Note ToNote()
    {
        return new Note
        {
            UserId = UserId,
            Title = Title,
            Description = Description,
            DateCreated = DateOnly.FromDateTime(DateTime.Today)
        };
    }
}