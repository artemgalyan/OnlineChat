using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Chatrooms;

namespace Entities;

/// <summary>
/// Data class
/// </summary>
public class User : IEquatable<User>
{
    [Key] public Guid Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    [NotMapped] public string FullName => string.Join(' ', Name, Surname); 
    public string Password { get; set; } = string.Empty;
    public List<ChatroomTicket> ChatroomTickets { get; set; } = new();
    [NotMapped] public IEnumerable<Chatroom> Chatrooms => ChatroomTickets.Select(t => t.Chatroom);
    public string Role { get; set; } = "User";

    public User() {}

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((User) obj);
    }

    public bool Equals(User? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Login == other.Login && Name == other.Name && Surname == other.Surname && Role == other.Role;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Login, Name, Surname, Role);
    }

    public static bool operator ==(User a, User b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(User a, User b)
    {
        return !(a == b);
    }

    public bool Leave(Chatroom c)
    {
        var ticket = ChatroomTickets.FirstOrDefault(t => t.Chatroom == c);
        return ticket is not null && ChatroomTickets.Remove(ticket);
    }

    public void Join(Chatroom chatroom)
    {
        var ticket = new ChatroomTicket(this, chatroom);
        ChatroomTickets.Add(ticket);
        chatroom.UserTickets.Add(ticket);
    }
}