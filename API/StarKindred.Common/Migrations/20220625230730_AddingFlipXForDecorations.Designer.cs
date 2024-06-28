﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StarKindred.Common.Services;

#nullable disable

namespace StarKindred.Common.Migrations
{
    [DbContext(typeof(Db))]
    [Migration("20220625230730_AddingFlipXForDecorations")]
    partial class AddingFlipXForDecorations
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("StarKindred.Common.Entities.Db.Alliance", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTimeOffset>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid>("LeaderId")
                        .HasColumnType("char(36)");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("Level");

                    b.ToTable("Alliances");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.AllianceLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("ActivityType")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<Guid>("AllianceId")
                        .HasColumnType("char(36)");

                    b.Property<DateTimeOffset>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("AllianceId");

                    b.HasIndex("CreatedOn");

                    b.ToTable("AllianceLogs");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.Announcement", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTimeOffset>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedOn");

                    b.ToTable("Announcements");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.Building", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTimeOffset>("LastHarvestedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<int>("Position")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("Position");

                    b.HasIndex("UserId");

                    b.ToTable("Buildings");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.Decoration", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("Type");

                    b.HasIndex("UserId");

                    b.ToTable("Decorations");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.Giant", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("AllianceId")
                        .HasColumnType("char(36)");

                    b.Property<int>("Damage")
                        .HasColumnType("int");

                    b.Property<string>("Element")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<DateTimeOffset>("ExpiresOn")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Health")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("StartsOn")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("AllianceId")
                        .IsUnique();

