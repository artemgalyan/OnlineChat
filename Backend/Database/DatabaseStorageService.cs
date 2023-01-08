﻿using Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Database;

public class DatabaseStorageService : IStorageService
{
    private readonly Database _database;

    public DatabaseStorageService(Database database)
    {
        _database = database;
    }

    public Task<User?> GetUser(Func<User, bool> predicate, CancellationToken cancellationToken)
    {
        return Task.FromResult(
            _database.Users
                     .Include(u => u.Chatrooms)
                     .FirstOrDefault(predicate));
    }

    public Task<Chatroom?> GetChatroom(Func<Chatroom, bool> predicate, CancellationToken cancellationToken)
    {
        var chatroom = _database.Chatrooms
                                .Include(c => c.Messages)
                                .Include(c => c.Users)
                                .FirstOrDefault(predicate);
        return Task.FromResult(chatroom);
    }

    public async Task AddChatroom(Chatroom chatroom, CancellationToken cancellationToken)
    {
        await _database.Chatrooms.AddAsync(chatroom, cancellationToken);
        await _database.SaveChangesAsync(cancellationToken);
    }

    public async Task AddUser(User user, CancellationToken cancellationToken)
    {
        await _database.Users.AddAsync(user, cancellationToken);
        await _database.SaveChangesAsync(cancellationToken);
    }

    public async Task Remove(User user, CancellationToken cancellationToken)
    {
        _database.Users.Remove(user);
        await _database.SaveChangesAsync(cancellationToken);
    }

    public async Task Remove(Chatroom chatroom, CancellationToken cancellationToken)
    {
        _database.Chatrooms.Remove(chatroom);
        await _database.SaveChangesAsync(cancellationToken);
    }

    public Task<List<Chatroom>> GetChatrooms(CancellationToken cancellationToken)
    {
        return _database.Chatrooms.ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _database.SaveChangesAsync(cancellationToken);
    }

    public async Task AddMessageTo(Chatroom chatroom, Message message, CancellationToken cancellationToken)
    {
        await SaveChangesAsync(cancellationToken);
    }
}