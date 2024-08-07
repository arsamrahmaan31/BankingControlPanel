CREATE TABLE [dbo].[bp_log_search_params] (
    [search_param_id]  INT           IDENTITY (1, 1) NOT NULL,
    [loggedIn_user_id] INT           NOT NULL,
    [filter_name]      NVARCHAR (50) NULL,
    [searched_at]      DATETIME      NOT NULL,
    CONSTRAINT [PK_bp_log_search_params] PRIMARY KEY CLUSTERED ([search_param_id] ASC)
);

