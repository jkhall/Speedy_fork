﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Speedy.Website.Data.Sqlite;

namespace Speedy.Website.Data.Sqlite.Migrations
{
    [DbContext(typeof(ContosoSqliteDatabase))]
    partial class ContosoSqliteDatabaseModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.4");

            modelBuilder.Entity("Speedy.Website.Samples.Entities.AccountEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasColumnType("INTEGER");

                    b.Property<long>("AddressId")
                        .HasColumnName("AddressId")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("AddressSyncId")
                        .HasColumnName("AddressSyncId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnName("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("EmailAddress")
                        .HasColumnType("TEXT")
                        .IsUnicode(false);

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastLoginDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnName("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("Name")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256)
                        .IsUnicode(false);

                    b.Property<string>("Nickname")
                        .HasColumnName("Nickname")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256)
                        .IsUnicode(false);

                    b.Property<string>("PasswordHash")
                        .HasColumnType("TEXT")
                        .IsUnicode(false);

                    b.Property<string>("Roles")
                        .HasColumnType("TEXT")
                        .IsUnicode(false);

                    b.Property<Guid>("SyncId")
                        .HasColumnName("SyncId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("AddressId")
                        .HasName("IX_Accounts_AddressId");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasName("IX_Accounts_Name");

                    b.HasIndex("Nickname")
                        .IsUnique()
                        .HasName("IX_Accounts_Nickname")
                        .HasFilter("Nickname IS NOT NULL");

                    b.HasIndex("SyncId")
                        .IsUnique()
                        .HasName("IX_Accounts_SyncId");

                    b.ToTable("Accounts","dbo");
                });

            modelBuilder.Entity("Speedy.Website.Samples.Entities.AddressEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasColumnType("INTEGER");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnName("City")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256)
                        .IsUnicode(false);

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnName("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Line1")
                        .IsRequired()
                        .HasColumnName("Line1")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256)
                        .IsUnicode(false);

                    b.Property<string>("Line2")
                        .IsRequired()
                        .HasColumnName("Line2")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256)
                        .IsUnicode(false);

                    b.Property<long?>("LinkedAddressId")
                        .HasColumnName("LinkedAddressId")
                        .HasColumnType("INTEGER");

