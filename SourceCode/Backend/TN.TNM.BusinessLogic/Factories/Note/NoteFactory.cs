using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.Note;
using TN.TNM.BusinessLogic.Messages.Requests.Note;
using TN.TNM.BusinessLogic.Messages.Responses.Note;
using TN.TNM.BusinessLogic.Models.Note;
using TN.TNM.Common;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.Note
{
    public class NoteFactory : BaseFactory, INote
    {
        private INoteDataAccess iNoteDataAccess;
        public NoteFactory(INoteDataAccess _iNoteDataAccess, ILogger<NoteFactory> _logger)

        {
            this.iNoteDataAccess = _iNoteDataAccess;
            this.logger = _logger;
        }

        /// <summary>
        /// CreateNote
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public CreateNoteResponse CreateNote(CreateNoteRequest request)
        {
            try
            {
                this.logger.LogInformation("Create Note");
                var parameter = request.ToParameter();
                var result = iNoteDataAccess.CreateNote(parameter);
                var response = new CreateNoteResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new CreateNoteResponse
                {
                    MessageCode = CommonMessage.Note.CREATE_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        /// <summary>
        /// DisableNote
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public DisableNoteResponse DisableNote(DisableNoteRequest request)
        {
            try
            {
                this.logger.LogInformation("Disable/Delete Note");
                var parameter = request.ToParameter();
                var result = iNoteDataAccess.DisableNote(parameter);
                var response = new DisableNoteResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new DisableNoteResponse
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public CreateNoteAndNoteDocumentResponse CreateNoteAndNoteDocument(CreateNoteAndNoteDocumentRequest request)
        {
            try
            {
                this.logger.LogInformation("Create Note and Note Document");
                var parameter = request.ToParameter();
                var result = iNoteDataAccess.CreateNoteAndNoteDocument(parameter);
                var response = new CreateNoteAndNoteDocumentResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new CreateNoteAndNoteDocumentResponse
                {
                    MessageCode = CommonMessage.Note.DISABLE_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public EditNoteByIdResponse EditNoteById(EditNoteByIdRequest request)
        {
            try
            {
                this.logger.LogInformation("Edit Note by Id");
                var parameter = request.ToParameter();
                var result = iNoteDataAccess.EditNoteById(parameter);
                var response = new EditNoteByIdResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new EditNoteByIdResponse
                {
                    MessageCode = CommonMessage.Note.EDIT_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public SearchNoteResponse SearchNote(SearchNoteRequest request)
        {
            try
            {
                this.logger.LogInformation("Search Note by parameter");
                var parameter = request.ToParameter();
                var result = iNoteDataAccess.SearchNote(parameter);
                var response = new SearchNoteResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    NoteList = new List<NoteModel>(),
                    MessageCode = result.Message
                };
                result.NoteList?.ForEach(n =>
                {
                    response.NoteList.Add(new NoteModel(n));
                });
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new SearchNoteResponse
                {
                    MessageCode = CommonMessage.Note.SEARCH_NOTE_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public CreateNoteForCustomerDetailResponse CreateNoteForCustomerDetail(CreateNoteForCustomerDetailRequest request)
        {
            try
            {
                this.logger.LogInformation("Create Note For Customer Detail");
                var parameter = request.ToParameter();
                var result = iNoteDataAccess.CreateNoteForCustomerDetail(parameter);
                var response = new CreateNoteForCustomerDetailResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListNote = new List<NoteModel>()
                };

                result.ListNote.ForEach(item =>
                {
                    response.ListNote.Add(new NoteModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new CreateNoteForCustomerDetailResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public CreateNoteForLeadDetailResponse CreateNoteForLeadDetail(CreateNoteForLeadDetailRequest request)
        {
            try
            {
                this.logger.LogInformation("Create Note For Lead Detail");
                var parameter = request.ToParameter();
                var result = iNoteDataAccess.CreateNoteForLeadDetail(parameter);
                var response = new CreateNoteForLeadDetailResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListNote = new List<NoteModel>()
                };

                result.ListNote.ForEach(item =>
                {
                    response.ListNote.Add(new NoteModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new CreateNoteForLeadDetailResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public CreateNoteForOrderDetailResponse CreateNoteForOrderDetail(CreateNoteForOrderDetailRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iNoteDataAccess.CreateNoteForOrderDetail(parameter);
                var response = new CreateNoteForOrderDetailResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    NoteId = result.NoteId,
                    ListNote = new List<NoteModel>()
                };

                result.ListNote.ForEach(item =>
                {
                    response.ListNote.Add(new NoteModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new CreateNoteForOrderDetailResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public CreateNoteForQuoteDetailResponse CreateNoteForQuoteDetail(CreateNoteForQuoteDetailRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iNoteDataAccess.CreateNoteForQuoteDetail(parameter);
                var response = new CreateNoteForQuoteDetailResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    listNote = new List<NoteModel>()
                };

                result.ListNote.ForEach(item =>
                {
                    response.listNote.Add(new NoteModel(item));
                });

                return response;
            }catch(Exception e)
            {
                return new CreateNoteForQuoteDetailResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public CreateNoteForSaleBiddingDetailResponse CreateNoteForSaleBiddingDetail(CreateNoteForSaleBiddingDetailRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iNoteDataAccess.CreateNoteForSaleBiddingDetail(parameter);
                var response = new CreateNoteForSaleBiddingDetailResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    listNote = new List<NoteModel>()
                };

                result.ListNote.ForEach(item =>
                {
                    response.listNote.Add(new NoteModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new CreateNoteForSaleBiddingDetailResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }
        public CreateNoteForContractResponse CreateNoteForContract(CreateNoteForContractRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iNoteDataAccess.CreateNoteForContract(parameter);
                var response = new CreateNoteForContractResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListNote = new List<NoteModel>()
                };

                result.ListNote.ForEach(item =>
                {
                    response.ListNote.Add(new NoteModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new CreateNoteForContractResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public CreateNoteForBillSaleDetailResponse CreateNoteForBillSaleDetail(CreateNoteForBillSaleDetailRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iNoteDataAccess.CreateNoteForBillSaleDetail(parameter);
                var response = new CreateNoteForBillSaleDetailResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    listNote = new List<NoteModel>()
                };

                result.ListNote?.ForEach(item =>
                {
                    response.listNote.Add(new NoteModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new CreateNoteForBillSaleDetailResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public CreateNoteForObjectResponse CreateNoteForObject(CreateNoteForObjectRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iNoteDataAccess.CreateNoteForObject(parameter);
                var response = new CreateNoteForObjectResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListNote = new List<NoteModel>()
                };

                result.ListNote.ForEach(item =>
                {
                    response.ListNote.Add(new NoteModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new CreateNoteForObjectResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }
    }
}
