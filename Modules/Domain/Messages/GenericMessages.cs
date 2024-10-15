namespace Domain.Messages
{
    public static class GenericMessages
    {
        public static string FieldRequired = "Campo obrigatório.";
        public static string CategoryId = "O ID da categoria é obrigatório.";
        public static string CategoryIdNumber = "O ID da categoria deve ser maior que zero.";
        public static string EnumExportPDFRequired = "Deve ser um desses valores: 0 (Todos), 1 (Somente checkados) e 2 (Somente os não checkados)";
        public static string FotoCaptionRequired = "O Caption da foto do relatório é obrigatório";
    }
}
