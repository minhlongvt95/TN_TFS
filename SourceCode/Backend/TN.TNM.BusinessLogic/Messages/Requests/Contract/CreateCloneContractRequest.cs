using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using TN.TNM.BusinessLogic.Models.Contract;
using TN.TNM.BusinessLogic.Models.Folder;
using TN.TNM.BusinessLogic.Models.Quote;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Messages.Parameters.Contract;
using TN.TNM.DataAccess.Models.Contract;

namespace TN.TNM.BusinessLogic.Messages.Requests.Contract
{
    public class CreateCloneContractRequest : BaseRequest<CreateCloneContractParameter>
    {
        public ContractModel Contract { get; set; }
        public List<AdditionalInformationModel> ListAdditionalInformation { get; set; }
        public List<ContractDetailModel> ContractDetail { get; set; }
        public List<ContractCostDetailEntityModel> ListContractCost { get; set; }
        public Guid ContractId { get; set; }

        public override CreateCloneContractParameter ToParameter()
        {
            var parameter = new CreateCloneContractParameter()
            {
                Contract = Contract.ToEntity(),
                UserId = UserId,
                ContractDetail = new List<ContractDetailEntityModel>(),
                ListAdditionalInformation = new List<AdditionalInformation>(),
                ListContractCost = new List<ContractCostDetailEntityModel>(),
                ContractId = ContractId
            };

            ContractDetail = ContractDetail ?? new List<ContractDetailModel>();
            ContractDetail.ForEach(item =>
            {
                var objectContractDetailEntityModel = item.ToEntityModel();
                var contractAttributes = new List<ContractDetailProductAttributeEntityModel>();
                if (item.ContractProductDetailProductAttributeValue != null)
                {
                    item.ContractProductDetailProductAttributeValue.ForEach(itemAttribute =>
                    {
                        contractAttributes.Add(itemAttribute.ToEntityModel());
                    });
                    objectContractDetailEntityModel.ContractProductDetailProductAttributeValue = contractAttributes;
                }
                else
                {
                    item.ContractProductDetailProductAttributeValue = new List<ContractDetailProductAttributeModel>();
                    objectContractDetailEntityModel.ContractProductDetailProductAttributeValue = contractAttributes;
                }
                parameter.ContractDetail.Add(objectContractDetailEntityModel);

            });

            ListContractCost = ListContractCost ?? new List<ContractCostDetailEntityModel>();
            parameter.ListContractCost = ListContractCost;

            ListAdditionalInformation = ListAdditionalInformation ?? new List<AdditionalInformationModel>();
            ListAdditionalInformation.ForEach(item =>
            {
                parameter.ListAdditionalInformation.Add(item.ToEntity());
            });

            return parameter;
        }
    }
}
