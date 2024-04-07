using NoteMinimalApi.ViewModels.NoteViewModel;

namespace NoteMinimalApi.Services;

public static class CacheRequestService
{
    public static Dictionary<int, NoteDetailViewModel> NotesRequest {get;set;} = new Dictionary<int, NoteDetailViewModel>();
}