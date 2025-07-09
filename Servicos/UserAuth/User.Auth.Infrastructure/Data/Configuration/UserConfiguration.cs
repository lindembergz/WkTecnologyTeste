using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserAuth.Dominio.Entidades;

namespace UserAuth.Infraestrutura.Data.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(100);
            builder.HasIndex(u => u.Username).IsUnique();

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);
            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.PasswordHash)
                .IsRequired();

            builder.Property(u => u.IsEmailConfirmed)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(u => u.EmailConfirmationToken)
                .HasMaxLength(255);

            builder.Property(u => u.IsTwoFactorEnabled)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(u => u.TwoFactorCode)
                .HasMaxLength(20);

            builder.Property(u => u.RefreshToken)
                .HasMaxLength(255);
        }
    }
}
