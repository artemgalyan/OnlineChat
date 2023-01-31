﻿using Entities;
using Entities.Chatrooms;
using Microsoft.EntityFrameworkCore;

namespace Database;

public class ChatDatabase : DbContext
{
    public ChatDatabase(DbContextOptions options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PublicChatroom>()
                    .HasOne(c => c.Administrators)
                    .WithOne(a => a.PublicChatroom)
                    .HasForeignKey<Administrators>(a => a.PublicChatroomId)
                    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Administrators>()
                    .HasOne(a => a.Owner)
                    .WithMany()
                    .OnDelete(DeleteBehavior.NoAction);
        
        modelBuilder.Entity<Administrators>()
                    .HasMany(a => a.Moderators)
                    .WithMany();

        modelBuilder.Entity<PublicChatroom>()
                    .HasBaseType<Chatroom>();

        modelBuilder.Entity<PrivateChatroom>()
                    .HasBaseType<Chatroom>();

        modelBuilder.Entity<Chatroom>()
                    .HasMany(c => c.Messages)
                    .WithOne()
                    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
                    .HasMany(u => u.ChatroomTickets)
                    .WithOne(t => t.User)
                    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ChatroomTicket>()
                    .HasKey(t => new
                                     {
                                         t.UserId, t.ChatroomId
                                     });
                    // .HasNoKey();

        modelBuilder.Entity<ChatroomTicket>()
                    .HasOne(u => u.User)
                    .WithMany(u => u.ChatroomTickets)
                    .OnDelete(DeleteBehavior.NoAction);
        
        modelBuilder.Entity<ChatroomTicket>()
                    .HasOne(u => u.Chatroom)
                    .WithMany(u => u.UserTickets)
                    .OnDelete(DeleteBehavior.Cascade);
                    
    }

    public DbSet<User> Users { get; set; } = null!;

    public DbSet<Chatroom> Chatroom { get; set; } = null!;
}