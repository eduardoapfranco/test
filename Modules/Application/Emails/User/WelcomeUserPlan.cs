namespace Application.Emails.User
{
    public static class WelcomeUserPlan
    {
        public static string FormatEmailRequestOrderPlan(string name)
        {
            var content = HeaderEmail.FormatHeaderEmail();
            content += @"
                    <table border='0' align='center' cellpadding='10' cellspacing='0' bgcolor='#FFFFFF' width='650'>
                        <tr>
                            <td>
                                <p align='left'><font face='Trebuchet MS, Arial, sans-serif' size='5' color='#000000'><b>Olá "+ name + @", obrigado por iniciar sua assinatura no Construa App</b></font></p>
                                <p align='left'><font face='Trebuchet MS, Arial, sans-serif' size='3' color='#000000'>Estamos aguardando a confirmação do pagamento.</font></p>
                                <p align='left'><font face='Trebuchet MS, Arial, sans-serif' size='3' color='#000000'>Nossos prazos são:  </font></p>
                                <p align='left'><font face='Trebuchet MS, Arial, sans-serif' size='3' color='#000000'>
                                    <ul>
                                         <li>Boleto: é processado em até 3 dias úteis após o pagamento (finais de semana e feriados não contam);</li>           
                                         <li>Débito online: o pagamento é efetuado diretamente na conta bancária e processado em até 10 horas;</li>           
                                         <li>Cartão de Crédito: em até 2 dias úteis após o pagamento;</li>           
                                         <li>Saldo PagSeguro: imediatamente.</li>           
                                    </ul>
                                </font></p>
                                <p align='left'><font face='Trebuchet MS, Arial, sans-serif' size='3' color='#000000'><b>Qualquer dúvida entre em contato: (18) 99790-5284</b></font></p><br>
                                <p align='left'><font face='Trebuchet MS, Arial, sans-serif' size='3' color='#000000'>Att.</font></p><br>
                            </td>
                        </tr>
                    </table>
                    ";
            content += FooterEmail.FormatFooterEmail();
            return content;
        }

        public static string FormatEmailWelcomePremiumPlan(string name)
        {
            var content = HeaderEmail.FormatHeaderEmail();
            content += @"
                    <table border='0' align='center' cellpadding='10' cellspacing='0' bgcolor='#FFFFFF' width='650'>
                        <tr>
                            <td>
                                <p align='left'><font face='Trebuchet MS, Arial, sans-serif' size='5' color='#000000'><b>Olá " + name + @", bem-vindo ao Construa App Pro</b></font></p>
                                <p align='left'><font face='Trebuchet MS, Arial, sans-serif' size='3' color='#FF0000'><b>O seu pagamento foi Aprovado!</b></font></p>
                                <p align='left'><font face='Trebuchet MS, Arial, sans-serif' size='3' color='#000000'>Bem-vindo(a) ao <b>Construa App Pro</b></font></p>
                                <p align='left'><font face='Trebuchet MS, Arial, sans-serif' size='3' color='#000000'>Vamos juntos melhorar o nível da construção civil do Brasil. </font></p>
                                <p align='left'><font face='Trebuchet MS, Arial, sans-serif' size='3' color='#000000'>Além do acesso por 30 dias ao Construa App Pro você tem acesso a um grupo do Whatsapp Privado, onde consegue tirar dúvidas em tempo real com a nossa equipe. </font></p>
                                <p align='left'><font face='Trebuchet MS, Arial, sans-serif' size='3' color='#000000'>Envie uma mensagem para nosso contato se deseja ser adicionado ao Grupo Vip. 
(18) 99790-5284 </font></p>
                                <p align='left'><font face='Trebuchet MS, Arial, sans-serif' size='3' color='#FF0000'><b>Refaça o seu login para aproveitar das vantagens do Construa App Pro</b></font></p>
                                <p align='left'><font face='Trebuchet MS, Arial, sans-serif' size='3' color='#FF0000'><b>Vantagens do Construa App Pro:</b></font></p>
                                <p align='left'><font face='Trebuchet MS, Arial, sans-serif' size='3' color='#000000'>
                                    <ul>
                                         <li>Atualização de Conteúdos: Estamos sempre em constante evolução! Você sempre será avisado sobre os conteúdos novos que estiverem disponíveis em nosso aplicativo;</li>           
                                         <li>Lembretes: Gere lembretes e integre com o calendário do seu celular para não esquecer de nenhum detalhe importante!;</li>           
                                         <li>Exporte relatórios. Com o Construa App Pro, além de acompanhar a execução do serviço através do nosso checklist, você pode exportar relatórios para guardar em seus registros.</li>  
                                    </ul>
                                </font></p><br>
                                
                                <p align='left'><font face='Trebuchet MS, Arial, sans-serif' size='3' color='#000000'><b>Somos uma empresa jovem, e queremos melhorar a cada dia! Para isso, sua opinião é muito importante para nós.</b></font></p>
                                <p align='left'><font face='Trebuchet MS, Arial, sans-serif' size='3' color='#000000'><b>Como está sendo a sua experiência no aplicativo?</b></font></p>
                                <p align='left'><font face='Trebuchet MS, Arial, sans-serif' size='3' color='#000000'><b>Como podemos melhorar?</b></font></p>
                                <p align='left'><font face='Trebuchet MS, Arial, sans-serif' size='3' color='#000000'><b>Qualquer dúvida entre em contato: (18) 99790-5284 ou contato@construa.app</b></font></p><br>
                                <p align='left'><font face='Trebuchet MS, Arial, sans-serif' size='3' color='#000000'>Obrigado</font></p><br>
                            </td>
                        </tr>
                    </table>
                    ";
            content += FooterEmail.FormatFooterEmail();
            return content;
        }

        public static string FormatEmailChangeUserPaymentMethod(string name)
        {
            var content = HeaderEmail.FormatHeaderEmail();
            content += @"
                    <table border='0' align='center' cellpadding='10' cellspacing='0' bgcolor='#FFFFFF' width='650'>
                        <tr>
                            <td>
                                <p align='left'><font face='Trebuchet MS, Arial, sans-serif' size='5' color='#000000'><b>Olá " + name + @", alteração de método de pagamento Construa App Pro</b></font></p>
                                <p align='left'><font face='Trebuchet MS, Arial, sans-serif' size='3' color='#FF0000'><b>O método de pagamento foi atualizado com sucesso!</b></font></p>                               

                                
                                <p align='left'><font face='Trebuchet MS, Arial, sans-serif' size='3' color='#000000'><b>Somos uma empresa jovem, e queremos melhorar a cada dia! Para isso, sua opinião é muito importante para nós.</b></font></p>
                                <p align='left'><font face='Trebuchet MS, Arial, sans-serif' size='3' color='#000000'><b>Como está sendo a sua experiência no aplicativo?</b></font></p>
                                <p align='left'><font face='Trebuchet MS, Arial, sans-serif' size='3' color='#000000'><b>Como podemos melhorar?</b></font></p>
                                <p align='left'><font face='Trebuchet MS, Arial, sans-serif' size='3' color='#000000'><b>Qualquer dúvida entre em contato: (18) 99790-5284 ou contato@construa.app</b></font></p><br>
                                <p align='left'><font face='Trebuchet MS, Arial, sans-serif' size='3' color='#000000'>Obrigado</font></p><br>
                            </td>
                        </tr>
                    </table>
                    ";
            content += FooterEmail.FormatFooterEmail();
            return content;
        }
    }
}
