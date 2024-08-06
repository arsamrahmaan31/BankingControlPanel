CREATE TABLE [dbo].[bp_users] (
    [user_id]    INT            IDENTITY (1, 1) NOT NULL,
    [role_id]    INT            NOT NULL,
    [first_name] NVARCHAR (50)  NOT NULL,
    [last_name]  NVARCHAR (50)  NOT NULL,
    [email]      NVARCHAR (50)  NOT NULL,
    [password]   NVARCHAR (100) NOT NULL,
    [created_at] DATETIME       CONSTRAINT [DF_bp_users_created_at] DEFAULT (getdate()) NOT NULL,
    [is_deleted] BIT            CONSTRAINT [DF_bp_users_is_deleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_bp_users] PRIMARY KEY CLUSTERED ([user_id] ASC),
    CONSTRAINT [FK_bp_users_bp_lookup_roles] FOREIGN KEY ([role_id]) REFERENCES [dbo].[bp_lookup_roles] ([role_id])
);

