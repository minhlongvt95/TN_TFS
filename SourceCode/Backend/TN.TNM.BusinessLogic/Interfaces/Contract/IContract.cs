using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Messages.Requests.Contract;
using TN.TNM.BusinessLogic.Messages.Responses.Contract;

namespace TN.TNM.BusinessLogic.Interfaces.Contract
{
    public interface IContract
    {
        GetMasterDataContractResponse GetMasterDataContract(GetMasterDataContractRequest request);
        CreateCloneContractResponse CreateCloneContract(CreateCloneContractRequest request);
        CreateOrUpdateContractRespone CreateOrUpdateContract(CreateOrUpdateContractRequest request);
        GetListMainContractResponses GetListMainContract(GetListMainContractRequests request);
        GetMasterDataSearchContractResponse GetMasterDataSearchContract(GetMasterDataSearchContractRequest request);
        SearchContractResponse SearchContract(SearchContractRequest request);
        ChangeContractStatusResponse ChangeContractStatus(ChangeContractStatusRequest request);
        GetMasterDataDashboardContractResponse GetMasterDataDashboardContract(GetMasterDataDashboardContractRequest request);
        DeleteContractResponse DeleteContract(DeleteContractRequest request);
        UploadFileResponse UploadFile(UploadFileRequest request);
        DeleteFileResponse DeleteFile(DeleteFileRequest request);
    }
}
