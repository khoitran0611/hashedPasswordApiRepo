namespace HashedPassword.Data
{
    public class Account
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PasswordHashed { get; set; }
        public string PasswordSalt { get; set; }
    }
}