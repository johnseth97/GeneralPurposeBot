﻿// <auto-generated />
using System;
using GeneralPurposeBot.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GeneralPurposeBot.Migrations
{
    [DbContext(typeof(BotDbContext))]
    [Migration("20200906004915_AssignableRoles")]
    partial class AssignableRoles
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("GeneralPurposeBot.Models.AssignableRole", b =>
                {
                    b.Property<ulong>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned");

                    b.Property<string>("RoleName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<ulong>("ServerId")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("RoleId");

                    b.ToTable("AssignableRoles");
                });

            modelBuilder.Entity("GeneralPurposeBot.Models.ServerModule", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<bool>("Disabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<ulong>("ServerId")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("Id");

                    b.ToTable("ServerModules");
                });

            modelBuilder.Entity("GeneralPurposeBot.Models.ServerProperties", b =>
                {
                    b.Property<ulong>("ServerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("LogChannelId")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("NsfwRoleId")
                        .HasColumnType("bigint unsigned");

                    b.Property<string>("Prefix")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("SimpleTempVc")
                        .HasColumnType("tinyint(1)");

                    b.Property<ulong>("SpamRoleId")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("TempVoiceCategoryId")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("TempVoiceCreateChannelId")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("ServerId");

                    b.ToTable("ServerProperties");
                });
#pragma warning restore 612, 618
        }
    }
}
