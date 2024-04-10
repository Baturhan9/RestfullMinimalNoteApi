using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NoteMinimalApi.Models;
using NoteMinimalApi.Services;
using NoteMinimalApi.ViewModels.NoteViewModel;

namespace NoteMinimalApi.Test;

public class NoteServiceTest
{
    private AppDbContext _db;

    public NoteServiceTest()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder()
            .UseSqlite(connection)
            .Options;
        _db = new AppDbContext(options);
        _db.Database.EnsureCreated();
            
        _db.Users.AddRange(
            new User {UserId = 1, FirstName = "John", Login="1", Notes = new List<Note>(), Password="1"},
            new User {UserId = 2, FirstName = "Ben", Login="2", Notes = new List<Note>(), Password="2"},
            new User {UserId = 3, FirstName = "Tom", Login="3", Notes = new List<Note>(), Password="3",IsDeleted = true}
        );

        _db.Notes.AddRange(
            new Note
            {
                NoteId = 1, 
                UserId = 1,
                Title = "note1",
                Description = "note1 description",
                DateCreated = DateOnly.Parse("1.04.2024"),
                DateModified = DateOnly.Parse("5.04.2024"),
            },
            new Note
            {
                NoteId = 2,
                UserId = 1,
                Title = "note2",
                Description = "note2 description",
                DateCreated = DateOnly.Parse("1.04.2024"),
                DateModified = DateOnly.Parse("5.04.2024")
            },
            new Note 
            {
                NoteId = 3,
                UserId = 2,
                Title = "note3",
                Description = "note3 description",
                DateCreated = DateOnly.Parse("1.04.2024"),
                IsDeleted = true
            }
        );

