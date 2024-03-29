USE [Doctrine]
GO
ALTER TABLE [dbo].[UserReadHistory] DROP CONSTRAINT [FK_UserReadHistory_User]
GO
ALTER TABLE [dbo].[UserReadHistory] DROP CONSTRAINT [FK_UserReadHistory_Article]
GO
ALTER TABLE [dbo].[UserFavorite] DROP CONSTRAINT [FK_UserFavorite_User]
GO
ALTER TABLE [dbo].[UserFavorite] DROP CONSTRAINT [FK_UserFavorite_Article]
GO
ALTER TABLE [dbo].[UserActivity] DROP CONSTRAINT [FK_UserActivity_Visitor]
GO
ALTER TABLE [dbo].[UserActivity] DROP CONSTRAINT [FK_UserActivity_User]
GO
ALTER TABLE [dbo].[CommentVote] DROP CONSTRAINT [FK_CommentVote_User]
GO
ALTER TABLE [dbo].[CommentVote] DROP CONSTRAINT [FK_CommentVote_Comment]
GO
ALTER TABLE [dbo].[CommentReply] DROP CONSTRAINT [FK_CommentReply_Comment1]
GO
ALTER TABLE [dbo].[CommentReply] DROP CONSTRAINT [FK_CommentReply_Comment]
GO
ALTER TABLE [dbo].[CommentEdit] DROP CONSTRAINT [FK_CommentEdit_Comment]
GO
ALTER TABLE [dbo].[Comment] DROP CONSTRAINT [FK_Comment_User]
GO
ALTER TABLE [dbo].[Comment] DROP CONSTRAINT [FK_Comment_Article]
GO
ALTER TABLE [dbo].[ArticleVisitor] DROP CONSTRAINT [FK_ArticleVisitor_Visitor]
GO
ALTER TABLE [dbo].[ArticleVisitor] DROP CONSTRAINT [FK_ArticleVisitor_Article]
GO
ALTER TABLE [dbo].[ArticleTag] DROP CONSTRAINT [FK_ArticleTag_Tag]
GO
ALTER TABLE [dbo].[ArticleTag] DROP CONSTRAINT [FK_ArticleTag_Article]
GO
ALTER TABLE [dbo].[Article] DROP CONSTRAINT [FK_Article_User]
GO
ALTER TABLE [dbo].[Article] DROP CONSTRAINT [FK_Article_Topic]
GO
/****** Object:  Table [dbo].[Visitor]    Script Date: 2/24/2015 9:18:10 PM ******/
DROP TABLE [dbo].[Visitor]
GO
/****** Object:  Table [dbo].[UserReadHistory]    Script Date: 2/24/2015 9:18:10 PM ******/
DROP TABLE [dbo].[UserReadHistory]
GO
/****** Object:  Table [dbo].[UserFavorite]    Script Date: 2/24/2015 9:18:10 PM ******/
DROP TABLE [dbo].[UserFavorite]
GO
/****** Object:  Table [dbo].[UserActivity]    Script Date: 2/24/2015 9:18:10 PM ******/
DROP TABLE [dbo].[UserActivity]
GO
/****** Object:  Table [dbo].[User]    Script Date: 2/24/2015 9:18:10 PM ******/
DROP TABLE [dbo].[User]
GO
/****** Object:  Table [dbo].[Topic]    Script Date: 2/24/2015 9:18:10 PM ******/
DROP TABLE [dbo].[Topic]
GO
/****** Object:  Table [dbo].[Tag]    Script Date: 2/24/2015 9:18:10 PM ******/
DROP TABLE [dbo].[Tag]
GO
/****** Object:  Table [dbo].[CommentVote]    Script Date: 2/24/2015 9:18:10 PM ******/
DROP TABLE [dbo].[CommentVote]
GO
/****** Object:  Table [dbo].[CommentReply]    Script Date: 2/24/2015 9:18:10 PM ******/
DROP TABLE [dbo].[CommentReply]
GO
/****** Object:  Table [dbo].[CommentEdit]    Script Date: 2/24/2015 9:18:10 PM ******/
DROP TABLE [dbo].[CommentEdit]
GO
/****** Object:  Table [dbo].[Comment]    Script Date: 2/24/2015 9:18:10 PM ******/
DROP TABLE [dbo].[Comment]
GO
/****** Object:  Table [dbo].[ArticleVisitor]    Script Date: 2/24/2015 9:18:10 PM ******/
DROP TABLE [dbo].[ArticleVisitor]
GO
/****** Object:  Table [dbo].[ArticleTag]    Script Date: 2/24/2015 9:18:10 PM ******/
DROP TABLE [dbo].[ArticleTag]
GO
/****** Object:  Table [dbo].[Article]    Script Date: 2/24/2015 9:18:10 PM ******/
DROP TABLE [dbo].[Article]
GO
USE [master]
GO
/****** Object:  Database [Doctrine]    Script Date: 2/24/2015 9:18:10 PM ******/
DROP DATABASE [Doctrine]
GO
/****** Object:  Database [Doctrine]    Script Date: 2/24/2015 9:18:10 PM ******/
CREATE DATABASE [Doctrine]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'doctrina', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\doctrina.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'doctrina_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\doctrina_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [Doctrine] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Doctrine].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Doctrine] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Doctrine] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Doctrine] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Doctrine] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Doctrine] SET ARITHABORT OFF 
GO
ALTER DATABASE [Doctrine] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Doctrine] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Doctrine] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Doctrine] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Doctrine] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Doctrine] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Doctrine] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Doctrine] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Doctrine] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Doctrine] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Doctrine] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Doctrine] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Doctrine] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Doctrine] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Doctrine] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Doctrine] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Doctrine] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Doctrine] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [Doctrine] SET  MULTI_USER 
GO
ALTER DATABASE [Doctrine] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Doctrine] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Doctrine] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Doctrine] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [Doctrine] SET DELAYED_DURABILITY = DISABLED 
GO
USE [Doctrine]
GO
/****** Object:  Table [dbo].[Article]    Script Date: 2/24/2015 9:18:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Article](
	[ArticleId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[Title] [nvarchar](150) NOT NULL,
	[Text] [nvarchar](max) NOT NULL,
	[TopicId] [int] NOT NULL,
	[PublicationDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Table_1] PRIMARY KEY CLUSTERED 
(
	[ArticleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ArticleTag]    Script Date: 2/24/2015 9:18:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ArticleTag](
	[ArticleId] [int] NOT NULL,
	[TagId] [int] NOT NULL,
 CONSTRAINT [PK_ArticleTag] PRIMARY KEY CLUSTERED 
(
	[ArticleId] ASC,
	[TagId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ArticleVisitor]    Script Date: 2/24/2015 9:18:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ArticleVisitor](
	[ArticleId] [int] NOT NULL,
	[VisitorId] [int] NOT NULL,
	[LastViewDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_ArticleVisitor] PRIMARY KEY CLUSTERED 
(
	[ArticleId] ASC,
	[VisitorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Comment]    Script Date: 2/24/2015 9:18:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Comment](
	[CommentId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[ArticleId] [int] NOT NULL,
	[Text] [nvarchar](max) NOT NULL,
	[TreeLevel] [tinyint] NOT NULL,
	[Date] [datetime2](7) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Comment] PRIMARY KEY CLUSTERED 
(
	[CommentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CommentEdit]    Script Date: 2/24/2015 9:18:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CommentEdit](
	[CommentId] [int] NOT NULL,
	[EditDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_CommentEdit] PRIMARY KEY CLUSTERED 
(
	[CommentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CommentReply]    Script Date: 2/24/2015 9:18:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CommentReply](
	[CommentId] [int] NOT NULL,
	[ReplyCommentId] [int] NOT NULL,
 CONSTRAINT [PK_CommentReply] PRIMARY KEY CLUSTERED 
(
	[CommentId] ASC,
	[ReplyCommentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CommentVote]    Script Date: 2/24/2015 9:18:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CommentVote](
	[CommentId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[IsPositive] [bit] NOT NULL,
 CONSTRAINT [PK_CommentVote] PRIMARY KEY CLUSTERED 
(
	[CommentId] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Tag]    Script Date: 2/24/2015 9:18:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tag](
	[TagId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Tag] PRIMARY KEY CLUSTERED 
(
	[TagId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Topic]    Script Date: 2/24/2015 9:18:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Topic](
	[TopicId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Topic] PRIMARY KEY CLUSTERED 
(
	[TopicId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[User]    Script Date: 2/24/2015 9:18:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[User](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](150) NOT NULL,
	[Password] [char](44) NOT NULL,
	[Salt] [char](44) NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[RegistrationDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserActivity]    Script Date: 2/24/2015 9:18:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserActivity](
	[ActivityId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[VisitorId] [int] NOT NULL,
	[LogOnDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_UserActivity] PRIMARY KEY CLUSTERED 
(
	[ActivityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserFavorite]    Script Date: 2/24/2015 9:18:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserFavorite](
	[UserId] [int] NOT NULL,
	[ArticleId] [int] NOT NULL,
	[AddDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_UserFavorite] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[ArticleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserReadHistory]    Script Date: 2/24/2015 9:18:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserReadHistory](
	[UserId] [int] NOT NULL,
	[ArticleId] [int] NOT NULL,
	[ReadDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_UserReadHistory] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[ArticleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Visitor]    Script Date: 2/24/2015 9:18:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Visitor](
	[VisitorId] [int] IDENTITY(1,1) NOT NULL,
	[IpAddress] [char](15) NOT NULL,
 CONSTRAINT [PK_Visitor] PRIMARY KEY CLUSTERED 
(
	[VisitorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[Article]  WITH CHECK ADD  CONSTRAINT [FK_Article_Topic] FOREIGN KEY([TopicId])
REFERENCES [dbo].[Topic] ([TopicId])
GO
ALTER TABLE [dbo].[Article] CHECK CONSTRAINT [FK_Article_Topic]
GO
ALTER TABLE [dbo].[Article]  WITH CHECK ADD  CONSTRAINT [FK_Article_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Article] CHECK CONSTRAINT [FK_Article_User]
GO
ALTER TABLE [dbo].[ArticleTag]  WITH CHECK ADD  CONSTRAINT [FK_ArticleTag_Article] FOREIGN KEY([ArticleId])
REFERENCES [dbo].[Article] ([ArticleId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ArticleTag] CHECK CONSTRAINT [FK_ArticleTag_Article]
GO
ALTER TABLE [dbo].[ArticleTag]  WITH CHECK ADD  CONSTRAINT [FK_ArticleTag_Tag] FOREIGN KEY([TagId])
REFERENCES [dbo].[Tag] ([TagId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ArticleTag] CHECK CONSTRAINT [FK_ArticleTag_Tag]
GO
ALTER TABLE [dbo].[ArticleVisitor]  WITH CHECK ADD  CONSTRAINT [FK_ArticleVisitor_Article] FOREIGN KEY([ArticleId])
REFERENCES [dbo].[Article] ([ArticleId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ArticleVisitor] CHECK CONSTRAINT [FK_ArticleVisitor_Article]
GO
ALTER TABLE [dbo].[ArticleVisitor]  WITH CHECK ADD  CONSTRAINT [FK_ArticleVisitor_Visitor] FOREIGN KEY([VisitorId])
REFERENCES [dbo].[Visitor] ([VisitorId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ArticleVisitor] CHECK CONSTRAINT [FK_ArticleVisitor_Visitor]
GO
ALTER TABLE [dbo].[Comment]  WITH CHECK ADD  CONSTRAINT [FK_Comment_Article] FOREIGN KEY([ArticleId])
REFERENCES [dbo].[Article] ([ArticleId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Comment] CHECK CONSTRAINT [FK_Comment_Article]
GO
ALTER TABLE [dbo].[Comment]  WITH CHECK ADD  CONSTRAINT [FK_Comment_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Comment] CHECK CONSTRAINT [FK_Comment_User]
GO
ALTER TABLE [dbo].[CommentEdit]  WITH CHECK ADD  CONSTRAINT [FK_CommentEdit_Comment] FOREIGN KEY([CommentId])
REFERENCES [dbo].[Comment] ([CommentId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CommentEdit] CHECK CONSTRAINT [FK_CommentEdit_Comment]
GO
ALTER TABLE [dbo].[CommentReply]  WITH CHECK ADD  CONSTRAINT [FK_CommentReply_Comment] FOREIGN KEY([CommentId])
REFERENCES [dbo].[Comment] ([CommentId])
GO
ALTER TABLE [dbo].[CommentReply] CHECK CONSTRAINT [FK_CommentReply_Comment]
GO
ALTER TABLE [dbo].[CommentReply]  WITH CHECK ADD  CONSTRAINT [FK_CommentReply_Comment1] FOREIGN KEY([ReplyCommentId])
REFERENCES [dbo].[Comment] ([CommentId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CommentReply] CHECK CONSTRAINT [FK_CommentReply_Comment1]
GO
ALTER TABLE [dbo].[CommentVote]  WITH CHECK ADD  CONSTRAINT [FK_CommentVote_Comment] FOREIGN KEY([CommentId])
REFERENCES [dbo].[Comment] ([CommentId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CommentVote] CHECK CONSTRAINT [FK_CommentVote_Comment]
GO
ALTER TABLE [dbo].[CommentVote]  WITH CHECK ADD  CONSTRAINT [FK_CommentVote_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[CommentVote] CHECK CONSTRAINT [FK_CommentVote_User]
GO
ALTER TABLE [dbo].[UserActivity]  WITH CHECK ADD  CONSTRAINT [FK_UserActivity_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserActivity] CHECK CONSTRAINT [FK_UserActivity_User]
GO
ALTER TABLE [dbo].[UserActivity]  WITH CHECK ADD  CONSTRAINT [FK_UserActivity_Visitor] FOREIGN KEY([VisitorId])
REFERENCES [dbo].[Visitor] ([VisitorId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserActivity] CHECK CONSTRAINT [FK_UserActivity_Visitor]
GO
ALTER TABLE [dbo].[UserFavorite]  WITH CHECK ADD  CONSTRAINT [FK_UserFavorite_Article] FOREIGN KEY([ArticleId])
REFERENCES [dbo].[Article] ([ArticleId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserFavorite] CHECK CONSTRAINT [FK_UserFavorite_Article]
GO
ALTER TABLE [dbo].[UserFavorite]  WITH CHECK ADD  CONSTRAINT [FK_UserFavorite_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[UserFavorite] CHECK CONSTRAINT [FK_UserFavorite_User]
GO
ALTER TABLE [dbo].[UserReadHistory]  WITH CHECK ADD  CONSTRAINT [FK_UserReadHistory_Article] FOREIGN KEY([ArticleId])
REFERENCES [dbo].[Article] ([ArticleId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserReadHistory] CHECK CONSTRAINT [FK_UserReadHistory_Article]
GO
ALTER TABLE [dbo].[UserReadHistory]  WITH CHECK ADD  CONSTRAINT [FK_UserReadHistory_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[UserReadHistory] CHECK CONSTRAINT [FK_UserReadHistory_User]
GO
USE [master]
GO
ALTER DATABASE [Doctrine] SET  READ_WRITE 
GO
