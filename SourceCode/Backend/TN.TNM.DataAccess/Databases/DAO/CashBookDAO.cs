using System;
using System.Linq;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.CashBook;
using TN.TNM.DataAccess.Messages.Results.CashBook;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class CashBookDAO : BaseDAO, ICashBookDataAccess
    {
        public CashBookDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
        }

        public GetDataSearchCashBookResult GetDataSearchCashBook(GetDataSearchCashBookParameter parameter)
        {
            try
            {
                var employees = context.Employee.OrderBy(c => c.EmployeeName).ToList();
                var organizations = context.Organization.Where(c => c.IsFinancialIndependence == true).ToList();

                return new GetDataSearchCashBookResult
                {
                    ListEmployee = employees,
                    ListOrganization = organizations,
                    Status = true,
                    Message = "Success"
                };
            }
            catch (Exception ex)
            {
                return new GetDataSearchCashBookResult
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public GetSurplusCashBookPerMonthResult GetSurplusCashBookPerMonth(GetSurplusCashBookPerMonthParameter parameter)
        {

            if (parameter.OrganizationList.Count > 0)
            {
                decimal SumOpeningBalance = 0;
                decimal SumClosingBalance = 0;
                parameter.OrganizationList.ForEach(item =>
                {
                    decimal itemOpeningBalance = 0;
                    decimal itemClosingBalance = 0;
                    var AmountOpeningCashBookToGet = (from cb in context.CashBook
                                                      where cb.PaidDate == (from mcb in context.CashBook
                                                                            where mcb.PaidDate < parameter.FromDate.Value
                                                                                && mcb.OrganizationId == item
                                                                            select mcb.PaidDate).Max().GetValueOrDefault()
                                                             && cb.OrganizationId == item
                                                      select cb.Amount).FirstOrDefault();
                    // Lay ra so du hien tai (tinh den ngay search)
                    var AmountClosingCashBookToGet = (from cb in context.CashBook
                                                      where cb.PaidDate == (from mcb in context.CashBook
                                                                            where mcb.PaidDate <= parameter.ToDate.Value
                                                                            && mcb.PaidDate >= parameter.FromDate.Value
                                                                            && mcb.OrganizationId == item
                                                                            select mcb.PaidDate).Max().GetValueOrDefault()
                                                             && cb.OrganizationId == item
                                                      select cb.Amount).FirstOrDefault();
                    if (AmountOpeningCashBookToGet.HasValue)
                    {
                        itemOpeningBalance = AmountOpeningCashBookToGet.Value;
                    }
                    if (AmountClosingCashBookToGet.HasValue)
                    {
                        itemClosingBalance = AmountClosingCashBookToGet.Value;
                    }
                    else
                    {
                        itemClosingBalance = itemOpeningBalance;
                    }
                    SumOpeningBalance = SumOpeningBalance + itemOpeningBalance;
                    SumClosingBalance = SumClosingBalance + itemClosingBalance;
                });
                return new GetSurplusCashBookPerMonthResult
                {
                    OpeningSurplus = SumOpeningBalance,
                    ClosingSurplus = SumClosingBalance,
                    Message = "Success",
                    Status = true
                };
            }
            else
            {
                var lst = (from o in context.Organization
                           where o.IsFinancialIndependence.Value == true
                           select o.OrganizationId).ToList();

                decimal SumOpeningBalance = 0;
                decimal SumClosingBalance = 0;
                lst.ForEach(item =>
                {
                    decimal itemOpeningBalance = 0;
                    decimal itemClosingBalance = 0;
                    var AmountOpeningCashBookToGet = (from cb in context.CashBook
                                                      where cb.PaidDate == (from mcb in context.CashBook
                                                                            where mcb.PaidDate < parameter.FromDate.Value
                                                                                && mcb.OrganizationId == item
                                                                            select mcb.PaidDate).Max().GetValueOrDefault()
                                                             && cb.OrganizationId == item
                                                      select cb.Amount).FirstOrDefault();
                    // Lay ra so du hien tai (tinh den ngay search)
                    var AmountClosingCashBookToGet = (from cb in context.CashBook
                                                      where cb.PaidDate == (from mcb in context.CashBook
                                                                            where mcb.PaidDate <= parameter.ToDate.Value
                                                                            && mcb.PaidDate >= parameter.FromDate.Value
                                                                                && mcb.OrganizationId == item
                                                                            select mcb.PaidDate).Max().GetValueOrDefault()
                                                             && cb.OrganizationId == item
                                                      select cb.Amount).FirstOrDefault();
                    if (AmountOpeningCashBookToGet.HasValue)
                    {
                        itemOpeningBalance = AmountOpeningCashBookToGet.Value;
                    }
                    if (AmountClosingCashBookToGet.HasValue)
                    {
                        itemClosingBalance = AmountClosingCashBookToGet.Value;
                    }
                    else
                    {
                        itemClosingBalance = itemOpeningBalance;
                    }
                    SumOpeningBalance = SumOpeningBalance + itemOpeningBalance;
                    SumClosingBalance = SumClosingBalance + itemClosingBalance;
                });
                return new GetSurplusCashBookPerMonthResult
                {
                    OpeningSurplus = SumOpeningBalance,
                    ClosingSurplus = SumClosingBalance,
                    Message = "Success",
                    Status = true
                };
            }
        }
    }
}
