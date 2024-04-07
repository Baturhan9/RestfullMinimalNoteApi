using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using NoteMinimalApi.Models;
using NoteMinimalApi.ViewModels.NoteViewModel;

namespace NoteMinimalApi.Services;

public class NoteService
{
    private AppDbContext _context;
    public NoteService(AppDbContext context)
    {
        _context = context; 
    }

    public async Task<int> CreateNoteAsync(CreateNoteCommand cmd)
    {
        var note = cmd.ToNote();
        _context.Notes.Add(note);
        await _context.SaveChangesAsync();
        return note.NoteId;
    }

    public async Task<List<NoteSummaryViewModel>> GetNotesAsync()
    {
        return await _context.Notes.Where(n => !n.IsDeleted).Select(n => new NoteSummaryViewModel
        {
            NoteId = n.NoteId,
            Title = n.Title,
            DateModified = n.DateModified ?? n.DateCreated
        }).ToListAsync();
    }

    public async Task<NoteDetailViewModel?> GetNoteDetailAsync(int id)
    {
        return await _context.Notes.Where(s => !s.IsDeleted && s.NoteId == id)
            .Select(n => new NoteDetailViewModel
            {
                NoteId = n.NoteId,
                Title = n.Title,
                Description = n.Description,
                DateCreated = n.DateCreated,
                DateModified = n.DateModified
            }).SingleOrDefaultAsync();
    }

    public async Task UpdateNoteAsync(UpdateNoteCommand cmd)
    {
        var note = await _context.Notes.FindAsync(cmd.NoteId);
        if(note is null)
            throw new Exception("Unable to find the note");
        if(note.IsDeleted)
            throw new Exception("Note deleted");
        

        note.Title = cmd.Title;
        note.Description = cmd.Description;
        note.DateModified = DateOnly.FromDateTime(DateTime.Today);

        await _context.SaveChangesAsync();
    }


    public async Task DeleteNoteAsync(int id)
    {
        var note = await _context.Notes.FindAsync(id);
        if(note is not null)
        {
            note.IsDeleted = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsAvailableForUpdate(int id)
    {
        return await _context.Notes.Where(x => !x.IsDeleted && x.NoteId == id).AnyAsync();
    }

    public async Task<bool> IsAvailableForCreate(int userId)
    {
        return await _context.Users.Where(u => !u.IsDeleted && u.UserId == userId).AnyAsync();
    }

}