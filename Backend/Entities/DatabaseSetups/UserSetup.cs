using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.DatabaseSetups;

public class UserSetup : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.HasMany(u => u.ChatroomTickets)
               .WithOne(t => t.User)
               .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(u => u.Login)
               .IsUnique();
    }
}