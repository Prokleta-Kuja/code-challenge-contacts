using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PublicContacts.Persistance.ContextConfigurations.AppDbContext
{
    public class Contact : IEntityTypeConfiguration<Domain.Contact>
    {
        public void Configure(EntityTypeBuilder<Domain.Contact> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                .HasMaxLength(Domain.Constants.MaxLengths.ContactName);

            builder.Property(e => e.Address)
                .HasMaxLength(Domain.Constants.MaxLengths.ContactAddress);

            builder.Property(e => e.Hash)
                .HasMaxLength(Domain.Constants.MaxLengths.ContactHash);
        }
    }
}