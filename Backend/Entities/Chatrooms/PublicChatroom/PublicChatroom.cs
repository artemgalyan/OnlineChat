using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Chatrooms.PublicChatroom;

[Table(nameof(PublicChatroom))]
public class PublicChatroom : Chatroom
{
    public string Name { get; set; }
    public Administrators Administrators { get; set; }
    public Guid AdministratorsId { get; set; }

    public PublicChatroom(Guid id, List<User> users, User owner, string name) : base(id, users)
    {
        if (!users.Contains(owner))
        {
            throw new ArgumentException("owner must be in chat");
        }
        Administrators = new Administrators(owner: owner, moderators: new List<User>(), this);
        Name = name;
    }

    public PublicChatroom() {}

    public bool Kick(User user)
    {
        if (!Users.Contains(user) || user == Administrators.Owner)
        {
            return false;
        }

        ForceKick(user);
        return true;
    }

    public void ForceKick(User user)
    {
        if (Administrators.Moderators.Contains(user))
        {
            Administrators.Moderators.Remove(user);
        }
        var ticket = UserTickets.FirstOrDefault(t => t.User == user);
        if (ticket is not null)
        {
            UserTickets.Remove(ticket);
            user.Leave(this);
        }
    }

    public void AddModerator(User user)
    {
        Administrators.Moderators.Add(user);
    }

    public void RemoveModerator(User user)
    {
        Administrators.Moderators.Add(user);
    }
}