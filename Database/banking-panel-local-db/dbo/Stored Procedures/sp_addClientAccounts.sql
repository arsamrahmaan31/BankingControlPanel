
-- Alter the stored procedure to add client accounts
CREATE PROCEDURE [dbo].[sp_addClientAccounts]
    @client_id INT,                -- The ID of the client to whom the account will be added
    @account_number VARCHAR(34)    -- The account number to be added
AS
BEGIN
    SET NOCOUNT ON; -- Prevents the message that shows the number of rows affected from being returned

    -- Insert the new client account into the bp_client_accounts table
    INSERT INTO bp_client_accounts (client_id, account_number)
    VALUES (@client_id, @account_number);
END;