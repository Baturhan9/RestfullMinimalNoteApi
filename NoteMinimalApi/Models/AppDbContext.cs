using Microsoft.EntityFrameworkCore;

namespace NoteMinimalApi.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Note> Notes { get; set; }
    public DbSet<User> Users {get;set;}
}