namespace Application.Emails
{
    public static class HeaderEmail
    {
        public static string FormatHeaderEmail()
        {
            return @"
            <html>
            <head>
                <title>Construa App</title>
                <meta http-equiv='Content-Type' content='text/html; charset=utf-8' />
            </head>
            <body leftmargin='0' topmargin='0' marginwidth='0' width='100%' bgcolor='#FAFAFA' marginheight='0' style='background-color:#FAFAFA !important'>
            <table border='0' align='center' cellpadding='0' cellspacing='0' bgcolor='#FAFAFA' width='650'>
                <tr>
                    <td> <p align='center'><hr></td>
                </tr>
            </table>
            <table border='0' align='center' cellpadding='0' cellspacing='0' bgcolor='#FFFFFF' width='650'>
                <tr>
                    <td> <p align='center'><br><p align='center'><img src='https://serverconstruaapp.com/images/logo-construa-1.png' style='width: 150px;'></p></td>
                </ tr>
            </table>
            ";
        }
    }
}