        _db.SaveChanges();
    }

    [Fact]
    public void GetAllNotes_CanLoadFromContext()
    {
        //arrange
        var service = new NoteService(_db);
        //act
        var listNotes = service.GetNotesAsync().Result; 
        //assert
        listNotes.Should().NotBeNull(); 
        listNotes.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public void GetAllUserNotes_ExistentUser_ReturnList()
    {
        //arrange
        var service = new NoteService(_db);
        int userId = 1; 
        //act
        var listNotes = service.GetUsersNotesAsync(userId).Result;
        //assert
        listNotes.Should().NotBeNull(); 
        listNotes!.Count.Should().BeGreaterThan(0);    
    }

    [Fact]
    public void GetAllUserNotes_NonExistentUser_ReturnEmptyList()
    {
        //arrange
        var service = new NoteService(_db);
        int userId = 123; 
        //act
        var listNotes = service.GetUsersNotesAsync(userId).Result;
        //assert
        listNotes.Should().BeEmpty();
    }

    [Fact]
    public void GetNoteDetail_CanLoadFromContext()
    {
        //arrange
        var service = new NoteService(_db); 
        int noteId = 1;
        //act
        var note = service.GetNoteDetailAsync(noteId).Result; 
        //assert
        
        note.Should().NotBeNull();
        note.NoteId.Should().Be(noteId);
        note.Title.Should().Be("note1");
        note.Description.Should().Be("note1 description");
        note.DateCreated.Should().Be(DateOnly.Parse("01.04.2024"));
        note.DateModified.Should().Be(DateOnly.Parse("05.04.2024"));
        
    }

    [Fact]
    public void GetNoteDetail_NonExistentNote_ReturnsNull()
    {
        //arrange
        var service = new NoteService(_db); 
        int noteId = 123;
        //act
        var note = service.GetNoteDetailAsync(noteId).Result;
        //assert
        note.Should().BeNull(); 
    }

    [Fact]
    public void CreateNote_ValidData_ReturnsId()
    {
        //arrange
        var service = new NoteService(_db); 
        var creatingNote = new CreateNoteCommand()
        {
            UserId = 1,
            Title = "new note",
            Description = "new note description"
        };
        //act
        var noteId = service.CreateNoteAsync(creatingNote).Result; 
        var note = service.GetNoteDetailAsync(noteId).Result;
        //assert
        note.Should().NotBeNull(); 
        note.NoteId.Should().Be(noteId);
        note.Title.Should().Be(creatingNote.Title);
        note.Description.Should().Be(creatingNote.Description);
        note.DateCreated.Should().Be(DateOnly.FromDateTime(DateTime.Today));
    }

    [Fact]
    public void UpdateNote_ValidData_SaveChanges()
    {
        //arrange
        var service = new NoteService(_db); 
        int noteId = 1;
        var updatingNote = new UpdateNoteCommand()
        {
            NoteId = noteId,
            Title = "note1",
            Description = "new new Description for note 1"
        };
        //act
        service.UpdateNoteAsync(updatingNote).Wait();
        var note = service.GetNoteDetailAsync(noteId).Result;
        //assert

        note.Should().NotBeNull();
        note.Title.Should().Be(updatingNote.Title);
        note.Description.Should().Be(updatingNote.Description);
        note.DateCreated.Should().Be(DateOnly.Parse("01.04.2024"));
        note.DateModified.Should().Be(DateOnly.FromDateTime(DateTime.Today));
    }

    [Fact]
    public void UpdateNote_NonExistentNote_ThrowException()
    {
        //arrange
        var service = new NoteService(_db); 
        int noteId = 123;
        var updatingNote = new UpdateNoteCommand()
        {
            NoteId = noteId,
            Title = "note1",
            Description = "new new Description for note 1"
        };
        //act
        var result = () => service.UpdateNoteAsync(updatingNote).Wait();
        //assert
        result.Should().Throw<Exception>()
            .WithMessage("Unable to find the note");
    }
    [Fact]
    public void UpdateNote_DeletedNote_ThrowException()
    {
        //arrange
        var service = new NoteService(_db); 
        int noteId = 3;
        var updatingNote = new UpdateNoteCommand()
        {
            NoteId = noteId,
            Title = "note1",
            Description = "new new Description for note 1"
        };
        //act
        var result = () => service.UpdateNoteAsync(updatingNote).Wait();
        //assert

        result.Should().Throw<Exception>()
            .WithMessage("Note deleted");
    }

    [Fact]
    public void DeletedNote_ValidId_ReturnNullWhenCalled()
    {
        //arrange
        var service = new NoteService(_db); 
        var noteId = 1;
        //act
        service.DeleteNoteAsync(noteId).Wait(); 
        var note = service.GetNoteDetailAsync(noteId).Result;
        //assert
        note.Should().BeNull(); 
    }

    [Fact]
    public void IsAvailableForUpdateMethod_ValidId_ReturnIsTrue()
    {
        //arrange
        var service = new NoteService(_db);
        var noteId = 2;
        //act
        var result = service.IsAvailableForUpdate(noteId).Result; 
        //assert
        result.Should().BeTrue(); 
    }
    [Fact]
    public void IsAvailableForUpdateMethod_InValidId_ReturnsFalse()
    {
        //arrange
        var service = new NoteService(_db);
        var noteId = 123;
        //act
        var result = service.IsAvailableForUpdate(noteId).Result; 
        //assert
        result.Should().BeFalse(); 
    }

    [Fact]
    public void IsAvailableForCreateMethod_ValidUserId_ReturnsTrue()
    {
        //arrange
        var service = new NoteService(_db); 
        var userId = 1;
        //act
        var result = service.IsAvailableForUpdate(userId).Result; 
        //assert
        result.Should().BeTrue(); 
    }

    [Fact]
    public void IsAvailableForCreateMethod_InValidUserId_ReturnsFalse()
    {
        //arrange
        var service = new NoteService(_db); 
        var userId = 1123;
        //act
        var result = service.IsAvailableForUpdate(userId).Result; 
        //assert
        result.Should().BeFalse(); 
    }

    [Fact]
    public void AddCache_FrequentlyCalledNote_ReturnNote()
    {
        //arrange
        var service = new NoteService(_db); 
        int noteId = 2;
        //act
        var noteFromService = service.GetNoteDetailAsync(noteId).Result;
        var noteFromCache = CacheRequestService.NotesRequest[noteId];
        //assert

        CacheRequestService.NotesRequest.Count.Should().BeGreaterThan(0);
        noteFromCache.Should().NotBeNull();
        noteFromCache.Should().Be(noteFromService);
    }
    [Fact]
    public void DeleteCache_DeletingNote_ReturnNull()
    {
        //arrange
        var service = new NoteService(_db) ;
        int noteId = 2;
        //act
        var noteFromService = service.GetNoteDetailAsync(noteId).Result; 
        service.DeleteNoteAsync(noteId).Wait();
        //assert

        CacheRequestService.NotesRequest.ContainsKey(noteId).Should().BeFalse();
    
    }
}