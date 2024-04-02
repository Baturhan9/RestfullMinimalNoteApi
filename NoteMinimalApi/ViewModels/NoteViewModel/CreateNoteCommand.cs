using NoteMinimalApi.Models;

namespace NoteMinimalApi.ViewModels.NoteViewModel;

public class CreateNoteCommand
{
    public int UserId { get; set; }
    public required string Title {get;set;} 
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