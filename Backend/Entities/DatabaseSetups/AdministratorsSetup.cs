using Entities.Chatrooms.PublicChatroom;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.DatabaseSetups;

public class AdministratorsSetup : IEntityTypeConfiguration<Administrators>
{
    public void Configure(EntityTypeBuilder<Administrators> builder)
    {
        builder.HasOne(a => a.PublicChatroom)
               .WithOne(c => c.Administrators)
               .HasForeignKey<Administrators>(a => a.PublicChatroomId)
               .HasForeignKey<PublicChatroom>(c => c.AdministratorsId)
               .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(a => a.Moderators)
               .WithMany()
               .UsingEntity<UserAdministrator>();
        builder.HasOne(a => a.Owner)
               .WithMany()
               .HasForeignKey(a => a.OwnerId);
    }
}