using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.Contact;
using TN.TNM.BusinessLogic.Messages.Requests.Contact;
using TN.TNM.BusinessLogic.Messages.Responses.Contact;
using TN.TNM.BusinessLogic.Models.Contact;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.Common;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.Contact
{
    public class ContactFactory : BaseFactory, IContact
    {
        private IContactDataAccess iContactDataAccess;

        public ContactFactory(IContactDataAccess _iContactDataAccess, ILogger<ContactFactory> _logger)
        {
            this.iContactDataAccess = _iContactDataAccess;
            this.logger = _logger;
        }

        public CreateContactResponse CreateContact(CreateContactRequest request)
        {
            try
            {
                this.logger.LogInformation("Create Contact");
                var parameter = request.ToParameter();
                var result = iContactDataAccess.CreateContact(parameter);
                var response = new CreateContactResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListContact = new List<CustomerOtherContactBusinessModel>()
                };

                result.ListContact.ForEach(item =>
                {
                    var contact = new CustomerOtherContactBusinessModel();
                    contact.ContactId = item.ContactId;
                    contact.ObjectId = item.ObjectId;
                    contact.ObjectType = item.ObjectType;
                    contact.FirstName = item.FirstName;
                    contact.LastName = item.LastName;
                    contact.ContactName = item.ContactName;
                    contact.Role = item.Role;
                    contact.Phone = item.Phone;
                    contact.Email = item.Email;
                    contact.Gender = item.Gender;
                    contact.GenderName = item.GenderName;
                    contact.Other = item.Other;
                    contact.CreatedDate = item.CreatedDate;
                    contact.DateOfBirth = item.DateOfBirth;
                    contact.Address = item.Address;

                    response.ListContact.Add(contact);
                });

                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new CreateContactResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetAllContactByObjectTypeResponse GetAllContactByObjectType(GetAllContactByObjectTypeRequest request)
        {
            try
            {
                this.logger.LogInformation("Get all Contact by ObjectType");
                var parameter = request.ToParameter();
                var result = iContactDataAccess.GetAllContactByObjectType(parameter);
                var response = new GetAllContactByObjectTypeResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    ContactList = new List<ContactModel>()
                };
                result.ContactList.ForEach(contactEntity =>
                {
                    response.ContactList.Add(new ContactModel(contactEntity));
                });
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new GetAllContactByObjectTypeResponse
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public SearchContactResponse SearchContact(SearchContactRequest request)
        {
            try
            {
                this.logger.LogInformation("Search Contact");
                var parameter = request.ToParameter();
                var result = iContactDataAccess.SearchContact(parameter);
                var response = new SearchContactResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    ContactList = new List<ContactModel>()
                };
                
                result.ContactList.ForEach(leadEntityModel =>
                {
                    //response.ContactList.Add(new ContactModel(leadEntityModel));
                });
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new SearchContactResponse()
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public GetContactByIdResponse GetContactById(GetContactByIdRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Contact by Id");
                if (request != null)
                {
                    var parameter = request.ToParameter();
                    var result = iContactDataAccess.GetContactById(parameter);
                    var response = new GetContactByIdResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        Contact = new ContactModel(result.Contact),
                        FullAddress = result.FullAddress,
                    };
                    return response;
                }
                else {
                    return new GetContactByIdResponse() {
                        StatusCode = HttpStatusCode.OK,
                        Contact = null,
                        FullAddress =string.Empty,
                    };
                }
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new GetContactByIdResponse()
                {
                    MessageCode = CommonMessage.Contact.GET_FAIL,
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public GetContactByIdResponse GetContactByObjectId(GetContactByObjectIdRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Contact by ObjectId");
                var parameter = request.ToParameter();
                var result = iContactDataAccess.GetContactByObjectId(parameter);
                var response = new GetContactByIdResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    Contact = new ContactModel(result.Contact),
                    FullAddress = result.FullAddress,
                };
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new GetContactByIdResponse()
                {
                    MessageCode = CommonMessage.Contact.GET_FAIL,
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }
        public EditContactByIdResponse EditContactById(EditContactByIdRequest request)
        {
            try
            {
                this.logger.LogInformation("Edit Contact by Id");
                var parameter = request.ToParameter();
                var result = iContactDataAccess.EditContactById(parameter);
                var response = new EditContactByIdResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new EditContactByIdResponse()
                {
                    MessageCode = CommonMessage.Contact.EDIT_FAIL,
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public DeleteContactByIdResponse DeleteContactById(DeleteContactByIdRequest request)
        {
            try
            {
                this.logger.LogInformation("Delete Contact");
                var parameter = request.ToParameter();
                var result = iContactDataAccess.DeleteContactById(parameter);
                var response = new DeleteContactByIdResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListContact = new List<CustomerOtherContactBusinessModel>()
                };

                result.ListContact.ForEach(item =>
                {
                    var contact = new CustomerOtherContactBusinessModel();
                    contact.ContactId = item.ContactId;
                    contact.ObjectId = item.ObjectId;
                    contact.ObjectType = item.ObjectType;
                    contact.FirstName = item.FirstName;
                    contact.LastName = item.LastName;
                    contact.ContactName = item.ContactName;
                    contact.Role = item.Role;
                    contact.Phone = item.Phone;
                    contact.Email = item.Email;
                    contact.Gender = item.Gender;
                    contact.GenderName = item.GenderName;
                    contact.Other = item.Other;
                    contact.CreatedDate = item.CreatedDate;

                    response.ListContact.Add(contact);
                });

                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new DeleteContactByIdResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public UpdatePersonalCustomerContactResponse UpdatePersonalCustomerContact(UpdatePersonalCustomerContactRequest request)
        {
            try
            {
                this.logger.LogInformation("Update Personal Customer Contact");
                var parameter = request.ToParameter();
                var result = iContactDataAccess.UpdatePersonalCustomerContact(parameter);
                var response = new UpdatePersonalCustomerContactResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                return new UpdatePersonalCustomerContactResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetAddressByObjectResponse GetAddressByObject(GetAddressByObjectRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iContactDataAccess.GetAddressByObject(parameter);
                var response = new GetAddressByObjectResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    Address = result.Address
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetAddressByObjectResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }
    }
}
