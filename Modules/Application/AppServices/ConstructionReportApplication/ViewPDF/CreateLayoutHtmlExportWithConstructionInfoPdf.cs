using Application.AppServices.ChecklistApplication.Input;
using Domain.Entities;
using Domain.Enum;
using Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using System.Globalization;

namespace Application.AppServices.ConstructionReportApplication.ViewPDF
{
    public static class CreateLayoutHtmlExportWithConstructionInfoPdf
    {
        public static readonly string App = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        public static readonly string Templates = Path.Combine(App, "Templates");
        public static readonly string TemplateRelatorio = Path.Combine(Templates, "relatorio.html");
        public static readonly string TemplateRelatorioIncludeComentarios = Path.Combine(Templates, "relatorio-include-comentarios.html");
        public static readonly string TemplateRelatorioIncludeFotos = Path.Combine(Templates, "relatorio-include-fotos.html");
        public static readonly string ImagemLogo = Path.Combine(Templates, "logo-construa.png");
        public static readonly string ImagemSeta = Path.Combine(Templates, "imagem-seta.png");
        public static readonly string ImagemCheckOn = Path.Combine(Templates, "Checkbox-on.png");
        public static readonly string ImagemCheckOff = Path.Combine(Templates, "Checkbox.png");

        public static string Create(IEnumerable<ChecklistSectionExportVO> checklists, Category category, ExportChecklistDadosInput dados)
        {
            string html = File.ReadAllText(TemplateRelatorio);
            html = GetHeaderHtml(html, category, dados);

            byte[] imageBytesSeta = System.IO.File.ReadAllBytes(ImagemSeta);
            string base64StringSeta = Convert.ToBase64String(imageBytesSeta);

            byte[] imageBytesCheckOn = System.IO.File.ReadAllBytes(ImagemCheckOn);
            string base64StringCheckOn = Convert.ToBase64String(imageBytesCheckOn);

            byte[] imageBytesCheckOff = System.IO.File.ReadAllBytes(ImagemCheckOff);
            string base64StringCheckOff = Convert.ToBase64String(imageBytesCheckOff);

            string htmlChecklists = GetChecklistsHtml(checklists, category, base64StringSeta, base64StringCheckOn, base64StringCheckOff);
            html = html.Replace("#TEMPLATE_CHECKLISTS#", htmlChecklists);

            string htmlOrcamento = GetOrcamentoHtml(dados);
            html = html.Replace("#AREA_ORCAMENTO#", htmlOrcamento);

            string htmlComentarios = GetComentariosHtml(dados);
            html = html.Replace("#AREA_COMENTARIOS#", htmlComentarios);

            string htmlGarantia = GetGarantiaHtml(dados);
            html = html.Replace("#AREA_GARANTIA#", htmlGarantia);

            string htmlFotos = GetFotosHtml(dados);
            html = html.Replace("#AREA_FOTOS#", htmlFotos);

            byte[] imageBytesLogo = System.IO.File.ReadAllBytes(ImagemLogo);
            string base64StringLogo = Convert.ToBase64String(imageBytesLogo);
            html = html.Replace("#IMAGEM_LOGO#", base64StringLogo);
            return html;
        }

        public static string GetHeaderHtml(string html, Category category, ExportChecklistDadosInput dados)
        {
            return html
                .Replace("#TituloRelatorio#", Text(dados.TituloRelatorioPDF))
                .Replace("#Obra#", Text(dados.Obra.Nome))
                .Replace("#Endereco#", Text(dados.Obra.Endereco))
                .Replace("#DisplayEndereco#", String.IsNullOrEmpty(dados.Obra.Endereco) ? "display:none;" : "")
                .Replace("#DataComprovante#", Text(dados.DataComprovante.Value.ToString("dd/MM/yyyy")))
                .Replace("#Contratante#", Text(dados.Obra.Contratante))
                .Replace("#DisplayContratante#", String.IsNullOrEmpty(dados.Obra.Contratante) ? "display:none;" : "")
                .Replace("#Responsavel#", Text(dados.Obra.Responsavel))
                .Replace("#DisplayResponsavel#", String.IsNullOrEmpty(dados.Obra.Responsavel) ? "display:none;" : "");
            }

        private static string Text(string texto)
        {
            if (String.IsNullOrEmpty(texto))
                {
                texto = String.Empty;
                }
            return texto.Replace("\n", "<br/>");
        }

        public static string Moeda(decimal valor)
            {
                return String.Format(new CultureInfo("pt-BR"), "{0:C}", valor);
            }

        public static string GetChecklistsHtml(IEnumerable<ChecklistSectionExportVO> checklists, Category category, string svgSeta, string imgCheckOn, string imgCheckOff)
        {
            string body = String.Empty;
            var checklistSectionExportVos = checklists as ChecklistSectionExportVO[] ?? checklists.ToArray();
            if (checklistSectionExportVos.Any())
            {
                body = $@"
                    <h3 style='font-size: 16px; font-weight: bold; padding-left: 30px'>
                        <span style='color:#FF4500; margin-right: 10px'>
                            <img style='width: 15px; height: 15px;color:#FF4500; margin-right: 10px;' src='data:image/png;base64,{svgSeta}' />
                        </span>
                        {category.Title}
                    </h3>";

                foreach (var check in checklistSectionExportVos)
                {
                    if (check.Type.Equals(ChecklistTypeEnum.GRUPO))
                    {
                        body += $@"
                                <h4 style='background:#EEF2FA; border:none; border-radius:5px; padding:8px 30px; margin-left:50px; text-align:left; font-weight:bold; font-size:16px; color: #5A5A5A'>
                                   {check.Title}
                                </h4>";
                    }
                    else
                    {
                        body += $@"
                                <div style='border-bottom:1px solid #B9B9B9; color:#5A5A5A; font-size:13px; margin-left:80px; margin-bottom:10px;'>
                                    <table border='0' width='100%'>
                                        <tbody>
                                            <tr>
                                                <td width='90%' align='left' valign='middle'>
		                                            {check.Title}
                                                </td>
                                                <td width='10%' align='right' valign='middle'>
                                                    <img style='width:15px;height:15px;margin-right:10px;' src='data:image/png;base64,{(check.IsCheck ? imgCheckOn : imgCheckOff)}' />
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>";
                    }
                }
            }
            return body;
        }

