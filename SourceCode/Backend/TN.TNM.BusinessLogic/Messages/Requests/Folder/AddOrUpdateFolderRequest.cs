using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Folder;
using TN.TNM.DataAccess.Messages.Parameters.Folder;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.BusinessLogic.Messages.Requests.Folder
{
    public class AddOrUpdateFolderRequest : BaseRequest<AddOrUpdateFolderParameter>
    {
        public List<FolderModel> ListFolder { get; set; }
        public override AddOrUpdateFolderParameter ToParameter()
        {
            var parameter = new AddOrUpdateFolderParameter()
            {
                ListFolder = new List<FolderEntityModel>(),
                UserId = UserId
            };
            ListFolder.ForEach(item =>
            {
                parameter.ListFolder.Add(item.ToEntityModel());
            });

            return parameter;
        }
    }
}
