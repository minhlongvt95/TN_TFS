using TN.TNM.DataAccess.Messages.Parameters.File;

namespace TN.TNM.BusinessLogic.Messages.Requests.File
{
    public class UploadImageRequest: BaseRequest<UploadImageParameter>
    {
        public string Base64Img { get; set; }
        public string ImageName { get; set; }

        public override UploadImageParameter ToParameter()
        {
            return new UploadImageParameter()
            {
                Base64Img = Base64Img,
                ImageName = ImageName
            };
        }
    }
}
