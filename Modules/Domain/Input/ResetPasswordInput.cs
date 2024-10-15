namespace Domain.Input
{
    public class ResetPasswordInput
    {
        public string Email { get; set; }
        public int CheckerNumber { get; set; }
        public string NewPasswordCrypto { get; set; }
    }
}
