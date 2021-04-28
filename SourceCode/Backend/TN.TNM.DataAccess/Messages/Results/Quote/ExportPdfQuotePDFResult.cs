namespace TN.TNM.DataAccess.Messages.Results.Quote
{
    public class ExportPdfQuotePDFResult:BaseResult
    {
        public byte[] QuotePdf { get; set; }
        public string Code { get; set; }
    }
}
