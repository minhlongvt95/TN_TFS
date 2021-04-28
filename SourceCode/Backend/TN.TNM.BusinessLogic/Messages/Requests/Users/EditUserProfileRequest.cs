using TN.TNM.DataAccess.Messages.Parameters.Users;

namespace TN.TNM.BusinessLogic.Messages.Requests.Users
{
    public class EditUserProfileRequest : BaseRequest<EditUserProfileParameter>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string AvatarUrl { get; set; }
        public override EditUserProfileParameter ToParameter()
        {
            return new EditUserProfileParameter()
            {
                UserId = UserId,
                Email = Email,
                AvatarUrl = AvatarUrl,
                FirstName = FirstName,
                LastName = LastName,
                Username = Username
            };
        }
    }
}
