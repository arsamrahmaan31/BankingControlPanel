
-- Alter the stored procedure to retrieve the last 3 search suggestions for a given user
CREATE PROCEDURE [dbo].[sp_getLast3SearchSuggestions]
    @loggedIn_user_id INT  -- ID of the user for whom search suggestions are being retrieved
AS
BEGIN
    -- Select the top 3 most recent search filters for the specified user
    SELECT TOP 3 filter_name         -- The name of the search filter
    FROM bp_log_search_params        -- Table where search parameters are logged
    WHERE loggedIn_user_id = @loggedIn_user_id  -- Filter by user ID
    ORDER BY searched_at DESC;       -- Order results by search date in descending order to get the most recent
END