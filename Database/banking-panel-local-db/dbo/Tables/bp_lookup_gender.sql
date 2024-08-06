CREATE TABLE [dbo].[bp_lookup_gender] (
    [gender_id]   INT           NOT NULL,
    [gender_name] NVARCHAR (20) NULL,
    CONSTRAINT [PK_bp_lookup_gender] PRIMARY KEY CLUSTERED ([gender_id] ASC)
);

