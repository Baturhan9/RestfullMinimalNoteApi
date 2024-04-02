namespace NoteMinimalApi.Models;

public class Note
{
    public int NoteId {get;set;} 
    public int UserId {get;set;}
    public required string Title {get;set;}
    public string? Description {get;set;}
    public DateOnly DateCreated {get;set;}
    public DateOnly? DateModified {get;set;}
    public bool IsDeleted {get;set;} = false;

}