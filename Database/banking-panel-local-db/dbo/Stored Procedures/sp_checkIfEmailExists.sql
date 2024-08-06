
CREATE PROCEDURE [dbo].[sp_checkIfEmailExists]
    @email NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        u.user_id,
        u.first_name,
        u.last_name,
        u.email,
        u.password,
        r.role_name,
        u.role_id
    FROM [bp_users] AS u
    INNER JOIN [bp_lookup_roles] AS r 
        ON u.role_id = r.role_id
    WHERE u.email = @email;
END