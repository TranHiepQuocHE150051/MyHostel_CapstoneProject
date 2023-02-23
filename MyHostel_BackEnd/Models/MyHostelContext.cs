using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MyHostel_BackEnd.Models
{
    public partial class MyHostelContext : DbContext
    {
        public MyHostelContext()
        {
        }

        public MyHostelContext(DbContextOptions<MyHostelContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Admin> Admins { get; set; } = null!;
        public virtual DbSet<AdministrativeRegion> AdministrativeRegions { get; set; } = null!;
        public virtual DbSet<AdministrativeUnit> AdministrativeUnits { get; set; } = null!;
        public virtual DbSet<Amenity> Amenities { get; set; } = null!;
        public virtual DbSet<Chat> Chats { get; set; } = null!;
        public virtual DbSet<District> Districts { get; set; } = null!;
        public virtual DbSet<Facility> Facilities { get; set; } = null!;
        public virtual DbSet<Hostel> Hostels { get; set; } = null!;
        public virtual DbSet<Member> Members { get; set; } = null!;
        public virtual DbSet<Message> Messages { get; set; } = null!;
        public virtual DbSet<NearbyFacility> NearbyFacilities { get; set; } = null!;
        public virtual DbSet<Participant> Participants { get; set; } = null!;
        public virtual DbSet<Province> Provinces { get; set; } = null!;
        public virtual DbSet<Resident> Residents { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Room> Rooms { get; set; } = null!;
        public virtual DbSet<Ward> Wards { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server =(local); database = MyHostel;uid=sa;pwd=quoc1910");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.AccountName)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("account_name");

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Admins)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Admins_Roles");
            });

            modelBuilder.Entity<AdministrativeRegion>(entity =>
            {
                entity.ToTable("administrative_regions");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.CodeName)
                    .HasMaxLength(255)
                    .HasColumnName("code_name");

                entity.Property(e => e.CodeNameEn)
                    .HasMaxLength(255)
                    .HasColumnName("code_name_en");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.NameEn)
                    .HasMaxLength(255)
                    .HasColumnName("name_en");
            });

            modelBuilder.Entity<AdministrativeUnit>(entity =>
            {
                entity.ToTable("administrative_units");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.CodeName)
                    .HasMaxLength(255)
                    .HasColumnName("code_name");

                entity.Property(e => e.CodeNameEn)
                    .HasMaxLength(255)
                    .HasColumnName("code_name_en");

                entity.Property(e => e.FullName)
                    .HasMaxLength(255)
                    .HasColumnName("full_name");

                entity.Property(e => e.FullNameEn)
                    .HasMaxLength(255)
                    .HasColumnName("full_name_en");

                entity.Property(e => e.ShortName)
                    .HasMaxLength(255)
                    .HasColumnName("short_name");

                entity.Property(e => e.ShortNameEn)
                    .HasMaxLength(255)
                    .HasColumnName("short_name_en");
            });

            modelBuilder.Entity<Amenity>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.AmenitiyName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("amenitiy_name");
            });

            modelBuilder.Entity<Chat>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .IsRowVersion()
                    .IsConcurrencyToken()
                    .HasColumnName("created_at");

                entity.Property(e => e.HostelId).HasColumnName("hostel_id");

                entity.Property(e => e.LastMsgAt)
                    .HasColumnType("datetime")
                    .HasColumnName("last_msg_at");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.HasOne(d => d.Hostel)
                    .WithMany(p => p.Chats)
                    .HasForeignKey(d => d.HostelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Chats_Hostels");
            });

            modelBuilder.Entity<District>(entity =>
            {
                entity.HasKey(e => e.Code)
                    .HasName("districts_pkey");

                entity.ToTable("districts");

                entity.Property(e => e.Code)
                    .HasMaxLength(20)
                    .HasColumnName("code");

                entity.Property(e => e.AdministrativeUnitId).HasColumnName("administrative_unit_id");

                entity.Property(e => e.CodeName)
                    .HasMaxLength(255)
                    .HasColumnName("code_name");

                entity.Property(e => e.FullName)
                    .HasMaxLength(255)
                    .HasColumnName("full_name");

                entity.Property(e => e.FullNameEn)
                    .HasMaxLength(255)
                    .HasColumnName("full_name_en");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.NameEn)
                    .HasMaxLength(255)
                    .HasColumnName("name_en");

                entity.Property(e => e.ProvinceCode)
                    .HasMaxLength(20)
                    .HasColumnName("province_code");

                entity.HasOne(d => d.AdministrativeUnit)
                    .WithMany(p => p.Districts)
                    .HasForeignKey(d => d.AdministrativeUnitId)
                    .HasConstraintName("districts_administrative_unit_id_fkey");

                entity.HasOne(d => d.ProvinceCodeNavigation)
                    .WithMany(p => p.Districts)
                    .HasForeignKey(d => d.ProvinceCode)
                    .HasConstraintName("districts_province_code_fkey");
            });

            modelBuilder.Entity<Facility>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.UtilityName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("utility_name");
            });

            modelBuilder.Entity<Hostel>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .IsRowVersion()
                    .IsConcurrencyToken()
                    .HasColumnName("created_at");

                entity.Property(e => e.GoogleLocationLat)
                    .HasMaxLength(50)
                    .HasColumnName("google_location_lat");

                entity.Property(e => e.GoogleLocationLnd)
                    .HasMaxLength(50)
                    .HasColumnName("google_location_lnd");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.WardsCode)
                    .HasMaxLength(20)
                    .HasColumnName("wards_code");

                entity.HasOne(d => d.WardsCodeNavigation)
                    .WithMany(p => p.Hostels)
                    .HasForeignKey(d => d.WardsCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Hostels_wards");

                entity.HasMany(d => d.Amenities)
                    .WithMany(p => p.Hosteds)
                    .UsingEntity<Dictionary<string, object>>(
                        "HostelAmenity",
                        l => l.HasOne<Amenity>().WithMany().HasForeignKey("AmenitiesId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Hostel_Amenities_Amenities"),
                        r => r.HasOne<Hostel>().WithMany().HasForeignKey("HostedId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Hostel_Amenities_Hostels"),
                        j =>
                        {
                            j.HasKey("HostedId", "AmenitiesId");

                            j.ToTable("Hostel_Amenities");

                            j.IndexerProperty<int>("HostedId").HasColumnName("hosted_id");

                            j.IndexerProperty<int>("AmenitiesId").HasColumnName("amenities_id");
                        });
            });

            modelBuilder.Entity<Member>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Avatar)
                    .HasMaxLength(255)
                    .HasColumnName("avatar");

                entity.Property(e => e.CreatedAt)
                    .IsRowVersion()
                    .IsConcurrencyToken()
                    .HasColumnName("created_at");

                entity.Property(e => e.FacebookId)
                    .HasMaxLength(255)
                    .HasColumnName("facebook_id");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(255)
                    .HasColumnName("first_name");

                entity.Property(e => e.GoogleId)
                    .HasMaxLength(255)
                    .HasColumnName("google_id");

                entity.Property(e => e.LastName)
                    .HasMaxLength(255)
                    .HasColumnName("last_name");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Members)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Members_Roles");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.AnonymousFlg).HasColumnName("anonymous_flg");

                entity.Property(e => e.ChatId).HasColumnName("chat_id");

                entity.Property(e => e.CreatedAt)
                    .IsRowVersion()
                    .IsConcurrencyToken()
                    .HasColumnName("created_at");

                entity.Property(e => e.MsgText).HasColumnName("msg_text");

                entity.Property(e => e.ParentMsgId).HasColumnName("parent_msg_id");

                entity.Property(e => e.SenderId).HasColumnName("sender_id");

                entity.HasOne(d => d.Chat)
                    .WithMany(p => p.Messages)
                    .HasForeignKey(d => d.ChatId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Messages_Chats");

                entity.HasOne(d => d.Sender)
                    .WithMany(p => p.Messages)
                    .HasForeignKey(d => d.SenderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Messages_Members");
            });

            modelBuilder.Entity<NearbyFacility>(entity =>
            {
                entity.ToTable("Nearby_Facilities");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Distance)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("distance");

                entity.Property(e => e.Duration)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("duration");

                entity.Property(e => e.HostelId).HasColumnName("hostel_id");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.UltilityId).HasColumnName("ultility_id");

                entity.HasOne(d => d.Hostel)
                    .WithMany(p => p.NearbyFacilities)
                    .HasForeignKey(d => d.HostelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Nearby_Facilities_Hostels");

                entity.HasOne(d => d.Ultility)
                    .WithMany(p => p.NearbyFacilities)
                    .HasForeignKey(d => d.UltilityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Nearby_Facilities_Facilities");
            });

            modelBuilder.Entity<Participant>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.AnonymousTime)
                    .HasColumnName("anonymous_time")
                    .HasDefaultValueSql("((3))");

                entity.Property(e => e.ChatId).HasColumnName("chat_id");

                entity.Property(e => e.JoinedAt)
                    .IsRowVersion()
                    .IsConcurrencyToken()
                    .HasColumnName("joined_at");

                entity.Property(e => e.MemberId).HasColumnName("member_id");

                entity.Property(e => e.Role).HasColumnName("role");

                entity.HasOne(d => d.Chat)
                    .WithMany(p => p.Participants)
                    .HasForeignKey(d => d.ChatId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Participants_Chats");

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.Participants)
                    .HasForeignKey(d => d.MemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Participants_Members");
            });

            modelBuilder.Entity<Province>(entity =>
            {
                entity.HasKey(e => e.Code)
                    .HasName("provinces_pkey");

                entity.ToTable("provinces");

                entity.Property(e => e.Code)
                    .HasMaxLength(20)
                    .HasColumnName("code");

                entity.Property(e => e.AdministrativeRegionId).HasColumnName("administrative_region_id");

                entity.Property(e => e.AdministrativeUnitId).HasColumnName("administrative_unit_id");

                entity.Property(e => e.CodeName)
                    .HasMaxLength(255)
                    .HasColumnName("code_name");

                entity.Property(e => e.FullName)
                    .HasMaxLength(255)
                    .HasColumnName("full_name");

                entity.Property(e => e.FullNameEn)
                    .HasMaxLength(255)
                    .HasColumnName("full_name_en");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.NameEn)
                    .HasMaxLength(255)
                    .HasColumnName("name_en");

                entity.HasOne(d => d.AdministrativeRegion)
                    .WithMany(p => p.Provinces)
                    .HasForeignKey(d => d.AdministrativeRegionId)
                    .HasConstraintName("provinces_administrative_region_id_fkey");

                entity.HasOne(d => d.AdministrativeUnit)
                    .WithMany(p => p.Provinces)
                    .HasForeignKey(d => d.AdministrativeUnitId)
                    .HasConstraintName("provinces_administrative_unit_id_fkey");
            });

            modelBuilder.Entity<Resident>(entity =>
            {
                entity.HasKey(e => new { e.HostelId, e.MemberId, e.RoomId });

                entity.Property(e => e.HostelId).HasColumnName("hostel_id");

                entity.Property(e => e.MemberId).HasColumnName("member_id");

                entity.Property(e => e.RoomId).HasColumnName("room_id");

                entity.Property(e => e.ActiveFlg).HasColumnName("active_flg");

                entity.Property(e => e.Comment)
                    .HasMaxLength(1000)
                    .HasColumnName("comment");

                entity.Property(e => e.CreatedAt)
                    .IsRowVersion()
                    .IsConcurrencyToken()
                    .HasColumnName("created_at");

                entity.Property(e => e.Rate).HasColumnName("rate");

                entity.HasOne(d => d.Hostel)
                    .WithMany(p => p.Residents)
                    .HasForeignKey(d => d.HostelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Residents_Hostels");

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.Residents)
                    .HasForeignKey(d => d.MemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Residents_Members");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.Residents)
                    .HasForeignKey(d => d.RoomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Residents_Rooms");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.RoleName)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("role_name");
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.HostelId).HasColumnName("hostel_id");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Ward>(entity =>
            {
                entity.HasKey(e => e.Code)
                    .HasName("wards_pkey");

                entity.ToTable("wards");

                entity.Property(e => e.Code)
                    .HasMaxLength(20)
                    .HasColumnName("code");

                entity.Property(e => e.AdministrativeUnitId).HasColumnName("administrative_unit_id");

                entity.Property(e => e.CodeName)
                    .HasMaxLength(255)
                    .HasColumnName("code_name");

                entity.Property(e => e.DistrictCode)
                    .HasMaxLength(20)
                    .HasColumnName("district_code");

                entity.Property(e => e.FullName)
                    .HasMaxLength(255)
                    .HasColumnName("full_name");

                entity.Property(e => e.FullNameEn)
                    .HasMaxLength(255)
                    .HasColumnName("full_name_en");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.NameEn)
                    .HasMaxLength(255)
                    .HasColumnName("name_en");

                entity.HasOne(d => d.AdministrativeUnit)
                    .WithMany(p => p.Wards)
                    .HasForeignKey(d => d.AdministrativeUnitId)
                    .HasConstraintName("wards_administrative_unit_id_fkey");

                entity.HasOne(d => d.DistrictCodeNavigation)
                    .WithMany(p => p.Wards)
                    .HasForeignKey(d => d.DistrictCode)
                    .HasConstraintName("wards_district_code_fkey");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
