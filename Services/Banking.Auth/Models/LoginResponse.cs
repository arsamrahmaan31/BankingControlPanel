﻿namespace Banking.Auth.Models
{
    public class LoginResponse
    {
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public string? email { get; set; }
        public string? role_name { get; set; }
    }
}
