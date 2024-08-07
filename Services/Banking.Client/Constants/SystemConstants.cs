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
        public const string StoreProcedure_GetAllClients = "sp_getAllClients";
        public const string StoreProcedure_GetLast3SearchSuggestions = "sp_getLast3SearchSuggestions";
        public const string StoredProcedure_LogSearchFilters = "sp_logSearchFilter";



    }
}
