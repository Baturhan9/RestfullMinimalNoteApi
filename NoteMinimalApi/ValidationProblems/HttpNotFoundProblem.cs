namespace NoteMinimalApi.ValidationProblems;

public class HttpNotFoundProblem 
{
    public required string Type {get;set;} 
    public required string Title {get;set;}
    public int Status {get;set;}

}