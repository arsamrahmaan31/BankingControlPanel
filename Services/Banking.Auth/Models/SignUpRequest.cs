using Banking.Auth.HelperHandlers;
using System.ComponentModel.DataAnnotations;

namespace Banking.Auth.Models
{
    public class SignUpRequest
    {
        [CompositeValidation]
        public int role_id { get; set; }
        [Required(ErrorMessage = "Frist Name is required.")]
        public string first_name { get; set; }
        [Required(ErrorMessage = "Last Name is required.")]
        public string last_name { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        public string email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string password { get; set; }
    }
}
