using Banking.Client.HelperHandlers;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Banking.Client.Models
{
    [CompositeValidation]
    public class AddClientRequest
    {
        
        [Required]
        [Range(1, 2, ErrorMessage = "gender_id must be either 1 (Male) or 2 (Female).")]
        public int gender_id { get; set; }
        [Required(ErrorMessage = "first_name is required.")]
        [StringLength(60, ErrorMessage = "first_name must be less than 60 characters.")]
        public string first_name { get; set; }
        [Required(ErrorMessage = "last_name is required.")]
        [StringLength(60, ErrorMessage = "last_name must be less than 60 characters.")]
        public string last_name { get; set; }
        [Required(ErrorMessage = "email is required.")]
        [EmailAddress(ErrorMessage = "Email format is not valid.")]
        public string email { get; set; }
        [Required(ErrorMessage = "personal_id is required.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "personal_id must be exactly 11 characters.")]
        public string personal_id { get; set; }
        public string mobile_number { get; set; }
        public IFormFile? profile_picture { get; set; }
        [Required(ErrorMessage = "At least one client_account is required.")]
        public List<ClientAccount> client_accounts { get; set; }
        public ClientAddress? address { get; set; }
    }
}
