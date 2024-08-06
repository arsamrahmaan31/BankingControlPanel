CREATE PROCEDURE [dbo].[sp_addNewClient]
    @personal_id CHAR(11),
    @gender_id INT,
    @profile_photo NVARCHAR(MAX),
    @email NVARCHAR(50),
    @first_name NVARCHAR(60),
    @last_name NVARCHAR(60),
    @mobile_number VARCHAR(20),
    @zip_code INT = 0,
    @city NVARCHAR(20) = NULL,
    @street NVARCHAR(50) = NULL,
    @country NVARCHAR(20) = NULL,
    @client_id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Insert the new client record into the database
        INSERT INTO bp_clients (
        personal_id,
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

    SET @client_id = SCOPE_IDENTITY();
END;