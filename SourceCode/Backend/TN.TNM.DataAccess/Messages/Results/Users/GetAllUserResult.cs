using System.Collections.Generic;
using TN.TNM.DataAccess.Models.User;

namespace TN.TNM.DataAccess.Messages.Results.Users
{
    public class GetAllUserResult : BaseResult
    {
      public List<UserEntityModel> lstUserEntityModel { get; set; }
    }
}