                    b.Property<Guid?>("LinkedAddressSyncId")
                        .HasColumnName("LinkedAddressSyncId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnName("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Postal")
                        .IsRequired()
                        .HasColumnName("Postal")
                        .HasColumnType("TEXT")
                        .HasMaxLength(128)
                        .IsUnicode(false);

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnName("State")
                        .HasColumnType("TEXT")
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

            modelBuilder.Entity("Speedy.Website.Samples.Entities.FoodEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnName("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnName("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("Name")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasName("IX_Foods_Name");

                    b.ToTable("Foods","dbo");
                });

            modelBuilder.Entity("Speedy.Website.Samples.Entities.FoodRelationshipEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ChildId")
                        .HasColumnName("ChildId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnName("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnName("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<int>("ParentId")
                        .HasColumnName("ParentId")
                        .HasColumnType("INTEGER");

                    b.Property<double>("Quantity")
                        .HasColumnName("Quantity")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.HasIndex("ChildId")
                        .HasName("IX_FoodRelationships_ChildId");

                    b.HasIndex("ParentId")
                        .HasName("IX_FoodRelationships_ParentId");

                    b.ToTable("FoodRelationships","dbo");
                });

            modelBuilder.Entity("Speedy.Website.Samples.Entities.GroupEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnName("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnName("Description")
                        .HasColumnType("TEXT")
                        .HasMaxLength(4000)
                        .IsUnicode(false);

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnName("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("Name")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.ToTable("Groups","dbo");
                });

            modelBuilder.Entity("Speedy.Website.Samples.Entities.GroupMemberEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnName("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<int>("GroupId")
                        .HasColumnName("GroupId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MemberId")
                        .HasColumnName("MemberId")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("MemberSyncId")
                        .HasColumnName("MemberSyncId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnName("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnName("Role")
                        .HasColumnType("TEXT")
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

            modelBuilder.Entity("Speedy.Website.Samples.Entities.LogEventEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasMaxLength(250);

                    b.Property<DateTime?>("AcknowledgedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Level")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LoggedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Message")
                        .HasColumnType("TEXT")
                        .IsUnicode(false);

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("SyncId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("LogEvents","dbo");
                });

            modelBuilder.Entity("Speedy.Website.Samples.Entities.PetEntity", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnName("Name")
                        .HasColumnType("TEXT")
                        .HasMaxLength(128)
                        .IsUnicode(false);

                    b.Property<int>("OwnerId")
                        .HasColumnName("OwnerId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnName("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnName("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("TypeId")
                        .HasColumnName("TypeId")
                        .HasColumnType("TEXT")
                        .HasMaxLength(25)
                        .IsUnicode(false);

                    b.HasKey("Name", "OwnerId");

                    b.HasIndex("OwnerId")
                        .HasName("IX_Pets_OwnerId");

                    b.HasIndex("TypeId")
                        .HasName("IX_Pets_TypeId");

                    b.HasIndex("Name", "OwnerId")
                        .IsUnique();

                    b.ToTable("Pets","dbo");
                });

            modelBuilder.Entity("Speedy.Website.Samples.Entities.PetTypeEntity", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnName("PetTypeId")
                        .HasColumnType("TEXT")
                        .HasMaxLength(25)
                        .IsUnicode(false);

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Type")
                        .HasColumnName("Type")
                        .HasColumnType("TEXT")
                        .HasMaxLength(200)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.ToTable("PetType","dbo");
                });

            modelBuilder.Entity("Speedy.Website.Samples.Entities.SettingEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnName("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnName("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("Name")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256)
                        .IsUnicode(false);

                    b.Property<Guid>("SyncId")
                        .HasColumnName("SyncId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnName("Value")
                        .HasColumnType("TEXT")
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasName("IX_Settings_Name");

                    b.HasIndex("SyncId")
                        .IsUnique()
                        .HasName("IX_Settings_SyncId");

                    b.ToTable("Settings","dbo");
                });

            modelBuilder.Entity("Speedy.Website.Samples.Entities.AccountEntity", b =>
                {
                    b.HasOne("Speedy.Website.Samples.Entities.AddressEntity", "Address")
                        .WithMany("Accounts")
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Speedy.Website.Samples.Entities.AddressEntity", b =>
                {
                    b.HasOne("Speedy.Website.Samples.Entities.AddressEntity", "LinkedAddress")
                        .WithMany("LinkedAddresses")
                        .HasForeignKey("LinkedAddressId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Speedy.Website.Samples.Entities.FoodRelationshipEntity", b =>
                {
                    b.HasOne("Speedy.Website.Samples.Entities.FoodEntity", "Child")
                        .WithMany("ParentRelationships")
                        .HasForeignKey("ChildId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Speedy.Website.Samples.Entities.FoodEntity", "Parent")
                        .WithMany("ChildRelationships")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Speedy.Website.Samples.Entities.GroupMemberEntity", b =>
                {
                    b.HasOne("Speedy.Website.Samples.Entities.GroupEntity", "Group")
                        .WithMany("Members")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Speedy.Website.Samples.Entities.AccountEntity", "Member")
                        .WithMany("Groups")
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Speedy.Website.Samples.Entities.PetEntity", b =>
                {
                    b.HasOne("Speedy.Website.Samples.Entities.AccountEntity", "Owner")
                        .WithMany("Pets")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Speedy.Website.Samples.Entities.PetTypeEntity", "Type")
                        .WithMany("Types")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.SetNull);
                });
#pragma warning restore 612, 618
        }
    }
}
