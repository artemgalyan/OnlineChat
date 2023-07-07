using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Chatrooms;

[Table(nameof(PrivateChatroom))]
public class PrivateChatroom : Chatroom
{
    public PrivateChatroom(Guid id, User a, User b) : base(id, new List<User> { a, b }) {}

    public PrivateChatroom(Guid id, List<User> users) : base(id, users)
    {
        if (users.Count != 2)
        {
            throw new ArgumentException("Private chat can be created for 2 users");
        } 
    }

    public PrivateChatroom() {}
}