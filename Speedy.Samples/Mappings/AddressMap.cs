#region References

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Diagnostics.CodeAnalysis;
using Speedy.Samples.Entities;

#endregion

namespace Speedy.Samples.Mappings
{
	[ExcludeFromCodeCoverage]
	public class AddressMap : EntityTypeConfiguration<Address>
	{
		#region Constructors

		public AddressMap()
		{
			// Primary Key
			HasKey(t => t.Id);

			// Table & Column Mappings
			ToTable("Addresses");
			Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			Property(t => t.CreatedOn).IsRequired().HasColumnType("datetime2").HasPrecision(7);
			Property(t => t.Line1).IsRequired().HasMaxLength(256).HasColumnAnnotation("Index", new IndexAnnotation(new[] { new IndexAttribute { IsUnique = true } }));
			Property(t => t.Line2).IsRequired().HasMaxLength(256);
			Property(t => t.City).IsRequired().HasMaxLength(256);
			Property(t => t.State).IsRequired().HasMaxLength(128);
			Property(t => t.Postal).IsRequired().HasMaxLength(128);
			Property(t => t.ModifiedOn).IsRequired().HasColumnType("datetime2").HasPrecision(7);

			// Relationships
			HasOptional(x => x.LinkedAddress)
				.WithMany()
				.HasForeignKey(x => x.LinkedAddressId);
		}

		#endregion
	}
}