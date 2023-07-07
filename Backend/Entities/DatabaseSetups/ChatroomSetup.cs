using Entities.Chatrooms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.DatabaseSetups;

public class ChatroomSetup : IEntityTypeConfiguration<Chatroom>
{
    public void Configure(EntityTypeBuilder<Chatroom> builder)
    {
        builder
            .HasMany(c => c.Messages)
            .WithOne(m => m.Chatroom)
            .HasForeignKey(m => m.ChatId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany<User>()
               .WithMany()
               .UsingEntity<ChatroomTicket>();
    }
}