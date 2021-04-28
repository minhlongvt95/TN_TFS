using TN.TNM.DataAccess.Messages.Parameters.Contact;

namespace TN.TNM.BusinessLogic.Messages.Requests.Contact
{
    public class GetAllContactByObjectTypeRequest : BaseRequest<GetAllContactByObjectTypeParameter>
    {
        public string ObjectType { get; set; }
        public override GetAllContactByObjectTypeParameter ToParameter() => new GetAllContactByObjectTypeParameter
        {
            ObjectType = ObjectType
        };
    }
}
