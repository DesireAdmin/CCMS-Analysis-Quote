﻿// <auto-generated />
using System;
using CCMS_Analysis_Quote.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CCMS_Analysis_Quote.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241028094316_UpdateQuoteModelIdType1")]
    partial class UpdateQuoteModelIdType1
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0-rc.2.24474.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CCMS_Analysis_Quote.Models.IncurredBreakout", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("CostType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("InitialCallSubTotal")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("QuoteModelId")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<decimal>("Rate")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Techs")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("QuoteModelId");

                    b.ToTable("IncurredBreakouts");
                });

            modelBuilder.Entity("CCMS_Analysis_Quote.Models.ProposedBreakout", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("CostType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("InitialCallSubTotal")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("QuoteModelId")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<decimal>("Rate")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Techs")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("QuoteModelId");

                    b.ToTable("ProposedBreakouts");
                });

            modelBuilder.Entity("CCMS_Analysis_Quote.Models.QuoteModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("CityState")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsIncurredCost")
                        .HasColumnType("bit");

                    b.Property<string>("LOCATION")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SMG_CLIENT")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SMG_Vendor_PO")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ServiceRepName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StoreNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Vendor")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("QuoteModel");
                });

            modelBuilder.Entity("CCMS_Analysis_Quote.Models.IncurredBreakout", b =>
                {
                    b.HasOne("CCMS_Analysis_Quote.Models.QuoteModel", null)
                        .WithMany("incurredBreakouts")
                        .HasForeignKey("QuoteModelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CCMS_Analysis_Quote.Models.ProposedBreakout", b =>
                {
                    b.HasOne("CCMS_Analysis_Quote.Models.QuoteModel", null)
                        .WithMany("proposedBreakouts")
                        .HasForeignKey("QuoteModelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CCMS_Analysis_Quote.Models.QuoteModel", b =>
                {
                    b.Navigation("incurredBreakouts");

                    b.Navigation("proposedBreakouts");
                });
#pragma warning restore 612, 618
        }
    }
}