                    b.ToTable("Giants");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.GiantContribution", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTimeOffset>("AttackDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Damage")
                        .HasColumnType("int");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("GiantContributions");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.Goodie", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("Location")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Goodies");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.Mission", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTimeOffset>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Missions");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.PersonalLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("ActivityType")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<DateTimeOffset>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("UserId");

                    b.ToTable("PersonalLogs");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.Relationship", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<int>("Minutes")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Relationships");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.Resource", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Resources");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.StatusEffect", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("Strength")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)");

                    b.Property<Guid>("VassalId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("VassalId");

                    b.ToTable("StatusEffects");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.TimedMission", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTimeOffset?>("CompletesOn")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTimeOffset>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Element")
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<int>("Location")
                        .HasColumnType("int");

                    b.Property<string>("Species")
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<DateTimeOffset?>("StartedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Treasure")
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Weapon")
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("TimedMissions");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.Town", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<bool>("CanDecorate")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTimeOffset>("LastGoodie")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<DateTimeOffset>("NextRumor")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Towns");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.TownDecoration", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<bool>("FlipX")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("Scale")
                        .HasColumnType("int");

                    b.Property<Guid>("TownId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<float>("X")
                        .HasColumnType("float");

                    b.Property<float>("Y")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("TownId");

                    b.ToTable("TownDecorations");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.Treasure", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Treasures");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Avatar")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTimeOffset>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<DateTimeOffset>("LastAttackedGiant")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTimeOffset?>("LastMissionCompletedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<string>("Passphrase")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("LastMissionCompletedOn");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.UserAlliance", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("AllianceId")
                        .HasColumnType("char(36)");

                    b.Property<DateTimeOffset>("JoinedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("AllianceId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("UserAlliances");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.UserSeekingAlliance", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(14)
                        .HasColumnType("varchar(14)");

                    b.Property<DateTimeOffset>("CodeGeneratedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTimeOffset?>("PublicSince")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.HasIndex("PublicSince");

                    b.HasIndex("UserId");

                    b.ToTable("UsersSeekingAlliances");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.UserSession", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTimeOffset>("ExpiresOn")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserSessions");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.UserVassalTag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("Title");

                    b.HasIndex("UserId", "Title")
                        .IsUnique();

                    b.ToTable("UserVassalTags");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.Vassal", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTimeOffset>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Element")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<Guid?>("MissionId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("varchar(30)");

                    b.Property<string>("Nature")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<string>("Portrait")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<string>("Sign")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<string>("Species")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<Guid?>("TimedMissionId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("WeaponId")
                        .HasColumnType("char(36)");

                    b.Property<int>("Willpower")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Level");

                    b.HasIndex("MissionId");

                    b.HasIndex("Name");

                    b.HasIndex("TimedMissionId");

                    b.HasIndex("UserId");

                    b.HasIndex("WeaponId")
                        .IsUnique();

                    b.ToTable("Vassals");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.Weapon", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("Durability")
                        .HasColumnType("int");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<int>("MaxDurability")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<string>("PrimaryBonus")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)");

                    b.Property<string>("SecondaryBonus")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("Level");

                    b.HasIndex("UserId");

                    b.ToTable("Weapons");
                });

            modelBuilder.Entity("RelationshipVassal", b =>
                {
                    b.Property<Guid>("RelationshipsId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("VassalsId")
                        .HasColumnType("char(36)");

                    b.HasKey("RelationshipsId", "VassalsId");

                    b.HasIndex("VassalsId");

                    b.ToTable("RelationshipVassal");
                });

            modelBuilder.Entity("UserVassalTagVassal", b =>
                {
                    b.Property<Guid>("TagsId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("VassalsId")
                        .HasColumnType("char(36)");

                    b.HasKey("TagsId", "VassalsId");

                    b.HasIndex("VassalsId");

                    b.ToTable("UserVassalTagVassal");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.AllianceLog", b =>
                {
                    b.HasOne("StarKindred.Common.Entities.Db.Alliance", "Alliance")
                        .WithMany("Logs")
                        .HasForeignKey("AllianceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Alliance");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.Building", b =>
                {
                    b.HasOne("StarKindred.Common.Entities.Db.User", "User")
                        .WithMany("Buildings")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.Decoration", b =>
                {
                    b.HasOne("StarKindred.Common.Entities.Db.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.Giant", b =>
                {
                    b.HasOne("StarKindred.Common.Entities.Db.Alliance", "Alliance")
                        .WithOne("Giant")
                        .HasForeignKey("StarKindred.Common.Entities.Db.Giant", "AllianceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Alliance");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.GiantContribution", b =>
                {
                    b.HasOne("StarKindred.Common.Entities.Db.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.Goodie", b =>
                {
                    b.HasOne("StarKindred.Common.Entities.Db.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.Mission", b =>
                {
                    b.HasOne("StarKindred.Common.Entities.Db.User", "User")
                        .WithMany("Missions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.PersonalLog", b =>
                {
                    b.HasOne("StarKindred.Common.Entities.Db.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.Resource", b =>
                {
                    b.HasOne("StarKindred.Common.Entities.Db.User", "User")
                        .WithMany("Resources")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.StatusEffect", b =>
                {
                    b.HasOne("StarKindred.Common.Entities.Db.Vassal", "Vassal")
                        .WithMany("StatusEffects")
                        .HasForeignKey("VassalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Vassal");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.TimedMission", b =>
                {
                    b.HasOne("StarKindred.Common.Entities.Db.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.Town", b =>
                {
                    b.HasOne("StarKindred.Common.Entities.Db.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.TownDecoration", b =>
                {
                    b.HasOne("StarKindred.Common.Entities.Db.Town", "Town")
                        .WithMany("Decorations")
                        .HasForeignKey("TownId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Town");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.Treasure", b =>
                {
                    b.HasOne("StarKindred.Common.Entities.Db.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.User", b =>
                {
                    b.OwnsOne("StarKindred.Common.Entities.HSL", "Color", b1 =>
                        {
                            b1.Property<Guid>("UserId")
                                .HasColumnType("char(36)");

                            b1.Property<int>("Hue")
                                .HasColumnType("int");

                            b1.Property<int>("Luminosity")
                                .HasColumnType("int");

                            b1.Property<int>("Saturation")
                                .HasColumnType("int");

                            b1.HasKey("UserId");

                            b1.ToTable("Users");

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.Navigation("Color")
                        .IsRequired();
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.UserAlliance", b =>
                {
                    b.HasOne("StarKindred.Common.Entities.Db.Alliance", "Alliance")
                        .WithMany("Members")
                        .HasForeignKey("AllianceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StarKindred.Common.Entities.Db.User", "User")
                        .WithOne("UserAlliance")
                        .HasForeignKey("StarKindred.Common.Entities.Db.UserAlliance", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Alliance");

                    b.Navigation("User");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.UserSeekingAlliance", b =>
                {
                    b.HasOne("StarKindred.Common.Entities.Db.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.UserSession", b =>
                {
                    b.HasOne("StarKindred.Common.Entities.Db.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.UserVassalTag", b =>
                {
                    b.HasOne("StarKindred.Common.Entities.Db.User", "User")
                        .WithMany("VassalTags")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.Vassal", b =>
                {
                    b.HasOne("StarKindred.Common.Entities.Db.Mission", "Mission")
                        .WithMany("Vassals")
                        .HasForeignKey("MissionId");

                    b.HasOne("StarKindred.Common.Entities.Db.TimedMission", "TimedMission")
                        .WithMany("Vassals")
                        .HasForeignKey("TimedMissionId");

                    b.HasOne("StarKindred.Common.Entities.Db.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StarKindred.Common.Entities.Db.Weapon", "Weapon")
                        .WithOne("Vassal")
                        .HasForeignKey("StarKindred.Common.Entities.Db.Vassal", "WeaponId");

                    b.Navigation("Mission");

                    b.Navigation("TimedMission");

                    b.Navigation("User");

                    b.Navigation("Weapon");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.Weapon", b =>
                {
                    b.HasOne("StarKindred.Common.Entities.Db.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("RelationshipVassal", b =>
                {
                    b.HasOne("StarKindred.Common.Entities.Db.Relationship", null)
                        .WithMany()
                        .HasForeignKey("RelationshipsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StarKindred.Common.Entities.Db.Vassal", null)
                        .WithMany()
                        .HasForeignKey("VassalsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("UserVassalTagVassal", b =>
                {
                    b.HasOne("StarKindred.Common.Entities.Db.UserVassalTag", null)
                        .WithMany()
                        .HasForeignKey("TagsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StarKindred.Common.Entities.Db.Vassal", null)
                        .WithMany()
                        .HasForeignKey("VassalsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.Alliance", b =>
                {
                    b.Navigation("Giant");

                    b.Navigation("Logs");

                    b.Navigation("Members");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.Mission", b =>
                {
                    b.Navigation("Vassals");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.TimedMission", b =>
                {
                    b.Navigation("Vassals");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.Town", b =>
                {
                    b.Navigation("Decorations");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.User", b =>
                {
                    b.Navigation("Buildings");

                    b.Navigation("Missions");

                    b.Navigation("Resources");

                    b.Navigation("UserAlliance");

                    b.Navigation("VassalTags");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.Vassal", b =>
                {
                    b.Navigation("StatusEffects");
                });

            modelBuilder.Entity("StarKindred.Common.Entities.Db.Weapon", b =>
                {
                    b.Navigation("Vassal");
                });
#pragma warning restore 612, 618
        }
    }
}
