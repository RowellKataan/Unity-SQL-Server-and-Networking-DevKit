IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_tblUser_UserID]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblUser] DROP CONSTRAINT [DF_tblUser_UserID]
END
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_tblUser_DateCreated]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblUser] DROP CONSTRAINT [DF_tblUser_DateCreated]
END
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_tblUser_DateUpdated]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblUser] DROP CONSTRAINT [DF_tblUser_DateUpdated]
END
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_tblUser_IsActive]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblUser] DROP CONSTRAINT [DF_tblUser_IsActive]
END
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_tblUser_Username]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblUser] DROP CONSTRAINT [DF_tblUser_Username]
END
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_tblUser_Password]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblUser] DROP CONSTRAINT [DF_tblUser_Password]
END
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_tblUser_FirstName]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblUser] DROP CONSTRAINT [DF_tblUser_FirstName]
END
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_tblUser_LastName]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblUser] DROP CONSTRAINT [DF_tblUser_LastName]
END
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_tblUser_EmailAddress]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblUser] DROP CONSTRAINT [DF_tblUser_EmailAddress]
END
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_tblUser_UserType]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblUser] DROP CONSTRAINT [DF_tblUser_UserType]
END
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_tblUser_Warnings]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblUser] DROP CONSTRAINT [DF_tblUser_Warnings]
END
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_tblUser_BanDate]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblUser] DROP CONSTRAINT [DF_tblUser_BanDate]
END
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_tblUser_BanDays]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblUser] DROP CONSTRAINT [DF_tblUser_BanDays]
END
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_tblUser_ConnectionCount]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblUser] DROP CONSTRAINT [DF_tblUser_ConnectionCount]
END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spUserGetByID]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spUserGetByID];
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spUserUpdate]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spUserUpdate];
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spUserGetByUserID]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spUserGetByUserID];
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spUserGetByUsername]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spUserGetByUsername];
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spUserGetByEmailAddress]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spUserGetByEmailAddress];
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblUser]') AND type in (N'U'))
DROP TABLE [dbo].[tblUser]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[tblUser](
[UserID] [INT] NOT NULL,
[DateCreated] [DATETIME] NOT NULL,
[DateUpdated] [DATETIME] NOT NULL,
[IsActive] [BIT] NOT NULL,
[Username] [VARCHAR](25) NOT NULL,
[Password] [VARCHAR](30) NOT NULL,
[FirstName] [VARCHAR](20) NOT NULL,
[LastName] [VARCHAR](25) NOT NULL,
[EmailAddress] [VARCHAR](75) NOT NULL,
[UserType] [INT] NOT NULL,
[Warnings] [INT] NOT NULL,
[BanDate] [DATETIME] NOT NULL,
[BanDays] [INT] NOT NULL,
[ConnectionCount] [INT] NOT NULL) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[tblUser] ADD CONSTRAINT [DF_tblUser_UserID] DEFAULT 0 FOR [UserID]
GO

ALTER TABLE [dbo].[tblUser] ADD CONSTRAINT [DF_tblUser_DateCreated] DEFAULT (getdate()) FOR [DateCreated]
GO

ALTER TABLE [dbo].[tblUser] ADD CONSTRAINT [DF_tblUser_DateUpdated] DEFAULT (getdate()) FOR [DateUpdated]
GO

ALTER TABLE [dbo].[tblUser] ADD CONSTRAINT [DF_tblUser_IsActive] DEFAULT ((1)) FOR [IsActive]
GO

ALTER TABLE [dbo].[tblUser] ADD CONSTRAINT [DF_tblUser_Username] DEFAULT ('') FOR [Username]
GO

ALTER TABLE [dbo].[tblUser] ADD CONSTRAINT [DF_tblUser_Password] DEFAULT ('') FOR [Password]
GO

ALTER TABLE [dbo].[tblUser] ADD CONSTRAINT [DF_tblUser_FirstName] DEFAULT ('') FOR [FirstName]
GO

ALTER TABLE [dbo].[tblUser] ADD CONSTRAINT [DF_tblUser_LastName] DEFAULT ('') FOR [LastName]
GO

ALTER TABLE [dbo].[tblUser] ADD CONSTRAINT [DF_tblUser_EmailAddress] DEFAULT ('') FOR [EmailAddress]
GO

ALTER TABLE [dbo].[tblUser] ADD CONSTRAINT [DF_tblUser_UserType] DEFAULT 0 FOR [UserType]
GO

ALTER TABLE [dbo].[tblUser] ADD CONSTRAINT [DF_tblUser_Warnings] DEFAULT 0 FOR [Warnings]
GO

ALTER TABLE [dbo].[tblUser] ADD CONSTRAINT [DF_tblUser_BanDate] DEFAULT (getdate()) FOR [BanDate]
GO

