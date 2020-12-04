#region References

using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Speedy.EntityFramework;
using Speedy.Website.Samples.Entities;

#endregion

namespace Speedy.Website.Samples.Mappings
{
	[ExcludeFromCodeCoverage]
	public class SettingMap : EntityMappingConfiguration<SettingEntity>
	{
		#region Methods

		public override void Map(EntityTypeBuilder<SettingEntity> b)
		{
			b.ToTable("Settings", "dbo");
			b.HasKey(x => x.Id);

			b.Property(x => x.CreatedOn).HasColumnName("CreatedOn").IsRequired();
			b.Property(x => x.Id).HasColumnName("Id").IsRequired();
			b.Property(x => x.ModifiedOn).HasColumnName("ModifiedOn").IsRequired();
			b.Property(x => x.Name).HasColumnName("Name").HasMaxLength(256).IsRequired();
			b.Property(x => x.Value).HasColumnName("Value").IsRequired();
			b.Property(x => x.SyncId).HasColumnName("SyncId").IsRequired();

			b.HasIndex(x => x.Name).HasDatabaseName("IX_Settings_Name").IsUnique();
			b.HasIndex(x => x.SyncId).HasDatabaseName("IX_Settings_SyncId").IsUnique();
		}

		#endregion
	}
}