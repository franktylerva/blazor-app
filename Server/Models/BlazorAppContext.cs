using Microsoft.EntityFrameworkCore;

namespace BlazorApp.Models;

public class BlazorAppContext : DbContext
{
    public BlazorAppContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<Contact> Contacts { get; set; } = null!;
}