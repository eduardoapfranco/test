using Application.AppServices.ChecklistApplication.Validators;
using Domain.Enum;
using Infra.CrossCutting.Validators;
using System;

namespace Application.AppServices.ChecklistApplication.Input
{
    public class ExportChecklistDadosInput : ValidationInput
    {
        public ExportTypeChecklistEnum TipoExportacao { get; set; }
        public ExportChecklistDadosObraInput Obra { get; set; }
        public ExportChecklistDadosFotoInput[] Fotos { get; set; }
        public string Comentarios { get; set; }
        public string Garantia { get; set; }
        public ConstructionReportType TipoRelatorio { get; set; }

        public string TituloRelatorioPDF { get; set; }
        public decimal? Valor { get; set; }
        public decimal? Desconto { get; set; }

        public DateTime? DataAssociada { get; set; }

        public decimal getTotal() { 
                if (Valor.HasValue && Desconto.HasValue && Desconto >= 0 && Valor >= 0)
                    {
                    return Valor.Value - Desconto.Value;
                    }
                return 0;
                }
                
        public System.DateTime? DataComprovante { get; set; }

        public ExportChecklistDadosInput()
        {
            Fotos = new ExportChecklistDadosFotoInput[] { };
            TituloRelatorioPDF = "Comprovante de Serviço";
            TipoRelatorio = ConstructionReportType.Comprovante;
        }

        public override bool IsValid()
            {
            ValidationResult = new ExportChecklistDadosInputValidator().Validate(this);
            return ValidationResult.IsValid;
            }
        }
}
