using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Parameters.Folder
{
    public class DeleteFolderParameter : BaseParameter
    {
        public FolderEntityModel Folder { get; set; }
    }
}
