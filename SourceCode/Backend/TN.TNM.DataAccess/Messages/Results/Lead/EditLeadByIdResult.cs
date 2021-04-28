namespace TN.TNM.DataAccess.Messages.Results.Lead
{
    public class EditLeadByIdResult : BaseResult
    {
        public bool IsChangePic { get; set; }
        public bool IsChangePotential { get; set; }
        public bool IsChangeStatus { get; set; }
        public string PicName { get; set; }
        public string Potential { get; set; }
        public string StatusName { get; set; }
        public DataAccess.Models.Email.SendEmailEntityModel SendEmailEntityModel { get; set; }
    }
}
