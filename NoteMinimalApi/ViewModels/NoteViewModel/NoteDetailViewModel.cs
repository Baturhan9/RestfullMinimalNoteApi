namespace NoteMinimalApi.ViewModels.NoteViewModel;

public class NoteDetailViewModel
{
    public int NoteId { get; set; } 
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateOnly DateCreated { get; set; }
    public DateOnly? DateModified { get; set; }
}