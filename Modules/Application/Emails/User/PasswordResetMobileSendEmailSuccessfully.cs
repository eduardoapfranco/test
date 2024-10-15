namespace Application.Emails.User
{
    public static class PasswordResetMobileSendEmailSuccessfully
    {
        public static string FormatEmailSendPasswordResetSuccessfully()
        {
            var content = HeaderEmail.FormatHeaderEmail();
            content += @"
                    <table border='0' align='center' cellpadding='10' cellspacing='0' bgcolor='#FFFFFF' width='650'>
                        <tr>
                            <td>
                                <p align='left'><font face='Trebuchet MS, Arial, sans-serif' size='5' color='#000000'><b>Senha alterada com sucesso!</b></font></p><br>
                                <p align='left'><font face='Trebuchet MS, Arial, sans-serif' size='3' color='#000000'>Acesse o Aplicativo Construa App e faça seu login.</b></font></p><br>
                                <p align='left'><font face='Trebuchet MS, Arial, sans-serif' size='3' color='#000000'>Att.</font></p><br>
                            </td>
                        </tr>
                    </table>
                    ";
            content += FooterEmail.FormatFooterEmail();
            return content;
        }
    }
}
