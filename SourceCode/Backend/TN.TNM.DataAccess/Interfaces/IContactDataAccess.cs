using TN.TNM.DataAccess.Messages.Parameters.Contact;
using TN.TNM.DataAccess.Messages.Results.Contact;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IContactDataAccess
    {
        /// <summary>
        /// CreateContact
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        CreateContactResult CreateContact(CreateContactParameter parameter);

        /// <summary>
        /// Get all contact by ObjectType
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        GetAllContactByObjectTypeResult GetAllContactByObjectType(GetAllContactByObjectTypeParameter parameter);

        /// <summary>
        /// Lấy ra tất cả các Contact theo điều kiện search
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        SearchContactResult SearchContact(SearchContactParameter parameter);

        GetContactByIdResult GetContactById(GetContactByIdParameter parameter);
        GetContactByIdResult GetContactByObjectId(GetContactByObjectIdParameter parameter);
        DeleteContactByIdResult DeleteContactById(DeleteContactByIdParameter parameter);
        EditContactByIdResult EditContactById(EditContactByIdParameters byIdParameters);

        UpdatePersonalCustomerContactResult UpdatePersonalCustomerContact(
            UpdatePersonalCustomerContactParameter parameter);

        GetAddressByObjectResult GetAddressByObject(GetAddressByObjectParameter parameter);
    }
}
