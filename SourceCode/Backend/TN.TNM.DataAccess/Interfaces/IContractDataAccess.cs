using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Contract;
using TN.TNM.DataAccess.Messages.Results.Contract;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IContractDataAccess
    {
        GetMaterDataContractResult GetMasterDataContract(GetMasterDataContractParameter parameter);
        CreateOrUpdateContractResult CreateOrUpdateContract(CreateOrUpdateContractParameter parameter);
        CreateCloneContractResult CreateCloneContract(CreateCloneContractParameter parameter);
        GetMasterDataSearchContractResult GetMasterDataSearchContract(GetMasterDataSearchContractParameter parameter);
        SearchContractResult SearchContract(SearchContractParameter parameter);
        ChangeContractStatusResult ChangeContractStatus(ChangeContractStatusParameter parameter);
        GetMasterDataDashBoardResult GetMasterDataDashBoard(GetMasterDataDashBoardParameter parameter);
        GetListMainContractResult GetListMainContract(GetListMainContractParameter parameter);
        DeleteContractResult DeleteContract(DeleteContractParamter paramter);
        UploadFileResult UploadFile(UploadFileParameter parameter);
        DeleteFileResult DeleteFile(DeleteFileParameter parameter);
    }
}
