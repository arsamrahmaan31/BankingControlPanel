
CREATE PROCEDURE [dbo].[sp_createNewUser]
    @role_id INT,
    @first_name NVARCHAR(255),
    @last_name NVARCHAR(255),
    @email NVARCHAR(50),
    @password NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    -- Insert a new user into the [bp_users] table
    INSERT INTO [bp_users] (role_id, first_name, last_name, email, [password])
    VALUES (@role_id, @first_name, @last_name, @email, @password);

    -- Retrieve the details of the newly created user
    SELECT 
        r.role_name,
        u.first_name,
        u.last_name,
        u.email
    FROM [bp_users] AS u
    INNER JOIN [bp_lookup_roles] AS r 
        ON r.role_id = u.role_id
    WHERE u.email = @email;
END