using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using TN.TNM.Common;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.CustomerCare;
using TN.TNM.DataAccess.Messages.Results.CustomerCare;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.CustomerCare;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class CustomerCareDAO : BaseDAO, ICustomerCareDataAccess
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public CustomerCareDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace, IHostingEnvironment _hostingEnvironment)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
            this.hostingEnvironment = _hostingEnvironment;
        }

        public CreateCustomerCareResult CreateCustomerCare(CreateCustomerCareParameter parameter)
        {
            try
            {
                //Lấy trạng thái mặc định(Chưa chăm sóc) của khách hàng
                var categoryTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TKH").CategoryTypeId;
                var statusId = context.Category.FirstOrDefault(ca => ca.CategoryCode == "CSO" && ca.CategoryTypeId == categoryTypeId).CategoryId;
                //context.Database.ExecuteSqlCommand("CreateStudents @p0, @p1", parameters: new[] { "Bill", "Gates" });
                //var T = context.Customer.FromSql("SELECT * From V_SearchCustomer").ToList();
                //Auto gen CustomerCode
                var categoryTypeTCSId = context.CategoryType.FirstOrDefault(e => e.CategoryTypeCode == "TCS").CategoryTypeId;
                var listCategory = context.Category.Where(x => x.CategoryTypeId == categoryTypeTCSId).ToList();
                var statusCodeChange = listCategory.FirstOrDefault(x => x.CategoryId == parameter.CustomerCare.StatusId).CategoryCode;

                int currentYear = DateTime.Now.Year % 100;
                int currentMonth = DateTime.Now.Month;
                int currentDate = DateTime.Now.Day;
                var lstRequestPayment = context.CustomerCare.Where(w => w.YearCode == currentYear && w.MonthCode == currentMonth && w.DateCode == currentDate).Select(s => s.NumberCode.Value).ToList();
                int MaxNumberCode = 0;
                if (lstRequestPayment.Count > 0)
                {
                    MaxNumberCode = lstRequestPayment.Max();
                }
                parameter.CustomerCare.CustomerCareCode = string.Format("CSKH{0}{1}{2}{3}", currentYear, currentMonth, currentDate, (MaxNumberCode + 1).ToString("D3"));
                parameter.CustomerCare.NumberCode = MaxNumberCode + 1;
                parameter.CustomerCare.YearCode = currentYear;
                parameter.CustomerCare.MonthCode = currentMonth;
                parameter.CustomerCare.DateCode = currentDate;
                parameter.CustomerCare.CreateDate = DateTime.Now;
                if (statusCodeChange == "Active")
                {
                    parameter.CustomerCare.ActiveDate = DateTime.Now;
                }

                if (parameter.ListTypeCustomer.Count == 0)
                {
                    parameter.CustomerCare.TypeCustomer = 1;
                }
                else if (parameter.ListTypeCustomer.Count == 2)
                {
                    parameter.CustomerCare.TypeCustomer = 4;
                }
                else
                {
                    if (parameter.ListTypeCustomer[0].Equals("HDO"))
                    {
                        parameter.CustomerCare.TypeCustomer = 2;
                    }
                    else
                    {
                        parameter.CustomerCare.TypeCustomer = 3;
                    }
                }
                context.CustomerCare.Add(parameter.CustomerCare);
                context.SaveChanges();

                //
                List<CustomerCareCustomer> lstCustomerCareCustomer = new List<CustomerCareCustomer>();
                parameter.CustomerId.ForEach(item =>
                {
                    CustomerCareCustomer CustomerCareCustomerE = new CustomerCareCustomer
                    {
                        CustomerCareId = parameter.CustomerCare.CustomerCareId,
                        CustomerId = item,
                        StatusId = statusId,
                        CreateDate = DateTime.Now,
                        CreateById = parameter.UserId,
                    };
                    lstCustomerCareCustomer.Add(CustomerCareCustomerE);
                });
                context.CustomerCareCustomer.AddRange(lstCustomerCareCustomer);
                //Tạo bộ lọc
                CustomerCareFilter customerCareFilter = new CustomerCareFilter
                {
                    CustomerCareId = parameter.CustomerCare.CustomerCareId,
                    QueryContent = parameter.QueryFilter,
                    CreateDate = DateTime.Now,
                    CreateById = parameter.UserId
                };

                context.CustomerCareFilter.Add(customerCareFilter);
                context.SaveChanges();

                var CustomerCareStatus = (from category in context.Category
                                          join categoryType in context.CategoryType on category.CategoryTypeId equals categoryType.CategoryTypeId
                                          where categoryType.CategoryTypeCode == "TCS" && category.CategoryCode == "Active"
                                          select new { CustomerCareStatusGuid = category.CategoryId }).DefaultIfEmpty(new { CustomerCareStatusGuid = Guid.Empty }).FirstOrDefault().CustomerCareStatusGuid;

                var TypesOfCustomerCare = (from category in context.Category
                                           join categoryType in context.CategoryType on category.CategoryTypeId equals categoryType.CategoryTypeId
                                           where categoryType.CategoryTypeCode == "HCS" && category.CategoryId == parameter.CustomerCare.CustomerCareContactType
                                           select category).FirstOrDefault();

                if (CustomerCareStatus != Guid.Empty)
                {
                    if (parameter.CustomerCare.StatusId == CustomerCareStatus)
                    {
                        if (parameter.CustomerId.Count > 0)
                        {
                            var EmployeeId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
                            var FromTo = context.Contact.FirstOrDefault(c => c.ObjectId == EmployeeId && c.ObjectType == "EMP")?.Email;

                            List<Queue> lstQueue = new List<Queue>();

                            if (TypesOfCustomerCare.CategoryCode == "Email")
                            {
                                if (FromTo != null)
                                {
                                    parameter.CustomerId.ForEach(item =>
                                    {
                                        // Ngọc comment
                                        //var SendTo = context.Contact.FirstOrDefault(c => c.ObjectId == item && c.ObjectType == "CUS")?.Email;
                                        var SendTo = context.Contact.Where(c => c.ObjectId == item).ToList();

                                        if (SendTo != null)
                                        {
                                            // Ngọc comment
                                            //Queue queueCreate = new Queue
                                            //{
                                            //    FromTo = FromTo,
                                            //    SendTo = SendTo,
                                            //    SendContent = replaceTokenForContent(parameter.CustomerCare.CustomerCareContentEmail, item),
                                            //    Method = TypesOfCustomerCare.CategoryCode,
                                            //    Title = parameter.CustomerCare.CustomerCareTitle + " - " + (context.Customer.FirstOrDefault(cus => cus.CustomerId == item) != null ?
                                            //                            context.Customer.FirstOrDefault(cus => cus.CustomerId == item).CustomerCode : ""),
                                            //    IsSend = false,
                                            //    SenDate = parameter.CustomerCare.IsSendEmailNow == true ? DateTime.Now : SetDate(parameter.CustomerCare.SendEmailDate, parameter.CustomerCare.SendEmailHour),
                                            //    CustomerId = item,
                                            //    CustomerCareId = parameter.CustomerCare.CustomerCareId,
                                            //    CreateDate = DateTime.Now,
                                            //    CreateById = parameter.UserId
                                            //};
                                            SendTo.ForEach(s =>
                                            {
                                                if (s.Email != null)
                                                {
                                                    Queue queueCreate = new Queue
                                                    {
                                                        FromTo = FromTo,
                                                        SendTo = s.Email,
                                                        SendContent = replaceTokenForContent(parameter.CustomerCare.CustomerCareContentEmail, item),
                                                        Method = TypesOfCustomerCare.CategoryCode,
                                                        Title = parameter.CustomerCare.CustomerCareTitle + " - " + (context.Customer.FirstOrDefault(cus => cus.CustomerId == item) != null ?
                                                                                context.Customer.FirstOrDefault(cus => cus.CustomerId == item).CustomerCode : ""),
                                                        IsSend = false,
                                                        SenDate = parameter.CustomerCare.IsSendEmailNow == true ? DateTime.Now : SetDate(parameter.CustomerCare.SendEmailDate, parameter.CustomerCare.SendEmailHour),
                                                        CustomerId = item,
                                                        CustomerCareId = parameter.CustomerCare.CustomerCareId,
                                                        CreateDate = DateTime.Now,
                                                        CreateById = parameter.UserId
                                                    };
                                                    lstQueue.Add(queueCreate);
                                                }
                                            });
                                        }
                                    });

                                    if (lstQueue.Count > 0)
                                    {
                                        context.Queue.AddRange(lstQueue);
                                        context.SaveChanges();
                                    }
                                }
                            }
                            else if (TypesOfCustomerCare.CategoryCode == "SMS")
                            {
                                List<Queue> lstQueue_sms = new List<Queue>();
                                parameter.CustomerId.ForEach(item =>
                                {
                                    var SendToSMS = context.Contact.FirstOrDefault(c => c.ObjectId == item && c.ObjectType == "CUS")?.Phone;

                                    if (SendToSMS != null)
                                    {
                                        Queue queueCreateSMS = new Queue
                                        {
                                            SendTo = SendToSMS,
                                            SendContent = replaceTokenForContent(parameter.CustomerCare.CustomerCareContentSms, item),
                                            Method = TypesOfCustomerCare.CategoryCode,
                                            Title = parameter.CustomerCare.CustomerCareTitle,
                                            IsSend = false,
                                            SenDate = parameter.CustomerCare.IsSendNow == true ? DateTime.Now : SetDate(parameter.CustomerCare.SendDate, parameter.CustomerCare.SendHour),
                                            CustomerId = item,
                                            CustomerCareId = parameter.CustomerCare.CustomerCareId,
                                            CreateDate = DateTime.Now,
                                            CreateById = parameter.UserId
                                        };
                                        lstQueue_sms.Add(queueCreateSMS);
                                    }
                                });
                                if (lstQueue_sms.Count > 0)
                                {
                                    context.Queue.AddRange(lstQueue_sms);
                                    context.SaveChanges();
                                }
                            }
                        }
                    }
                }

                return new CreateCustomerCareResult
                {
                    CustomerCareId = parameter.CustomerCare.CustomerCareId,
                    Message = "Tạo chương trình chăm sóc khách hàng thành công.",
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new CreateCustomerCareResult
                {
                    Message = "Tạo chương trình chăm sóc thất bại.",
                    Status = false
                };
            }

        }

        public CreateCustomerCareFeedBackResult CreateCustomerCareFeedBack(CreateCustomerCareFeedBackParameter parameter)
        {
            try
            {
                context.CustomerCareFeedBack.Add(parameter.CustomerCareFeedBack);

                //Cập nhật lại trạng thái của khách hàng trong chương trình CSKH
                var customerCategoryTypeId = context.CategoryType.FirstOrDefault(catype => catype.CategoryTypeCode == "TKH").CategoryTypeId;
                var statusId = context.Category.FirstOrDefault(ca => ca.CategoryTypeId == customerCategoryTypeId && ca.CategoryCode == "DSO").CategoryId;
                var customer = context.CustomerCareCustomer.FirstOrDefault(cus => cus.CustomerId == parameter.CustomerCareFeedBack.CustomerId && cus.CustomerCareId == parameter.CustomerCareFeedBack.CustomerCareId);
                customer.StatusId = statusId;
                context.SaveChanges();

                return new CreateCustomerCareFeedBackResult
                {
                    CustomerCareFeedBackId = parameter.CustomerCareFeedBack.CustomerCareFeedBackId,
                    Message = "Phản hồi đã được lưu",
                    Status = true
                };
            }
            catch (Exception)
            {

                return new CreateCustomerCareFeedBackResult
                {
                    Message = "Đã có lỗi khi lưu phản hổi",
                    Status = false
                };
            }
        }

        public FilterCustomerResult FilterCustomer(FilterCustomerParameter parameter)
        {
            try
            {
                var tenantId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).TenantId;
                var listStatusId = context.Category.Where(x => parameter.CustomerStatusCode.Contains(x.CategoryCode))
                    .ToList();


                var sqlCondition = string.Empty;
                if (!string.IsNullOrEmpty(parameter.SqlQuery))
                {
                    sqlCondition = parameter.SqlQuery + " AND TenantId = '" + tenantId.ToString() + "'";
                }
                else
                {
                    sqlCondition = parameter.SqlQuery + " TenantId = '" + tenantId.ToString() + "'";
                }

                var listCustomerId = new List<Guid>();
                using (var command = context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SearchCustomer";
                    DbParameter param1 = command.CreateParameter();
                    param1.ParameterName = "@FilterWhere";
                    param1.DbType = DbType.String;
                    param1.Value = sqlCondition;
                    command.Parameters.Add(param1);

                    command.CommandType = CommandType.StoredProcedure;

                    context.Database.OpenConnection();

                    var dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        listCustomerId.Add(dataReader.GetGuid(0));
                    }

                    context.Database.CloseConnection();
                }

                //var lstCustomer = context.Customer.FromSql("SearchCustomer @p0", sqlCondition).Select(s => new CustomerEntityModel(s)).ToList();
                var customerList = new List<CustomerEntityModel>();

                if (listCustomerId.Count > 0)
                {
                    var lstCustomer = new List<Customer>();

                    if (listStatusId.Count > 0)
                    {
                        lstCustomer = context.Customer.Where(x => listCustomerId.Contains(x.CustomerId) && listStatusId.Contains(x.Status)).ToList();
                    }
                    else
                    {
                        lstCustomer = context.Customer.Where(x => listCustomerId.Contains(x.CustomerId)).ToList();
                    }


                    customerList = (from cus in lstCustomer
                                    join c in context.Contact on cus.CustomerId equals c.ObjectId
                                    where c.ObjectType == "CUS"
                                    select new CustomerEntityModel
                                    {
                                        CustomerId = cus.CustomerId,
                                        CustomerCode = cus.CustomerCode,
                                        ContactId = c.ContactId,
                                        CustomerGroupId = cus.CustomerGroupId,
                                        CustomerName = cus.CustomerName,
                                        PersonInChargeId = cus.PersonInChargeId,
                                        PicName = "",
                                        CustomerEmail = c.Email,
                                        CustomerPhone = c.Phone,
                                        DateOfBirth = cus.CustomerType == 1 ? cus.BusinessRegistrationDate : c.DateOfBirth,
                                        CustomerType = cus.CustomerType,
                                        CreatedDate = cus.CreatedDate,
                                    }).OrderByDescending(date => date.CreatedDate).ToList();

                    var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();

                    //Lấy tên người phụ trách
                    customerList.ForEach(item =>
                    {
                        var employee = listAllEmployee.FirstOrDefault(e => e.EmployeeId == item.PersonInChargeId);

                        if (employee != null)
                        {
                            item.PicName = employee.EmployeeName.Trim();
                        }
                    });
                }

                return new FilterCustomerResult
                {
                    ListCustomer = customerList,
                    Message = "",
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new FilterCustomerResult
                {
                    Message = ex.ToString(),
                    Status = false
                };
            }
        }

        public GetCustomerCareByIdResult GetCustomerCareById(GetCustomerCareByIdParameter parameter)
        {
            try
            {
                var customerCare = context.CustomerCare.FirstOrDefault(w => w.CustomerCareId == parameter.CustomerCareId);
                var ListCustomerCareCustomer = (from CustomerCareC in context.CustomerCareCustomer
                                                join Cus in context.Customer on CustomerCareC.CustomerId equals Cus.CustomerId
                                                where CustomerCareC.CustomerCareId == parameter.CustomerCareId
                                                select new CustomerEntityModel
                                                {
                                                    ContactId = context.Contact.FirstOrDefault(cont => cont.ObjectId == Cus.CustomerId && cont.ObjectType == "CUS") != null ?
                                                                  context.Contact.FirstOrDefault(cont => cont.ObjectId == Cus.CustomerId && cont.ObjectType == "CUS").ContactId : Guid.Empty,
                                                    CustomerId = Cus.CustomerId,
                                                    CustomerCode = Cus.CustomerCode,
                                                    CustomerGroupId = Cus.CustomerGroupId,
                                                    CustomerName = Cus.CustomerName,
                                                    CustomerCareStaff = Cus.CustomerCareStaff,
                                                    CustomerEmail = context.Contact.FirstOrDefault(cont => cont.ObjectId == Cus.CustomerId && cont.ObjectType == "CUS") != null ?
                                                                  context.Contact.FirstOrDefault(cont => cont.ObjectId == Cus.CustomerId && cont.ObjectType == "CUS").Email : "",
                                                    LeadId = Cus.LeadId,
                                                    StatusId = Cus.StatusId,
                                                    CustomerCareCustomerId = CustomerCareC.CustomerCareCustomerId,
                                                    CustomerCareCustomerStatusId = CustomerCareC.StatusId,
                                                    CustomerCareCustomerStatusName = context.Category.FirstOrDefault(ca => ca.CategoryId == CustomerCareC.StatusId) != null ?
                                                                                   context.Category.FirstOrDefault(ca => ca.CategoryId == CustomerCareC.StatusId).CategoryName : "",
                                                    CustomerServiceLevelId = Cus.CustomerServiceLevelId,
                                                    PersonInChargeId = Cus.PersonInChargeId,
                                                    PicName = context.Employee.FirstOrDefault(e => e.EmployeeId == Cus.PersonInChargeId) != null ?
                                                                            context.Employee.FirstOrDefault(e => e.EmployeeId == Cus.PersonInChargeId).EmployeeName : "",
                                                    CustomerType = Cus.CustomerType,
                                                    PaymentId = Cus.PaymentId,
                                                    FieldId = Cus.FieldId,
                                                    ScaleId = Cus.ScaleId,
                                                    MaximumDebtValue = Cus.MaximumDebtValue,
                                                    MaximumDebtDays = Cus.MaximumDebtDays,
                                                    MainBusinessSector = Cus.MainBusinessSector,
                                                    TotalSaleValue = Cus.TotalSaleValue,
                                                    TotalReceivable = Cus.TotalReceivable,
                                                    NearestDateTransaction = Cus.NearestDateTransaction,
                                                    TotalCapital = Cus.TotalCapital,
                                                    BusinessRegistrationDate = Cus.BusinessRegistrationDate,
                                                    EnterpriseType = Cus.EnterpriseType,
                                                    TotalEmployeeParticipateSocialInsurance = Cus.TotalEmployeeParticipateSocialInsurance,
                                                    TotalRevenueLastYear = Cus.TotalRevenueLastYear,
                                                    BusinessType = Cus.BusinessType,
                                                    BusinessScale = Cus.BusinessScale,
                                                    Active = Cus.Active,
                                                    CreatedById = Cus.CreatedById,
                                                    CreatedDate = Cus.CreatedDate,
                                                    UpdatedById = Cus.UpdatedById,
                                                    UpdatedDate = Cus.UpdatedDate,
                                                }).ToList();
                context.CustomerCareCustomer.Where(w => w.CustomerCareId == parameter.CustomerCareId).ToList();

                var QueryFilter = context.CustomerCareFilter.Where(w => w.CustomerCareId == parameter.CustomerCareId).Select(s => new { FilterContent = s.QueryContent }).DefaultIfEmpty(new { FilterContent = string.Empty }).FirstOrDefault().FilterContent;

                var ListCustomerCareFeedBack = context.CustomerCareFeedBack.Where(w => w.CustomerCareId == parameter.CustomerCareId).Select(
                      s => new CustomerCareFeedBackEntityModel(s)
                    ).ToList();

                var customerType = customerCare.TypeCustomer;

                return new GetCustomerCareByIdResult
                {
                    CustomerCare = new CustomerCareEntityModel(customerCare),
                    ListCustomer = ListCustomerCareCustomer,
                    CustomerCareFeedBack = ListCustomerCareFeedBack,
                    QueryFilter = QueryFilter,
                    TypeCutomer = customerType,
                    Status = true,
                };
            }
            catch (Exception e)
            {
                return new GetCustomerCareByIdResult
                {
                    Message = "Không có thông tin của chăm sóc khách hàng",
                    Status = false
                };
            }

        }

        public GetCustomerCareFeedBackByCusIdAndCusCareIdResult GetCustomerCareFeedBackByCusIdAndCusCareId(GetCustomerCareFeedBackByCusIdAndCusCareIdParameter parameter)
        {
            try
            {
                var customerCareFeedBack = context.CustomerCareFeedBack.FirstOrDefault(w => w.CustomerId == parameter.CustomerId && w.CustomerCareId == parameter.CustomerCareId);
                if (customerCareFeedBack == null)
                {
                    customerCareFeedBack.CustomerCareFeedBackId = Guid.Empty;
                    customerCareFeedBack.FeedBackContent = "";
                }

                return new GetCustomerCareFeedBackByCusIdAndCusCareIdResult
                {
                    CustomerCareFeedBack = new CustomerCareFeedBackEntityModel(customerCareFeedBack),
                    Status = true,
                };
            }
            catch (Exception e)
            {
                return new GetCustomerCareFeedBackByCusIdAndCusCareIdResult
                {
                    Message = "Không có thông tin của phản hồi khách hàng",
                    Status = false
                };
            }
        }

        public GetTimeLineCustomerCareByCustomerIdResult GetTimeLineCustomerCareByCustomerId(GetTimeLineCustomerCareByCustomerIdParameter parameter)
        {
            try
            {

                #region Comment By Hung
                //var listCustomerCare = (from customerCareCustomer in context.CustomerCareCustomer
                //                        join customerCare in context.CustomerCare on customerCareCustomer.CustomerCareId equals customerCare.CustomerCareId
                //                        where (customerCareCustomer.CustomerId == customer.CustomerId &&
                //                              customerCare.StatusId != customerStatusId_NewCreate &&
                //                              (parameter.First_day == null || parameter.First_day == DateTime.MinValue || parameter.First_day.Date <= customerCare.EffecttiveFromDate.Value.Date) &&
                //                              (parameter.Last_day == null || parameter.Last_day == DateTime.MinValue || parameter.Last_day.Date >= customerCare.EffecttiveFromDate.Value.Date))
                //                        select new
                //                        {
                //                            CustomerId = customer.CustomerId,
                //                            CustomerCareId = customerCare.CustomerCareId,
                //                            CustomerCareTitle = customerCare.CustomerCareTitle,
                //                            CustomerCareContactType = customerCare.CustomerCareContactType,
                //                            CustomerCareType = customerCare.CustomerCareType,
                //                            CustomerCareContactTypeName = context.Category.FirstOrDefault(category => category.CategoryId == customerCare.CustomerCareContactType) != null ?
                //                                                        context.Category.FirstOrDefault(category => category.CategoryId == customerCare.CustomerCareContactType).CategoryName : "",
                //                            EffecttiveFromDate = customerCare.EffecttiveFromDate,
                //                            EffecttiveToDate = customerCare.EffecttiveToDate,
                //                            CustomerCareFeedBackId = context.CustomerCareFeedBack.FirstOrDefault(fb => fb.CustomerId == customer.CustomerId && fb.CustomerCareId == customerCare.CustomerCareId) != null ?
                //                                                    context.CustomerCareFeedBack.FirstOrDefault(fb => fb.CustomerId == customer.CustomerId && fb.CustomerCareId == customerCare.CustomerCareId).CustomerCareFeedBackId : Guid.Empty,
                //                            FeedBackCode = context.CustomerCareFeedBack.FirstOrDefault(fb => fb.CustomerId == customer.CustomerId && fb.CustomerCareId == customerCare.CustomerCareId) != null ?
                //                                                    context.CustomerCareFeedBack.FirstOrDefault(fb => fb.CustomerId == customer.CustomerId && fb.CustomerCareId == customerCare.CustomerCareId).FeedBackCode : Guid.Empty,
                //                            FeedBackCodeName = context.Category.FirstOrDefault(category => category.CategoryId == (context.CustomerCareFeedBack.FirstOrDefault(fb => fb.CustomerId == customer.CustomerId && fb.CustomerCareId == customerCare.CustomerCareId) != null ?
                //                                             context.CustomerCareFeedBack.FirstOrDefault(fb => fb.CustomerId == customer.CustomerId && fb.CustomerCareId == customerCare.CustomerCareId).FeedBackCode : Guid.Empty)) != null ?
                //                                             context.Category.FirstOrDefault(category => category.CategoryId == (context.CustomerCareFeedBack.FirstOrDefault(fb => fb.CustomerId == customer.CustomerId && fb.CustomerCareId == customerCare.CustomerCareId) != null ?
                //                                                    context.CustomerCareFeedBack.FirstOrDefault(fb => fb.CustomerId == customer.CustomerId && fb.CustomerCareId == customerCare.CustomerCareId).FeedBackCode : Guid.Empty)).CategoryName : "",
                //                            FeedBackContent = context.CustomerCareFeedBack.FirstOrDefault(fb => fb.CustomerId == customer.CustomerId && fb.CustomerCareId == customerCare.CustomerCareId) != null ?
                //                                                    context.CustomerCareFeedBack.FirstOrDefault(fb => fb.CustomerId == customer.CustomerId && fb.CustomerCareId == customerCare.CustomerCareId).FeedBackContent : "",
                //                        }).OrderByDescending(x => x.EffecttiveFromDate).ToList();
                #endregion

                #region Add By Hung
                List<dynamic> lstResult = new List<dynamic>();
                var customer = context.Customer.FirstOrDefault(c => c.CustomerId == parameter.CustomerId);
                if (customer != null)
                {
                    Guid categoryTypeId = Guid.Empty;
                    Guid customerStatusId_NewCreate = Guid.Empty;
                    var categoryType = context.CategoryType.FirstOrDefault(ca => ca.CategoryTypeCode == "TCS");
                    if (categoryType != null)
                        categoryTypeId = categoryType.CategoryTypeId;
                    var listCategory = context.Category.ToList();
                    var customerStatus_NewCreate = listCategory.FirstOrDefault(c => c.CategoryCode == "New" && c.CategoryTypeId == categoryTypeId);
                    if (customerStatus_NewCreate != null)
                        customerStatusId_NewCreate = customerStatus_NewCreate.CategoryId;
                    var listCustomerCareCustomer = context.CustomerCareCustomer.Where(w => w.CustomerId == customer.CustomerId).ToList();

                    if (listCustomerCareCustomer.Count > 0)
                    {
                        List<Guid> listCustomerCareId = new List<Guid>();
                        listCustomerCareCustomer.ForEach(item =>
                        {
                            if (item.CustomerCareId != null && !listCustomerCareId.Contains(item.CustomerCareId.Value))
                            {
                                listCustomerCareId.Add(item.CustomerCareId.Value);
                            }
                        });

                        var listCustomerCare = (from customerCare in context.CustomerCare
                                                where (customerCare.StatusId != customerStatusId_NewCreate && listCustomerCareId.Contains(customerCare.CustomerCareId) &&
                                                      (parameter.First_day == null || parameter.First_day == DateTime.MinValue || parameter.First_day.Date <= customerCare.EffecttiveFromDate.Value.Date) &&
                                                      (parameter.Last_day == null || parameter.Last_day == DateTime.MinValue || parameter.Last_day.Date >= customerCare.EffecttiveFromDate.Value.Date))
                                                select new
                                                {
                                                    CustomerId = customer.CustomerId,
                                                    CustomerCareId = customerCare.CustomerCareId,
                                                    CustomerCareTitle = customerCare.CustomerCareTitle,
                                                    CustomerCareContactType = customerCare.CustomerCareContactType,
                                                    CustomerCareType = customerCare.CustomerCareType,
                                                    CustomerCareContactTypeName = "",
                                                    EffecttiveFromDate = customerCare.EffecttiveFromDate,
                                                    EffecttiveToDate = customerCare.EffecttiveToDate,
                                                    CustomerCareFeedBackId = Guid.Empty,
                                                    FeedBackCode = Guid.Empty,
                                                    FeedBackCodeName = "",
                                                    FeedBackContent = "",
                                                }).OrderByDescending(x => x.EffecttiveFromDate).ToList();

                        var listCustomerCareFeedBack = context.CustomerCareFeedBack.Where(w => w.CustomerId == customer.CustomerId).ToList();

                        listCustomerCare.ForEach(item =>
                        {
                            var category = listCategory.FirstOrDefault(f => f.CategoryId == item.CustomerCareContactType);
                            var customerCareFeedBack = listCustomerCareFeedBack.FirstOrDefault(f => f.CustomerCareId == item.CustomerCareId);
                            Guid customerCareFeedBackId = Guid.Empty;
                            Guid feedBackCode = Guid.Empty;
                            string feedBackContent = "";
                            string feedBackCodeName = "";
                            if (customerCareFeedBack != null)
                            {
                                customerCareFeedBackId = customerCareFeedBack.CustomerCareFeedBackId;
                                feedBackCode = customerCareFeedBack.FeedBackCode.Value;
                                feedBackContent = customerCareFeedBack.FeedBackContent;
                                if (feedBackCode != null)
                                {
                                    var feedBack = listCategory.FirstOrDefault(fCat => fCat.CategoryId == feedBackCode);
                                    if (feedBack != null)
                                    {
                                        feedBackCodeName = feedBack.CategoryName;
                                    }
                                }
                            }
                            var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                            sampleObject.Add("customerId", item.CustomerId);
                            sampleObject.Add("customerCareId", item.CustomerCareId);
                            sampleObject.Add("customerCareTitle", item.CustomerCareTitle);
                            sampleObject.Add("customerCareContactType", item.CustomerCareContactType);
                            sampleObject.Add("customerCareType", item.CustomerCareType);
                            sampleObject.Add("customerCareContactTypeName", category != null ? category.CategoryName : "");
                            sampleObject.Add("effecttiveFromDate", item.EffecttiveFromDate);
                            sampleObject.Add("effecttiveToDate", item.EffecttiveToDate);
                            sampleObject.Add("customerCareFeedBackId", customerCareFeedBackId);
                            sampleObject.Add("feedBackCode", feedBackCode);
                            sampleObject.Add("feedBackCodeName", feedBackCodeName);
                            sampleObject.Add("feedBackContent", feedBackContent);
                            lstResult.Add(sampleObject);
                        });
                    }
                }
                #endregion
                return new GetTimeLineCustomerCareByCustomerIdResult
                {
                    ListCustomerCare = lstResult
                };
            }
            catch (Exception e)
            {
                return new GetTimeLineCustomerCareByCustomerIdResult
                {
                    Message = "Không có thông tin của chăm sóc khách hàng",
                    Status = false
                };
            }
        }

        public SearchCustomerCareResult SearchCustomerCare(SearchCustomerCareParameter parameter)
        {
            try
            {
                #region Common dữ liệu
                var listCommonEmployee = context.Employee.ToList();
                var listCategoryTypeCode = new List<string> { "HCS", "TCS" };
                var listTypeId = context.CategoryType.Where(c => listCategoryTypeCode.Contains(c.CategoryTypeCode)).Select(c => c.CategoryTypeId).ToList();
                var listCommonCategory = context.Category.Where(c => listTypeId.Contains(c.CategoryTypeId)).ToList();
                #endregion

                var lstResult = context.CustomerCare.Where(c =>
                                    (parameter.FromDate == null || parameter.FromDate == DateTime.MinValue || c.CreateDate.Value.Date >= parameter.FromDate.Value.Date) &&
                                    (parameter.ToDate == null || parameter.ToDate == DateTime.MinValue || c.CreateDate.Value.Date <= parameter.ToDate.Value.Date) &&
                                    (parameter.CustomerCareTitle == null || parameter.CustomerCareTitle.Trim() == null || c.CustomerCareTitle.ToLower().Contains(parameter.CustomerCareTitle.Trim().ToLower())) &&
                                    (parameter.CustomerCareCode == null || parameter.CustomerCareCode.Trim() == null || c.CustomerCareCode.ToLower().Contains(parameter.CustomerCareCode.Trim().ToLower())) &&
                                    (parameter.PicName == null || parameter.PicName.Count == 0 || parameter.PicName.Contains(c.EmployeeCharge.Value)) &&
                                    (parameter.Status == null || parameter.Status.Count == 0 || parameter.Status.Contains(c.StatusId.Value)) &&
                                    (parameter.ListTypeCusCareId == null || parameter.ListTypeCusCareId.Count == 0 || parameter.ListTypeCusCareId.Contains(c.CustomerCareContactType.Value)) &&
                                    (parameter.CustomerCareContent == null || parameter.CustomerCareContent.Trim() == null || c.CustomerCareContent.Contains(parameter.CustomerCareContent.Trim())) &&
                                    (parameter.ProgramType == null || parameter.ProgramType.Count == 0 || parameter.ProgramType.Contains(c.ProgramType.Value)))
                    .Select(m => new CustomerCareEntityModel(m)
                    {
                        EmployeeChargeName = "",
                        CustomerCareContactTypeName = "",
                        StatusName = "",
                        StatusCode = "",
                    }).ToList();

                lstResult.ForEach(item =>
                {
                    item.EmployeeChargeName = listCommonEmployee.FirstOrDefault(c => c.EmployeeId == item.EmployeeCharge)?.EmployeeName ?? "";
                    item.CustomerCareContactTypeName = listCommonCategory.FirstOrDefault(c => c.CategoryId == item.CustomerCareContactType)?.CategoryName ?? "";
                    item.StatusName = listCommonCategory.FirstOrDefault(c => c.CategoryId == item.StatusId)?.CategoryName ?? "";
                    item.StatusCode = listCommonCategory.FirstOrDefault(c => c.CategoryId == item.StatusId)?.CategoryCode ?? "";
                });


                return new SearchCustomerCareResult
                {
                    LstCustomerCare = lstResult,
                    Message = "Success",
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new SearchCustomerCareResult
                {
                    LstCustomerCare = null,
                    Message = ex.ToString(),
                    Status = false
                };
            }
        }

        public SendQuickEmailResult SendQuickEmail(SendQuickEmailParameter parameter)
        {
            try
            {
                //Tạo CSKH
                var customerCare = new CustomerCare();
                var categoryTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TKH").CategoryTypeId;
                var statusId = context.Category.FirstOrDefault(ca => ca.CategoryCode == "DSO" && ca.CategoryTypeId == categoryTypeId).CategoryId;
                var employeeId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
                var customerCareStatusTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TCS").CategoryTypeId;
                var customerCareStatusId = context.Category.FirstOrDefault(ca => ca.CategoryCode == "Closed" && ca.CategoryTypeId == customerCareStatusTypeId).CategoryId;
                var customerCareContactTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "HCS").CategoryTypeId;
                var customerCareContactType = context.Category.FirstOrDefault(ca => ca.CategoryCode == "Email" && ca.CategoryTypeId == customerCareContactTypeId).CategoryId;

                int currentYear = DateTime.Now.Year % 100;
                int currentMonth = DateTime.Now.Month;
                int currentDate = DateTime.Now.Day;
                var lstRequestPayment = context.CustomerCare.Where(w => w.YearCode == currentYear && w.MonthCode == currentMonth && w.DateCode == currentDate).Select(s => s.NumberCode.Value).ToList();
                int MaxNumberCode = 0;
                if (lstRequestPayment.Count > 0)
                {
                    MaxNumberCode = lstRequestPayment.Max();
                }
                customerCare.CustomerCareCode = string.Format("CSKH{0}{1}{2}", currentYear, currentMonth, (MaxNumberCode + 1).ToString("D3"));
                customerCare.NumberCode = MaxNumberCode + 1;
                customerCare.YearCode = currentYear;
                customerCare.MonthCode = currentMonth;
                customerCare.DateCode = currentDate;
                customerCare.EmployeeCharge = employeeId;
                customerCare.EffecttiveFromDate = DateTime.Now;
                customerCare.EffecttiveToDate = DateTime.Now;
                customerCare.CustomerCareContactType = customerCareContactType;
                customerCare.CustomerCareTitle = parameter.Queue.Title;
                customerCare.CustomerCareContent = "";
                customerCare.CustomerCareContentEmail = parameter.Queue.SendContent;
                customerCare.IsSendEmailNow = true;
                customerCare.CustomerCareType = 1;
                customerCare.StatusId = customerCareStatusId;
                customerCare.CreateDate = DateTime.Now;
                customerCare.CreateById = parameter.UserId;
                customerCare.ActiveDate = DateTime.Now;
                context.CustomerCare.Add(customerCare);
                context.SaveChanges();

                //Tạo khách hàng của chương trình CSKH
                CustomerCareCustomer CustomerCareCustomerE = new CustomerCareCustomer
                {
                    CustomerCareId = customerCare.CustomerCareId,
                    CustomerId = parameter.Queue.CustomerId,
                    StatusId = statusId,
                    CreateDate = DateTime.Now,
                    CreateById = parameter.UserId,
                };
                context.CustomerCareCustomer.Add(CustomerCareCustomerE);

                //Tạo bộ lọc
                CustomerCareFilter customerCareFilter = new CustomerCareFilter
                {
                    CustomerCareId = customerCare.CustomerCareId,
                    QueryContent = "",
                    CreateDate = DateTime.Now,
                    CreateById = parameter.UserId
                };
                context.CustomerCareFilter.Add(customerCareFilter);
                context.SaveChanges();

                //Gửi email
                parameter.Queue.Title = parameter.Queue.Title + " - " +
                                        (context.Customer.FirstOrDefault(cus =>
                                             cus.CustomerId == parameter.Queue.CustomerId.Value) != null
                                            ? context.Customer.FirstOrDefault(cus =>
                                                cus.CustomerId == parameter.Queue.CustomerId.Value).CustomerName
                                            : "");
                parameter.Queue.SendContent =
                    replaceTokenForContent(parameter.Queue.SendContent, parameter.Queue.CustomerId.Value);
                parameter.Queue.CustomerCareId = customerCare.CustomerCareId;
                parameter.Queue.SenDate = DateTime.Now;
                parameter.Queue.CreateDate = DateTime.Now;
                context.Queue.Add(parameter.Queue);
                context.SaveChanges();

                return new SendQuickEmailResult
                {
                    QueueId = parameter.Queue.QueueId,
                    Message = "Gửi email thành công",
                    Status = true
                };
            }
            catch (Exception e)
            {
                return new SendQuickEmailResult
                {
                    Message = "Đã có lỗi khi gửi email",
                    Status = false
                };
            }
        }

        public SendQuickGiftResult SendQuickGift(SendQuickGiftParameter parameter)
        {
            try
            {
                //Tạo CSKH
                var customerCare = new CustomerCare();
                var categoryTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TKH").CategoryTypeId;
                var statusId = context.Category.FirstOrDefault(ca => ca.CategoryCode == "DSO" && ca.CategoryTypeId == categoryTypeId).CategoryId;
                var employeeId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
                var customerCareStatusTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TCS").CategoryTypeId;
                var customerCareStatusId = context.Category.FirstOrDefault(ca => ca.CategoryCode == "Closed" && ca.CategoryTypeId == customerCareStatusTypeId).CategoryId;
                var customerCareContactTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "HCS").CategoryTypeId;
                var customerCareContactType = context.Category.FirstOrDefault(ca => ca.CategoryCode == "Gift" && ca.CategoryTypeId == customerCareContactTypeId).CategoryId;

                int currentYear = DateTime.Now.Year % 100;
                int currentMonth = DateTime.Now.Month;
                int currentDate = DateTime.Now.Day;
                var lstRequestPayment = context.CustomerCare.Where(w => w.YearCode == currentYear && w.MonthCode == currentMonth && w.DateCode == currentDate).Select(s => s.NumberCode.Value).ToList();
                int MaxNumberCode = 0;
                if (lstRequestPayment.Count > 0)
                {
                    MaxNumberCode = lstRequestPayment.Max();
                }
                customerCare.CustomerCareCode = string.Format("CSKH{0}{1}{2}", currentYear, currentMonth, (MaxNumberCode + 1).ToString("D3"));
                customerCare.NumberCode = MaxNumberCode + 1;
                customerCare.YearCode = currentYear;
                customerCare.MonthCode = currentMonth;
                customerCare.DateCode = currentDate;
                customerCare.EmployeeCharge = employeeId;
                customerCare.EffecttiveFromDate = DateTime.Now;
                customerCare.EffecttiveToDate = DateTime.Now;
                customerCare.CustomerCareContactType = customerCareContactType;
                customerCare.CustomerCareTitle = parameter.Title;
                customerCare.CustomerCareContent = "";

                customerCare.GiftCustomerType1 = parameter.GiftCustomerType1;
                customerCare.GiftTypeId1 = parameter.GiftTypeId1;
                customerCare.GiftTotal1 = parameter.GiftTotal1;

                customerCare.GiftCustomerType2 = parameter.GiftCustomerType2;
                customerCare.GiftTypeId2 = parameter.GiftTypeId2;
                customerCare.GiftTotal2 = parameter.GiftTotal2;

                customerCare.CustomerCareType = 1;
                customerCare.StatusId = customerCareStatusId;
                customerCare.ActiveDate = DateTime.Now;
                customerCare.CreateDate = DateTime.Now;
                customerCare.CreateById = parameter.UserId;
                context.CustomerCare.Add(customerCare);
                context.SaveChanges();

                //Tạo khách hàng của chương trình CSKH
                CustomerCareCustomer CustomerCareCustomerE = new CustomerCareCustomer
                {
                    CustomerCareId = customerCare.CustomerCareId,
                    CustomerId = parameter.CustomerId,
                    StatusId = statusId,
                    CreateDate = DateTime.Now,
                    CreateById = parameter.UserId,
                };
                context.CustomerCareCustomer.Add(CustomerCareCustomerE);

                //Tạo bộ lọc
                CustomerCareFilter customerCareFilter = new CustomerCareFilter
                {
                    CustomerCareId = customerCare.CustomerCareId,
                    QueryContent = "",
                    CreateDate = DateTime.Now,
                    CreateById = parameter.UserId
                };
                context.CustomerCareFilter.Add(customerCareFilter);
                context.SaveChanges();

                return new SendQuickGiftResult
                {
                    CustomerCareId = customerCare.CustomerCareId,
                    Message = "Gửi quà thành công",
                    Status = true
                };
            }
            catch (Exception)
            {
                return new SendQuickGiftResult
                {
                    Message = "Đã có lỗi khi gửi quà",
                    Status = false
                };
            }
        }

        public SendQuickSMSResult SendQuickSMS(SendQuickSMSParameter parameter)
        {
            try
            {
                //Tạo CSKH
                var customerCare = new CustomerCare();
                var categoryTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TKH").CategoryTypeId;
                var statusId = context.Category.FirstOrDefault(ca => ca.CategoryCode == "DSO" && ca.CategoryTypeId == categoryTypeId).CategoryId;
                var employeeId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
                var customerCareStatusTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TCS").CategoryTypeId;
                var customerCareStatusId = context.Category.FirstOrDefault(ca => ca.CategoryCode == "Closed" && ca.CategoryTypeId == customerCareStatusTypeId).CategoryId;
                var customerCareContactTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "HCS").CategoryTypeId;
                var customerCareContactType = context.Category.FirstOrDefault(ca => ca.CategoryCode == "SMS" && ca.CategoryTypeId == customerCareContactTypeId).CategoryId;

                int currentYear = DateTime.Now.Year % 100;
                int currentMonth = DateTime.Now.Month;
                int currentDate = DateTime.Now.Day;
                var lstRequestPayment = context.CustomerCare.Where(w => w.YearCode == currentYear && w.MonthCode == currentMonth && w.DateCode == currentDate).Select(s => s.NumberCode.Value).ToList();
                int MaxNumberCode = 0;
                if (lstRequestPayment.Count > 0)
                {
                    MaxNumberCode = lstRequestPayment.Max();
                }
                customerCare.CustomerCareCode = string.Format("CSKH{0}{1}{2}", currentYear, currentMonth, (MaxNumberCode + 1).ToString("D3"));
                customerCare.NumberCode = MaxNumberCode + 1;
                customerCare.YearCode = currentYear;
                customerCare.MonthCode = currentMonth;
                customerCare.DateCode = currentDate;
                customerCare.EmployeeCharge = employeeId;
                customerCare.EffecttiveFromDate = DateTime.Now;
                customerCare.EffecttiveToDate = DateTime.Now;
                customerCare.CustomerCareContactType = customerCareContactType;
                customerCare.CustomerCareTitle = parameter.Queue.Title;
                customerCare.CustomerCareContent = "";
                customerCare.CustomerCareContentSms = parameter.Queue.SendContent;
                customerCare.IsSendNow = true;
                customerCare.CustomerCareType = 1;
                customerCare.StatusId = customerCareStatusId;
                customerCare.CreateDate = DateTime.Now;
                customerCare.CreateById = parameter.UserId;
                customerCare.ActiveDate = DateTime.Now;
                context.CustomerCare.Add(customerCare);
                context.SaveChanges();

                //Tạo khách hàng của chương trình CSKH
                CustomerCareCustomer CustomerCareCustomerE = new CustomerCareCustomer
                {
                    CustomerCareId = customerCare.CustomerCareId,
                    CustomerId = parameter.Queue.CustomerId,
                    StatusId = statusId,
                    CreateDate = DateTime.Now,
                    CreateById = parameter.UserId,
                };
                context.CustomerCareCustomer.Add(CustomerCareCustomerE);

                //Tạo bộ lọc
                CustomerCareFilter customerCareFilter = new CustomerCareFilter
                {
                    CustomerCareId = customerCare.CustomerCareId,
                    QueryContent = "",
                    CreateDate = DateTime.Now,
                    CreateById = parameter.UserId
                };
                context.CustomerCareFilter.Add(customerCareFilter);
                context.SaveChanges();

                //Gửi SMS
                parameter.Queue.SendContent = replaceTokenForContent(parameter.Queue.SendContent, parameter.Queue.CustomerId.Value);
                parameter.Queue.CustomerCareId = customerCare.CustomerCareId;
                parameter.Queue.SenDate = DateTime.Now;
                parameter.Queue.CreateDate = DateTime.Now;
                context.Queue.Add(parameter.Queue);
                context.SaveChanges();

                return new SendQuickSMSResult
                {
                    QueueId = parameter.Queue.QueueId,
                    Message = "Gửi SMS thành công",
                    Status = true
                };
            }
            catch (Exception)
            {
                return new SendQuickSMSResult
                {
                    Message = "Đã có lỗi khi gửi SMS",
                    Status = false
                };
            }
        }

        public UpdateCustomerCareResult UpdateCustomerCare(UpdateCustomerCareParameter parameter)
        {
            try
            {
                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                var customerCare = context.CustomerCare.FirstOrDefault(w => w.CustomerCareId == parameter.CustomerCare.CustomerCareId);

                customerCare.EmployeeCharge = parameter.CustomerCare.EmployeeCharge;
                customerCare.EffecttiveFromDate = parameter.CustomerCare.EffecttiveFromDate;
                customerCare.EffecttiveToDate = parameter.CustomerCare.EffecttiveToDate;
                customerCare.CustomerCareTitle = parameter.CustomerCare.CustomerCareTitle;
                customerCare.CustomerCareContent = parameter.CustomerCare.CustomerCareContent;
                customerCare.CustomerCareContactType = parameter.CustomerCare.CustomerCareContactType;  //Hình thức CSKH 
                customerCare.ExpectedAmount = parameter.CustomerCare.ExpectedAmount;    //Chi phí dự kiến


                var categoryTypeId1 = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TCS")
                    .CategoryTypeId;
                var listStatusOfCustomerCare = context.Category.Where(x => x.CategoryTypeId == categoryTypeId1).ToList();
                var statusActive = listStatusOfCustomerCare.FirstOrDefault(x => x.CategoryCode == "Active").CategoryId;
                var statusStoped = listStatusOfCustomerCare.FirstOrDefault(x => x.CategoryCode == "Stoped").CategoryId;
                var statusClosed = listStatusOfCustomerCare.FirstOrDefault(x => x.CategoryCode == "Closed").CategoryId;
                var statusNew = listStatusOfCustomerCare.FirstOrDefault(x => x.CategoryCode == "New").CategoryId;

                #region Kiểm tra nếu Chương trình CSKH đang có trạng thái Kích hoạt thì không cho Kích hoạt nữa

                if (customerCare.StatusId == statusActive && parameter.CustomerCare.StatusId == statusActive)
                {
                    return new UpdateCustomerCareResult()
                    {
                        Status = false,
                        Message = "Chương trình đã được Kích hoạt không thể Kích hoạt lại"
                    };
                }

                #endregion

                if (parameter.CustomerCare.StatusId == statusActive)
                {
                    //Nếu trạng thái của chương trình CSKH là Kích hoạt thì cập nhật trường ActiveDate
                    //Nếu trường ActiveDate khác null thì không cập nhật vì nó đã được kích hoạt rồi
                    if (customerCare.ActiveDate == null)
                    {
                        customerCare.ActiveDate = DateTime.Now;
                    }
                }
                else if (parameter.CustomerCare.StatusId == statusStoped ||
                         parameter.CustomerCare.StatusId == statusClosed)
                {
                    /*
                     * Nếu trạng thái là Đóng hoặc Đã dừng thì xóa hết các bản ghi có CustomerCareId nếu
                     * SenDate > Thời điểm hiện tại (Những Email hoặc SMS chưa được gửi)
                     */
                    var listQueueRemove = context.Queue.Where(x =>
                        x.CustomerCareId == customerCare.CustomerCareId && x.SenDate > DateTime.Now).ToList();
                    context.Queue.RemoveRange(listQueueRemove);
                    context.SaveChanges();
                }

                var TypesOfCustomerCare = (from category in context.Category
                                           join categoryType in context.CategoryType on category.CategoryTypeId equals categoryType.CategoryTypeId
                                           where categoryType.CategoryTypeCode == "HCS" && category.CategoryId == parameter.CustomerCare.CustomerCareContactType
                                           select category).FirstOrDefault();

                var listContact = context.Contact.ToList();

                switch (TypesOfCustomerCare.CategoryCode)
                {
                    case "Email":

                        #region cập nhật các trường liên quan đến email

                        customerCare.CustomerCareContentEmail = parameter.CustomerCare.CustomerCareContentEmail;
                        customerCare.IsSendEmailNow = parameter.CustomerCare.IsSendEmailNow;
                        customerCare.SendEmailDate = parameter.CustomerCare.IsSendEmailNow.Value
                            ? null
                            : parameter.CustomerCare.SendEmailDate;
                        customerCare.SendEmailHour = parameter.CustomerCare.IsSendEmailNow.Value
                            ? null
                            : parameter.CustomerCare.SendEmailHour;

                        #endregion

                        #region clear các trường không liên quan đến email

                        customerCare.CustomerCareContentSms = null;
                        customerCare.IsSendNow = null;
                        customerCare.SendDate = null;
                        customerCare.SendHour = null;
                        customerCare.IsEvent = null;
                        customerCare.CustomerCareEvent = null;
                        customerCare.CustomerCareEventHour = null;
                        customerCare.CustomerCareVoucher = null;
                        customerCare.DiscountAmount = null;
                        customerCare.PercentDiscountAmount = null;
                        customerCare.GiftCustomerType1 = null;
                        customerCare.GiftTypeId1 = null;
                        customerCare.GiftTotal1 = null;
                        customerCare.GiftCustomerType2 = null;
                        customerCare.GiftTypeId2 = null;
                        customerCare.GiftTotal2 = null;

                        #endregion

                        break;

                    case "SMS":

                        #region cập nhật các trường liên quan đến SMS

                        customerCare.CustomerCareContentSms = parameter.CustomerCare.CustomerCareContentSms;
                        customerCare.IsSendNow = parameter.CustomerCare.IsSendNow;
                        customerCare.SendDate = parameter.CustomerCare.IsSendNow.Value
                            ? null
                            : parameter.CustomerCare.SendDate;
                        customerCare.SendHour = parameter.CustomerCare.IsSendNow.Value
                            ? null
                            : parameter.CustomerCare.SendHour;

                        #endregion

                        #region clear các trường không liên quan đến SMS

                        customerCare.CustomerCareContentEmail = null;
                        customerCare.IsSendEmailNow = null;
                        customerCare.SendEmailDate = null;
                        customerCare.SendEmailHour = null;
                        customerCare.IsEvent = null;
                        customerCare.CustomerCareEvent = null;
                        customerCare.CustomerCareEventHour = null;
                        customerCare.CustomerCareVoucher = null;
                        customerCare.DiscountAmount = null;
                        customerCare.PercentDiscountAmount = null;
                        customerCare.GiftCustomerType1 = null;
                        customerCare.GiftTypeId1 = null;
                        customerCare.GiftTotal1 = null;
                        customerCare.GiftCustomerType2 = null;
                        customerCare.GiftTypeId2 = null;
                        customerCare.GiftTotal2 = null;

                        #endregion

                        break;

                    case "Gift":

                        #region cập nhật các trường liên quan đến Gift

                        customerCare.GiftCustomerType1 = parameter.CustomerCare.GiftCustomerType1;
                        customerCare.GiftTypeId1 = parameter.CustomerCare.GiftTypeId1;
                        customerCare.GiftTotal1 = parameter.CustomerCare.GiftTotal1;
                        customerCare.GiftCustomerType2 = parameter.CustomerCare.GiftCustomerType2;
                        customerCare.GiftTypeId2 = parameter.CustomerCare.GiftTypeId2;
                        customerCare.GiftTotal2 = parameter.CustomerCare.GiftTotal2;

                        #endregion

                        #region clear các trường không liên quan đến Gift

                        customerCare.CustomerCareContentSms = null;
                        customerCare.IsSendNow = null;
                        customerCare.SendDate = null;
                        customerCare.SendHour = null;
                        customerCare.CustomerCareContentEmail = null;
                        customerCare.IsSendEmailNow = null;
                        customerCare.SendEmailDate = null;
                        customerCare.SendEmailHour = null;
                        customerCare.IsEvent = null;
                        customerCare.CustomerCareEvent = null;
                        customerCare.CustomerCareEventHour = null;
                        customerCare.CustomerCareVoucher = null;
                        customerCare.DiscountAmount = null;
                        customerCare.PercentDiscountAmount = null;

                        #endregion

                        break;

                    case "CallPhone":

                        #region clear các trường không liên quan đến Gọi điện

                        customerCare.CustomerCareContentSms = null;
                        customerCare.IsSendNow = null;
                        customerCare.SendDate = null;
                        customerCare.SendHour = null;
                        customerCare.CustomerCareContentEmail = null;
                        customerCare.IsSendEmailNow = null;
                        customerCare.SendEmailDate = null;
                        customerCare.SendEmailHour = null;
                        customerCare.IsEvent = null;
                        customerCare.CustomerCareEvent = null;
                        customerCare.CustomerCareEventHour = null;
                        customerCare.CustomerCareVoucher = null;
                        customerCare.DiscountAmount = null;
                        customerCare.PercentDiscountAmount = null;
                        customerCare.GiftCustomerType1 = null;
                        customerCare.GiftTypeId1 = null;
                        customerCare.GiftTotal1 = null;
                        customerCare.GiftCustomerType2 = null;
                        customerCare.GiftTypeId2 = null;
                        customerCare.GiftTotal2 = null;

                        #endregion

                        break;
                }

                customerCare.StatusId = parameter.CustomerCare.StatusId;
                customerCare.UpdateDate = DateTime.Now;
                customerCare.UpdateById = parameter.UserId;
                // Update Customer Care
                context.CustomerCare.Update(customerCare);
                context.SaveChanges();

                // Push to Queue
                if (statusActive != Guid.Empty)
                {
                    if (parameter.CustomerCare.StatusId == statusActive)
                    {
                        if (parameter.CustomerId.Count > 0)
                        {
                            var EmployeeId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
                            var FromTo = listContact.FirstOrDefault(c => c.ObjectId == EmployeeId && c.ObjectType == "EMP")?.Email;
                            List<Queue> lstQueue = new List<Queue>();

                            if (TypesOfCustomerCare.CategoryCode == "Email")
                            {
                                if (FromTo != null)
                                {
                                    parameter.CustomerId.ForEach(item =>
                                    {
                                        var listSendTo = listContact.Where(x =>
                                                x.ObjectId == item &&
                                                (x.ObjectType == "CUS" || x.ObjectType == "CUS_CON"))
                                            .ToList();

                                        if (listSendTo.Count > 0)
                                        {
                                            listSendTo.ForEach(cont =>
                                            {
                                                var sendToEmail = cont.Email;
                                                var sendToWorkEmail = cont.WorkEmail;
                                                var sendToOtherEmail = cont.OtherEmail;

                                                if (!string.IsNullOrEmpty(sendToEmail))
                                                {
                                                    Queue queueCreate = new Queue
                                                    {
                                                        FromTo = FromTo,
                                                        SendTo = sendToEmail,
                                                        SendContent = replaceTokenForContent(parameter.CustomerCare.CustomerCareContentEmail, item),
                                                        Method = TypesOfCustomerCare.CategoryCode,
                                                        Title = parameter.CustomerCare.CustomerCareTitle + " - " + (context.Customer.FirstOrDefault(cus => cus.CustomerId == item) != null ?
                                                                    context.Customer.FirstOrDefault(cus => cus.CustomerId == item).CustomerCode : ""),
                                                        IsSend = false,
                                                        SenDate = parameter.CustomerCare.IsSendEmailNow == true ? DateTime.Now : SetDate(parameter.CustomerCare.SendEmailDate, parameter.CustomerCare.SendEmailHour),
                                                        CustomerId = item,
                                                        CustomerCareId = parameter.CustomerCare.CustomerCareId,
                                                        CreateDate = DateTime.Now,
                                                        CreateById = parameter.UserId
                                                    };
                                                    lstQueue.Add(queueCreate);
                                                }

                                                if (!string.IsNullOrEmpty(sendToWorkEmail))
                                                {
                                                    Queue queueCreate = new Queue
                                                    {
                                                        FromTo = FromTo,
                                                        SendTo = sendToWorkEmail,
                                                        SendContent = replaceTokenForContent(parameter.CustomerCare.CustomerCareContentEmail, item),
                                                        Method = TypesOfCustomerCare.CategoryCode,
                                                        Title = parameter.CustomerCare.CustomerCareTitle + " - " + (context.Customer.FirstOrDefault(cus => cus.CustomerId == item) != null ?
                                                                    context.Customer.FirstOrDefault(cus => cus.CustomerId == item).CustomerCode : ""),
                                                        IsSend = false,
                                                        SenDate = parameter.CustomerCare.IsSendEmailNow == true ? DateTime.Now : SetDate(parameter.CustomerCare.SendEmailDate, parameter.CustomerCare.SendEmailHour),
                                                        CustomerId = item,
                                                        CustomerCareId = parameter.CustomerCare.CustomerCareId,
                                                        CreateDate = DateTime.Now,
                                                        CreateById = parameter.UserId
                                                    };
                                                    lstQueue.Add(queueCreate);
                                                }

                                                if (!string.IsNullOrEmpty(sendToOtherEmail))
                                                {
                                                    Queue queueCreate = new Queue
                                                    {
                                                        FromTo = FromTo,
                                                        SendTo = sendToOtherEmail,
                                                        SendContent = replaceTokenForContent(parameter.CustomerCare.CustomerCareContentEmail, item),
                                                        Method = TypesOfCustomerCare.CategoryCode,
                                                        Title = parameter.CustomerCare.CustomerCareTitle + " - " + (context.Customer.FirstOrDefault(cus => cus.CustomerId == item) != null ?
                                                                    context.Customer.FirstOrDefault(cus => cus.CustomerId == item).CustomerCode : ""),
                                                        IsSend = false,
                                                        SenDate = parameter.CustomerCare.IsSendEmailNow == true ? DateTime.Now : SetDate(parameter.CustomerCare.SendEmailDate, parameter.CustomerCare.SendEmailHour),
                                                        CustomerId = item,
                                                        CustomerCareId = parameter.CustomerCare.CustomerCareId,
                                                        CreateDate = DateTime.Now,
                                                        CreateById = parameter.UserId
                                                    };
                                                    lstQueue.Add(queueCreate);
                                                }
                                            });
                                        }
                                    });

                                    if (lstQueue.Count > 0)
                                    {
                                        context.Queue.AddRange(lstQueue);
                                        context.SaveChanges();
                                    }
                                }
                            }
                            else if (TypesOfCustomerCare.CategoryCode == "SMS")
                            {
                                List<Queue> lstQueue_sms = new List<Queue>();
                                parameter.CustomerId.ForEach(item =>
                                {
                                    var listSendTo = listContact.Where(x =>
                                            x.ObjectId == item && (x.ObjectType == "CUS" || x.ObjectType == "CUS_CON"))
                                        .ToList();

                                    if (listSendTo.Count > 0)
                                    {
                                        listSendTo.ForEach(cont =>
                                        {
                                            var sendToPhone = cont.Phone;
                                            var sendToWorkPhone = cont.WorkPhone;
                                            var sendToOtherPhone = cont.OtherPhone;

                                            if (!string.IsNullOrEmpty(sendToPhone))
                                            {
                                                Queue queueCreateSMS = new Queue
                                                {
                                                    SendTo = sendToPhone,
                                                    SendContent = replaceTokenForContent(parameter.CustomerCare.CustomerCareContentSms, item),
                                                    Method = TypesOfCustomerCare.CategoryCode,
                                                    Title = parameter.CustomerCare.CustomerCareTitle,
                                                    IsSend = false,
                                                    SenDate = parameter.CustomerCare.IsSendNow == true ? DateTime.Now : SetDate(parameter.CustomerCare.SendDate, parameter.CustomerCare.SendHour),
                                                    CustomerId = item,
                                                    CustomerCareId = parameter.CustomerCare.CustomerCareId,
                                                    CreateDate = DateTime.Now,
                                                    CreateById = parameter.UserId
                                                };
                                                lstQueue_sms.Add(queueCreateSMS);
                                            }

                                            if (!string.IsNullOrEmpty(sendToWorkPhone))
                                            {
                                                Queue queueCreateSMS = new Queue
                                                {
                                                    SendTo = sendToWorkPhone,
                                                    SendContent = replaceTokenForContent(parameter.CustomerCare.CustomerCareContentSms, item),
                                                    Method = TypesOfCustomerCare.CategoryCode,
                                                    Title = parameter.CustomerCare.CustomerCareTitle,
                                                    IsSend = false,
                                                    SenDate = parameter.CustomerCare.IsSendNow == true ? DateTime.Now : SetDate(parameter.CustomerCare.SendDate, parameter.CustomerCare.SendHour),
                                                    CustomerId = item,
                                                    CustomerCareId = parameter.CustomerCare.CustomerCareId,
                                                    CreateDate = DateTime.Now,
                                                    CreateById = parameter.UserId
                                                };
                                                lstQueue_sms.Add(queueCreateSMS);
                                            }

                                            if (!string.IsNullOrEmpty(sendToOtherPhone))
                                            {
                                                Queue queueCreateSMS = new Queue
                                                {
                                                    SendTo = sendToOtherPhone,
                                                    SendContent = replaceTokenForContent(parameter.CustomerCare.CustomerCareContentSms, item),
                                                    Method = TypesOfCustomerCare.CategoryCode,
                                                    Title = parameter.CustomerCare.CustomerCareTitle,
                                                    IsSend = false,
                                                    SenDate = parameter.CustomerCare.IsSendNow == true ? DateTime.Now : SetDate(parameter.CustomerCare.SendDate, parameter.CustomerCare.SendHour),
                                                    CustomerId = item,
                                                    CustomerCareId = parameter.CustomerCare.CustomerCareId,
                                                    CreateDate = DateTime.Now,
                                                    CreateById = parameter.UserId
                                                };
                                                lstQueue_sms.Add(queueCreateSMS);
                                            }
                                        });
                                    }
                                });

                                if (lstQueue_sms.Count > 0)
                                {
                                    context.Queue.AddRange(lstQueue_sms);
                                    context.SaveChanges();
                                }
                            }
                        }
                    }
                }
                return new UpdateCustomerCareResult
                {
                    Message = "Cập nhật chương trình chăm sóc thành công",
                    Status = true
                };
            }
            catch (Exception e)
            {
                return new UpdateCustomerCareResult
                {
                    Message = e.Message,
                    Status = false
                };
            }
        }

        public UpdateCustomerCareFeedBackResult UpdateCustomerCareFeedBack(UpdateCustomerCareFeedBackParameter parameter)
        {
            try
            {
                context.CustomerCareFeedBack.Update(parameter.CustomerCareFeedBack);
                context.SaveChanges();
                return new UpdateCustomerCareFeedBackResult
                {
                    CustomerCareFeedBackId = parameter.CustomerCareFeedBack.CustomerCareFeedBackId,
                    Message = "Thông tin phản hồi đã được cập nhật",
                    Status = true
                };
            }
            catch (Exception)
            {
                return new UpdateCustomerCareFeedBackResult
                {
                    Message = "Đã có lỗi khi lưu thông tin phản hồi",
                    Status = false
                };
            }
        }

        public UpdateStatusCustomerCareResult UpdateStatusCustomerCare(UpdateStatusCustomerCareParameter parameter)
        {
            try
            {
                var customerCare = context.CustomerCare.FirstOrDefault(c => c.CustomerCareId == parameter.CustomerCareId);
                if (customerCare != null)
                {
                    var categoryTypeId = context.CategoryType.FirstOrDefault(e => e.CategoryTypeCode == "TCS").CategoryTypeId;
                    if (categoryTypeId != null)
                    {
                        var listCategory = context.Category.Where(x => x.CategoryTypeId == categoryTypeId).ToList();

                        if (listCategory.Count > 0)
                        {
                            var statusCode = listCategory.FirstOrDefault(x => x.CategoryId == customerCare.StatusId).CategoryCode;
                            var statusCodeChange = listCategory.FirstOrDefault(x => x.CategoryId == parameter.StatusId).CategoryCode;
                            var statusIdChange = listCategory.FirstOrDefault(x => x.CategoryId == parameter.StatusId).CategoryId;

                            if (statusCode == "New" && statusCodeChange == "Stoped")
                            {
                                customerCare.StatusId = parameter.StatusId;
                            }
                            else if (statusCode == "New" && statusCodeChange == "Active")
                            {
                                customerCare.StatusId = parameter.StatusId;
                                customerCare.ActiveDate = DateTime.Now;
                                var listCustomer = context.CustomerCareCustomer.Where(x => x.CustomerCareId == parameter.CustomerCareId).ToList();
                                if (parameter.TypeCusCareCode == "Email")
                                {
                                    if (parameter.IsSendNow == true)
                                    {
                                        customerCare.IsSendEmailNow = true;
                                        customerCare.SendEmailDate = null;
                                        customerCare.SendEmailHour = null;
                                    }
                                    else
                                    {
                                        customerCare.IsSendEmailNow = false;
                                        customerCare.SendEmailDate = parameter.SendDate;
                                        customerCare.SendEmailHour = parameter.SendHour;

                                        var today = DateTime.Now;
                                        var sendDate = SetDate(customerCare.SendEmailDate, customerCare.SendEmailHour);
                                        var compare = DateTime.Compare(sendDate, today);
                                        if (compare <= 0)
                                        {
                                            //Nếu thời gian gửi là ở thời điểm quá khứ thì báo lỗi
                                            return new UpdateStatusCustomerCareResult
                                            {
                                                Message = "Không thể kích hoạt chương trình kiểm tra lại thời gian gửi",
                                                Status = false
                                            };
                                        }
                                    }

                                    //add vào bảng Queue
                                    var EmployeeId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
                                    var FromTo = context.Contact.FirstOrDefault(c => c.ObjectId == EmployeeId && c.ObjectType == "EMP")?.Email;

                                    List<Queue> lstQueue = new List<Queue>();
                                    if (FromTo != null)
                                    {
                                        if (listCustomer.Count > 0)
                                        {
                                            listCustomer.ForEach(item =>
                                            {
                                                //var sendToEmail = context.Contact.FirstOrDefault(x => x.ObjectId == item.CustomerId && x.ObjectType == "CUS")?.Email;
                                                //var sendToWorkEmail = context.Contact.FirstOrDefault(x => x.ObjectId == item.CustomerId && x.ObjectType == "CUS")?.WorkEmail;
                                                //var sendToOtherEmail = context.Contact.FirstOrDefault(x => x.ObjectId == item.CustomerId && x.ObjectType == "CUS")?.OtherEmail;
                                                var sendToEmail = context.Contact.Where(x => x.ObjectId == item.CustomerId).ToList();
                                                var sendToWorkEmail = context.Contact.Where(x => x.ObjectId == item.CustomerId).ToList();
                                                var sendToOtherEmail = context.Contact.Where(x => x.ObjectId == item.CustomerId).ToList();
                                                if (sendToEmail != null)
                                                {
                                                    sendToEmail.ForEach(itemEmail =>
                                                    {
                                                        if (itemEmail.Email != null)
                                                        {
                                                            Queue queueCreate = new Queue
                                                            {
                                                                FromTo = FromTo,
                                                                SendTo = itemEmail.Email,
                                                                SendContent = replaceTokenForContent(customerCare.CustomerCareContentEmail, item.CustomerId.Value),
                                                                Method = parameter.TypeCusCareCode,
                                                                Title = customerCare.CustomerCareTitle + " - " + (context.Customer.FirstOrDefault(cus => cus.CustomerId == item.CustomerId.Value) != null ?
                                                                        context.Customer.FirstOrDefault(cus => cus.CustomerId == item.CustomerId.Value).CustomerCode : ""),
                                                                IsSend = false,
                                                                SenDate = parameter.IsSendNow == true ? DateTime.Now : SetDate(customerCare.SendEmailDate, customerCare.SendEmailHour),
                                                                CustomerId = item.CustomerId.Value,
                                                                CustomerCareId = customerCare.CustomerCareId,
                                                                CreateDate = DateTime.Now,
                                                                CreateById = parameter.UserId
                                                            };
                                                            lstQueue.Add(queueCreate);
                                                        }
                                                    });
                                                }
                                                if (sendToWorkEmail != null)
                                                {
                                                    sendToWorkEmail.ForEach(itemEmail =>
                                                    {
                                                        if (itemEmail.WorkEmail != null)
                                                        {
                                                            Queue queueCreate = new Queue
                                                            {
                                                                FromTo = FromTo,
                                                                SendTo = itemEmail.WorkEmail,
                                                                SendContent = replaceTokenForContent(customerCare.CustomerCareContentEmail, item.CustomerId.Value),
                                                                Method = parameter.TypeCusCareCode,
                                                                Title = customerCare.CustomerCareTitle + " - " + (context.Customer.FirstOrDefault(cus => cus.CustomerId == item.CustomerId.Value) != null ?
                                                                        context.Customer.FirstOrDefault(cus => cus.CustomerId == item.CustomerId.Value).CustomerCode : ""),
                                                                IsSend = false,
                                                                SenDate = parameter.IsSendNow == true ? DateTime.Now : SetDate(customerCare.SendEmailDate, customerCare.SendEmailHour),
                                                                CustomerId = item.CustomerId.Value,
                                                                CustomerCareId = customerCare.CustomerCareId,
                                                                CreateDate = DateTime.Now,
                                                                CreateById = parameter.UserId
                                                            };
                                                            lstQueue.Add(queueCreate);
                                                        }
                                                    });
                                                }
                                                if (sendToOtherEmail != null)
                                                {
                                                    sendToOtherEmail.ForEach(itemEmail =>
                                                    {
                                                        if (itemEmail.OtherEmail != null)
                                                        {
                                                            Queue queueCreate = new Queue
                                                            {
                                                                FromTo = FromTo,
                                                                SendTo = itemEmail.OtherEmail,
                                                                SendContent = replaceTokenForContent(customerCare.CustomerCareContentEmail, item.CustomerId.Value),
                                                                Method = parameter.TypeCusCareCode,
                                                                Title = customerCare.CustomerCareTitle + " - " + (context.Customer.FirstOrDefault(cus => cus.CustomerId == item.CustomerId.Value) != null ?
                                                                        context.Customer.FirstOrDefault(cus => cus.CustomerId == item.CustomerId.Value).CustomerCode : ""),
                                                                IsSend = false,
                                                                SenDate = parameter.IsSendNow == true ? DateTime.Now : SetDate(customerCare.SendEmailDate, customerCare.SendEmailHour),
                                                                CustomerId = item.CustomerId.Value,
                                                                CustomerCareId = customerCare.CustomerCareId,
                                                                CreateDate = DateTime.Now,
                                                                CreateById = parameter.UserId
                                                            };
                                                            lstQueue.Add(queueCreate);
                                                        }
                                                    });
                                                }
                                            });
                                        }
                                        else
                                        {
                                            return new UpdateStatusCustomerCareResult
                                            {
                                                Message = "Chương trình CSKH này hiện không có khách hàng",
                                                Status = false
                                            };
                                        }
                                    }
                                    else
                                    {
                                        return new UpdateStatusCustomerCareResult
                                        {
                                            Message = "Bạn không có email để gửi",
                                            Status = false
                                        };
                                    }

                                    if (lstQueue.Count > 0)
                                    {
                                        context.Queue.AddRange(lstQueue);
                                    }
                                }
                                else if (parameter.TypeCusCareCode == "SMS")
                                {
                                    if (parameter.IsSendNow == true)
                                    {
                                        customerCare.IsSendNow = true;
                                        customerCare.SendDate = null;
                                        customerCare.SendHour = null;
                                    }
                                    else
                                    {
                                        customerCare.IsSendNow = false;
                                        customerCare.SendDate = parameter.SendDate;
                                        customerCare.SendHour = parameter.SendHour;

                                        var today = DateTime.Now;
                                        var sendDate = SetDate(customerCare.SendDate, customerCare.SendHour);
                                        var compare = DateTime.Compare(sendDate, today);
                                        if (compare <= 0)
                                        {
                                            //Nếu thời gian gửi là ở thời điểm quá khứ thì báo lỗi
                                            return new UpdateStatusCustomerCareResult
                                            {
                                                Message = "Không thể kích hoạt chương trình kiểm tra lại thời gian gửi",
                                                Status = false
                                            };
                                        }
                                    }

                                    //add vào bảng Queue
                                    List<Queue> lstQueue = new List<Queue>();
                                    if (listCustomer.Count > 0)
                                    {
                                        listCustomer.ForEach(item =>
                                        {
                                            var sendToPhone = context.Contact.FirstOrDefault(x => x.ObjectId == item.CustomerId && x.ObjectType == "CUS")?.Phone;
                                            var sendToWorkPhone = context.Contact.FirstOrDefault(x => x.ObjectId == item.CustomerId && x.ObjectType == "CUS")?.WorkPhone;
                                            var sendToOtherPhone = context.Contact.FirstOrDefault(x => x.ObjectId == item.CustomerId && x.ObjectType == "CUS")?.OtherPhone;
                                            if (sendToPhone != null)
                                            {
                                                Queue queueCreate = new Queue
                                                {
                                                    SendTo = sendToPhone,
                                                    SendContent = replaceTokenForContent(customerCare.CustomerCareContentSms, item.CustomerId.Value),
                                                    Method = parameter.TypeCusCareCode,
                                                    Title = customerCare.CustomerCareTitle,
                                                    IsSend = false,
                                                    SenDate = parameter.IsSendNow == true ? DateTime.Now : SetDate(customerCare.SendDate, customerCare.SendHour),
                                                    CustomerId = item.CustomerId.Value,
                                                    CustomerCareId = customerCare.CustomerCareId,
                                                    CreateDate = DateTime.Now,
                                                    CreateById = parameter.UserId
                                                };
                                                lstQueue.Add(queueCreate);
                                            }
                                            if (sendToWorkPhone != null)
                                            {
                                                Queue queueCreate = new Queue
                                                {
                                                    SendTo = sendToPhone,
                                                    SendContent = replaceTokenForContent(customerCare.CustomerCareContentSms, item.CustomerId.Value),
                                                    Method = parameter.TypeCusCareCode,
                                                    Title = customerCare.CustomerCareTitle,
                                                    IsSend = false,
                                                    SenDate = parameter.IsSendNow == true ? DateTime.Now : SetDate(customerCare.SendDate, customerCare.SendHour),
                                                    CustomerId = item.CustomerId.Value,
                                                    CustomerCareId = customerCare.CustomerCareId,
                                                    CreateDate = DateTime.Now,
                                                    CreateById = parameter.UserId
                                                };
                                                lstQueue.Add(queueCreate);
                                            }
                                            if (sendToOtherPhone != null)
                                            {
                                                Queue queueCreate = new Queue
                                                {
                                                    SendTo = sendToPhone,
                                                    SendContent = replaceTokenForContent(customerCare.CustomerCareContentSms, item.CustomerId.Value),
                                                    Method = parameter.TypeCusCareCode,
                                                    Title = customerCare.CustomerCareTitle,
                                                    IsSend = false,
                                                    SenDate = parameter.IsSendNow == true ? DateTime.Now : SetDate(customerCare.SendDate, customerCare.SendHour),
                                                    CustomerId = item.CustomerId.Value,
                                                    CustomerCareId = customerCare.CustomerCareId,
                                                    CreateDate = DateTime.Now,
                                                    CreateById = parameter.UserId
                                                };
                                                lstQueue.Add(queueCreate);
                                            }
                                        });
                                    }
                                    else
                                    {
                                        return new UpdateStatusCustomerCareResult
                                        {
                                            Message = "Chương trình CSKH này hiện không có khách hàng",
                                            Status = false
                                        };
                                    }

                                    if (lstQueue.Count > 0)
                                    {
                                        context.Queue.AddRange(lstQueue);
                                    }
                                }
                            }
                            else if (statusCode == "New" && statusCodeChange == "Closed")
                            {
                                customerCare.StatusId = parameter.StatusId;
                            }
                            else if (statusCode == "Active" && statusCodeChange == "Stoped")
                            {
                                customerCare.StatusId = parameter.StatusId;
                                if (parameter.TypeCusCareCode == "Email")
                                {
                                    if (customerCare.IsSendEmailNow == true)
                                    {
                                        return new UpdateStatusCustomerCareResult
                                        {
                                            Message = "Chương trình CSKH này đã được thực hiện nên không thể dừng được",
                                            Status = false
                                        };
                                    }
                                    else
                                    {
                                        //Nếu chương trình kích hoạt vào một khoảng thời gian thì kiểm tra thời điểm hiện tại và thời điểm kích hoạt
                                        var today = DateTime.Now;
                                        var sendDate = SetDate(customerCare.SendEmailDate, customerCare.SendEmailHour);
                                        int compare = DateTime.Compare(sendDate, today);
                                        if (compare <= 0)
                                        {
                                            //Nếu đã được gửi thì báo lỗi
                                            return new UpdateStatusCustomerCareResult
                                            {
                                                Message = "Chương trình CSKH này đã được thực hiện nên không thể dừng được",
                                                Status = false
                                            };
                                        }
                                        else
                                        {
                                            //Nếu chưa được gửi thì xóa các bản ghi trong bảng Queue
                                            var listQueue = context.Queue.Where(q => q.CustomerCareId == customerCare.CustomerCareId).ToList();
                                            if (listQueue.Count > 0)
                                            {
                                                context.Queue.RemoveRange(listQueue);
                                            }
                                        }
                                    }
                                }
                                else if (parameter.TypeCusCareCode == "SMS")
                                {
                                    if (customerCare.IsSendNow == true)
                                    {
                                        return new UpdateStatusCustomerCareResult
                                        {
                                            Message = "Chương trình CSKH này đã được thực hiện nên không thể dừng được",
                                            Status = false
                                        };
                                    }
                                    else
                                    {
                                        //Nếu chương trình kích hoạt vào một khoảng thời gian thì kiểm tra thời điểm hiện tại và thời điểm kích hoạt
                                        var today = DateTime.Now;
                                        var sendDate = SetDate(customerCare.SendDate, customerCare.SendHour);
                                        int compare = DateTime.Compare(sendDate, today);
                                        if (compare <= 0)
                                        {
                                            //Nếu đã được gửi thì báo lỗi
                                            return new UpdateStatusCustomerCareResult
                                            {
                                                Message = "Chương trình CSKH này đã được thực hiện nên không thể dừng được",
                                                Status = false
                                            };
                                        }
                                        else
                                        {
                                            //Nếu chưa được gửi thì xóa các bản ghi trong bảng Queue
                                            var listQueue = context.Queue.Where(q => q.CustomerCareId == customerCare.CustomerCareId).ToList();
                                            if (listQueue.Count > 0)
                                            {
                                                context.Queue.RemoveRange(listQueue);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (statusCode == "Active" && statusCodeChange == "Closed")
                            {
                                customerCare.StatusId = parameter.StatusId;
                                if (parameter.TypeCusCareCode == "Email")
                                {
                                    var today = DateTime.Now;
                                    var sendDate = SetDate(customerCare.SendEmailDate, customerCare.SendEmailHour);
                                    var compare = DateTime.Compare(sendDate, today);
                                    if (compare > 0)
                                    {
                                        //Nếu chương trình chưa được thực hiện thì xóa hết bản ghi trong bảng Queue
                                        var listQueue = context.Queue.Where(q => q.CustomerCareId == customerCare.CustomerCareId).ToList();
                                        if (listQueue.Count > 0)
                                        {
                                            context.Queue.RemoveRange(listQueue);
                                        }
                                    }
                                }
                                else if (parameter.TypeCusCareCode == "SMS")
                                {
                                    var today = DateTime.Now;
                                    var sendDate = SetDate(customerCare.SendDate, customerCare.SendHour);
                                    var compare = DateTime.Compare(sendDate, today);
                                    if (compare > 0)
                                    {
                                        //Nếu chương trình chưa được thực hiện thì xóa hết bản ghi trong bảng Queue
                                        var listQueue = context.Queue.Where(q => q.CustomerCareId == customerCare.CustomerCareId).ToList();
                                        if (listQueue.Count > 0)
                                        {
                                            context.Queue.RemoveRange(listQueue);
                                        }
                                    }
                                }
                            }
                            else if (statusCode == "Stoped" && statusCodeChange == "Active")
                            {
                                customerCare.StatusId = parameter.StatusId;
                                customerCare.ActiveDate = DateTime.Now;
                                var listCustomer = context.CustomerCareCustomer.Where(x => x.CustomerCareId == parameter.CustomerCareId).ToList();
                                if (parameter.TypeCusCareCode == "Email")
                                {
                                    if (parameter.IsSendNow == true)
                                    {
                                        customerCare.IsSendEmailNow = true;
                                        customerCare.SendEmailDate = null;
                                        customerCare.SendEmailHour = null;
                                    }
                                    else
                                    {
                                        customerCare.IsSendEmailNow = false;
                                        customerCare.SendEmailDate = parameter.SendDate;
                                        customerCare.SendEmailHour = parameter.SendHour;

                                        var today = DateTime.Now;
                                        var sendDate = SetDate(customerCare.SendEmailDate, customerCare.SendEmailHour);
                                        var compare = DateTime.Compare(sendDate, today);
                                        if (compare <= 0)
                                        {
                                            //Nếu thời gian gửi là ở thời điểm quá khứ thì báo lỗi
                                            return new UpdateStatusCustomerCareResult
                                            {
                                                Message = "Không thể kích hoạt chương trình kiểm tra lại thời gian gửi",
                                                Status = false
                                            };
                                        }
                                    }

                                    //add vào bảng Queue
                                    var EmployeeId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
                                    var FromTo = context.Contact.FirstOrDefault(c => c.ObjectId == EmployeeId && c.ObjectType == "EMP")?.Email;

                                    List<Queue> lstQueue = new List<Queue>();
                                    if (FromTo != null)
                                    {
                                        if (listCustomer.Count > 0)
                                        {
                                            listCustomer.ForEach(item =>
                                            {
                                                var sendToEmail = context.Contact.FirstOrDefault(x => x.ObjectId == item.CustomerId && x.ObjectType == "CUS")?.Email;
                                                var sendToWorkEmail = context.Contact.FirstOrDefault(x => x.ObjectId == item.CustomerId && x.ObjectType == "CUS")?.WorkEmail;
                                                var sendToOtherEmail = context.Contact.FirstOrDefault(x => x.ObjectId == item.CustomerId && x.ObjectType == "CUS")?.OtherEmail;
                                                if (sendToEmail != null)
                                                {
                                                    Queue queueCreate = new Queue
                                                    {
                                                        FromTo = FromTo,
                                                        SendTo = sendToEmail,
                                                        SendContent = replaceTokenForContent(customerCare.CustomerCareContentEmail, item.CustomerId.Value),
                                                        Method = parameter.TypeCusCareCode,
                                                        Title = customerCare.CustomerCareTitle + " - " + (context.Customer.FirstOrDefault(cus => cus.CustomerId == item.CustomerId.Value) != null ?
                                                                context.Customer.FirstOrDefault(cus => cus.CustomerId == item.CustomerId.Value).CustomerCode : ""),
                                                        IsSend = false,
                                                        SenDate = parameter.IsSendNow == true ? DateTime.Now : SetDate(customerCare.SendEmailDate, customerCare.SendEmailHour),
                                                        CustomerId = item.CustomerId.Value,
                                                        CustomerCareId = customerCare.CustomerCareId,
                                                        CreateDate = DateTime.Now,
                                                        CreateById = parameter.UserId
                                                    };
                                                    lstQueue.Add(queueCreate);
                                                }
                                                if (sendToWorkEmail != null)
                                                {
                                                    Queue queueCreate = new Queue
                                                    {
                                                        FromTo = FromTo,
                                                        SendTo = sendToWorkEmail,
                                                        SendContent = replaceTokenForContent(customerCare.CustomerCareContentEmail, item.CustomerId.Value),
                                                        Method = parameter.TypeCusCareCode,
                                                        Title = customerCare.CustomerCareTitle + " - " + (context.Customer.FirstOrDefault(cus => cus.CustomerId == item.CustomerId.Value) != null ?
                                                                context.Customer.FirstOrDefault(cus => cus.CustomerId == item.CustomerId.Value).CustomerCode : ""),
                                                        IsSend = false,
                                                        SenDate = parameter.IsSendNow == true ? DateTime.Now : SetDate(customerCare.SendEmailDate, customerCare.SendEmailHour),
                                                        CustomerId = item.CustomerId.Value,
                                                        CustomerCareId = customerCare.CustomerCareId,
                                                        CreateDate = DateTime.Now,
                                                        CreateById = parameter.UserId
                                                    };
                                                    lstQueue.Add(queueCreate);
                                                }
                                                if (sendToOtherEmail != null)
                                                {
                                                    Queue queueCreate = new Queue
                                                    {
                                                        FromTo = FromTo,
                                                        SendTo = sendToOtherEmail,
                                                        SendContent = replaceTokenForContent(customerCare.CustomerCareContentEmail, item.CustomerId.Value),
                                                        Method = parameter.TypeCusCareCode,
                                                        Title = customerCare.CustomerCareTitle + " - " + (context.Customer.FirstOrDefault(cus => cus.CustomerId == item.CustomerId.Value) != null ?
                                                                context.Customer.FirstOrDefault(cus => cus.CustomerId == item.CustomerId.Value).CustomerCode : ""),
                                                        IsSend = false,
                                                        SenDate = parameter.IsSendNow == true ? DateTime.Now : SetDate(customerCare.SendEmailDate, customerCare.SendEmailHour),
                                                        CustomerId = item.CustomerId.Value,
                                                        CustomerCareId = customerCare.CustomerCareId,
                                                        CreateDate = DateTime.Now,
                                                        CreateById = parameter.UserId
                                                    };
                                                    lstQueue.Add(queueCreate);
                                                }
                                            });
                                        }
                                        else
                                        {
                                            return new UpdateStatusCustomerCareResult
                                            {
                                                Message = "Chương trình CSKH này hiện không có khách hàng",
                                                Status = false
                                            };
                                        }
                                    }
                                    else
                                    {
                                        return new UpdateStatusCustomerCareResult
                                        {
                                            Message = "Bạn không có email để gửi",
                                            Status = false
                                        };
                                    }

                                    if (lstQueue.Count > 0)
                                    {
                                        context.Queue.AddRange(lstQueue);
                                    }
                                }
                                else if (parameter.TypeCusCareCode == "SMS")
                                {
                                    if (parameter.IsSendNow == true)
                                    {
                                        customerCare.IsSendNow = true;
                                        customerCare.SendDate = null;
                                        customerCare.SendHour = null;
                                    }
                                    else
                                    {
                                        customerCare.IsSendNow = false;
                                        customerCare.SendDate = parameter.SendDate;
                                        customerCare.SendHour = parameter.SendHour;

                                        var today = DateTime.Now;
                                        var sendDate = SetDate(customerCare.SendDate, customerCare.SendHour);
                                        var compare = DateTime.Compare(sendDate, today);
                                        if (compare <= 0)
                                        {
                                            //Nếu thời gian gửi là ở thời điểm quá khứ thì báo lỗi
                                            return new UpdateStatusCustomerCareResult
                                            {
                                                Message = "Không thể kích hoạt chương trình kiểm tra lại thời gian gửi",
                                                Status = false
                                            };
                                        }
                                    }

                                    //add vào bảng Queue
                                    List<Queue> lstQueue = new List<Queue>();
                                    if (listCustomer.Count > 0)
                                    {
                                        listCustomer.ForEach(item =>
                                        {
                                            var sendToPhone = context.Contact.FirstOrDefault(x => x.ObjectId == item.CustomerId && x.ObjectType == "CUS")?.Phone;
                                            var sendToWorkPhone = context.Contact.FirstOrDefault(x => x.ObjectId == item.CustomerId && x.ObjectType == "CUS")?.WorkPhone;
                                            var sendToOtherPhone = context.Contact.FirstOrDefault(x => x.ObjectId == item.CustomerId && x.ObjectType == "CUS")?.OtherPhone;
                                            if (sendToPhone != null)
                                            {
                                                Queue queueCreate = new Queue
                                                {
                                                    SendTo = sendToPhone,
                                                    SendContent = replaceTokenForContent(customerCare.CustomerCareContentSms, item.CustomerId.Value),
                                                    Method = parameter.TypeCusCareCode,
                                                    Title = customerCare.CustomerCareTitle,
                                                    IsSend = false,
                                                    SenDate = parameter.IsSendNow == true ? DateTime.Now : SetDate(customerCare.SendDate, customerCare.SendHour),
                                                    CustomerId = item.CustomerId.Value,
                                                    CustomerCareId = customerCare.CustomerCareId,
                                                    CreateDate = DateTime.Now,
                                                    CreateById = parameter.UserId
                                                };
                                                lstQueue.Add(queueCreate);
                                            }
                                            if (sendToWorkPhone != null)
                                            {
                                                Queue queueCreate = new Queue
                                                {
                                                    SendTo = sendToPhone,
                                                    SendContent = replaceTokenForContent(customerCare.CustomerCareContentSms, item.CustomerId.Value),
                                                    Method = parameter.TypeCusCareCode,
                                                    Title = customerCare.CustomerCareTitle,
                                                    IsSend = false,
                                                    SenDate = parameter.IsSendNow == true ? DateTime.Now : SetDate(customerCare.SendDate, customerCare.SendHour),
                                                    CustomerId = item.CustomerId.Value,
                                                    CustomerCareId = customerCare.CustomerCareId,
                                                    CreateDate = DateTime.Now,
                                                    CreateById = parameter.UserId
                                                };
                                                lstQueue.Add(queueCreate);
                                            }
                                            if (sendToOtherPhone != null)
                                            {
                                                Queue queueCreate = new Queue
                                                {
                                                    SendTo = sendToPhone,
                                                    SendContent = replaceTokenForContent(customerCare.CustomerCareContentSms, item.CustomerId.Value),
                                                    Method = parameter.TypeCusCareCode,
                                                    Title = customerCare.CustomerCareTitle,
                                                    IsSend = false,
                                                    SenDate = parameter.IsSendNow == true ? DateTime.Now : SetDate(customerCare.SendDate, customerCare.SendHour),
                                                    CustomerId = item.CustomerId.Value,
                                                    CustomerCareId = customerCare.CustomerCareId,
                                                    CreateDate = DateTime.Now,
                                                    CreateById = parameter.UserId
                                                };
                                                lstQueue.Add(queueCreate);
                                            }
                                        });
                                    }
                                    else
                                    {
                                        return new UpdateStatusCustomerCareResult
                                        {
                                            Message = "Chương trình CSKH này hiện không có khách hàng",
                                            Status = false
                                        };
                                    }

                                    if (lstQueue.Count > 0)
                                    {
                                        context.Queue.AddRange(lstQueue);
                                    }
                                }
                            }
                            else if (statusCode == "Stoped" && statusCodeChange == "Closed")
                            {
                                customerCare.StatusId = parameter.StatusId;
                            }
                            else
                            {
                                return new UpdateStatusCustomerCareResult
                                {
                                    Message = "Trạng thái thay đổi không hợp lệ",
                                    Status = false
                                };
                            }

                            customerCare.UpdateById = parameter.UserId;
                            customerCare.UpdateDate = DateTime.Now;
                            context.CustomerCare.Update(customerCare);
                            context.SaveChanges();

                            return new UpdateStatusCustomerCareResult
                            {
                                Message = "Cập nhật trạng thái thành công",
                                Status = true
                            };
                        }
                        else
                        {
                            return new UpdateStatusCustomerCareResult
                            {
                                Message = "Mã danh mục chương trình CSKH không tồn tại trên hệ thống",
                                Status = false
                            };
                        }
                    }
                    else
                    {
                        return new UpdateStatusCustomerCareResult
                        {
                            Message = "Mã danh mục chương trình CSKH không tồn tại trên hệ thống",
                            Status = false
                        };
                    }
                }
                else
                {
                    return new UpdateStatusCustomerCareResult
                    {
                        Message = "Không tồn tại chương trình CSKH này trên hệ thống",
                        Status = false
                    };
                }
            }
            catch (Exception)
            {
                return new UpdateStatusCustomerCareResult
                {
                    Message = "Đã có lỗi khi cập nhật trạng thái",
                    Status = false
                };
            }
        }

        public UpdateStatusCustomerCareCustomerByIdResult UpdateStatusCustomerCareCustomerById(UpdateStatusCustomerCareCustomerByIdParameter parameter)
        {
            try
            {
                var customerCareCustomer = context.CustomerCareCustomer.FirstOrDefault(cusCareCus => cusCareCus.CustomerCareCustomerId == parameter.CustomerCareCustomerId);
                customerCareCustomer.StatusId = parameter.StatusId;
                customerCareCustomer.UpdateById = parameter.UserId;
                customerCareCustomer.UpdateDate = DateTime.Now;
                context.CustomerCareCustomer.Update(customerCareCustomer);
                context.SaveChanges();

                return new UpdateStatusCustomerCareCustomerByIdResult
                {
                    CustomerCareCustomerId = customerCareCustomer.CustomerCareCustomerId,
                    Message = "Trạng thái khách hàng đã được cập nhật",
                    Status = true
                };
            }
            catch (Exception)
            {
                return new UpdateStatusCustomerCareCustomerByIdResult
                {
                    Message = "Đã có lỗi khi lưu thông tin phản hồi",
                    Status = false
                };
            }
        }

        private string replaceTokenForContent(string current_content, Guid CustomerId)
        {
            var Name = TokenForContent.Name;
            var Hotline = TokenForContent.Hotline;
            var Address = TokenForContent.Address;

            if (current_content.Contains(Name))
            {
                current_content = current_content.Replace(Name, context.Customer.FirstOrDefault(cus => cus.CustomerId == CustomerId) != null ?
                                                                    context.Customer.FirstOrDefault(cus => cus.CustomerId == CustomerId).CustomerName : "");
            }
            if (current_content.Contains(Hotline))
            {
                current_content = current_content.Replace(Hotline, context.Contact.FirstOrDefault(c => c.ObjectId == CustomerId && c.ObjectType == "CUS") != null ?
                                                                    context.Contact.FirstOrDefault(c => c.ObjectId == CustomerId && c.ObjectType == "CUS").Phone : "");
            }
            if (current_content.Contains(Address))
            {
                current_content = current_content.Replace(Address, context.Contact.FirstOrDefault(c => c.ObjectId == CustomerId && c.ObjectType == "CUS")?.Address);
            }

            return current_content;
        }

        private DateTime SetDate(DateTime? SendEmailDate, TimeSpan? SendEmailHour)
        {
            DateTime newDate = DateTime.Now;
            if (SendEmailDate.HasValue && SendEmailHour.HasValue)
            {
                int Year = SendEmailDate.Value.Year;
                int Month = SendEmailDate.Value.Month;
                int Day = SendEmailDate.Value.Day;

                newDate = new DateTime(Year, Month, Day) + SendEmailHour.Value;
            }
            return newDate;
        }

        public GetCustomerBirthDayResult GetCustomerBirthDay(GetCustomerBirthDayParameter parameter)
        {
            try
            {
                var customerBirthDay = new List<Contact>();
                DateTime getNow = DateTime.Now;
                DateTime getTwoWeek = DateTime.Now.AddDays(14);
                var contacts = context.Contact.Where(c => c.Active == true).ToList();
                var customers = context.Customer.Where(c => c.Active == true).ToList();
                var employees = context.Employee.Where(e => e.Active == true).ToList();
                var customer1BirthDay = new List<Contact>();
                var customer2BirthDay = new List<Customer>();
                if (getNow.Month == getTwoWeek.Month)
                {
                    customer1BirthDay = contacts.Where(con => con.DateOfBirth != null && Convert.ToDateTime(con.DateOfBirth).Day >= getNow.Day && Convert.ToDateTime(con.DateOfBirth).Day <= getTwoWeek.Day && Convert.ToDateTime(con.DateOfBirth).Month >= getNow.Month && Convert.ToDateTime(con.DateOfBirth).Month <= getTwoWeek.Month && con.ObjectType == "CUS").OrderBy(cus => Convert.ToDateTime(cus.DateOfBirth).Day).ToList().Take(3).ToList();
                    customer2BirthDay = customers.Where(cus => cus.BusinessRegistrationDate != null && Convert.ToDateTime(cus.BusinessRegistrationDate).Day >= getNow.Day && Convert.ToDateTime(cus.BusinessRegistrationDate).Day <= getTwoWeek.Day && Convert.ToDateTime(cus.BusinessRegistrationDate).Month >= getNow.Month && Convert.ToDateTime(cus.BusinessRegistrationDate).Month <= getTwoWeek.Month).OrderBy(cus => Convert.ToDateTime(cus.BusinessRegistrationDate).Day).ToList().Take(3).ToList();

                    customer2BirthDay.ForEach(item =>
                    {
                        var contactbyid = contacts.FirstOrDefault(cn => cn.ObjectId == item.CustomerId && cn.ObjectType == "CUS" && cn.Active == true);
                        contactbyid.DateOfBirth = item.BusinessRegistrationDate;
                        customer1BirthDay.Add(contactbyid);
                    });

                    customer1BirthDay = customer1BirthDay.OrderBy(st => Convert.ToDateTime(st.DateOfBirth).Day).ToList().Take(3).ToList();
                }
                else
                {
                    customer1BirthDay = contacts.Where(con => con.DateOfBirth != null &&
                        (Convert.ToDateTime(con.DateOfBirth).Day >= getNow.Day &&
                        Convert.ToDateTime(con.DateOfBirth).Day <= 31 &&
                        Convert.ToDateTime(con.DateOfBirth).Month == getNow.Month) &&
                        con.ObjectType == "CUS").OrderBy(cus => Convert.ToDateTime(cus.DateOfBirth).Day).ToList().Take(3).ToList();
                    customer2BirthDay = customers.Where(cus => cus.BusinessRegistrationDate != null &&
                        (Convert.ToDateTime(cus.BusinessRegistrationDate).Day >= getNow.Day &&
                        Convert.ToDateTime(cus.BusinessRegistrationDate).Day <= 31 &&
                        Convert.ToDateTime(cus.BusinessRegistrationDate).Month == getNow.Month)
                        ).OrderBy(cus => Convert.ToDateTime(cus.BusinessRegistrationDate).Day).ToList().Take(3).ToList();

                    customer2BirthDay.ForEach(item =>
                    {
                        var contactbyid = contacts.FirstOrDefault(cn => cn.ObjectId == item.CustomerId && cn.ObjectType == "CUS" && cn.Active == true);
                        contactbyid.DateOfBirth = item.BusinessRegistrationDate;
                        customer1BirthDay.Add(contactbyid);
                    });

                    customer1BirthDay = customer1BirthDay.OrderBy(st => Convert.ToDateTime(st.DateOfBirth).Day).ToList().Take(3).ToList();

                    if (customer1BirthDay.Count() < 3)
                    {
                        var contactBirthNextMonth = new List<Contact>();
                        var cusBirthNextMonth = new List<Customer>();

                        int contactCount = 3 - customer1BirthDay.Count();
                        var contactBirthNext1 = contacts.Where(con => con.DateOfBirth != null &&
                        (Convert.ToDateTime(con.DateOfBirth).Day >= 1 &&
                        Convert.ToDateTime(con.DateOfBirth).Day <= getTwoWeek.Day &&
                        Convert.ToDateTime(con.DateOfBirth).Month == getTwoWeek.Month) &&
                        con.ObjectType == "CUS").OrderBy(
                        cus => Convert.ToDateTime(cus.DateOfBirth).Day).ToList().Take(contactCount).ToList();

                        contactBirthNext1.ForEach(item =>
                        {
                            contactBirthNextMonth.Add(item);
                        });

                        var contactBirthNext = customers.Where(cus => cus.BusinessRegistrationDate != null &&
                        (Convert.ToDateTime(cus.BusinessRegistrationDate).Day >= 1 &&
                        Convert.ToDateTime(cus.BusinessRegistrationDate).Day <= getTwoWeek.Day &&
                        Convert.ToDateTime(cus.BusinessRegistrationDate).Month == getTwoWeek.Month)
                        ).OrderBy(cus => Convert.ToDateTime(cus.BusinessRegistrationDate).Day).ToList().Take(contactCount).ToList();
                        contactBirthNext.ForEach(item =>
                        {
                            cusBirthNextMonth.Add(item);
                        });

                        cusBirthNextMonth.ForEach(item =>
                        {
                            var contactbyid = contacts.FirstOrDefault(cn => cn.ObjectId == item.CustomerId && cn.ObjectType == "CUS" && cn.Active == true);
                            contactbyid.DateOfBirth = item.BusinessRegistrationDate;
                            contactBirthNextMonth.Add(contactbyid);
                        });

                        contactBirthNextMonth = contactBirthNextMonth.OrderBy(st => Convert.ToDateTime(st.DateOfBirth).Day).ToList().Take(contactCount).ToList();
                        contactBirthNextMonth.ForEach(item =>
                        {
                            customer1BirthDay.Add(item);
                        });
                    }

                }
                customerBirthDay = customer1BirthDay;

                List<GetCustomerBirthDayEntityModel> listBirth = new List<GetCustomerBirthDayEntityModel>();
                customerBirthDay.ForEach(item =>
                {
                    var customerBith = customers.FirstOrDefault(c => c.CustomerId == item.ObjectId);
                    var dayOfBirth = new GetCustomerBirthDayEntityModel();
                    dayOfBirth.ContactId = item.ContactId;
                    dayOfBirth.ObjectId = item.ObjectId;
                    dayOfBirth.CustomerName = customerBith.CustomerName;
                    dayOfBirth.Phone = item.Phone;
                    dayOfBirth.BirthDay = item.DateOfBirth;
                    dayOfBirth.Email = item.Email;
                    dayOfBirth.EmployeeID = customerBith.PersonInChargeId;
                    dayOfBirth.EmployeeName = customerBith.PersonInChargeId == null ? "Không có người phụ trách" : employees.FirstOrDefault(c => c.EmployeeId == customerBith.PersonInChargeId).EmployeeName;
                    dayOfBirth.AvataUrl = item.AvatarUrl;
                    listBirth.Add(dayOfBirth);
                });

                return new GetCustomerBirthDayResult
                {
                    ListBirthDay = listBirth,
                    Message = "Success",
                    Status = true
                };
            }
            catch (Exception)
            {
                return new GetCustomerBirthDayResult
                {
                    Message = "Đã có lỗi khi tìm kiếm",
                    Status = false
                };
            }
        }

        public GetTotalInteractiveResult GetTotalInteractive(GetTotalInteractiveParameter parameter)
        {
            try
            {
                var categoryTypeByCode = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "HCS" && ct.Active == true).CategoryTypeId;
                var categoryHCS = context.Category.Where(c => c.CategoryTypeId == categoryTypeByCode && c.Active == true).ToList();
                var categoryTypeByCodeCSKH = context.CategoryType.FirstOrDefault(ctb => ctb.CategoryTypeCode == "TKH" && ctb.Active == true).CategoryTypeId;
                var categoryIdTKH = context.Category.FirstOrDefault(cb => cb.CategoryTypeId == categoryTypeByCodeCSKH && cb.Active == true && cb.CategoryCode == "DSO").CategoryId;
                var customerCares = context.CustomerCare.ToList();
                var customerCareCustomers = context.CustomerCareCustomer.ToList();
                var queues = context.Queue.ToList();
                int totalCare = 0;
                List<GetTotalInteractiveEntityModel> listCategory = new List<GetTotalInteractiveEntityModel>();

                categoryHCS.ForEach(item =>
                {
                    GetTotalInteractiveEntityModel totalInteractive = new GetTotalInteractiveEntityModel();
                    totalInteractive.CategoryName = item.CategoryName;
                    if (item.CategoryCode == "SMS" || item.CategoryCode == "Email")
                    {
                        totalInteractive.Total = queues.Count(q => q.IsSend == true && Convert.ToDateTime(q.SenDate).Month == parameter.Month && Convert.ToDateTime(q.SenDate).Year == parameter.Year);
                    }
                    else
                    {
                        var cusCareList = customerCares.Where(cc => cc.CustomerCareContactType == item.CategoryId &&
                         Convert.ToDateTime(cc.EffecttiveFromDate).Month <= parameter.Month && Convert.ToDateTime(cc.EffecttiveFromDate).Year <= parameter.Year &&
                         Convert.ToDateTime(cc.EffecttiveToDate).Month >= parameter.Month && Convert.ToDateTime(cc.EffecttiveToDate).Year >= parameter.Year).Select(cc => cc.CustomerCareId)?.ToList();
                        totalInteractive.Total = customerCareCustomers.Where(cuc => cuc.StatusId == categoryIdTKH && cusCareList.Contains(Guid.Parse(cuc.CustomerCareId.ToString()))).Count();
                    }
                    totalCare += totalInteractive.Total;
                    listCategory.Add(totalInteractive);
                });

                return new GetTotalInteractiveResult
                {
                    ListCate = listCategory,
                    TotalCare = totalCare,
                    Message = "Success",
                    Status = true
                };
            }
            catch (Exception)
            {
                return new GetTotalInteractiveResult
                {
                    Message = "Đã có lỗi khi tìm kiếm",
                    Status = false
                };
            }
        }

        public GetCustomerNewCSResult GetCustomerNewCS(GetCustomerNewCSParameter parameter)
        {
            try
            {
                var customers = context.Customer.Where(c => c.Active == true).ToList();
                var contacts = context.Contact.Where(ct => ct.Active == true && ct.ObjectType == "CUS").ToList();
                var employees = context.Employee.ToList();
                List<GetCustomerNewCSEntityModel> listCustomerNewOrder = new List<GetCustomerNewCSEntityModel>();
                var getTopOrderCustomer = context.CustomerOrder.OrderByDescending(co => co.OrderDate).Select(co => co.CustomerId).ToList();
                var getTop5OrderCustomer = getTopOrderCustomer.Distinct().Take(3).ToList();

                getTop5OrderCustomer.ForEach(item =>
                {
                    var customerById = customers.FirstOrDefault(cb => cb.CustomerId == item);
                    var contactById = contacts.FirstOrDefault(ctb => ctb.ObjectId == item);
                    var dayOfBirth = new GetCustomerNewCSEntityModel();
                    dayOfBirth.ContactId = contactById.ContactId;
                    dayOfBirth.ObjectId = contactById.ObjectId;
                    dayOfBirth.CustomerName = customerById.CustomerName;
                    dayOfBirth.Phone = contactById.Phone;
                    dayOfBirth.BirthDay = contactById.DateOfBirth;
                    dayOfBirth.Email = contactById.Email;
                    dayOfBirth.EmployeeID = customerById.PersonInChargeId;
                    var emp = employees.FirstOrDefault(c => c.EmployeeId == customerById.PersonInChargeId);
                    dayOfBirth.EmployeeName = customerById.PersonInChargeId == null
                        ? "Không có người phụ trách"
                        : (emp == null ? "Không có người phụ trách" : emp.EmployeeName);
                    dayOfBirth.AvataUrl = contactById.AvatarUrl;
                    listCustomerNewOrder.Add(dayOfBirth);
                });

                return new GetCustomerNewCSResult
                {
                    ListCustomerNewOrder = listCustomerNewOrder,
                    Message = "Success",
                    Status = true
                };
            }
            catch (Exception)
            {
                return new GetCustomerNewCSResult
                {
                    Message = "Đã có lỗi khi tìm kiếm",
                    Status = false
                };
            }
        }

        public GetCustomerCareActiveResult GetCustomerCareActive(GetCustomerCareActiveParameter parameter)
        {
            try
            {
                var categoryTypeActive = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TCS" && ct.Active == true).CategoryTypeId;
                var categoryActive = context.Category.FirstOrDefault(c => c.CategoryTypeId == categoryTypeActive && c.CategoryCode == "Active" && c.Active == true).CategoryId;
                var categoryTypeCSKH = context.CategoryType.FirstOrDefault(ctcs => ctcs.CategoryTypeCode == "TKH" && ctcs.Active == true).CategoryTypeId;
                var categoryCSKH = context.Category.FirstOrDefault(ccs => ccs.CategoryTypeId == categoryTypeCSKH && ccs.CategoryCode == "DSO" && ccs.Active == true).CategoryId;
                var customerCareCustomers = context.CustomerCareCustomer.ToList();
                var categoryCares = context.CustomerCare.Where(cr => cr.StatusId == categoryActive).OrderByDescending(cr => cr.ActiveDate).Take(3).ToList();
                var listCategoryCare = new List<GetCustomerCareActiveEntityModel>();
                DateTime dateNow = Convert.ToDateTime(DateTime.Now);
                categoryCares.ForEach(item =>
                {
                    var customerCareCusById = customerCareCustomers.Where(cuc => cuc.CustomerCareId == item.CustomerCareId);
                    double daCS = customerCareCusById.Where(cs => cs.StatusId == categoryCSKH).Count();
                    double totalT = customerCareCusById.Count();
                    var categoryCare = new GetCustomerCareActiveEntityModel();

                    double tygia = Math.Round((daCS / totalT) * 100, 2);

                    categoryCare.CustomerCareId = item.CustomerCareId;
                    categoryCare.CustomerCareTitle = item.CustomerCareTitle;
                    categoryCare.CustomerTotal = customerCareCusById.Count();
                    categoryCare.Status = categoryCare.CustomerTotal == 0 ? "100% hoàn thành" : (tygia + "% hoàn thành");
                    categoryCare.CategoryCare = context.Category.FirstOrDefault(c => c.CategoryId == item.CustomerCareContactType && c.Active == true).CategoryCode;

                    DateTime dateCreate = Convert.ToDateTime(item.CreateDate);
                    TimeSpan Time = dateNow - dateCreate;
                    int TongSoNgay = Time.Days;
                    categoryCare.DateCreate = Time.Days == 0 ? Time.Hours + " giờ trước" : Time.Days + " ngày trước";
                    listCategoryCare.Add(categoryCare);
                });

                return new GetCustomerCareActiveResult
                {
                    ListCategoryCare = listCategoryCare,
                    Message = "Success",
                    Status = true
                };
            }
            catch (Exception)
            {
                return new GetCustomerCareActiveResult
                {
                    Message = "Đã có lỗi khi tìm kiếm",
                    Status = false
                };
            }
        }

        public GetCharCustomerCSResult GetCharCustomerCS(GetCharCustomerCSParameter parameter)
        {
            try
            {
                var getYear = Convert.ToDateTime(DateTime.Now).Year;
                var getMonth = Convert.ToDateTime(DateTime.Now).Month;
                var customerCares = context.CustomerCare.Where(c => c.ActiveDate != null).ToList();
                var customerCareCustomers = context.CustomerCareCustomer.ToList();
                var categoryTypeActive = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TCS" && ct.Active == true).CategoryTypeId;
                var categoryActive = context.Category.FirstOrDefault(c => c.CategoryTypeId == categoryTypeActive && c.CategoryCode == "Active" && c.Active == true).CategoryId;
                var categoryTypeByCodeCSKH = context.CategoryType.FirstOrDefault(ctb => ctb.CategoryTypeCode == "TKH" && ctb.Active == true).CategoryTypeId;
                var categoryIdTKH = context.Category.FirstOrDefault(cb => cb.CategoryTypeId == categoryTypeByCodeCSKH && cb.Active == true && cb.CategoryCode == "DSO").CategoryId;

                List<GetCharCustomerCSEntityModel> charList = new List<GetCharCustomerCSEntityModel>();
                for (int i = 1; i <= getMonth; i++)
                {
                    GetCharCustomerCSEntityModel charMonth = new GetCharCustomerCSEntityModel();
                    var cusCareList = customerCares.Where(cc => Convert.ToDateTime(cc.ActiveDate).Month == i && Convert.ToDateTime(cc.ActiveDate).Year == getYear).Select(cc => cc.CustomerCareId)?.ToList();
                    charMonth.Month = "T" + i;
                    charMonth.TotalCustomerProgram = customerCareCustomers.Where(cuc => cusCareList.Contains(Guid.Parse(cuc.CustomerCareId.ToString()))).Count();
                    charMonth.TotalCustomerCSKH = customerCareCustomers.Where(cuc => cuc.StatusId == categoryIdTKH && cusCareList.Contains(Guid.Parse(cuc.CustomerCareId.ToString()))).Count();
                    charList.Add(charMonth);
                }

                return new GetCharCustomerCSResult
                {
                    ListChar = charList,
                    Message = "Success",
                    Status = true
                };
            }
            catch (Exception)
            {
                return new GetCharCustomerCSResult
                {
                    Message = "Đã có lỗi khi tìm kiếm",
                    Status = false
                };
            }
        }

        public GetMasterDataCustomerCareListResult GetMasterDataCustomerCareList(GetMasterDataCustomerCareListParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);
                if (user == null)
                {
                    return new GetMasterDataCustomerCareListResult
                    {
                        Message = "Tài khoản không tồn tại trong hệ thống",
                        Status = false
                    };
                }
                var employee = context.Employee.FirstOrDefault(c => c.EmployeeId == user.EmployeeId);
                var listEmployee = new List<EmployeeEntityModel>();

                #region Trạng thái của chăm sóc khách hàng
                var statusTypeCusCareId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TCS")?.CategoryTypeId;
                var listAllStatusCusCare = context.Category.Where(c => c.Active == true && c.CategoryTypeId == statusTypeCusCareId)
                    .Select(m => new CategoryEntityModel
                    {
                        CategoryId = m.CategoryId,
                        CategoryCode = m.CategoryCode,
                        CategoryName = m.CategoryName,
                    }).OrderBy(c => c.CategoryName).ToList();
                #endregion

                #region Hình thức chăm sóc khách hàng
                var formTypeCusCareId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "HCS")?.CategoryTypeId;
                var listAllFormCusCare = context.Category.Where(c => c.Active == true && c.CategoryTypeId == formTypeCusCareId)
                    .Select(m => new CategoryEntityModel
                    {
                        CategoryId = m.CategoryId,
                        CategoryCode = m.CategoryCode,
                        CategoryName = m.CategoryName,
                    }).OrderBy(c => c.CategoryName).ToList();
                #endregion

                if (employee.IsManager)
                {
                    //Nếu là quản lý:
                    //Lấy danh sách tất cả nhân viên cấp dưới thuộc phòng ban quản lý
                    //Bước 1: Lấy danh sách tất cả phòng ban cấp dưới nếu có
                    List<Guid?> listOrganizationChildrenId = new List<Guid?>
                    {
                        employee.OrganizationId
                    };
                    var organizationList = context.Organization.ToList();
                    GetOrganizationChildrenId(organizationList, employee.OrganizationId, listOrganizationChildrenId);

                    //Bước 2: Lấy danh sách tất cả nhân viên thuộc danh sách phòng ban vừa lấy được
                    listEmployee = context.Employee.Where(c => listOrganizationChildrenId.Contains(c.OrganizationId) || listOrganizationChildrenId.Count == 0)
                        .Select(m => new EmployeeEntityModel
                        {
                            EmployeeId = m.EmployeeId,
                            EmployeeCode = m.EmployeeCode,
                            EmployeeName = m.EmployeeName,
                            IsManager = m.IsManager,
                            Active = m.Active
                        }).OrderBy(c => c.EmployeeName).ToList();
                }
                else
                {
                    //Nếu là nhân viên:
                    //Lấy danh sách các nhân viên trong cùng phòng ban với nhân viên đang đăng nhập
                    listEmployee = context.Employee.Where(c => c.EmployeeId == employee.EmployeeId)
                        .Select(m => new EmployeeEntityModel
                        {
                            EmployeeId = m.EmployeeId,
                            EmployeeCode = m.EmployeeCode,
                            EmployeeName = m.EmployeeName,
                            IsManager = m.IsManager,
                            Active = m.Active
                        }).OrderBy(c => c.EmployeeName).ToList();
                }

                return new GetMasterDataCustomerCareListResult
                {
                    Message = "Success",
                    Status = true,
                    ListStatus = listAllStatusCusCare,
                    ListFormCusCare = listAllFormCusCare,
                    ListEmployee = listEmployee
                };
            }
            catch (Exception ex)
            {
                return new GetMasterDataCustomerCareListResult
                {
                    Message = ex.Message,
                    Status = false
                };
            }
        }

        private List<Guid?> GetOrganizationChildrenId(List<Organization> organizationList, Guid? id, List<Guid?> list)
        {
            var organizations = organizationList.Where(o => o.ParentId == id).ToList();
            organizations.ForEach(item =>
            {
                list.Add(item.OrganizationId);
                GetOrganizationChildrenId(organizationList, item.OrganizationId, list);
            });

            return list;
        }

        public UpdateStatusCusCareResult UpdateStatusCusCare(UpdateStatusCusCareParameter parameter)
        {
            try
            {
                var cusCare = context.CustomerCare.FirstOrDefault(c => c.CustomerCareId == parameter.CustomerCareId);
                var statusTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TCS")?.CategoryTypeId;
                var listAllStatus = context.Category.Where(c => c.CategoryTypeId == statusTypeId).ToList();

                var closeStatusId = listAllStatus.FirstOrDefault(c => c.CategoryCode == "Closed")?.CategoryId;
                if (cusCare.StatusId == parameter.StatusId || cusCare.StatusId == closeStatusId)
                {
                    return new UpdateStatusCusCareResult
                    {
                        Message = "Trạng thái chăm sóc khách hàng không hợp lện",
                        Status = false
                    };
                }

                cusCare.StatusId = parameter.StatusId;
                context.CustomerCare.Update(cusCare);
                context.SaveChanges();

                return new UpdateStatusCusCareResult
                {
                    Message = "Success",
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new UpdateStatusCusCareResult
                {
                    Message = ex.Message,
                    Status = false
                };
            }
        }

        public UpdateCustomerMeettingResult UpdateCustomerMeeting(UpdateCustomerMettingParameter paramter)
        {
            try
            {
                var customerMeeting = context.CustomerMeeting.FirstOrDefault(c => c.CustomerMeetingId == paramter.CustomerMeetingId);
                if (customerMeeting == null)
                {
                    return new UpdateCustomerMeettingResult
                    {
                        Status = false,
                        Message = "Lịch hẹn không tồn tại trong hệ thống",
                    };
                }
                var timeStart = paramter.StartDate.TimeOfDay;
                var timeEnd = paramter.EndDate?.TimeOfDay;
                customerMeeting.StartDate = SetDate(paramter.StartDate, timeStart);
                customerMeeting.StartHours = timeStart;
                customerMeeting.EndDate = SetDate(paramter.EndDate, timeEnd);
                customerMeeting.EndHours = timeEnd;

                context.CustomerMeeting.Update(customerMeeting);
                var user = context.User.FirstOrDefault(x => x.UserId == paramter.UserId);
                var employeeContact =
                    context.Contact.FirstOrDefault(x => x.ObjectId == user.EmployeeId && x.ObjectType == "EMP");

                var listEmployeeId = new List<string>();
                var listEmployeeContact = new List<Contact>();
                if (customerMeeting.Participants != null && customerMeeting.Participants != "")
                {

                    listEmployeeId = customerMeeting.Participants.Split(';').ToList();
                    listEmployeeContact = context.Contact.Where(c => listEmployeeId.Contains(c.ObjectId.ToString()) && c.ObjectType == "EMP").ToList();
                }

                List<string> listEmail = new List<string>();
                listEmployeeContact.ForEach(item =>
                {
                    listEmail.Add(item.Email);
                });
                var title = customerMeeting.Title + " - " + customerMeeting.StartDate.Value.ToString("dd/MM/yyyy HH:mm");
                var body = $"Địa điểm: {customerMeeting.LocationMeeting}. Nội dung: {customerMeeting.Content}";
                Emailer.SendMailWithIcsAttachment(context, listEmail, null, title, body, customerMeeting.StartDate ?? DateTime.Now, customerMeeting.EndDate, customerMeeting.CustomerMeetingId, customerMeeting.LocationMeeting, false);

                context.SaveChanges();

                return new UpdateCustomerMeettingResult
                {
                    Status = true,
                    Message = "Success"
                };
            }
            catch (Exception ex)
            {
                return new UpdateCustomerMeettingResult
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public RemoveCustomerMeetingResult RemoveCustomerMeeting(RemoveCustomerMettingParameter parameter)
        {
            try
            {
                var customerMeeting = context.CustomerMeeting.FirstOrDefault(c => c.CustomerMeetingId == parameter.CustomerMeetingId);
                if (customerMeeting == null)
                {
                    return new RemoveCustomerMeetingResult
                    {
                        Message = "Lịch hẹn không tồn tại trong hệ thống",
                        Status = false
                    };
                }

                context.CustomerMeeting.Remove(customerMeeting);
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employeeContact =
                    context.Contact.FirstOrDefault(x => x.ObjectId == user.EmployeeId && x.ObjectType == "EMP");

                var listEmployeeId = new List<string>();
                var listEmployeeContact = new List<Contact>();
                if (customerMeeting.Participants != null && customerMeeting.Participants != "")
                {

                    listEmployeeId = customerMeeting.Participants.Split(';').ToList();
                    listEmployeeContact = context.Contact.Where(c => listEmployeeId.Contains(c.ObjectId.ToString()) && c.ObjectType == "EMP").ToList();
                }

                List<string> listEmail = new List<string>();
                listEmployeeContact.ForEach(item =>
                {
                    listEmail.Add(item.Email);
                });
                var title = customerMeeting.Title + " - " + customerMeeting.StartDate.Value.ToString("dd/MM/yyyy HH:mm");
                var body = $"Địa điểm: {customerMeeting.LocationMeeting}. Nội dung: {customerMeeting.Content}";
                Emailer.SendMailWithIcsAttachment(context, listEmail, null, title, body, customerMeeting.StartDate ?? DateTime.Now, customerMeeting.EndDate, customerMeeting.CustomerMeetingId, customerMeeting.LocationMeeting, true);
                context.SaveChanges();

                /*Send Mail thông báo*/

                return new RemoveCustomerMeetingResult
                {
                    Message = "Success",
                    Status = true,
                };

            }
            catch (Exception ex)
            {
                return new RemoveCustomerMeetingResult
                {
                    Message = ex.Message,
                    Status = false,
                };
            }
        }
    }
}
