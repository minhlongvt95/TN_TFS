using TN.TNM.DataAccess.Messages.Parameters.Admin;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin
{
    public class GetMenuByModuleCodeRequest : BaseRequest<GetMenuByModuleCodeParameter>
    {
        public string ModuleCode { get; set; }
        
        public override GetMenuByModuleCodeParameter ToParameter()
        {
            return new GetMenuByModuleCodeParameter
            {
                UserId = this.UserId,
                ModuleCode = this.ModuleCode
            };
        }
    }
}
