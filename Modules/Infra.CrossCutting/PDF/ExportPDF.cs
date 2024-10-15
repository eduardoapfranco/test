
using DinkToPdf;
using DinkToPdf.Contracts;
using Infra.CrossCutting.PDF.Interfaces;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Infra.CrossCutting.PDF
{
    [ExcludeFromCodeCoverage]
    public class ExportPdf: IExportPDF
    {
        private readonly IConverter _converter;
        public ExportPdf(IConverter converter)
        {
            _converter = converter;
        }

        public byte[] ExportHTMLToPDF(string htmlContent)
        {

            try
            {
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings { Top = 10 },
                    DocumentTitle = "",
                    DPI = 96,
                };

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = htmlContent,
                    WebSettings = { 
                        DefaultEncoding = "utf-8",
                        PrintMediaType = true,
                        Background = false,
                        EnableIntelligentShrinking = true,
                        LoadImages = true
                        }
                };

                var pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };

                var pdfBytes = _converter.Convert(pdf);
                return pdfBytes;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
