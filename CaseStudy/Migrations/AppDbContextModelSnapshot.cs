﻿// <auto-generated />
using System;
using CaseStudy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CaseStudy.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.15")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CaseStudy.Models.Fund", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("varchar");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar");

                    b.HasKey("Id");

                    b.ToTable("fund", (string)null);
                });

            modelBuilder.Entity("CaseStudy.Models.FundReturn", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal?>("FiveYearReturn")
                        .HasPrecision(18, 4)
                        .HasColumnType("numeric(18,4)")
                        .HasColumnOrder(8);

                    b.Property<int>("FundId")
                        .HasColumnType("integer");

                    b.Property<decimal?>("OneMonthReturn")
                        .HasPrecision(18, 4)
                        .HasColumnType("numeric(18,4)");

                    b.Property<decimal?>("OneYearReturn")
                        .HasPrecision(18, 4)
                        .HasColumnType("numeric(18,4)");

                    b.Property<decimal?>("SixMonthReturn")
                        .HasPrecision(18, 4)
                        .HasColumnType("numeric(18,4)");

                    b.Property<decimal?>("ThreeMonthReturn")
                        .HasPrecision(18, 4)
                        .HasColumnType("numeric(18,4)");

                    b.Property<decimal?>("ThreeYearReturn")
                        .HasPrecision(18, 4)
                        .HasColumnType("numeric(18,4)")
                        .HasColumnOrder(7);

                    b.Property<decimal?>("YearToDateReturn")
                        .HasPrecision(18, 4)
                        .HasColumnType("numeric(18,4)");

                    b.HasKey("Id");

                    b.HasIndex("FundId");

                    b.ToTable("fund_returns", (string)null);
                });

            modelBuilder.Entity("CaseStudy.Models.FundReturn", b =>
                {
                    b.HasOne("CaseStudy.Models.Fund", "Fund")
                        .WithMany("Returns")
                        .HasForeignKey("FundId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Fund");
                });

            modelBuilder.Entity("CaseStudy.Models.Fund", b =>
                {
                    b.Navigation("Returns");
                });
#pragma warning restore 612, 618
        }
    }
}
