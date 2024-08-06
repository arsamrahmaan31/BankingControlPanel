CREATE PROCEDURE [dbo].[sp_addClientAccounts]
    @client_id INT,
	@account_number VARCHAR(34)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO bp_client_accounts(client_id, account_number)
    VALUES (@client_id, @account_number);
END;