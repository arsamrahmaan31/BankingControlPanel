namespace Banking.Client.Constants
{
    public class SystemConstants
    {
        public const string JwtTokenPath = "AppSettings:Token";
        public const string DefaultConnection = "DefaultConnection";

        //Store Procedure constants
        public const string StoreProcedure_AddNewClient = "sp_addNewClient";
        public const string StoreProcedure_AddClientAccounts = "sp_addClientAccounts";
        public const string StoreProcedure_GetInsertedClient = "sp_getInsertedClient";

    }
}
