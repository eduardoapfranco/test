namespace Infra.CrossCutting.PDF.Interfaces
{
    public interface IExportPDF
    {
        byte[] ExportHTMLToPDF(string htmlContent);
    }
}