ALTER TABLE [dbo].[tblUser] ADD CONSTRAINT [DF_tblUser_BanDays] DEFAULT 0 FOR [BanDays]
GO

ALTER TABLE [dbo].[tblUser] ADD CONSTRAINT [DF_tblUser_ConnectionCount] DEFAULT 0 FOR [ConnectionCount]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spUserGetByID] (
	@ID					INT = 0,
	@ACTIVEONLY	BIT = FALSE
) AS
BEGIN

	IF (@ID = 0)
	BEGIN

		SELECT * 
		  FROM tblUser
		 WHERE ((@ACTIVEONLY = ((1)) AND IsActive = ((1)) ) OR @ACTIVEONLY = ((0)) )
		 ORDER BY UserID;

	END
	ELSE
	BEGIN

		SELECT TOP 1 * 
		  FROM tblUser
		 WHERE UserID = @ID
		   AND ((@ACTIVEONLY = ((1)) AND IsActive = ((1)) ) OR @ACTIVEONLY = ((0)) );

	END

END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spUserGetByUserID] (
	@FIND				INT,
	@ACTIVEONLY	BIT = TRUE
) AS
BEGIN

	SELECT TOP 1 * 
	  FROM tblUser
	 WHERE UserID = @FIND
		AND ((@ACTIVEONLY = ((1)) AND IsActive = ((1))) OR @ACTIVEONLY = ((0)) );

END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spUserGetByUsername] (
	@FIND				VARCHAR(25),
	@ACTIVEONLY	BIT = TRUE
) AS
BEGIN

	SELECT TOP 1 * 
	  FROM tblUser
	 WHERE LOWER(Username) = LOWER(@FIND)
		AND ((@ACTIVEONLY = ((1)) AND IsActive = ((1))) OR @ACTIVEONLY = ((0)) );

END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spUserGetByEmailAddress] (
	@FIND				VARCHAR(75),
	@ACTIVEONLY	BIT = TRUE
) AS
BEGIN

	SELECT TOP 1 * 
	  FROM tblUser
	 WHERE LOWER(EmailAddress) = LOWER(@FIND)
		AND ((@ACTIVEONLY = ((1)) AND IsActive = ((1))) OR @ACTIVEONLY = ((0)) );

END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spUserUpdate]( 
	@USERID		INT,
	@ISACTIVE		BIT,
	@USERNAME		VARCHAR(25),
	@PASSWORD		VARCHAR(30),
	@FIRSTNAME		VARCHAR(20),
	@LASTNAME		VARCHAR(25),
	@EMAILADDRESS		VARCHAR(75),
	@USERTYPE		INT,
	@WARNINGS		INT,
	@BANDATE		DATETIME,
	@BANDAYS		INT,
	@CONNECTIONCOUNT		INT,
	@NEWID				INT OUT
)
AS
BEGIN

	SET NOCOUNT ON;
	DECLARE @Find AS INT;
	DECLARE @Max  AS INT;

	IF (@USERID = 0)
	BEGIN

		SELECT TOP 1 @USERID = UserID
		  FROM tblUser
		 WHERE UserID = @USERID

	END;

	SELECT @Find=COUNT(UserID) FROM tblUser WHERE UserID = @USERID;

	IF (@Find > 0 AND @USERID > 0)
	BEGIN

		UPDATE tblUser SET
							IsActive	= @ISACTIVE,
							Username	= @USERNAME,
							Password	= @PASSWORD,
							FirstName	= @FIRSTNAME,
							LastName	= @LASTNAME,
							EmailAddress	= @EMAILADDRESS,
							UserType	= @USERTYPE,
							Warnings	= @WARNINGS,
							BanDate	= @BANDATE,
							BanDays	= @BANDAYS,
							ConnectionCount	= @CONNECTIONCOUNT
				WHERE UserID = @USERID;
			SET @NEWID = @USERID;

	END
	ELSE
	BEGIN

		SELECT @Max = MAX(UserID)+1 FROM tblUser;

		IF (@Max IS NULL OR @Max < 1)
		BEGIN
			SELECT @Max=COUNT(UserID)+1 FROM tblUser;
		END

		INSERT INTO tblUser 
		(UserID, DateCreated, DateUpdated, IsActive, Username, Password, FirstName, LastName, EmailAddress, UserType, Warnings, BanDate, BanDays, ConnectionCount)
		VALUES 
		(@Max, GETDATE(), GETDATE(), @ISACTIVE, @USERNAME, @PASSWORD, @FIRSTNAME, @LASTNAME, @EMAILADDRESS, @USERTYPE, @WARNINGS, @BANDATE, @BANDAYS, @CONNECTIONCOUNT);

		SET @NEWID = @Max 

	END

END

GO

