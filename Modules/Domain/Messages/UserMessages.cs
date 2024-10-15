namespace Domain.Messages
{
    public static class UserMessages
    {
        public static string NameRequired = "O campo nome é obrigatório.";
        public static string NameLength = "O campo nome deve conter no mínimo 3 e no máximo 190 caracteres.";
        public static string EmailRequired = "O campo e-mail é obrigatório.";
        public static string EmailInvalid = "O campo e-mail está invalido.";
        public static string EmailLength = "O campo nome deve conter no mínimo 3 e no máximo 190 caracteres.";
        public static string EmailEqual = "Os e-mails não conferem, digite o e-mail e a confirmação de e-mail.";
        public static string EmailEqualInvalid = "O campo confirma e-mail está invalido.";
        public static string PasswordRequired = "O campo senha é obrigatório.";
        public static string PasswordLength = "O campo senha deve conter no mínimo 6 e no máximo 20 caracteres.";
        public static string PasswordEqual = "As senhas não conferem, digite a senha e a confirmação de senha.";
        public static string CheckerNumberRequired = "O campo número de verificação é obrigatório.";
    }
}
