using TN.TNM.BusinessLogic.Messages.Requests.File;
using TN.TNM.BusinessLogic.Messages.Responses.File;

namespace TN.TNM.BusinessLogic.Interfaces.File
{
    public interface IUploadFile
    {
        UploadImageResponse UploadImage(UploadImageRequest request);
    }
}
