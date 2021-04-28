namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class DownloadTemplateSerialResult : BaseResult
    {
        public byte[] ExcelFile { get; set; }
        public string NameFile { get; set; }
    }
}
