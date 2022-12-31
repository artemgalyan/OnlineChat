﻿using System.Collections.Immutable;

namespace Database.Entities;

public class Chatroom
{
    public enum ChatType
    {
        Public,
        Private
    }

    public Guid Id { get; set; }
    public List<User> Users { get; }
    public List<Message> Messages { get; } = new();
    public ChatType Type { get; }

    public Chatroom(Guid id, ChatType type, List<User> users)
    {
        Id = id;
        Type = type;
        Users = new List<User>(
            type == ChatType.Private
                ? users.ToImmutableList()
                : users);
        if (users.Count == 0)
        {
            throw new ArgumentException($"{nameof(users)} contains no users");
        }

        if (type == ChatType.Private && users.Count != 2)
        {
            throw new ArgumentException($"Private chat must contain only 2 members");
        }
    }
    
    public Chatroom() {}
    
    public bool IsEmpty()
    {
        return Users.Count == 0;
    }
}