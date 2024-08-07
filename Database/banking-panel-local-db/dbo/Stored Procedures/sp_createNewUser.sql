
-- Alter the stored procedure to create a new user and retrieve their details
CREATE PROCEDURE [dbo].[sp_createNewUser]
    @role_id INT,                     -- ID of the role assigned to the new user
    @first_name NVARCHAR(255),        -- First name of the new user
    @last_name NVARCHAR(255),         -- Last name of the new user
    @email NVARCHAR(50),              -- Email address of the new user
    @password NVARCHAR(255)           -- Password for the new user
AS
BEGIN
    SET NOCOUNT ON; -- Prevents sending the number of affected rows to reduce network traffic

    -- Insert a new user record into the [bp_users] table
    INSERT INTO [bp_users] (role_id, first_name, last_name, email, [password])
    VALUES (@role_id, @first_name, @last_name, @email, @password);

    -- Retrieve and return the details of the newly created user
    SELECT 
        r.role_name,                 -- Role name associated with the user
        u.first_name,                -- User's first name
        u.last_name,                 -- User's last name
        u.email                      -- User's email address
    FROM [bp_users] AS u
    INNER JOIN [bp_lookup_roles] AS r 
        ON r.role_id = u.role_id     -- Join to get the role details from the lookup table
    WHERE u.email = @email;         -- Filter to get the newly created user by email
END