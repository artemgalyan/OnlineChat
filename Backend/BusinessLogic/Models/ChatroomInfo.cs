using System.Diagnostics;
using Entities;
using Entities.Chatrooms;
using Entities.Chatrooms.PublicChatroom;

namespace BusinessLogic.Models;

public class ChatroomInfoBase
{
    public Guid Id { get; set; }
    public List<string> Users { get; set; }
    public ChatType Type { get; set; }
    public int UnreadMessages { get; set; }
    public Message? LastMessage { get; set; }

    public ChatroomInfoBase(Guid id, List<string> users, ChatType type, int unreadMessages, Message? lastMessage)
    {
        Id = id;
        Users = users;
        Type = type;
        UnreadMessages = unreadMessages;
        LastMessage = lastMessage;
    }
}

public static class ChatroomInfo
{
    public static object Of(ChatroomTicket ticket) => ticket.Chatroom switch
        {
            PublicChatroom pc => PublicChatroomInfo.Of(pc, ticket.LastMessageRead),
            PrivateChatroom pr => PrivateChatroomInfo.Of(pr, ticket.LastMessageRead),
            _ => throw new UnreachableException()
        };

    public static ChatroomInfoBase OfNew(Chatroom chatroom) => chatroom switch
        {
            PublicChatroom pc => PublicChatroomInfo.Of(pc, 0),
            PrivateChatroom p => PrivateChatroomInfo.Of(p, 0),
            _ => throw new UnreachableException()
        };
}

public class PrivateChatroomInfo : ChatroomInfoBase
{
    public static PrivateChatroomInfo Of(PrivateChatroom pc, int messagesRead)
    {
        return new PrivateChatroomInfo(
            id: pc.Id,
            users: pc.Users.Select(u => u.Username).ToList(),
            unreadMessages: pc.MessagesCount - messagesRead,
            lastMessage: pc.Messages.LastOrDefault()
        );
    }

    public PrivateChatroomInfo(Guid id, List<string> users, int unreadMessages, Message? lastMessage)
        : base(id, users, ChatType.Private, unreadMessages, lastMessage)
    {
    }
}

public class PublicChatroomInfo : ChatroomInfoBase
{
    public string Owner { get; set; }
    public List<string> Moderators { get; set; }
    public string Name { get; set; }

    public static PublicChatroomInfo Of(PublicChatroom pc, int messagesRead)
    {
        return new PublicChatroomInfo(
            id: pc.Id,
            users: Enumerable.Empty<string>().ToList(),
            unreadMessages: pc.MessagesCount - messagesRead,
            owner: pc.Administrators.Owner.Username,
            moderators: pc.Administrators.Moderators.Select(m => m.Username).ToList(),
            name: pc.Name,
            lastMessage: pc.Messages.LastOrDefault()
        );
    }

    public PublicChatroomInfo(Guid id, List<string> users, int unreadMessages, string owner,
        List<string> moderators, string name, Message? lastMessage)
        : base(id, users, ChatType.Public, unreadMessages, lastMessage)
    {
        Owner = owner;
        Moderators = moderators;
        Name = name;
    }
}