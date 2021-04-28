namespace TN.TNM.DataAccess.Messages.Results.Users
{
    public class GetUserProfileResult : BaseResult
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string AvatarUrl { get; set; }
    }
}
