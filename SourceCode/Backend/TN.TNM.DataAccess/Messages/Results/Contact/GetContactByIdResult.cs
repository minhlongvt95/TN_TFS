namespace TN.TNM.DataAccess.Messages.Results.Contact
{
    public class GetContactByIdResult : BaseResult
    {
        public Databases.Entities.Contact Contact { get; set; }
        public string FullAddress { get; set; }
    }
}
