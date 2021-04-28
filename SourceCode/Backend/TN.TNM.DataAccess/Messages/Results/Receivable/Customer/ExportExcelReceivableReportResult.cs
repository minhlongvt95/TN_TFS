namespace TN.TNM.DataAccess.Messages.Results.Receivable.Customer
{
    public class ExportExcelReceivableReportResult : BaseResult
    {
        public byte[] ExcelFile { get; set; }
        public string CustomerName { get; set; }
    }
}
