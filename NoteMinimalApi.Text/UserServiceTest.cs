using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NoteMinimalApi.Models;
using NoteMinimalApi.Services;

namespace NoteMinimalApi.Text;

public class UserServiceTest
{
    private AppDbContext _db;
    public UserServiceTest()
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
            new User {UserId = 3, FirstName = "Tom", Login="3", Notes = new List<Note>(), Password="3"}
        );
        _db.SaveChanges();
    }

    [Fact]
    public async Task GetUsersList_CanLoadFromContextAsync()
    {
        // Given
        var service = new UserService(_db);

        // When
        var listUsers = await service.GetUsersAsync();

        // Then
        listUsers.Should().NotBeNull();
        listUsers.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetUserDetail_CanLoadFromContextAsync()
    {
        //arrange
        var service = new UserService(_db);
        //act
        var userDetail = await service.GetUserDetailAsync(2); 
        //assert
        userDetail.Should().NotBeNull();
        userDetail.UserId.Should().Be(2);
        userDetail.FirstName.Should().Be("Ben");
        userDetail.Login.Should().Be("2");
        userDetail.Password.Should().Be("2");
    
    }

   

    
}