using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Contact;
using TN.TNM.BusinessLogic.Messages.Requests.Contact;
using TN.TNM.BusinessLogic.Messages.Responses.Contact;

namespace TN.TNM.Api.Controllers
{
    public class ContactController : Controller
    {
        private readonly IContact _iContact;
        public ContactController(IContact iContact)
        {
            this._iContact = iContact;
        }

        /// <summary>
        /// Create a new contact
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/contact/create")]
        [Authorize(Policy = "Member")]
        public CreateContactResponse CreateContact([FromBody]CreateContactRequest request)
        {
            return this._iContact.CreateContact(request);
        }

        /// <summary>
        /// Get all Contact by ObjectType
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/contact/getAllContactByObjectType")]
        [Authorize(Policy = "Member")]
        public GetAllContactByObjectTypeResponse GetAllContactByObjectType([FromBody]GetAllContactByObjectTypeRequest request)
        {
            return this._iContact.GetAllContactByObjectType(request);
        }

        /// <summary>
        /// Search contact
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/contact/searchContact")]
        [Authorize(Policy = "Member")]
        public SearchContactResponse SearchContact([FromBody]SearchContactRequest request)
        {
            return this._iContact.SearchContact(request);
        }

        /// <summary>
        /// Get contact by Id
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/contact/getContactById")]
        [Authorize(Policy = "Member")]
        public GetContactByIdResponse GetContactById([FromBody]GetContactByIdRequest request)
        {
            return this._iContact.GetContactById(request);
        }

        /// <summary>
        /// Get contact by ObjectId
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/contact/getContactByObjectId")]
        [Authorize(Policy = "Member")]
        public GetContactByIdResponse GetContactByObjectId([FromBody]GetContactByObjectIdRequest request)
        {
            return this._iContact.GetContactByObjectId(request);
        }
        /// <summary>
        /// Edit contact by Id
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/contact/editContactById")]
        [Authorize(Policy = "Member")]
        public EditContactByIdResponse EditContactById([FromBody]EditContactByIdRequest request)
        {
            return this._iContact.EditContactById(request);
        }

        /// <summary>
        /// Delete contact by Id
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/contact/deleteContactById")]
        [Authorize(Policy = "Member")]
        public DeleteContactByIdResponse DeleteContactById([FromBody]DeleteContactByIdRequest request)
        {
            return this._iContact.DeleteContactById(request);
        }

        //
        /// <summary>
        /// Update Personal Customer Contact
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/contact/updatePersonalCustomerContact")]
        [Authorize(Policy = "Member")]
        public UpdatePersonalCustomerContactResponse UpdatePersonalCustomerContact([FromBody]UpdatePersonalCustomerContactRequest request)
        {
            return this._iContact.UpdatePersonalCustomerContact(request);
        }

        //
        [HttpPost]
        [Route("api/contact/getAddressByObject")]
        [Authorize(Policy = "Member")]
        public GetAddressByObjectResponse GetAddressByObject([FromBody]GetAddressByObjectRequest request)
        {
            return this._iContact.GetAddressByObject(request);
        }
    }
}
