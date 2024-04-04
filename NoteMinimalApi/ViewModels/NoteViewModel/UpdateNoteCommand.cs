using System.ComponentModel.DataAnnotations;

namespace NoteMinimalApi.ViewModels.NoteViewModel;

public class UpdateNoteCommand
{
    [Required, Range(0, int.MaxValue)]
    public int NoteId{get;set;}
    [Required, StringLength(100)]
    public required string Title {get;set;} 
    [StringLength(100)]
    public string? Description {get;set;}
}