using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using TN.TNM.DataAccess.Models.Contract;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Parameters.Contract
{
    public class CreateCloneContractParameter : BaseParameter
    {
        public Databases.Entities.Contract Contract { get; set; }
        public List<ContractDetailEntityModel> ContractDetail { get; set; }
        public List<Databases.Entities.AdditionalInformation> ListAdditionalInformation { get; set; }
        public List<ContractCostDetailEntityModel> ListContractCost { get; set; }
        public Guid ContractId { get; set; }
    }
}
