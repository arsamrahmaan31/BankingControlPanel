CREATE TABLE [dbo].[bp_clients] (
    [client_id]     INT            IDENTITY (1, 1) NOT NULL,
    [gender_id]     INT            NOT NULL,
    [first_name]    NVARCHAR (60)  NOT NULL,
    [last_name]     NVARCHAR (60)  NOT NULL,
    [email]         NVARCHAR (50)  NOT NULL,
    [personal_id]   CHAR (11)      NOT NULL,
    [profile_photo] NVARCHAR (MAX) NULL,
    [mobile_number] VARCHAR (20)   NOT NULL,
    [country]       NVARCHAR (20)  NULL,
    [city]          NVARCHAR (20)  NULL,
    [street]        NVARCHAR (50)  NULL,
    [zip_code]      INT            NULL,
    [created_at]    DATETIME       CONSTRAINT [DF_bp_clients_created_at] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_bp_clients] PRIMARY KEY CLUSTERED ([client_id] ASC),
    CONSTRAINT [FK_bp_clients_bp_lookup_gender] FOREIGN KEY ([gender_id]) REFERENCES [dbo].[bp_lookup_gender] ([gender_id])
);



