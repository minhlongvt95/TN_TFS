using TN.TNM.BusinessLogic.Messages.Requests.Contact;
using TN.TNM.BusinessLogic.Messages.Responses.Contact;

namespace TN.TNM.BusinessLogic.Interfaces.Contact
{
    /// <summary>
    /// TODO: Thuc hien cac chuc nang lien quan den Contact
    /// </summary>
    public interface IContact
    {
        /// <summary>
        /// TODO: Tao 1 Contact moi
        /// </summary>
        /// <param name="request">Cac parameter truyen vao</param>
        /// <returns></returns>
        CreateContactResponse CreateContact(CreateContactRequest request);

        /// <summary>
        /// TODO: Lấy ra tất cả Contact theo ObjectType
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetAllContactByObjectTypeResponse GetAllContactByObjectType(GetAllContactByObjectTypeRequest request);

        /// <summary>
        /// TODO: Lấy ra tất cả Contact theo điều kiện search
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        SearchContactResponse SearchContact(SearchContactRequest request);

        GetContactByIdResponse GetContactById(GetContactByIdRequest request);
        GetContactByIdResponse GetContactByObjectId(GetContactByObjectIdRequest request);
        EditContactByIdResponse EditContactById(EditContactByIdRequest request);
        DeleteContactByIdResponse DeleteContactById(DeleteContactByIdRequest request);

        UpdatePersonalCustomerContactResponse UpdatePersonalCustomerContact(
            UpdatePersonalCustomerContactRequest request);

        GetAddressByObjectResponse GetAddressByObject(GetAddressByObjectRequest request);
    }
}
