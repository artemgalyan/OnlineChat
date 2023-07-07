using Entities.Chatrooms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.DatabaseSetups;

public class PrivateChatroomSetup : IEntityTypeConfiguration<PrivateChatroom>
{
    public void Configure(EntityTypeBuilder<PrivateChatroom> builder)
    {
        builder.HasBaseType<Chatroom>();
    }
}