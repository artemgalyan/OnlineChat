using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.DatabaseSetups;

public class ChatroomTicketSetup : IEntityTypeConfiguration<ChatroomTicket>
{
    public void Configure(EntityTypeBuilder<ChatroomTicket> builder)
    {
        builder.HasKey(t => new { t.UserId, t.ChatroomId });
        builder.HasOne(t => t.User)
               .WithMany(u => u.ChatroomTickets)
               .HasForeignKey(t => t.UserId)
               .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(t => t.Chatroom)
               .WithMany(c => c.UserTickets)
               .HasForeignKey(t => t.ChatroomId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}