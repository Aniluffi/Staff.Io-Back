using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffIo.Data.Models;

namespace StaffIo.Data.Configs
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);

            // User → Deportament (One-to-Many через Owner)
            builder.HasMany(u => u.Deportament)
                   .WithOne(u => u.Owner)
                   .HasForeignKey(u => u.OwnerId)
                   .OnDelete(DeleteBehavior.Restrict);

            // User → Session (One-to-One)
            builder.HasOne(u => u.Session)
                   .WithOne(s => s.User)
                   .HasForeignKey<Session>(s => s.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // User → Account (One-to-One)
            builder.HasOne(u => u.Account)
                   .WithOne(a => a.User)
                   .HasForeignKey<Account>(a => a.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(u => u.Salary)
                    .HasColumnType("decimal(18,2)");
        }

    }
}
