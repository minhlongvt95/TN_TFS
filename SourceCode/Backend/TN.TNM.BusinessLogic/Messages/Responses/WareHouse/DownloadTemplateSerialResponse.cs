namespace TN.TNM.BusinessLogic.Messages.Responses.WareHouse
{
    public class DownloadTemplateSerialResponse : BaseResponse
    {
        public byte[] ExcelFile { get; set; }
        public string NameFile { get; set; }
    }
}
