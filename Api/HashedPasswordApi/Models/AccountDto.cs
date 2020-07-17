namespace HashedPasswordApi.Models
{
    public class AccountDto
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PasswordHashed { get; set; }
        public string PasswordSalt { get; set; }
    }
}