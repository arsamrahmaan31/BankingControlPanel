CREATE TABLE [dbo].[bp_client_accounts] (
    [account_id]     INT          IDENTITY (1, 1) NOT NULL,
    [client_id]      INT          NOT NULL,
    [account_number] VARCHAR (34) NOT NULL,
    CONSTRAINT [PK_bp_client_accounts] PRIMARY KEY CLUSTERED ([account_id] ASC),
    CONSTRAINT [FK_bp_client_accounts_bp_clients] FOREIGN KEY ([client_id]) REFERENCES [dbo].[bp_clients] ([client_id])
);

