using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NoteMinimalApi.Models;
using NoteMinimalApi.Services;
using NoteMinimalApi.ViewModels.UserViewModel;

namespace NoteMinimalApi.Test;

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
            new User {UserId = 3, FirstName = "Tom", Login="3", Notes = new List<Note>(), Password="3",IsDeleted = true}
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
        userDetail!.UserId.Should().Be(2);
        userDetail.FirstName.Should().Be("Ben");
        userDetail.Login.Should().Be("2");
        userDetail.Password.Should().Be("2"); 
    }

    [Fact]
    public void GetUserDetail_NonExistentUser_ReturnsNull()
    {
        //arrange
        int id = 123; 
        var service = new UserService(_db);
        //act
        var user = service.GetUserDetailAsync(id).Result; 
        //assert
        user.Should().BeNull(); 
    }

    [Fact]
    public void CreateUser_ValidData_ReturnId()
    {
        //arrange
        var user = new CreateUserCommand()
        {
            Login = "login",
            Password = "123",
            FirstName = "firstName",
            LastName = "lastName",
        };
        var service = new UserService(_db);
        //act
        var id = service.CreateUserAsync(user).Result; 
        var userDetail = service.GetUserDetailAsync(id).Result;
        //assert
        userDetail.Should().NotBeNull(); 
        userDetail.Login.Should().Be(user.Login);
        userDetail.Password.Should().Be(user.Password);
        userDetail.FirstName.Should().Be(user.FirstName);
        userDetail.LastName.Should().Be(user.LastName);
        userDetail.UserId.Should().Be(id);
    }
    [Fact]
    public void UpdateUser_ValidData_SaveChanges()
    {
        //arrange
        var service = new UserService(_db);
        int id = 1;
        var user = service.GetUserDetailAsync(id).Result!;
        var userUpdate = new UpdateUserCommand()
        {
            UserId = user.UserId,
            FirstName = "John123123",
            LastName = "lastName",
            Login = user.Login,
            Password = user.Password
        };
        //act
        service.UpdateUserAsync(userUpdate); 
        user = service.GetUserDetailAsync(id).Result!;
        //assert

        user.Should().NotBeNull();
        user.UserId.Should().Be(userUpdate.UserId);
        user.FirstName.Should().Be(userUpdate.FirstName);
        user.LastName.Should().Be(userUpdate.LastName);
        user.Login.Should().Be(userUpdate.Login);
        user.Password.Should().Be(userUpdate.Password);
    }

    [Fact]
    public void UpdateUser_NonExistentUser_ThrowsException()
    {
        //arrange
        var service = new UserService(_db);
        var userUpdate = new UpdateUserCommand()
        {
            UserId = 123,
            FirstName = "John",
            LastName = "Doe",
            Login = "1",
            Password = "1"
        };
        //act
        var updating = () => service.UpdateUserAsync(userUpdate).Wait();
        //assert
        updating.Should().Throw<Exception>("Not Found")
            .WithMessage("Unable to find the user");
    }

    [Fact]
    public void UpdateUser_DeletedUser_ThrowsException()
    {
        //arrange
        var service = new UserService(_db);
        var userUpdate = new UpdateUserCommand()
        {
            UserId = 3,
            FirstName = "John",
            LastName = "Doe",
            Login = "1",
            Password = "1"
        };
        //act
        var updating = () => service.UpdateUserAsync(userUpdate).Wait();
        //assert
        updating.Should().Throw<Exception>("user is deleted")
            .WithMessage("User deleted");
    }
   
    [Fact]
    public void DeletedUser_ExistentUser_ReturnsNullUser()
    {
        //arrange
        var service = new UserService(_db); 
        int id = 1;
        //act
        service.DeleteUserAsync(id).Wait(); 
        var user = service.GetUserDetailAsync(id).Result;
        //assert
        user.Should().BeNull();
    
    }

    [Fact]
    public void IsAvailableForUpdateMethod_ValidId_ReturnsTrue()
    {
        //arrange
        var service = new UserService(_db); 
        int id = 2;
        //act
        var result = service.IsAvailableForUpdate(id).Result; 
        //assert
        result.Should().BeTrue(); 
    } 
    [Fact]
    public void IsAvailableForUpdateMethod_InValidId_ReturnsFalse()
    {
        //arrange
        var service = new UserService(_db); 
        int id = 3;
        //act
        var result = service.IsAvailableForUpdate(id).Result; 
        //assert
        result.Should().BeFalse(); 
    } 

    [Fact]
    public void IsAvailableForCreateMethod_ValidLogin_ReturnsTrue()
    {
        //arrange
        var service = new UserService(_db); 
        string login = "newLogin";
        //act
        var result = service.IsAvailableForCreate(login).Result;
        //assert
        result.Should().BeTrue(); 
    }

    [Fact]
    public void IsAvailableForCreateMethod_InValidLogin_ReturnsFalse()
    {
        //arrange
        var service = new UserService(_db); 
        string login = "2";
        //act
        var result = service.IsAvailableForCreate(login).Result;
        //assert
        result.Should().BeFalse(); 
    }
}