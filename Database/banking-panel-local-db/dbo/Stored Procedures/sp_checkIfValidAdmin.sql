CREATE PROCEDURE [dbo].[sp_checkIfValidAdmin]
    @added_by_id INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Declare a variable to hold the validity status
    DECLARE @isValid BIT;

    -- Check if the provided user ID exists and has a role ID of 2
    SELECT @isValid = CASE 
                        WHEN COUNT(*) > 0 THEN 1  -- User is valid if found with role_id = 2
                        ELSE 0                     -- Otherwise, user is not valid
                      END
    FROM bp_users
    WHERE user_id = @added_by_id AND role_id = 2;

    -- Return the validity status using a SELECT statement
    SELECT @isValid AS IsValid;
END;