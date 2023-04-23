using BusinessLogic.Models;
using Entities;

namespace BusinessLogic.Hubs.Chat;

public interface IChatClientInterface
{
    public Task Receive(Message message);
    public Task PromoteToTop(string chatId, Message latestMessage);
    public Task AddChatroom(ChatroomInfoBase chatroom);
}