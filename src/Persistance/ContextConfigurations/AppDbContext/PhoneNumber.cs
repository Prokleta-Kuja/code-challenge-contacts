using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PublicContacts.Persistance.ContextConfigurations.AppDbContext
{
    public class PhoneNumber : IEntityTypeConfiguration<Domain.PhoneNumber>
    {
        public void Configure(EntityTypeBuilder<Domain.PhoneNumber> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Contact)
                .WithMany(e => e!.PhoneNumbers)
                .HasForeignKey(e => e.ContactId);

            builder.Property(e => e.Number)
                .HasMaxLength(Domain.Constants.MaxLengths.PhoneNumberNumber);
        }
    }
}