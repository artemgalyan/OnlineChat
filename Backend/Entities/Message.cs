using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Chatrooms;

namespace Entities;

[Table("Messages")]
public class Message
{
    [Key]
    public Guid Id { get; set; }

    public Guid SenderId { get; set; }
    public Guid ChatId { get; set; }
    public Chatroom Chatroom { get; set; }
    public User Sender { get; set; }

    public string Text { get; set; } = string.Empty;
    public DateTime SendingTime { get; set; }

    public Message(Guid senderId, string text)
    {
        SenderId = senderId;
        Text = text;
        SendingTime = DateTime.Now;
    }

    public Message() {}
}