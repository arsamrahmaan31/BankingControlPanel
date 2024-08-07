
-- Alter the stored procedure to log search filter parameters for a user
CREATE PROCEDURE [dbo].[sp_logSearchFilter]
    @filter NVARCHAR(MAX),             -- Search filter parameter to be logged
    @loggedIn_user_id INT,             -- ID of the user performing the search
    @searched_at DATETIME              -- Timestamp of when the search was performed
AS
BEGIN
    -- Insert a new log entry into the bp_log_search_params table
    INSERT INTO bp_log_search_params (filter_name, loggedIn_user_id, searched_at)
    VALUES (@filter, @loggedIn_user_id, @searched_at);
END