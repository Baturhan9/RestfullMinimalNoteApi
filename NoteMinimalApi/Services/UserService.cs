using Microsoft.EntityFrameworkCore;
using NoteMinimalApi.Models;
using NoteMinimalApi.ViewModels.UserViewModel;

namespace NoteMinimalApi.Services;

public class UserService
{
    private AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateUserAsync(CreateUserCommand cmd)
    {
        var user = cmd.ToUser();
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user.UserId;
    }

    public async Task<List<UserSummaryViewModel>> GetUsersAsync()
    {
        return await _context.Users.Where(u => !u.IsDeleted)
            .Select(u => new UserSummaryViewModel
            {
                UserId = u.UserId,
                UserName = $"{u.FirstName} {u.LastName} aka {u.Login}"
            })
            .ToListAsync();
    }

    public async Task<UserDetailViewModel?> GetUserDetailAsync(int id)
    {
        return await _context.Users.Where(u => u.UserId == id && !u.IsDeleted)
            .Select(u => new UserDetailViewModel
            {
                UserId = u.UserId,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Login = u.Login,
                Password = u.Password
            })
            .SingleOrDefaultAsync();
    }

    public async Task UpdateUserAsync(UpdateUserCommand cmd)
    {
        var user = await _context.Users.FindAsync(cmd.UserId);
        if(user is null)
            throw new Exception("Unable to find the user");
        if(user.IsDeleted)
            throw new Exception("User deleted");

        user.FirstName = cmd.FirstName;
        user.LastName = cmd.LastName;
        user.Login = cmd.Login;
        user.Password = cmd.Password;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if(user is not null)
        {
            user.IsDeleted = true;
            await _context.SaveChangesAsync();
        } 
    }

    public async Task<bool> IsAvailableForUpdate(int id)
    {
        return await _context.Users.Where(u => !u.IsDeleted && u.UserId == id).AnyAsync();
    }

    public async Task<bool> IsAvailableForCreate(string login)
    {
        return !await _context.Users.Where(u=>!u.IsDeleted)
            .Where(u => u.Login == login).AnyAsync();
    }

}