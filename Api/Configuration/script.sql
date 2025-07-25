-- Cria o banco de dados se ele ainda não existir
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'CrowlerDB')
BEGIN
    CREATE DATABASE [CrowlerDB];
END
GO

-- Muda o contexto para o banco de dados recém-criado
USE [CrowlerDB];
GO

-- Criação da tabela FacebookPosts
IF OBJECT_ID('dbo.FacebookPosts', 'U') IS NULL
BEGIN
    CREATE TABLE FacebookPosts (
        Id NVARCHAR(100) NOT NULL PRIMARY KEY,
        Url NVARCHAR(1000) NOT NULL,
        Message NVARCHAR(MAX) NULL,
        Timestamp BIGINT NOT NULL,
        CommentsCount INT NOT NULL,
        ReactionsCount INT NOT NULL,
        AuthorId NVARCHAR(100) NULL,
        AuthorName NVARCHAR(200) NULL,
        AuthorUrl NVARCHAR(1000) NULL,
        AuthorProfilePictureUrl NVARCHAR(1000) NULL,
        Image NVARCHAR(MAX) NULL,
        Video NVARCHAR(MAX) NULL,
        AttachedPostUrl NVARCHAR(MAX) NULL,
        PageUrl NVARCHAR(1000) NULL,
        Topic NVARCHAR(100) NULL,
        CreatedAt DATETIME2 NOT NULL
    );
    CREATE INDEX IX_FacebookPosts_PageUrl ON FacebookPosts(PageUrl);
    CREATE INDEX IX_FacebookPosts_CreatedAt ON FacebookPosts(CreatedAt);
    CREATE INDEX IX_FacebookPosts_Timestamp ON FacebookPosts(Timestamp);
    CREATE INDEX IX_FacebookPosts_AuthorId ON FacebookPosts(AuthorId);
END

-- Criação da tabela InstagramPosts
IF OBJECT_ID('dbo.InstagramPosts', 'U') IS NULL
BEGIN
    CREATE TABLE InstagramPosts (
        Id NVARCHAR(100) NOT NULL PRIMARY KEY,
        Type NVARCHAR(50) NULL,
        ShortCode NVARCHAR(50) NULL,
        Caption NVARCHAR(MAX) NULL,
        Url NVARCHAR(500) NULL,
        CommentsCount INT NULL,
        DimensionsHeight INT NULL,
        DimensionsWidth INT NULL,
        DisplayUrl NVARCHAR(1000) NULL,
        Images NVARCHAR(MAX) NULL,
        VideoUrl NVARCHAR(1000) NULL,
        Alt NVARCHAR(500) NULL,
        LikesCount INT NULL,
        VideoViewCount INT NULL,
        VideoPlayCount INT NULL,
        Timestamp NVARCHAR(100) NULL,
        ChildPosts NVARCHAR(MAX) NULL,
        OwnerFullName NVARCHAR(200) NULL,
        OwnerUsername NVARCHAR(100) NULL,
        OwnerId NVARCHAR(100) NULL,
        ProductType NVARCHAR(50) NULL,
        VideoDuration DECIMAL(18,2) NULL,
        IsSponsored BIT NULL,
        TaggedUsers NVARCHAR(MAX) NULL,
        MusicInfo NVARCHAR(MAX) NULL,
        CoauthorProducers NVARCHAR(MAX) NULL,
        IsCommentsDisabled BIT NULL,
        InputUrl NVARCHAR(1000) NULL,
        Topic NVARCHAR(100) NULL,
        CreatedAt DATETIME2 NOT NULL
    );
    CREATE INDEX IX_InstagramPosts_OwnerId ON InstagramPosts(OwnerId);
    CREATE INDEX IX_InstagramPosts_CreatedAt ON InstagramPosts(CreatedAt);
    CREATE INDEX IX_InstagramPosts_Timestamp ON InstagramPosts(Timestamp);
END

