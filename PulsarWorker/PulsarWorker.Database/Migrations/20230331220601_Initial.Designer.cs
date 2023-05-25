﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PulsarWorker.Database.Context;

#nullable disable

namespace PulsarWorker.Database.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20230331220601_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("PulsarWorker.Data.Entities.PulsarMessageEntity", b =>
                {
                    b.Property<string>("MessageId")
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime>("ReceivedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)");

                    b.HasKey("MessageId");

                    b.ToTable("PulsarMessageEntity");
                });
#pragma warning restore 612, 618
        }
    }
}