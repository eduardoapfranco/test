namespace Infra.CrossCutting.Email
{
    public class CcoInput
    {
        public string Email { get; set; }
        public string Name { get; set; }

        public CcoInput(string email, string name)
        {
            Email = email;
            Name = name;
        }
    }
}
