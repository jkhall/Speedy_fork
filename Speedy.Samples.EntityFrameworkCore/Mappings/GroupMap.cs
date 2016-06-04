#region References

using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Speedy.EntityFrameworkCore;
using Speedy.Samples.Entities;

#endregion

namespace Speedy.Samples.EntityFrameworkCore.Mappings
{
	[ExcludeFromCodeCoverage]
	public class GroupMap : IEntityTypeConfiguration
	{
		#region Methods

		public void Configure(ModelBuilder instance)
		{
			var mapping = instance.Entity<Group>();

			mapping.HasKey(t => t.Id);
			mapping.ToTable("Groups");
			mapping.Property(t => t.Id).UseSqlServerIdentityColumn();
			mapping.Property(t => t.CreatedOn).IsRequired().HasColumnType("datetime2");
			mapping.Property(t => t.Description).IsRequired();
			mapping.Property(t => t.ModifiedOn).IsRequired().HasColumnType("datetime2");
			mapping.Property(t => t.Name).IsRequired().HasMaxLength(256);
		}

		#endregion
	}
}