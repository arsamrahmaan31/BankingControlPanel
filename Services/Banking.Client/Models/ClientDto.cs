namespace Banking.Client.Models
{
    public class ClientDto
    {
        public string gender_name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string personal_id { get; set; }
        public string profile_photo { get; set; }
        public string mobile_number { get; set; }
        public string client_accounts { get; set; } // JSON string
        public string address { get; set; } // JSON string
    }
}
