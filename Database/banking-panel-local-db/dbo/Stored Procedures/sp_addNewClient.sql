
-- Alter the stored procedure to add a new client
CREATE PROCEDURE [dbo].[sp_addNewClient]
    @personal_id CHAR(11),                -- Personal identification number of the client
	@added_by_id INT,                     -- Explains which admin added the client
    @gender_id INT,                      -- Gender ID of the client
    @profile_photo NVARCHAR(MAX),        -- Path or URL to the profile photo of the client
    @email NVARCHAR(50),                 -- Email address of the client
    @first_name NVARCHAR(60),            -- First name of the client
    @last_name NVARCHAR(60),             -- Last name of the client
    @mobile_number VARCHAR(20),          -- Mobile number of the client
    @zip_code INT = 0,                   -- Zip code of the client's address (default 0 if not provided)
    @city NVARCHAR(20) = NULL,           -- City of the client's address (default NULL if not provided)
    @street NVARCHAR(50) = NULL,         -- Street of the client's address (default NULL if not provided)
    @country NVARCHAR(20) = NULL,        -- Country of the client's address (default NULL if not provided)
    @client_id INT OUTPUT                -- Output parameter to return the ID of the newly inserted client
AS
BEGIN
    SET NOCOUNT ON; -- Prevents sending the row count message to reduce network traffic

    -- Insert the new client record into the bp_clients table
    INSERT INTO bp_clients (
        personal_id,
		added_by_id,
        gender_id,
        profile_photo,
        email,
        first_name,
        last_name,
        mobile_number,
        zip_code,
        city,
        street,
        country
    )
    VALUES (
        @personal_id,
		@added_by_id,
        @gender_id,
        @profile_photo,
        @email,
        @first_name,
        @last_name,
        @mobile_number,
        @zip_code,
        @city,
        @street,
        @country
    );

    -- Retrieve the identity value of the newly inserted record and assign it to the output parameter
    SET @client_id = SCOPE_IDENTITY();
END;