namespace Entities.Chatrooms.PublicChatroom;

public class Administrators
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PublicChatroomId { get; set; }
    public User Owner { get; set; }
    public Guid OwnerId { get; set; }

    public List<User> Moderators { get; set; }
    public PublicChatroom PublicChatroom { get; set; }

    public Administrators() {}

    public Administrators(User owner, List<User> moderators, PublicChatroom chatroom)
    {
        Owner = owner;
        Moderators = moderators;
        PublicChatroom = chatroom;
    }
}