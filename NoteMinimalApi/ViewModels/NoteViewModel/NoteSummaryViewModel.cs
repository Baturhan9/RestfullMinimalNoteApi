namespace NoteMinimalApi.ViewModels.NoteViewModel;

public class NoteSummaryViewModel
{
    public int NoteId {get;set;} 
    public required string Title {get;set;}
    public DateOnly DateModified {get;set;}
}