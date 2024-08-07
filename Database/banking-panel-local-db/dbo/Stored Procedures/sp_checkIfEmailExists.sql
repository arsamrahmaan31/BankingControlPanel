
-- Alter the stored procedure to check if an email already exists in the system
CREATE PROCEDURE [dbo].[sp_checkIfEmailExists]
    @email NVARCHAR(50)    -- Email address to be checked for existence
AS
BEGIN
    SET NOCOUNT ON; -- Prevents sending the number of affected rows to reduce network traffic

    -- Select user details where the email matches the input parameter
    SELECT 
        u.user_id,           -- User ID
        u.first_name,        -- User's first name
        u.last_name,         -- User's last name
        u.email,             -- User's email address
        u.password,          -- User's password (consider if it's necessary to return this)
        r.role_name,         -- Role name from the lookup table
        u.role_id            -- Role ID of the user
    FROM [bp_users] AS u
    INNER JOIN [bp_lookup_roles] AS r 
        ON u.role_id = r.role_id -- Join with role lookup table to get role details
    WHERE u.email = @email;       -- Filter records based on the provided email
END