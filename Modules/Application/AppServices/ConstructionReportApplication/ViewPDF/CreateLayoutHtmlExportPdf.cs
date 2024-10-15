using Domain.Entities;
using Domain.Enum;
using Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;

namespace Application.AppServices.ConstructionReportApplication.ViewPDF
{
    public static class CreateLayoutHtmlExportPdf
    {
        
        public static string Create(IEnumerable<ChecklistSectionExportVO> checklists, Category category)
        {
            var html = "<html>";
            html += Header(category);
            html += Body(checklists, category);
            html += "</html>";
            return html;
        }
        
        public static string Header(Category category)
        {
            var header = @"<head>
                <meta http-equiv='Content-Type' content='text/html; charset=utf-8' />
                <title>PDF - Serviço - " + category.Title + @"</title>
            </head>";
            return header;
        }
    
        public static string Body(IEnumerable<ChecklistSectionExportVO> checklists, Category category)
        {
            var body = @"
                        <body style='color: #2c2e2f; font-size: 13px;'>
                        <div style='width:100%; text-align: center;'>
                            <h1>Relatório de Execução</h1>
                        </div>
                        <div style='float:left; width: 80%;'>
                            <table width='100%' style='border-collapse: collapse;'>           
                                <tr class='' style='padding-bottom: 5px;'>
                                    <td  style='font-size: 13px; color: #000000; font-weight: bold;'>Nome do Serviço:</td>
                                    <td  style='font-size: 13px;'>" + category.Title + @"</td>
                                </tr>
                                <tr class='' style='padding-bottom: 5px;'>
                                    <td style='font-size: 13px; color: #000000; font-weight: bold;'>Descrição do Serviço:</td>
                                    <td style='font-size: 13px;'>" + category.Content + @"</td>
                                </tr>
                            </table>
                        </div>";
            body += @"<div style='clear: both;'></div>
                      <div style='padding: 5px !important; width: 100%;'>";

            var checklistSectionExportVos = checklists as ChecklistSectionExportVO[] ?? checklists.ToArray();
            if (checklistSectionExportVos.Any())
            {
                foreach (var check in checklistSectionExportVos)
                {
                    if (check.Type.Equals(ChecklistTypeEnum.GRUPO))
                    {
                        body += @"
                                <div style='background: #f4f4f4; border: 1px solid #e8e8e8; border-radius:5px; width: 100%; padding: 10px; text-align: center; font-weight: bold; font-size: 16px;'>
                                    "+check.Title+@"
                                </div>";
                    } else
                    {
                        body += @"<div style='clear: both; width: 100%;'>";
                        body += @"<div style='float: left; font-size:16px; padding-top: 5px; width: 90%; padding-left: 5px;'>"+ check.Title+"</div>";
                        body += @"<div style='float: right; font-size:16px; width: 5%; padding: 5px;'>";
                        if(check.IsCheck)
                        {
                            body += @"<div style='float:right'><div style='width:30px; height: 30px; border-radius: 50%; display: inline-block; background-color: #00a654;'></div></div>";
                        } else
                        {
                            body += @"<div style='float:right'><div style='width:30px; height: 30px; border-radius: 50%; display: inline-block; border: 1px solid #717273;'></div></div>";
                        }
                        body += "</div>";
                        body += "</div>";
                        body += @"<div style='clear: both; width: 100%;'></div>";
                        body += @"<div style='border-bottom: 1px solid #e8e8e8; width: 100%; padding: 5px;'></div>";
                    }
                }
            }
            body += @"</div>";
            body += "<div style='float: left; width: 50%; margin-top: 10px; font-size: 16px;'>www.construa.app</div>";
            body += "<div style='float: right; width: 50%; margin-top: 0px; text-align: right;'><img src='https://serverconstruaapp.com/images/logo-construa-1.png' style='width: 150px;'></div>";
            body += "</body>";
            return body;
        }
    }
}
