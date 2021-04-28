using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Contract;
using TN.TNM.BusinessLogic.Models.Folder;
using TN.TNM.BusinessLogic.Models.Quote;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Messages.Parameters.Contract;
using TN.TNM.DataAccess.Models.Contract;

namespace TN.TNM.BusinessLogic.Messages.Requests.Contract
{
    public class CreateOrUpdateContractRequest : BaseRequest<CreateOrUpdateContractParameter>
    {
        public ContractModel Contract { get; set; }
        public List<AdditionalInformationModel> ListAdditionalInformation { get; set; }
        public List<ContractDetailModel> ContractDetail { get; set; }
        public List<ContractCostDetailEntityModel> ListContractCost { get; set; }
        public List<IFormFile> ListFormFile { get; set; }
        public List<FileInFolderModel> ListFile { get; set; }
        public Boolean IsCreate { get; set; }

        public override CreateOrUpdateContractParameter ToParameter()
        {
            var parameter = new CreateOrUpdateContractParameter()
            {
                Contract = Contract.ToEntity(),
                UserId = UserId,
                ContractDetail = new List<ContractDetailEntityModel>(),
                ListAdditionalInformation = new List<AdditionalInformation>(),
                ListContractCost = new List<ContractCostDetailEntityModel>(),
                ListFormFile = ListFormFile,
                ListFile = new List<DataAccess.Models.Folder.FileInFolderEntityModel>(),
                IsCreate = IsCreate
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

            ListFile = ListFile ?? new List<FileInFolderModel>();
            ListFile.ForEach(item =>
            {
                var temp = item.ToEntityModel();
                parameter.ListFile.Add(temp);
            });

            return parameter;
        }
    }
}
