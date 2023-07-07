using Entities.Chatrooms.PublicChatroom;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.DatabaseSetups;

public class UserAdministratorSetup : IEntityTypeConfiguration<UserAdministrator>
{
    public void Configure(EntityTypeBuilder<UserAdministrator> builder)
    {
        builder.HasNoKey();
    }
}