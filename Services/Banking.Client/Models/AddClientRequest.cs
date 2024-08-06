namespace Banking.Client.Models
{
    public class AddClientRequest
    {
        public int gender_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string personal_id { get; set; }
        public string profile_photo { get; set; }
        public string mobile_number { get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public string street { get; set; }
        public string zip_code { get; set; }
    }
}
