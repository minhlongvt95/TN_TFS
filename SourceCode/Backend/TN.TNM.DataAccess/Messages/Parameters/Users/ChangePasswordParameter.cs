namespace TN.TNM.DataAccess.Messages.Parameters.Users
{
    public class ChangePasswordParameter : BaseParameter
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
