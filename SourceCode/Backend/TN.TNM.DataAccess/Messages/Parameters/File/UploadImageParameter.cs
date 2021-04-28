namespace TN.TNM.DataAccess.Messages.Parameters.File
{
    public class UploadImageParameter : BaseParameter
    {
        public string Base64Img { get; set; }
        public string ImageName { get; set; }
    }
}
