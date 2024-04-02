namespace NoteMinimalApi.ViewModels.NoteViewModel;

public class UpdateNoteCommand
{
    public int NoteId{get;set;}
    public required string Title {get;set;} 
    public string? Description {get;set;}
}