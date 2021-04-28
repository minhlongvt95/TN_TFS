using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.Admin.Organization;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.Organization;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Organization;
using TN.TNM.BusinessLogic.Models.Admin;
using TN.TNM.Common;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.Admin.Organization
{
    public class OrganizationFactory : BaseFactory, IOrganization
    {
        private IOrganizationDataAccess iOrganizationDataAccess;
        public OrganizationFactory(IOrganizationDataAccess _iOrganizationDataAccess, ILogger<OrganizationFactory> _logger)
        {
            this.iOrganizationDataAccess = _iOrganizationDataAccess;
            this.logger = _logger;
        }

        public GetAllOrganizationResponse GetAllOrganization(GetAllOrganizationRequest request)
        {
            try
            {
                this.logger.LogInformation("Get All Organization");
                var parameter = request.ToParameter();
                var result = iOrganizationDataAccess.GetAllOrganization(parameter);
                var response = new GetAllOrganizationResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    OrganizationList = new List<OrganizationModel>(),
                    ListAll = new List<OrganizationModel>(),
                    ListGeographicalArea = result.ListGeographicalArea,
                    ListProvince = result.ListProvince,
                    ListDistrict = result.ListDistrict,
                    ListWard = result.ListWard,
                    ListSatellite = result.ListSatellite
                };

                result.OrganizationList.ForEach(orgEntity =>
                {
                    response.OrganizationList.Add(new OrganizationModel(orgEntity));
                });

                result.ListAll.ForEach(orgEntity =>
                {
                    response.ListAll.Add(new OrganizationModel(orgEntity));
                });
                
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
                return new GetAllOrganizationResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public CreateOrganizationResponse CreateOrganization(CreateOrganizationRequest request)
        {
            try
            {
                this.logger.LogInformation("Create Organization");
                var parameter = request.ToParameter();
                var result = iOrganizationDataAccess.CreateOrganization(parameter);
                var response = new CreateOrganizationResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message,
                    CreatedOrgId = result.CreatedOrgId
            };
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
                return new CreateOrganizationResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public GetOrganizationByIdResponse GetOrganizationById(GetOrganizationByIdRequest request)
        {
            try
            {
                this.logger.LogInformation("Get All Organization");
                var parameter = request.ToParameter();
                var result = iOrganizationDataAccess.GetOrganizationById(parameter);
                var response = new GetOrganizationByIdResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    Organization = new OrganizationModel(result.Organization),
                    MessageCode = result.Message
                };
                
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
                return new GetOrganizationByIdResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public EditOrganizationByIdResponse EditOrganizationById(EditOrganizationByIdRequest request)
        {
            try
            {
                this.logger.LogInformation("Edit Organization");
                var parameter = request.ToParameter();
                var result = iOrganizationDataAccess.EditOrganizationById(parameter);
                var response = new EditOrganizationByIdResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
                return new EditOrganizationByIdResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = CommonMessage.Organization.EDIT_FAIL
                };
            }
        }

        public DeleteOrganizationByIdResponse DeleteOrganizationById(DeleteOrganizationByIdRequest request)
        {
            try
            {
                this.logger.LogInformation("Edit Organization");
                var parameter = request.ToParameter();
                var result = iOrganizationDataAccess.DeleteOrganizationById(parameter);
                var response = new DeleteOrganizationByIdResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.Accepted : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
                return new DeleteOrganizationByIdResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = CommonMessage.Organization.DELETE_FAIL
                };
            }
        }

        public GetAllOrganizationCodeResponse GetAllOrganizationCode(GetAllOrganizationCodeRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Organization code");
                var parameter = request.ToParameter();
                var result = iOrganizationDataAccess.GetAllOrganizationCode(parameter);
                var response = new GetAllOrganizationCodeResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.Accepted : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    OrgCodeList = result.OrgCodeList
                };

                return response;
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
                return new GetAllOrganizationCodeResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = CommonMessage.Organization.GETALL_FAIL
                };
            }
        }

        public GetFinancialindependenceOrgResponse GetFinancialindependenceOrg(GetFinancialindependenceOrgRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Financial Independence Organization");
                var parameter = request.ToParameter();
                var result = iOrganizationDataAccess.GetFinancialindependenceOrg(parameter);
                var response = new GetFinancialindependenceOrgResponse()
                {
                    StatusCode = HttpStatusCode.Accepted,
                    ListOrg = new List<OrganizationModel>()
                };

                result.ListOrg.ForEach(org => {
                    response.ListOrg.Add(new OrganizationModel(org));
                });

                return response;
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
                return new GetFinancialindependenceOrgResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = CommonMessage.Organization.GETALL_FAIL
                };
            }
        }

        public GetChildrenOrganizationByIdResponse GetChildrenOrganizationById(GetChildrenOrganizationByIdRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Children Organization By Id");
                var parameter = request.ToParameter();
                var result = iOrganizationDataAccess.GetChildrenOrganizationById(parameter);
                var response = new GetChildrenOrganizationByIdResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    listOrganization = new List<OrganizationModel>(),
                    organizationParent = result.organizationParent,
                    isManager = result.isManager
                };
                result.listOrganization.ForEach(orgEntity =>
                {
                    response.listOrganization.Add(new OrganizationModel(orgEntity));
                });
                response.MessageCode = result.Message;
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
                return new GetChildrenOrganizationByIdResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public GetOrganizationByEmployeeIdResponse GetOrganizationByEmployeeId(GetOrganizationByEmployeeIdRequest request)
        {
            try
            {
                this.logger.LogInformation("Get All Organization");
                var parameter = request.ToParameter();
                var result = iOrganizationDataAccess.GetOrganizationByEmployeeId(parameter);
                var response = new GetOrganizationByEmployeeIdResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    Organization = new OrganizationModel(result.Organization),
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
                return new GetOrganizationByEmployeeIdResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public GetChildrenByOrganizationIdResponse GetChildrenByOrganizationId(GetChildrenByOrganizationIdRequest request)
        {
            try
            {
                this.logger.LogInformation("Get All Organization");
                var parameter = request.ToParameter();
                var result = iOrganizationDataAccess.GetChildrenByOrganizationId(parameter);
                var response = new GetChildrenByOrganizationIdResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    OrganizationList = new List<OrganizationModel>()
                };
                result.OrganizationList.ForEach(orgEntity =>
                {
                    response.OrganizationList.Add(new OrganizationModel(orgEntity));
                });
                response.MessageCode = result.Message;
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
                return new GetChildrenByOrganizationIdResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public UpdateOrganizationByIdResponse UpdateOrganizationById(UpdateOrganizationByIdRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iOrganizationDataAccess.UpdateOrganizationById(parameter);
                var response = new UpdateOrganizationByIdResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };
                
                return response;
            }
            catch (Exception e)
            {
                return new UpdateOrganizationByIdResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetOrganizationByUserResponse GetOrganizationByUser(GetOrganizationByUserRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iOrganizationDataAccess.GetOrganizationByUser(parameter);
                var response = new GetOrganizationByUserResponse
                {
                    ListOrganization = result.ListOrganization,
                    ListValidSelectionOrganization = result.ListValidSelectionOrganization,
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetOrganizationByUserResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }
    }
}
