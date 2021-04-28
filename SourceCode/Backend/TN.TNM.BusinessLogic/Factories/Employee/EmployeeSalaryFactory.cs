using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.Employee;
using TN.TNM.BusinessLogic.Messages.Requests.Employee;
using TN.TNM.BusinessLogic.Messages.Responses.Employee;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.Common;
using TN.TNM.Common.CommonObject;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.Employee
{
    public class EmployeeSalaryFactory : BaseFactory, IEmployeeSalary
    {
        private IEmployeeSalaryDataAccess iEmployeeSalaryDataAccess;
        public EmployeeSalaryFactory(IEmployeeSalaryDataAccess _iEmployeeSalaryDataAccess, ILogger<EmployeeSalaryFactory> _logger)
        {
            iEmployeeSalaryDataAccess = _iEmployeeSalaryDataAccess;
            logger = _logger;
        }
        public EmployeeTimeSheetImportResponse EmployeeTimeSheetImport(EmployeeTimeSheetImportRequest request)
        {
            try
            {
                logger.LogInformation("Create Employee");
                var parameter = request.ToParameter();
                var result = iEmployeeSalaryDataAccess.EmployeeTimeSheetImport(parameter);


                var response = new EmployeeTimeSheetImportResponse()
                {
                    ExcelFile = result.ExcelFile,
                    NameFile = result.NameFile,
                    lstEmployeeMonthySalary = new List<EmployeeMonthySalaryModel>(),
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message, //result.Status ? result.Message : CommonMessage.EmployeeSalary.IMPORT_FAIL,
                };
                if (result.lstEmployeeMonthySalary != null)
                {
                    result.lstEmployeeMonthySalary.ForEach(item =>
                    {
                        response.lstEmployeeMonthySalary.Add(new EmployeeMonthySalaryModel(item));
                    });
                }
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new EmployeeTimeSheetImportResponse()
                {
                    MessageCode = CommonMessage.EmployeeSalary.IMPORT_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }
        public GetEmployeeSalaryByEmpIdResponse GetEmployeeSalaryByEmpId(GetEmployeeSalaryByEmpIdRequest request)
        {
            var parameter = request.ToParameter();
            var result = iEmployeeSalaryDataAccess.GetEmployeeSalaryByEmpId(parameter);
            var response = new GetEmployeeSalaryByEmpIdResponse()
            {
                StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                MessageCode = result.Status ? result.Message : CommonMessage.EmployeeSalary.IMPORT_FAIL,
            };
            response.ListEmployeeSalary = new List<Models.Employee.EmployeeSalaryModel>();
            result.ListEmployeeSalary.ForEach(empslr =>
            {
                if (empslr != null)
                {
                    response.ListEmployeeSalary.Add(new Models.Employee.EmployeeSalaryModel(empslr));
                }
            });
            return response;
        }

        public CreateEmployeeSalaryResponse CreateEmployeeSalary(CreateEmployeeSalaryRequest request)
        {
            var parameter = request.ToParameter();
            var result = iEmployeeSalaryDataAccess.CreateEmployeeSalary(parameter);
            var response = new CreateEmployeeSalaryResponse()
            {
                StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                MessageCode = result.Status ? result.Message : CommonMessage.EmployeeSalary.IMPORT_FAIL,
            };
            return response;
        }

        public EmployeeSalaryHandmadeResponse EmployeeSalaryHandmadeImport(EmployeeSalaryHandmadeRequest request)
        {
            try
            {
                logger.LogInformation("EmployeeSalaryHandmadeImport");
                var parameter = request.ToParameter();
                var result = iEmployeeSalaryDataAccess.EmployeeSalaryHandmadeImport(parameter);


                var response = new EmployeeSalaryHandmadeResponse()
                {
                    lstEmployeeMonthySalary = new List<EmployeeMonthySalaryModel>(),
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode =result.Message, //result.Status ? result.Message : CommonMessage.EmployeeSalary.IMPORT_FAIL,
                };
                if (result.lstEmployeeMonthySalary != null)
                {
                    result.lstEmployeeMonthySalary.ForEach(item =>
                    {
                        response.lstEmployeeMonthySalary.Add(new EmployeeMonthySalaryModel(item));
                    });
                }
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new EmployeeSalaryHandmadeResponse()
                {
                    MessageCode = CommonMessage.EmployeeSalary.IMPORT_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public DownloadEmployeeTimeSheetTemplateResponse DownloadEmployeeTimeSheetTemplate(DownloadEmployeeTimeSheetTemplateRequest request)
        {
            try
            {
                logger.LogInformation("DownloadEmployeeTimeSheetTemplate");
                var parameter = request.ToParameter();
                var result = iEmployeeSalaryDataAccess.DownloadEmployeeTimeSheetTemplate(parameter);


                var response = new DownloadEmployeeTimeSheetTemplateResponse()
                {
                    ExcelFile = result.ExcelFile,
                    NameFile = result.NameFile,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Status ? result.Message : CommonMessage.EmployeeSalary.Download_Fail,
                };
                return response;
            }
            catch (Exception e)
            {

                logger.LogError(e.Message);
                return new DownloadEmployeeTimeSheetTemplateResponse()
                {
                    MessageCode = CommonMessage.EmployeeSalary.Download_Fail,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public FindEmployeeMonthySalaryResponse FindEmployeeMonthySalary(FindEmployeeMonthySalaryRequest request)
        {
            try
            {
                logger.LogInformation("FindEmployeeMonthySalary");
                var parameter = request.ToParameter();
                var result = iEmployeeSalaryDataAccess.FindEmployeeMonthySalary(parameter);


                var response = new FindEmployeeMonthySalaryResponse()
                {
                    lstEmployeeMonthySalary = new List<EmployeeMonthySalaryModel>(),
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Status ? result.Message : CommonMessage.EmployeeSalary.Search_Fail,
                    Notes=new List<NoteObject>(),
                };
                response.Notes = result.Notes;
                result.lstEmployeeMonthySalary.ForEach(item =>
                {
                    response.lstEmployeeMonthySalary.Add(new EmployeeMonthySalaryModel(item));
                });
                return response;
            }
            catch (Exception e)
            {

                logger.LogError(e.Message);
                return new FindEmployeeMonthySalaryResponse()
                {
                    MessageCode = CommonMessage.EmployeeSalary.Search_Fail,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetTeacherSalaryResponse GetTeacherSalary(GetTeacherSalaryRequest request)
        {
            try
            {
                logger.LogInformation("GetTeacherSalary");
                var parameter = request.ToParameter();
                var result = iEmployeeSalaryDataAccess.GetTeacherSalary(parameter);
                var response = new GetTeacherSalaryResponse()
                {
                    lstColumn = result.lstColumn,
                    lstResult = result.lstResult,
                    ExcelFile = result.ExcelFile,
                    NameFile = result.NameFile,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Status ? result.Message : CommonMessage.EmployeeSalary.Search_Fail,
                };
                return response;

            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetTeacherSalaryResponse()
                {
                    MessageCode = CommonMessage.EmployeeSalary.GetTeacherSalary_Fail,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public TeacherSalaryHandmadeResponse TeacherSalaryHandmadeImport(TeacherSalaryHandmadeRequest request)
        {
            try
            {
                logger.LogInformation("TeacherSalaryHandmadeImport");
                var parameter = request.ToParameter();
                var result = iEmployeeSalaryDataAccess.TeacherSalaryHandmadeImport(parameter);
                var response = new TeacherSalaryHandmadeResponse()
                {
                    lstColumn = result.lstColumn,
                    lstResult = result.lstResult,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode =result.Message, //result.Status ? result.Message : CommonMessage.EmployeeSalary.Search_Fail,
                };
                return response;

            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new TeacherSalaryHandmadeResponse()
                {
                    MessageCode = CommonMessage.EmployeeSalary.GetTeacherSalary_Fail,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }

        }

        public FindTeacherMonthySalaryResponse FindTeacherMonthySalary(FindTeacherMonthySalaryRequest request)
        {
            try
            {
                logger.LogInformation("FindTeacherMonthySalary");
                var parameter = request.ToParameter();
                var result = iEmployeeSalaryDataAccess.FindTeacherMonthySalary(parameter);


                var response = new FindTeacherMonthySalaryResponse()
                {
                    lstResult=result.lstResult,
                    lstColumn=result.lstColumn,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Status ? result.Message : CommonMessage.EmployeeSalary.Search_Fail,
                    Notes = new List<NoteObject>(),
                };
                response.Notes = result.Notes;
                return response;
            }
            catch (Exception e)
            {

                logger.LogError(e.Message);
                return new FindTeacherMonthySalaryResponse()
                {
                    MessageCode = CommonMessage.EmployeeSalary.Search_Fail,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public ExportAssistantResponse ExportAssistant(ExportAssistantRequest request)
        {
            try
            {
                logger.LogInformation("ExportAssistant");
                var parameter = request.ToParameter();
                var result = iEmployeeSalaryDataAccess.ExportAssistant(parameter);


                var response = new ExportAssistantResponse()
                {
                    NameFile = result.NameFile,
                    ExcelFile = result.ExcelFile,
                    lstEmployeeMonthySalary = new List<EmployeeMonthySalaryModel>(),
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Status ? result.Message : CommonMessage.EmployeeSalary.Search_Fail,
                };
                result.lstEmployeeMonthySalary.ForEach(item => {
                    response.lstEmployeeMonthySalary.Add(new EmployeeMonthySalaryModel(item));
                });
                return response;
            }
            catch (Exception e)
            {

                logger.LogError(e.Message);
                return new ExportAssistantResponse()
                {
                    MessageCode = CommonMessage.EmployeeSalary.Search_Fail,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public AssistantSalaryHandmadeResponse AssistantSalaryHandmadeImport(AssistantSalaryHandmadeRequest request)
        {
            try
            {
                logger.LogInformation("AssistantSalaryHandmadeImport");
                var parameter = request.ToParameter();
                var result = iEmployeeSalaryDataAccess.AssistantSalaryHandmadeImport(parameter);

                var response = new AssistantSalaryHandmadeResponse()
                {
                    lstEmployeeMonthySalary = new List<EmployeeMonthySalaryModel>(),
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode =result.Message, //result.Status ? result.Message : CommonMessage.EmployeeSalary.Search_Fail,
                };
                if (result.lstEmployeeMonthySalary != null)
                {
                    result.lstEmployeeMonthySalary.ForEach(item =>
                    {
                        response.lstEmployeeMonthySalary.Add(new EmployeeMonthySalaryModel(item));
                    });
                }
                return response;
            }
            catch (Exception e)
            {

                logger.LogError(e.Message);
                return new AssistantSalaryHandmadeResponse()
                {
                    MessageCode = CommonMessage.EmployeeSalary.Search_Fail,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public FindAssistantMonthySalaryResponse FindAssistantMonthySalary(FindAssistantMonthySalaryRequest request)
        {
            try
            {
                logger.LogInformation("FindAssistantMonthySalary");
                var parameter = request.ToParameter();
                var result = iEmployeeSalaryDataAccess.FindAssistantMonthySalary(parameter);


                var response = new FindAssistantMonthySalaryResponse()
                {
                    lstEmployeeMonthySalary = new List<EmployeeMonthySalaryModel>(),
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Status ? result.Message : CommonMessage.EmployeeSalary.Search_Fail,
                    Notes = new List<NoteObject>(),
                };
                response.Notes = result.Notes;
                result.lstEmployeeMonthySalary.ForEach(item => {
                    response.lstEmployeeMonthySalary.Add(new EmployeeMonthySalaryModel(item));
                });
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new FindAssistantMonthySalaryResponse()
                {
                    MessageCode = CommonMessage.EmployeeSalary.Search_Fail,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public ExportEmployeeSalaryResponse ExportEmployeeSalary(ExportEmployeeSalaryRequest request)
        {
            try
            {
                logger.LogInformation("ExportEmployeeSalary");
                var parameter = request.ToParameter();
                var result = iEmployeeSalaryDataAccess.ExportEmployeeSalary(parameter);


                var response = new ExportEmployeeSalaryResponse()
                {
                    NameFile = result.NameFile,
                    ExcelFile = result.ExcelFile,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Status ? result.Message : CommonMessage.EmployeeSalary.ExportEmployeeSalary_Fail,
                };

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new ExportEmployeeSalaryResponse()
                {
                    MessageCode = CommonMessage.EmployeeSalary.ExportEmployeeSalary_Fail,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }

        }

        public ExportTeacherSalaryResponse ExportTeacherSalary(ExportTeacherSalaryRequest request)
        {
            try
            {
                logger.LogInformation("ExportTeacherSalary");
                var parameter = request.ToParameter();
                var result = iEmployeeSalaryDataAccess.ExportTeacherSalary(parameter);


                var response = new ExportTeacherSalaryResponse()
                {
                    NameFile = result.NameFile,
                    ExcelFile = result.ExcelFile,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Status ? result.Message : CommonMessage.EmployeeSalary.ExportTeacherSalary_Fail,
                };

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new ExportTeacherSalaryResponse()
                {
                    MessageCode = CommonMessage.EmployeeSalary.ExportTeacherSalary_Fail,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public ExportAssistantSalaryResponse ExportAssistantSalary(ExportAssistantSalaryRequest request)
        {
            try
            {
                logger.LogInformation("ExportAssistantSalary");
                var parameter = request.ToParameter();
                var result = iEmployeeSalaryDataAccess.ExportAssistantSalary(parameter);


                var response = new ExportAssistantSalaryResponse()
                {
                    NameFile = result.NameFile,
                    ExcelFile = result.ExcelFile,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Status ? result.Message : CommonMessage.EmployeeSalary.ExportAssistantSalary_Fail,
                };

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new ExportAssistantSalaryResponse()
                {
                    MessageCode = CommonMessage.EmployeeSalary.ExportAssistantSalary_Fail,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetEmployeeSalaryStatusResponse GetEmployeeSalaryStatus(GetEmployeeSalaryStatusRequest request)
        {
            try
            {
                logger.LogInformation("GetEmployeeSalaryStatus");
                var parameter = request.ToParameter();
                var result = iEmployeeSalaryDataAccess.GetEmployeeSalaryStatus(parameter);

                return new GetEmployeeSalaryStatusResponse() {
                    IsApproved = result.IsApproved,
                    IsInApprovalProgress = result.IsInApprovalProgress,
                    IsRejected = result.IsRejected,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    StatusName = result.StatusName,
                    ApproverId = result.ApproverId,
                    PositionId = result.PositionId
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetEmployeeSalaryStatusResponse()
                {
                    MessageCode = CommonMessage.EmployeeSalary.Search_Fail,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }
    }
}
