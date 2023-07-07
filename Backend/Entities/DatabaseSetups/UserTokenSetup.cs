using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.DatabaseSetups;

public class UserTokenSetup : IEntityTypeConfiguration<UserToken>
{
    public void Configure(EntityTypeBuilder<UserToken> builder)
    {
        builder.HasKey(ut => ut.JwtToken);
        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(ut => ut.UserId);
    }
}