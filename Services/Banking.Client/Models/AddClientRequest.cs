using Banking.Client.HelperHandlers;
using System.Net;

namespace Banking.Client.Models
{
    [CompositeValidation]
    public class AddClientRequest
    {
        public int gender_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string personal_id { get; set; }
        public string profile_photo { get; set; }
        public string mobile_number { get; set; }
        
        public List<ClientAccount> client_accounts { get; set; }
        public ClientAddress address { get; set; }
    }
}
