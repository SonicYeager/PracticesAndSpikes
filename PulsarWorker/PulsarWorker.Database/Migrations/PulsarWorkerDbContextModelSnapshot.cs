﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PulsarWorker.Database.Context;

#nullable disable

namespace PulsarWorker.Database.Migrations
{
    [DbContext(typeof(PulsarWorkerDbContext))]
    partial class PulsarWorkerDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("PulsarWorker.Data.Entities.PulsarMessageEntity", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime>("ReceivedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("PulsarMessageEntity");
                });

            modelBuilder.Entity("PulsarWorker.Data.Entities.SettingsEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("SettingsEntity");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Key = "Pulsar Host",
                            UserId = 1,
                            Value = ""
                        });
                });

            modelBuilder.Entity("PulsarWorker.Data.Entities.UserEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("UserEntity");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "PulsarWorker",
                            Surname = "Test"
                        });
                });

            modelBuilder.Entity("PulsarWorker.Data.Entities.SettingsEntity", b =>
                {
                    b.HasOne("PulsarWorker.Data.Entities.UserEntity", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
