using Ecommerce.IdentitySettings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Configurations.IdentitySettings;

public class IdentitySettingConfiguration : IEntityTypeConfiguration<IdentitySetting>
{
    public void Configure(EntityTypeBuilder<IdentitySetting> builder)
    {
        builder.ToTable(EcommerceConsts.DbTablePrefix + "IdentitySettings", EcommerceConsts.DbSchema);
        builder.HasKey(x => x.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);

    }
}