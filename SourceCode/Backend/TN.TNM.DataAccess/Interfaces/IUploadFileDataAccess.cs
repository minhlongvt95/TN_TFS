using TN.TNM.DataAccess.Messages.Parameters.File;
using TN.TNM.DataAccess.Messages.Results.File;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IUploadFileDataAccess
    {
        UploadImageResult UploadImage(UploadImageParameter parameter);
    }
}
