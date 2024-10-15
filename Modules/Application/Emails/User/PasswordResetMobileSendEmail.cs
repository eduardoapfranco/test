namespace Application.Emails.User
{
    public static class PasswordResetMobileSendEmail
    {
        public static string FormatEmailSendPasswordResetMobile(int checkerNumber)
        {
            var content = HeaderEmail.FormatHeaderEmail();
            content += @"
                    <table border='0' align='center' cellpadding='10' cellspacing='0' bgcolor='#FFFFFF' width='650'>
                        <tr>
                            <td>
                                <p align='left'><font face='Trebuchet MS, Arial, sans-serif' size='5' color='#000000'><b>Numero de Verificação: "+ checkerNumber + @"</b></font></p><br>
                                <p align='left'><font face='Trebuchet MS, Arial, sans-serif' size='3' color='#000000'>Acesse o Aplicativo Construa App e altere a sua senha.</b></font></p><br>
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