        public static string GetOrcamentoHtml(ExportChecklistDadosInput dados)
            {
            if (!dados.Valor.HasValue || dados.Valor <= 0 || dados.TipoRelatorio == ConstructionReportType.Recibo)
                {
                return String.Empty;
                }

            string htmlValores = String.Empty;
            string tituloArea = "Orçamento";
            if (dados.TipoRelatorio == ConstructionReportType.Orcamento)
                {
                htmlValores = "<b>Valor: </b>" + Moeda(dados.Valor.Value) + "<br />";
                if (dados.Desconto.HasValue && dados.Desconto.Value >= 0)
                    {
                    htmlValores += "<b>Desconto: </b>" + Moeda(dados.Desconto.Value) + "<br />";
                    htmlValores += "<b>Total Orçamento: </b>" + Moeda(dados.getTotal()) + "<br />";
                    }
                } else if(dados.TipoRelatorio == ConstructionReportType.Comprovante)
                {
                tituloArea = "Cobrança";
                htmlValores = "<b>Valor: </b>" + Moeda(dados.Valor.Value) + "<br />";
                if (dados.Desconto.HasValue && dados.Desconto.Value >= 0)
                    {
                    htmlValores += "<b>Desconto: </b>" + Moeda(dados.Desconto.Value) + "<br />";
                    htmlValores += "<b>Total: </b>" + Moeda(dados.getTotal()) + "<br />";
                    }
                if(dados.DataAssociada.HasValue){
                    htmlValores += "<b>Data de vencimento: </b>" + Text(dados.DataAssociada.Value.ToString("dd/MM/yyyy"));
                    }
                }
            

            string htmlOrcamento = File.ReadAllText(TemplateRelatorioIncludeComentarios);
            return htmlOrcamento.Replace("#TituloComentario#", tituloArea)
                .Replace("#Comentarios#", htmlValores);
            }

        public static string GetComentariosHtml(ExportChecklistDadosInput dados)
        {
            if (String.IsNullOrEmpty(dados.Comentarios))
            {
                return String.Empty;
            }
            string htmlComentarios = File.ReadAllText(TemplateRelatorioIncludeComentarios);
            return htmlComentarios.Replace("#TituloComentario#", "Comentários")
                .Replace("#Comentarios#", Text(dados.Comentarios));
        }

        public static string GetGarantiaHtml(ExportChecklistDadosInput dados)
            {
            if (String.IsNullOrEmpty(dados.Garantia))
                {
                return String.Empty;
                }
            string titulo = "Garantia";
            if (dados.TipoRelatorio.Equals(ConstructionReportType.Orcamento))
                {
                titulo = "Prazo";
                }
            else if (dados.TipoRelatorio.Equals(ConstructionReportType.Recibo))
                {
                return String.Empty;
                }
            string htmlGarantia = File.ReadAllText(TemplateRelatorioIncludeComentarios);
            return htmlGarantia.Replace("#TituloComentario#", titulo)
                .Replace("#Comentarios#", Text(dados.Garantia));
            }

        public static string GetFotosHtml(ExportChecklistDadosInput dados)
        {
            var relatorioFotos = dados.Fotos.ToArray();
            if (relatorioFotos.Length == 0)
            {
                return String.Empty;
            }
            else
            {
                string body = String.Empty;
                int index = 0;
                foreach (ExportChecklistDadosFotoInput foto in dados.Fotos)
                {
                    index++;
                    if (index % 2 != 0)
                    {
                        body += "<tr style='page-break-inside:avoid; page-break-after:auto'>";
                    }
                    if (index % 2 == 0)
                    {
                        body += "<td style='width: 40px;'> &nbsp;</td>";
                    }

                    body += @"<td width='50%'>
                                <table width='100%' style='page-break-inside:auto '>
                                    <tbody>
                                        <tr style='page-break-inside:avoid; page-break-after:auto'>
                                            <td align='center'>
                                                <div style='width: 400px'>
                                                    <img src='" + foto.Base64 + @"' style='object-fit:contain;object-position:top;display:block;width:100%;' /> <br>
                                                    <p style='font-size:12px; color:#666666; padding:10px; text-align:center;'>" + Text(foto.Caption) + @"</p>
                                                </div>
                                            </td>
                                        </tr>                                       
                                        </tbody>
                                </table>
                         </td>";

                    if (index % 2 == 0)
                    {
                        body += "</tr>";
                        body += "<div style='clear: both !important;'></div>";
                    }
                }

                string htmlFotos = File.ReadAllText(TemplateRelatorioIncludeFotos);
                return htmlFotos.Replace("#TEMPLATE_FOTOS#", body);
            }
        }
    }
}
