using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MyHostel_Admin.Models
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
        public virtual DbSet<HostelAmenity> HostelAmenities { get; set; } = null!;
        public virtual DbSet<HostelImage> HostelImages { get; set; } = null!;
        public virtual DbSet<Member> Members { get; set; } = null!;
        public virtual DbSet<Message> Messages { get; set; } = null!;
        public virtual DbSet<MessageImage> MessageImages { get; set; } = null!;
        public virtual DbSet<NearbyFacility> NearbyFacilities { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<Participant> Participants { get; set; } = null!;
        public virtual DbSet<Province> Provinces { get; set; } = null!;
        public virtual DbSet<Resident> Residents { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Room> Rooms { get; set; } = null!;
        public virtual DbSet<Transaction> Transactions { get; set; } = null!;
        public virtual DbSet<Ward> Wards { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
  .SetBasePath(Directory.GetCurrentDirectory())
  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();

            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DBContext"));
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

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
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AmenitiyName)
                    .HasMaxLength(50)
                    .HasColumnName("amenitiy_name");

                entity.Property(e => e.Checked).HasColumnName("checked");

                entity.Property(e => e.Icon)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("icon");
            });

            modelBuilder.Entity<Chat>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AvatarUrl)
                    .HasMaxLength(1000)
                    .HasColumnName("avatarUrl");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.HostelId).HasColumnName("hostel_id");

                entity.Property(e => e.IsGroup).HasColumnName("is_group");

                entity.Property(e => e.LastMsgAt)
                    .HasColumnType("datetime")
                    .HasColumnName("last_msg_at");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.HasOne(d => d.Hostel)
                    .WithMany(p => p.Chats)
                    .HasForeignKey(d => d.HostelId)
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
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.UtilityName)
                    .HasMaxLength(50)
                    .HasColumnName("utility_name");
            });

            modelBuilder.Entity<Hostel>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Capacity)
                    .HasMaxLength(10)
                    .HasColumnName("capacity")
                    .IsFixedLength();

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.Description)
                    .HasMaxLength(1000)
                    .HasColumnName("description");

                entity.Property(e => e.DetailLocation)
                    .HasMaxLength(255)
                    .HasColumnName("detail_Location");

                entity.Property(e => e.Electricity)
                    .HasColumnType("money")
                    .HasColumnName("electricity");

                entity.Property(e => e.GoogleLocationLat)
                    .HasMaxLength(50)
                    .HasColumnName("google_location_lat");

                entity.Property(e => e.GoogleLocationLnd)
                    .HasMaxLength(50)
                    .HasColumnName("google_location_lnd");

                entity.Property(e => e.Internet)
                    .HasColumnType("money")
                    .HasColumnName("internet");

                entity.Property(e => e.LandlordId).HasColumnName("landlord_id");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.OtherCost)
                    .HasMaxLength(1000)
                    .HasColumnName("other_cost");

                entity.Property(e => e.Phone)
                    .HasMaxLength(10)
                    .HasColumnName("phone")
                    .IsFixedLength();

                entity.Property(e => e.Price)
                    .HasColumnType("money")
                    .HasColumnName("price");

                entity.Property(e => e.RoomArea)
                    .HasMaxLength(10)
                    .HasColumnName("room_Area")
                    .IsFixedLength();

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.WardsCode)
                    .HasMaxLength(20)
                    .HasColumnName("wards_code");

                entity.Property(e => e.Water)
                    .HasColumnType("money")
                    .HasColumnName("water");

                entity.HasOne(d => d.Landlord)
                    .WithMany(p => p.Hostels)
                    .HasForeignKey(d => d.LandlordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Hostels_Members");

                entity.HasOne(d => d.WardsCodeNavigation)
                    .WithMany(p => p.Hostels)
                    .HasForeignKey(d => d.WardsCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Hostels_wards");
            });

            modelBuilder.Entity<HostelAmenity>(entity =>
            {
                entity.ToTable("Hostel_Amenities");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AmenitiesId).HasColumnName("amenities_id");

                entity.Property(e => e.HostedId).HasColumnName("hosted_id");

                entity.HasOne(d => d.Amenities)
                    .WithMany(p => p.HostelAmenities)
                    .HasForeignKey(d => d.AmenitiesId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Hostel_Amenities_Amenities");

                entity.HasOne(d => d.Hosted)
                    .WithMany(p => p.HostelAmenities)
                    .HasForeignKey(d => d.HostedId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Hostel_Amenities_Hostels");
            });

            modelBuilder.Entity<HostelImage>(entity =>
            {
                entity.ToTable("Hostel_Images");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.HostelId).HasColumnName("hostel_id");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(1000)
                    .HasColumnName("imageURL");

                entity.HasOne(d => d.Hostel)
                    .WithMany(p => p.HostelImages)
                    .HasForeignKey(d => d.HostelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Hostel_Images_Hostels");
            });

            modelBuilder.Entity<Member>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Avatar)
                    .HasMaxLength(255)
                    .HasColumnName("avatar");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.FacebookId)
                    .HasMaxLength(255)
                    .HasColumnName("facebook_id");

                entity.Property(e => e.FcmToken)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("fcm_token");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(255)
                    .HasColumnName("first_name");

                entity.Property(e => e.GoogleId)
                    .HasMaxLength(255)
                    .HasColumnName("google_id");

                entity.Property(e => e.InviteCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("inviteCode")
                    .IsFixedLength();

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
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AnonymousFlg).HasColumnName("anonymous_flg");

                entity.Property(e => e.ChatId).HasColumnName("chat_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.MsgText).HasColumnName("msg_text");

                entity.Property(e => e.ParentMsgId).HasColumnName("parent_msg_id");

                entity.Property(e => e.SenderId).HasColumnName("sender_id");

                entity.Property(e => e.Status).HasColumnName("status");

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

            modelBuilder.Entity<MessageImage>(entity =>
            {
                entity.ToTable("Message_Images");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(1000)
                    .HasColumnName("imageURL");

                entity.Property(e => e.MessageId).HasColumnName("message_id");

                entity.HasOne(d => d.Message)
                    .WithMany(p => p.MessageImages)
                    .HasForeignKey(d => d.MessageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Message_Images_Messages");
            });

            modelBuilder.Entity<NearbyFacility>(entity =>
            {
                entity.ToTable("Nearby_Facilities");

                entity.Property(e => e.Id).HasColumnName("id");

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

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("create_at");

                entity.Property(e => e.Message)
                    .HasMaxLength(1000)
                    .HasColumnName("message");

                entity.Property(e => e.SendAt)
                    .HasColumnType("datetime")
                    .HasColumnName("send_at");

                entity.Property(e => e.SendAtHour).HasColumnName("send_at_hour");

                entity.Property(e => e.SendTo).HasColumnName("send_to");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.HasOne(d => d.SendToNavigation)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.SendTo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Notifications_Members");
            });

            modelBuilder.Entity<Participant>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AnonymousTime)
                    .HasColumnName("anonymous_time")
                    .HasDefaultValueSql("((3))");

                entity.Property(e => e.ChatId).HasColumnName("chat_id");

                entity.Property(e => e.JoinedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("joined_at");

                entity.Property(e => e.MemberId).HasColumnName("member_id");

                entity.Property(e => e.NickName)
                    .HasMaxLength(50)
                    .HasColumnName("nick_name");

                entity.Property(e => e.Role).HasColumnName("role");

                entity.Property(e => e.Status).HasColumnName("status");

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
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Comment)
                    .HasMaxLength(1000)
                    .HasColumnName("comment");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.HostelId).HasColumnName("hostel_id");

                entity.Property(e => e.IsAnonymousComment).HasColumnName("isAnonymousComment");

                entity.Property(e => e.LeftAt)
                    .HasColumnType("datetime")
                    .HasColumnName("left_at");

                entity.Property(e => e.MemberId).HasColumnName("member_id");

                entity.Property(e => e.Rate).HasColumnName("rate");

                entity.Property(e => e.RoomId).HasColumnName("room_id");

                entity.Property(e => e.Status).HasColumnName("status");

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
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.RoleName)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("role_name");
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.HostelId).HasColumnName("hostel_id");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.Price)
                    .HasColumnType("money")
                    .HasColumnName("price");

                entity.Property(e => e.RoomArea).HasColumnName("room_Area");

                entity.HasOne(d => d.Hostel)
                    .WithMany(p => p.Rooms)
                    .HasForeignKey(d => d.HostelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Rooms_Hostels");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AtTime)
                    .HasMaxLength(7)
                    .IsUnicode(false)
                    .HasColumnName("at_time");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.Electricity)
                    .HasColumnType("money")
                    .HasColumnName("electricity");

                entity.Property(e => e.Internet)
                    .HasColumnType("money")
                    .HasColumnName("internet");

                entity.Property(e => e.Other)
                    .HasMaxLength(1000)
                    .HasColumnName("other");

                entity.Property(e => e.PaidAmount)
                    .HasColumnType("money")
                    .HasColumnName("paid_amount");

                entity.Property(e => e.PaidAt)
                    .HasColumnType("datetime")
                    .HasColumnName("paid_at");

                entity.Property(e => e.Rent)
                    .HasColumnType("money")
                    .HasColumnName("rent");

                entity.Property(e => e.RoomId).HasColumnName("room_id");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Water)
                    .HasColumnType("money")
                    .HasColumnName("water");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.RoomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Transactions_Rooms");
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
