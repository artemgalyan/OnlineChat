using Entities;
using Entities.Chatrooms;
using Entities.Chatrooms.PublicChatroom;
using Entities.DatabaseSetups;
using Microsoft.EntityFrameworkCore;

namespace Database;

public class ChatDatabase : DbContext
{
    public ChatDatabase(DbContextOptions options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AdministratorsSetup).Assembly);
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Chatroom> Chatroom { get; set; } = null!;
    public DbSet<ChatroomTicket> ChatroomTicket { get; set; } = null!;
    public DbSet<Administrators> Administrators { get; set; } = null!;
    public DbSet<UserAdministrator> UserAdministrators { get; set; } = null!;
    public DbSet<Message> Messages { get; set; } = null!;
    public DbSet<UserToken> UserTokens { get; set; } = null!;
}