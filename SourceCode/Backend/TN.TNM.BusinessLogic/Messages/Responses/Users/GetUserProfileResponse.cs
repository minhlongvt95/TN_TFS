namespace TN.TNM.BusinessLogic.Messages.Responses.Users
{
    public class GetUserProfileResponse : BaseResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string AvatarUrl { get; set; }
    }
}
