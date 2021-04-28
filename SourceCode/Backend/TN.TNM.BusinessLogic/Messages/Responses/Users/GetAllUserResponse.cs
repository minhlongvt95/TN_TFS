using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Admin;

namespace TN.TNM.BusinessLogic.Messages.Responses.Users
{
    public class GetAllUserResponse : BaseResponse
    {
        public List<UserModel> lstUserEntityModel { get; set; }
    }
}
