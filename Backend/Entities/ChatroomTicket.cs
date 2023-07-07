using System.ComponentModel.DataAnnotations;
using Entities.Chatrooms;

namespace Entities;

public class ChatroomTicket : IEquatable<ChatroomTicket>
{
    [Key] public Guid UserId { get; set; }
    [Key] public Guid ChatroomId { get; set; }
    public User User { get; set; }
    public Chatroom Chatroom { get; set; }
    public int LastMessageRead { get; set; }

    public ChatroomTicket() {} // for EF Core

    public ChatroomTicket(User user, Chatroom chatroom)
    {
        User = user;
        Chatroom = chatroom;
        LastMessageRead = chatroom.MessagesCount;
        UserId = user.Id;
        ChatroomId = chatroom.Id;
    }

    public bool Equals(ChatroomTicket? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return UserId.Equals(other.UserId) && ChatroomId.Equals(other.ChatroomId);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((ChatroomTicket) obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(UserId, ChatroomId);
    }

    public static bool operator==(ChatroomTicket a, ChatroomTicket b)
    {
        return a.UserId == b.UserId && a.ChatroomId == b.ChatroomId;
    }

    public static bool operator!=(ChatroomTicket a, ChatroomTicket b)
    {
        return !(a == b);
    }
}