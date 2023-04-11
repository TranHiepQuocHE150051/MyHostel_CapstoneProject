USE [master]
GO
/****** Object:  Database [MyHostel]    Script Date: 11/04/2023 3:51:16 CH ******/
CREATE DATABASE [MyHostel]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'MyHostel', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\MyHostel.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'MyHostel_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\MyHostel_log.ldf' , SIZE = 73728KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [MyHostel] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [MyHostel].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [MyHostel] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [MyHostel] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [MyHostel] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [MyHostel] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [MyHostel] SET ARITHABORT OFF 
GO
ALTER DATABASE [MyHostel] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [MyHostel] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [MyHostel] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [MyHostel] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [MyHostel] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [MyHostel] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [MyHostel] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [MyHostel] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [MyHostel] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [MyHostel] SET  ENABLE_BROKER 
GO
ALTER DATABASE [MyHostel] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [MyHostel] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [MyHostel] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [MyHostel] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [MyHostel] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [MyHostel] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [MyHostel] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [MyHostel] SET RECOVERY FULL 
GO
ALTER DATABASE [MyHostel] SET  MULTI_USER 
GO
ALTER DATABASE [MyHostel] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [MyHostel] SET DB_CHAINING OFF 
GO
ALTER DATABASE [MyHostel] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [MyHostel] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [MyHostel] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [MyHostel] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'MyHostel', N'ON'
GO
ALTER DATABASE [MyHostel] SET QUERY_STORE = OFF
GO
USE [MyHostel]
GO
/****** Object:  Table [dbo].[administrative_regions]    Script Date: 11/04/2023 3:51:17 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[administrative_regions](
	[id] [int] NOT NULL,
	[name] [nvarchar](255) NOT NULL,
	[name_en] [nvarchar](255) NOT NULL,
	[code_name] [nvarchar](255) NULL,
	[code_name_en] [nvarchar](255) NULL,
 CONSTRAINT [administrative_regions_pkey] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[administrative_units]    Script Date: 11/04/2023 3:51:17 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[administrative_units](
	[id] [int] NOT NULL,
	[full_name] [nvarchar](255) NULL,
	[full_name_en] [nvarchar](255) NULL,
	[short_name] [nvarchar](255) NULL,
	[short_name_en] [nvarchar](255) NULL,
	[code_name] [nvarchar](255) NULL,
	[code_name_en] [nvarchar](255) NULL,
 CONSTRAINT [administrative_units_pkey] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Admins]    Script Date: 11/04/2023 3:51:17 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Admins](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[account_name] [varchar](25) NOT NULL,
	[password] [varchar](50) NOT NULL,
	[role_id] [int] NOT NULL,
 CONSTRAINT [PK_Admins] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Amenities]    Script Date: 11/04/2023 3:51:17 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Amenities](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[amenitiy_name] [nvarchar](50) NOT NULL,
	[icon] [varchar](50) NULL,
 CONSTRAINT [PK_Amenities] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Chats]    Script Date: 11/04/2023 3:51:17 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Chats](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[hostel_id] [int] NOT NULL,
	[name] [varchar](255) NOT NULL,
	[created_at] [datetime] NOT NULL,
	[last_msg_at] [datetime] NOT NULL,
 CONSTRAINT [PK_Chats] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[districts]    Script Date: 11/04/2023 3:51:17 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[districts](
	[code] [nvarchar](20) NOT NULL,
	[name] [nvarchar](255) NOT NULL,
	[name_en] [nvarchar](255) NULL,
	[full_name] [nvarchar](255) NULL,
	[full_name_en] [nvarchar](255) NULL,
	[code_name] [nvarchar](255) NULL,
	[province_code] [nvarchar](20) NULL,
	[administrative_unit_id] [int] NULL,
 CONSTRAINT [districts_pkey] PRIMARY KEY CLUSTERED 
(
	[code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Facilities]    Script Date: 11/04/2023 3:51:17 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Facilities](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[utility_name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Facilities] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Hostel_Amenities]    Script Date: 11/04/2023 3:51:17 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Hostel_Amenities](
	[hosted_id] [int] NOT NULL,
	[amenities_id] [int] NOT NULL,
	[id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_Hostel_Amenities_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Hostel_Images]    Script Date: 11/04/2023 3:51:17 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Hostel_Images](
	[hostel_id] [int] NOT NULL,
	[imageURL] [nvarchar](1000) NOT NULL,
	[id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_Hostel_Images] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Hostels]    Script Date: 11/04/2023 3:51:17 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Hostels](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](255) NOT NULL,
	[google_location_lat] [nvarchar](50) NULL,
	[google_location_lnd] [nvarchar](50) NULL,
	[created_at] [datetime] NOT NULL,
	[wards_code] [nvarchar](20) NOT NULL,
	[price] [money] NOT NULL,
	[capacity] [nchar](10) NOT NULL,
	[phone] [nchar](10) NOT NULL,
	[landlord_id] [int] NOT NULL,
	[description] [nvarchar](1000) NOT NULL,
	[room_Area] [nchar](10) NOT NULL,
	[detail_Location] [nvarchar](255) NOT NULL,
	[electricity] [money] NOT NULL,
	[water] [money] NOT NULL,
	[internet] [money] NOT NULL,
	[status] [int] NOT NULL,
	[other_cost] [nvarchar](1000) NULL,
 CONSTRAINT [PK_Hostels] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Members]    Script Date: 11/04/2023 3:51:17 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Members](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[google_id] [nvarchar](255) NULL,
	[facebook_id] [nvarchar](255) NULL,
	[first_name] [nvarchar](255) NOT NULL,
	[last_name] [nvarchar](255) NOT NULL,
	[avatar] [nvarchar](255) NOT NULL,
	[created_at] [datetime] NOT NULL,
	[role_id] [int] NOT NULL,
	[fcm_token] [varchar](255) NOT NULL,
	[inviteCode] [char](20) NULL,
 CONSTRAINT [PK_Members] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Messages]    Script Date: 11/04/2023 3:51:17 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Messages](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[chat_id] [int] NOT NULL,
	[sender_id] [int] NOT NULL,
	[msg_text] [nvarchar](max) NOT NULL,
	[parent_msg_id] [int] NOT NULL,
	[created_at] [datetime] NOT NULL,
	[anonymous_flg] [bit] NOT NULL,
 CONSTRAINT [PK_Messages] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Nearby_Facilities]    Script Date: 11/04/2023 3:51:17 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Nearby_Facilities](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ultility_id] [int] NOT NULL,
	[hostel_id] [int] NOT NULL,
	[distance] [decimal](18, 0) NOT NULL,
	[duration] [decimal](18, 0) NOT NULL,
	[name] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_Nearby_Facilities] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Notifications]    Script Date: 11/04/2023 3:51:17 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Notifications](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[send_to] [int] NOT NULL,
	[message] [nvarchar](1000) NULL,
	[send_at] [datetime] NOT NULL,
	[type] [smallint] NULL,
	[send_at_hour] [int] NULL,
	[create_at] [datetime] NULL,
 CONSTRAINT [PK_Notifications] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Participants]    Script Date: 11/04/2023 3:51:17 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Participants](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[chat_id] [int] NOT NULL,
	[member_id] [int] NOT NULL,
	[joined_at] [datetime] NOT NULL,
	[role] [tinyint] NOT NULL,
	[anonymous_time] [int] NOT NULL,
 CONSTRAINT [PK_Participants] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[provinces]    Script Date: 11/04/2023 3:51:17 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[provinces](
	[code] [nvarchar](20) NOT NULL,
	[name] [nvarchar](255) NOT NULL,
	[name_en] [nvarchar](255) NULL,
	[full_name] [nvarchar](255) NOT NULL,
	[full_name_en] [nvarchar](255) NULL,
	[code_name] [nvarchar](255) NULL,
	[administrative_unit_id] [int] NULL,
	[administrative_region_id] [int] NULL,
 CONSTRAINT [provinces_pkey] PRIMARY KEY CLUSTERED 
(
	[code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Residents]    Script Date: 11/04/2023 3:51:17 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Residents](
	[hostel_id] [int] NOT NULL,
	[member_id] [int] NOT NULL,
	[room_id] [int] NOT NULL,
	[status] [tinyint] NOT NULL,
	[rate] [tinyint] NOT NULL,
	[comment] [nvarchar](1000) NOT NULL,
	[created_at] [datetime] NOT NULL,
	[id] [int] IDENTITY(1,1) NOT NULL,
	[left_at] [datetime] NULL,
 CONSTRAINT [PK_Residents] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 11/04/2023 3:51:17 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[role_name] [varchar](25) NOT NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Rooms]    Script Date: 11/04/2023 3:51:17 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Rooms](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[hostel_id] [int] NOT NULL,
	[name] [nvarchar](255) NOT NULL,
	[room_Area] [int] NULL,
	[price] [money] NULL,
 CONSTRAINT [PK_Rooms] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Transactions]    Script Date: 11/04/2023 3:51:17 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Transactions](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[room_id] [int] NOT NULL,
	[rent] [money] NULL,
	[electricity] [money] NULL,
	[water] [money] NULL,
	[internet] [money] NULL,
	[other] [nvarchar](1000) NULL,
	[paid_at] [datetime] NULL,
	[created_at] [datetime] NULL,
	[at_time] [varchar](7) NULL,
 CONSTRAINT [PK_Transactions] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[wards]    Script Date: 11/04/2023 3:51:17 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[wards](
	[code] [nvarchar](20) NOT NULL,
	[name] [nvarchar](255) NOT NULL,
	[name_en] [nvarchar](255) NULL,
	[full_name] [nvarchar](255) NULL,
	[full_name_en] [nvarchar](255) NULL,
	[code_name] [nvarchar](255) NULL,
	[district_code] [nvarchar](20) NULL,
	[administrative_unit_id] [int] NULL,
 CONSTRAINT [wards_pkey] PRIMARY KEY CLUSTERED 
(
	[code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Participants] ADD  CONSTRAINT [DF_Participants_anonymous_time]  DEFAULT ((3)) FOR [anonymous_time]
GO
ALTER TABLE [dbo].[Admins]  WITH CHECK ADD  CONSTRAINT [FK_Admins_Roles] FOREIGN KEY([role_id])
REFERENCES [dbo].[Roles] ([id])
GO
ALTER TABLE [dbo].[Admins] CHECK CONSTRAINT [FK_Admins_Roles]
GO
ALTER TABLE [dbo].[Chats]  WITH CHECK ADD  CONSTRAINT [FK_Chats_Hostels] FOREIGN KEY([hostel_id])
REFERENCES [dbo].[Hostels] ([id])
GO
ALTER TABLE [dbo].[Chats] CHECK CONSTRAINT [FK_Chats_Hostels]
GO
ALTER TABLE [dbo].[districts]  WITH CHECK ADD  CONSTRAINT [districts_administrative_unit_id_fkey] FOREIGN KEY([administrative_unit_id])
REFERENCES [dbo].[administrative_units] ([id])
GO
ALTER TABLE [dbo].[districts] CHECK CONSTRAINT [districts_administrative_unit_id_fkey]
GO
ALTER TABLE [dbo].[districts]  WITH CHECK ADD  CONSTRAINT [districts_province_code_fkey] FOREIGN KEY([province_code])
REFERENCES [dbo].[provinces] ([code])
GO
ALTER TABLE [dbo].[districts] CHECK CONSTRAINT [districts_province_code_fkey]
GO
ALTER TABLE [dbo].[Hostel_Amenities]  WITH CHECK ADD  CONSTRAINT [FK_Hostel_Amenities_Amenities] FOREIGN KEY([amenities_id])
REFERENCES [dbo].[Amenities] ([id])
GO
ALTER TABLE [dbo].[Hostel_Amenities] CHECK CONSTRAINT [FK_Hostel_Amenities_Amenities]
GO
ALTER TABLE [dbo].[Hostel_Amenities]  WITH CHECK ADD  CONSTRAINT [FK_Hostel_Amenities_Hostels] FOREIGN KEY([hosted_id])
REFERENCES [dbo].[Hostels] ([id])
GO
ALTER TABLE [dbo].[Hostel_Amenities] CHECK CONSTRAINT [FK_Hostel_Amenities_Hostels]
GO
ALTER TABLE [dbo].[Hostel_Images]  WITH CHECK ADD  CONSTRAINT [FK_Hostel_Images_Hostels] FOREIGN KEY([hostel_id])
REFERENCES [dbo].[Hostels] ([id])
GO
ALTER TABLE [dbo].[Hostel_Images] CHECK CONSTRAINT [FK_Hostel_Images_Hostels]
GO
ALTER TABLE [dbo].[Hostels]  WITH CHECK ADD  CONSTRAINT [FK_Hostels_Members] FOREIGN KEY([landlord_id])
REFERENCES [dbo].[Members] ([id])
GO
ALTER TABLE [dbo].[Hostels] CHECK CONSTRAINT [FK_Hostels_Members]
GO
ALTER TABLE [dbo].[Hostels]  WITH CHECK ADD  CONSTRAINT [FK_Hostels_wards] FOREIGN KEY([wards_code])
REFERENCES [dbo].[wards] ([code])
GO
ALTER TABLE [dbo].[Hostels] CHECK CONSTRAINT [FK_Hostels_wards]
GO
ALTER TABLE [dbo].[Members]  WITH CHECK ADD  CONSTRAINT [FK_Members_Roles] FOREIGN KEY([role_id])
REFERENCES [dbo].[Roles] ([id])
GO
ALTER TABLE [dbo].[Members] CHECK CONSTRAINT [FK_Members_Roles]
GO
ALTER TABLE [dbo].[Messages]  WITH CHECK ADD  CONSTRAINT [FK_Messages_Chats] FOREIGN KEY([chat_id])
REFERENCES [dbo].[Chats] ([id])
GO
ALTER TABLE [dbo].[Messages] CHECK CONSTRAINT [FK_Messages_Chats]
GO
ALTER TABLE [dbo].[Messages]  WITH CHECK ADD  CONSTRAINT [FK_Messages_Members] FOREIGN KEY([sender_id])
REFERENCES [dbo].[Members] ([id])
GO
ALTER TABLE [dbo].[Messages] CHECK CONSTRAINT [FK_Messages_Members]
GO
ALTER TABLE [dbo].[Nearby_Facilities]  WITH CHECK ADD  CONSTRAINT [FK_Nearby_Facilities_Facilities] FOREIGN KEY([ultility_id])
REFERENCES [dbo].[Facilities] ([id])
GO
ALTER TABLE [dbo].[Nearby_Facilities] CHECK CONSTRAINT [FK_Nearby_Facilities_Facilities]
GO
ALTER TABLE [dbo].[Nearby_Facilities]  WITH CHECK ADD  CONSTRAINT [FK_Nearby_Facilities_Hostels] FOREIGN KEY([hostel_id])
REFERENCES [dbo].[Hostels] ([id])
GO
ALTER TABLE [dbo].[Nearby_Facilities] CHECK CONSTRAINT [FK_Nearby_Facilities_Hostels]
GO
ALTER TABLE [dbo].[Notifications]  WITH CHECK ADD  CONSTRAINT [FK_Notifications_Members] FOREIGN KEY([send_to])
REFERENCES [dbo].[Members] ([id])
GO
ALTER TABLE [dbo].[Notifications] CHECK CONSTRAINT [FK_Notifications_Members]
GO
ALTER TABLE [dbo].[Participants]  WITH CHECK ADD  CONSTRAINT [FK_Participants_Chats] FOREIGN KEY([chat_id])
REFERENCES [dbo].[Chats] ([id])
GO
ALTER TABLE [dbo].[Participants] CHECK CONSTRAINT [FK_Participants_Chats]
GO
ALTER TABLE [dbo].[Participants]  WITH CHECK ADD  CONSTRAINT [FK_Participants_Members] FOREIGN KEY([member_id])
REFERENCES [dbo].[Members] ([id])
GO
ALTER TABLE [dbo].[Participants] CHECK CONSTRAINT [FK_Participants_Members]
GO
ALTER TABLE [dbo].[provinces]  WITH CHECK ADD  CONSTRAINT [provinces_administrative_region_id_fkey] FOREIGN KEY([administrative_region_id])
REFERENCES [dbo].[administrative_regions] ([id])
GO
ALTER TABLE [dbo].[provinces] CHECK CONSTRAINT [provinces_administrative_region_id_fkey]
GO
ALTER TABLE [dbo].[provinces]  WITH CHECK ADD  CONSTRAINT [provinces_administrative_unit_id_fkey] FOREIGN KEY([administrative_unit_id])
REFERENCES [dbo].[administrative_units] ([id])
GO
ALTER TABLE [dbo].[provinces] CHECK CONSTRAINT [provinces_administrative_unit_id_fkey]
GO
ALTER TABLE [dbo].[Residents]  WITH CHECK ADD  CONSTRAINT [FK_Residents_Hostels] FOREIGN KEY([hostel_id])
REFERENCES [dbo].[Hostels] ([id])
GO
ALTER TABLE [dbo].[Residents] CHECK CONSTRAINT [FK_Residents_Hostels]
GO
ALTER TABLE [dbo].[Residents]  WITH CHECK ADD  CONSTRAINT [FK_Residents_Members] FOREIGN KEY([member_id])
REFERENCES [dbo].[Members] ([id])
GO
ALTER TABLE [dbo].[Residents] CHECK CONSTRAINT [FK_Residents_Members]
GO
ALTER TABLE [dbo].[Residents]  WITH CHECK ADD  CONSTRAINT [FK_Residents_Rooms] FOREIGN KEY([room_id])
REFERENCES [dbo].[Rooms] ([id])
GO
ALTER TABLE [dbo].[Residents] CHECK CONSTRAINT [FK_Residents_Rooms]
GO
ALTER TABLE [dbo].[Rooms]  WITH CHECK ADD  CONSTRAINT [FK_Rooms_Hostels] FOREIGN KEY([hostel_id])
REFERENCES [dbo].[Hostels] ([id])
GO
ALTER TABLE [dbo].[Rooms] CHECK CONSTRAINT [FK_Rooms_Hostels]
GO
ALTER TABLE [dbo].[Transactions]  WITH CHECK ADD  CONSTRAINT [FK_Transactions_Rooms] FOREIGN KEY([room_id])
REFERENCES [dbo].[Rooms] ([id])
GO
ALTER TABLE [dbo].[Transactions] CHECK CONSTRAINT [FK_Transactions_Rooms]
GO
ALTER TABLE [dbo].[wards]  WITH CHECK ADD  CONSTRAINT [wards_administrative_unit_id_fkey] FOREIGN KEY([administrative_unit_id])
REFERENCES [dbo].[administrative_units] ([id])
GO
ALTER TABLE [dbo].[wards] CHECK CONSTRAINT [wards_administrative_unit_id_fkey]
GO
ALTER TABLE [dbo].[wards]  WITH CHECK ADD  CONSTRAINT [wards_district_code_fkey] FOREIGN KEY([district_code])
REFERENCES [dbo].[districts] ([code])
GO
ALTER TABLE [dbo].[wards] CHECK CONSTRAINT [wards_district_code_fkey]
GO
USE [master]
GO
ALTER DATABASE [MyHostel] SET  READ_WRITE 
GO
