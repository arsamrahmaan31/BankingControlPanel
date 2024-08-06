CREATE TABLE [dbo].[bp_lookup_roles] (
    [role_id]   INT           NOT NULL,
    [role_name] NVARCHAR (20) NOT NULL,
    CONSTRAINT [PK_bp_roles] PRIMARY KEY CLUSTERED ([role_id] ASC)
);

