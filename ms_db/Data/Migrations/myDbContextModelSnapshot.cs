﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ms_db.Data;

#nullable disable

namespace ms_db.Data.Migrations
{
    [DbContext(typeof(myDbContext))]
    partial class myDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("CommonClasses.Models.School", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("CreatedAt")
                        .HasColumnType("longtext");

                    b.Property<int>("DistrictId")
                        .HasColumnType("int");

                    b.Property<string>("ExpiresAt")
                        .HasColumnType("longtext");

                    b.Property<int>("LicenseId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("UpdatedAt")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Schools");
                });
#pragma warning restore 612, 618
        }
    }
}
