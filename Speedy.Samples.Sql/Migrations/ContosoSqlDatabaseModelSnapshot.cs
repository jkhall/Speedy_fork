﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Speedy.Samples.Sql;

namespace Speedy.Samples.Sql.Migrations
{
    [DbContext(typeof(ContosoSqlDatabase))]
    partial class ContosoSqlDatabaseModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Speedy.Samples.Entities.AddressEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnName("City")
                        .HasMaxLength(256)
                        .IsUnicode(false);

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnName("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("Line1")
                        .IsRequired()
                        .HasColumnName("Line1")
                        .HasMaxLength(256)
                        .IsUnicode(false);

                    b.Property<string>("Line2")
                        .IsRequired()
                        .HasColumnName("Line2")
                        .HasMaxLength(256)
                        .IsUnicode(false);

                    b.Property<long?>("LinkedAddressId")
                        .HasColumnName("LinkedAddressId");

                    b.Property<Guid?>("LinkedAddressSyncId")
                        .HasColumnName("LinkedAddressSyncId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnName("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Postal")
                        .IsRequired()
                        .HasColumnName("Postal")
                        .HasMaxLength(128)
                        .IsUnicode(false);

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnName("State")
                        .HasMaxLength(128)
                        .IsUnicode(false);

                    b.Property<Guid>("SyncId")
                        .HasColumnName("SyncId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("LinkedAddressId")
                        .HasName("IX_Address_LinkedAddressId");

                    b.HasIndex("SyncId")
                        .IsUnique()
                        .HasName("IX_Address_SyncId");

                    b.ToTable("Addresses","dbo");
                });

            modelBuilder.Entity("Speedy.Samples.Entities.FoodEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnName("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnName("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("Name")
                        .HasMaxLength(256)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.ToTable("Foods","dbo");
                });

            modelBuilder.Entity("Speedy.Samples.Entities.FoodRelationshipEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ChildId")
                        .HasColumnName("ChildId");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnName("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnName("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<int>("ParentId")
                        .HasColumnName("ParentId");

                    b.Property<double>("Quantity")
                        .HasColumnName("Quantity");

                    b.HasKey("Id");

                    b.HasIndex("ChildId")
                        .HasName("IX_FoodRelationships_ChildId");

                    b.HasIndex("ParentId")
                        .HasName("IX_FoodRelationships_ParentId");

                    b.ToTable("FoodRelationships","dbo");
                });

            modelBuilder.Entity("Speedy.Samples.Entities.GroupEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnName("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnName("Description")
                        .HasMaxLength(4000)
                        .IsUnicode(false);

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnName("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("Name")
                        .HasMaxLength(256)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.ToTable("Groups","dbo");
                });

            modelBuilder.Entity("Speedy.Samples.Entities.GroupMemberEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnName("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<int>("GroupId")
                        .HasColumnName("GroupId");

                    b.Property<int>("MemberId")
                        .HasColumnName("MemberId");

                    b.Property<Guid>("MemberSyncId")
                        .HasColumnName("MemberSyncId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnName("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnName("Role")
                        .HasMaxLength(4000)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.HasIndex("GroupId")
                        .HasName("IX_GroupMembers_GroupId");

                    b.HasIndex("MemberId")
                        .HasName("IX_GroupMembers_MemberId");

                    b.HasIndex("MemberSyncId")
                        .HasName("IX_GroupMembers_MemberSyncId");

                    b.ToTable("GroupMembers","dbo");
                });

            modelBuilder.Entity("Speedy.Samples.Entities.LogEventEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasMaxLength(250)
                        .IsUnicode(false);

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnName("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<int>("Level");

                    b.Property<string>("Message")
                        .HasColumnName("Message")
                        .IsUnicode(false);

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("LogEvents","dbo");
                });

            modelBuilder.Entity("Speedy.Samples.Entities.PersonEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("AddressId")
                        .HasColumnName("AddressId");

                    b.Property<Guid>("AddressSyncId")
                        .HasColumnName("AddressSyncId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnName("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnName("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("Name")
                        .HasMaxLength(256)
                        .IsUnicode(false);

                    b.Property<Guid>("SyncId")
                        .HasColumnName("SyncId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("AddressId")
                        .HasName("IX_People_AddressId");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasName("IX_People_Name");

                    b.HasIndex("SyncId")
                        .IsUnique()
                        .HasName("IX_People_SyncId");

                    b.ToTable("People","dbo");
                });

            modelBuilder.Entity("Speedy.Samples.Entities.PetEntity", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnName("Name")
                        .HasMaxLength(128)
                        .IsUnicode(false);

                    b.Property<int>("OwnerId")
                        .HasColumnName("OwnerId");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnName("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnName("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("TypeId")
                        .HasColumnName("TypeId")
                        .HasMaxLength(25)
                        .IsUnicode(false);

                    b.HasKey("Name", "OwnerId");

                    b.HasIndex("OwnerId")
                        .HasName("IX_Pets_OwnerId");

                    b.HasIndex("TypeId")
                        .HasName("IX_Pets_TypeId");

                    b.ToTable("Pets","dbo");
                });

            modelBuilder.Entity("Speedy.Samples.Entities.PetTypeEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("PetTypeId")
                        .HasMaxLength(25)
                        .IsUnicode(false);

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Type")
                        .HasColumnName("Type")
                        .HasMaxLength(200)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.ToTable("PetType","dbo");
                });

            modelBuilder.Entity("Speedy.Samples.Entities.AddressEntity", b =>
                {
                    b.HasOne("Speedy.Samples.Entities.AddressEntity", "LinkedAddress")
                        .WithMany("LinkedAddresses")
                        .HasForeignKey("LinkedAddressId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Speedy.Samples.Entities.FoodRelationshipEntity", b =>
                {
                    b.HasOne("Speedy.Samples.Entities.FoodEntity", "Child")
                        .WithMany("ParentRelationships")
                        .HasForeignKey("ChildId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Speedy.Samples.Entities.FoodEntity", "Parent")
                        .WithMany("ChildRelationships")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Speedy.Samples.Entities.GroupMemberEntity", b =>
                {
                    b.HasOne("Speedy.Samples.Entities.GroupEntity", "Group")
                        .WithMany("Members")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Speedy.Samples.Entities.PersonEntity", "Member")
                        .WithMany("Groups")
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Speedy.Samples.Entities.PersonEntity", b =>
                {
                    b.HasOne("Speedy.Samples.Entities.AddressEntity", "Address")
                        .WithMany("People")
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Speedy.Samples.Entities.PetEntity", b =>
                {
                    b.HasOne("Speedy.Samples.Entities.PersonEntity", "Owner")
                        .WithMany("Owners")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Speedy.Samples.Entities.PetTypeEntity", "Type")
                        .WithMany("Types")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.SetNull);
                });
#pragma warning restore 612, 618
        }
    }
}
