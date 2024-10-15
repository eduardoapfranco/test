using System;

namespace Application.Emails
{
    public static class FooterEmail
    {
        public static string FormatFooterEmail()
        {
            return @"
               <table border='0' align='center' cellpadding='0' cellspacing='0' bgcolor='#FAFAFA' width='650'>
                    <tr>
                        <td> <p align='center'><hr></td>
                    </tr>
                </table>
                <table border='0' align='center' cellpadding='0' cellspacing='0' bgcolor='#FAFAFA' width='650'>
                    <tr>
                        <td>
                            <p align='center'><font face='Trebuchet MS, Arial, sans-serif' size='3' color='#656565'>
                                    Copyright © <b><i>Construa App</i></b> - " + DateTime.Now.Date.Year.ToString() + @", All rights reserved.
                                </font></p>
                        </td>
                    </tr>
                </table>
                </body>
                </html>
                ";
        }
    }
}
