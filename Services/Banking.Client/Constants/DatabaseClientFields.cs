namespace Banking.Client.Constants
{
    public static class DatabaseClientFields
    {

        // Get all client list
        public const string GenderName = "gender_name";
        public const string FirstName = "first_name";
        public const string LastName = "last_name";
        public const string Email = "email";
        public const string PersonalId = "personal_id";
        public const string ProfilePhoto = "profile_photo";
        public const string MobileNumber = "mobile_number";
        public const string Country = "country";
        public const string City = "city";
        public const string Street = "street";
        public const string ZipCode = "zip_code";
        public const string ClientAccounts = "client_accounts";

        // Add Client SQL params
        public const string Personal_Id = "@personal_id";
        public const string Gender_Id = "@gender_id";
        public const string Profile_Photo = "@profile_photo";
        public const string Email_Add = "@email";
        public const string First_Name = "@first_name";
        public const string Last_Name = "@last_name";
        public const string Mobile_Number = "@mobile_number";
        public const string Zip_Code = "@zip_code";
        public const string City_ = "@city";
        public const string Street_ = "@street";
        public const string Country_ = "@country";
        public const string Client_Id = "@client_id";

        // Filters for get clients
        public const string Filter = "@Filter";
        public const string SortBy = "@SortBy";
        public const string SortDescending = "@SortDescending";
        public const string PageNumber = "@PageNumber";
        public const string PageSize = "@PageSize";

    }

}
