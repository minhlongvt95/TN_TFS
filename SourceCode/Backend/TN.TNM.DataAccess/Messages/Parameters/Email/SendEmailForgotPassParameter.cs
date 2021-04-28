namespace TN.TNM.DataAccess.Messages.Parameters.Email
{
    public class SendEmailForgotPassParameter : BaseParameter
    {
        public string EmailAddress { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
    }
}
