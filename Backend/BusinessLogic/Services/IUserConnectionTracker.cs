namespace BusinessLogic.Services;

public interface IUserConnectionTracker
{
    public void Add(string username, string connectionId, Guid? chatId);
    public void Remove(string username);
    public (string ConnectionId, Guid? ChatId)? GetInfo(string username);
    public void SetChatId(string username, Guid? chatId);
}

public class UserConnectionTracker : IUserConnectionTracker
{
    private readonly Dictionary<string, (string ConnectionId, Guid? ChatId)> _data = new();
    public void Add(string username, string connectionId, Guid? chatId)
    {
        lock (_data)
        {
            _data[username] = (connectionId, chatId);
        }
    }

    public void Remove(string username)
    {
        lock (_data)
        {
            _data.Remove(key: username);
        }
    }

    public (string ConnectionId, Guid? ChatId)? GetInfo(string username)
    {
        lock (_data)
        {
            return _data.GetValueOrDefault(key: username);
        }
    }

    public void SetChatId(string username, Guid? chatId)
    {
        lock (_data)
        {
            var data = _data.GetValueOrDefault(username);
            _data[username] = (data.ConnectionId, chatId);
        }
    }
}