-- Criação da tabela InstagramHashtag
IF OBJECT_ID('dbo.InstagramHashtags', 'U') IS NULL
BEGIN
    CREATE TABLE InstagramHashtags (
        PostId NVARCHAR(100) NOT NULL,
        Hashtag NVARCHAR(100) NOT NULL,
        CreatedAt DATETIME2 NOT NULL,
        CONSTRAINT PK_InstagramHashtags PRIMARY KEY (PostId, Hashtag),
        CONSTRAINT FK_InstagramHashtags_PostId FOREIGN KEY (PostId) REFERENCES InstagramPosts(Id)
    );
END

-- Criação da tabela InstagramMention
IF OBJECT_ID('dbo.InstagramMentions', 'U') IS NULL
BEGIN
    CREATE TABLE InstagramMentions (
        PostId NVARCHAR(100) NOT NULL,
        MentionedUsername NVARCHAR(100) NOT NULL,
        MentionedUserId NVARCHAR(100) NULL,
        MentionedFullName NVARCHAR(200) NULL,
        MentionedProfilePicUrl NVARCHAR(500) NULL,
        IsVerified BIT NULL,
        CreatedAt DATETIME2 NOT NULL,
        CONSTRAINT PK_InstagramMentions PRIMARY KEY (PostId, MentionedUsername),
        CONSTRAINT FK_InstagramMentions_PostId FOREIGN KEY (PostId) REFERENCES InstagramPosts(Id)
    );
END

-- Criação da tabela InstagramComment
IF OBJECT_ID('dbo.InstagramComments', 'U') IS NULL
BEGIN
    CREATE TABLE InstagramComments (
        Id NVARCHAR(100) NOT NULL PRIMARY KEY,
        PostId NVARCHAR(100) NOT NULL,
        Text NVARCHAR(MAX) NULL,
        OwnerUsername NVARCHAR(100) NULL,
        OwnerId NVARCHAR(100) NULL,
        OwnerProfilePicUrl NVARCHAR(1000) NULL,
        Timestamp NVARCHAR(100) NULL,
        RepliesCount INT NULL,
        LikesCount INT NULL,
        Replies NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2 NOT NULL,
        CONSTRAINT FK_InstagramComments_PostId FOREIGN KEY (PostId) REFERENCES InstagramPosts(Id)
    );
END

IF OBJECT_ID('dbo.Users', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Users] (
    UserId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Email NVARCHAR(255) NOT NULL UNIQUE,
    Name NVARCHAR(100) NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL DEFAULT 'User'
);

-- Criar um usuário administrador padrão (senha: Admin@123)
INSERT INTO Users (UserId, Email, Name, Password, Role)
VALUES (
    NEWID(), 
    'admin@example.com', 
    'Administrador', 
    -- Nota: Em produção, use um hash adequado em vez de senha em texto puro
    'AQAAAAEAACcQAAAAEBIoFdlczQaDwfSYrI6DjKkhVU4P5ZHfDnGNhrxu3Y6ehhVl01L+3W+Fe5S7zHZhGg==', 
    'Admin'
);
END

IF OBJECT_ID('dbo.UserTopicPreferences', 'U') IS NULL
BEGIN
CREATE TABLE UserTopicPreferences (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    Topic NVARCHAR(100) NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

-- Índice para consultas de preferência
CREATE INDEX IX_UserTopicPreferences_UserId ON UserTopicPreferences(UserId);
END


IF OBJECT_ID('dbo.Notifications', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Notifications] (
        [NotificationId] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        [UserId] UNIQUEIDENTIFIER NOT NULL,
        [PostId] NVARCHAR(100) NOT NULL,
        [SentAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [IsRead] BIT NOT NULL DEFAULT 0,
        CONSTRAINT FK_Notifications_UserId FOREIGN KEY (UserId) REFERENCES Users(UserId)
    );
END

IF ObJECT_ID('dbo.UserTopicPreferences', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[UserTopicPreferences] (
        [PreferenceId] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        [UserId] UNIQUEIDENTIFIER NOT NULL,
        [Topic] NVARCHAR(100) NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT UC_UserTopic UNIQUE (UserId, Topic),
        CONSTRAINT FK_UserTopicPreferences_UserId FOREIGN KEY (UserId) REFERENCES Users(UserId)
    );
END



