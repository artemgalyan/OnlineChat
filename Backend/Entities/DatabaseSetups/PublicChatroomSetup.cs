using Entities.Chatrooms;
using Entities.Chatrooms.PublicChatroom;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.DatabaseSetups;

public class PublicChatroomSetup : IEntityTypeConfiguration<PublicChatroom>
{
    public void Configure(EntityTypeBuilder<PublicChatroom> builder)
    {
        builder.HasBaseType<Chatroom>();

        builder.HasOne(c => c.Administrators)
               .WithOne(a => a.PublicChatroom)
               .HasForeignKey<Administrators>(adm => adm.PublicChatroomId)
               .HasForeignKey<PublicChatroom>(pc => pc.AdministratorsId)
               .OnDelete(DeleteBehavior.Cascade);

    }
}