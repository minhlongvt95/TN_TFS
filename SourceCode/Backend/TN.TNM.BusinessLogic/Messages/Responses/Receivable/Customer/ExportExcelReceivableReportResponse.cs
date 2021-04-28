namespace TN.TNM.BusinessLogic.Messages.Responses.Receivable.Customer
{
    public class ExportExcelReceivableReportResponse : BaseResponse
    {
        public byte[] ExcelFile { get; set; }
        public string CustomerName { get; set; }
    }
}
