using System;
using System.Collections.Generic;
using System.Linq;
using TN.TNM.Common;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Lead;
using TN.TNM.DataAccess.Messages.Results.Lead;
using TN.TNM.DataAccess.Models.Lead;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class LeadDashboardDAO : BaseDAO, ILeadDashboardDataAccess
    {
        public LeadDashboardDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
        }

        private void GetOrgTreeId(Guid OrgParent, List<Guid> lstChid)
        {
            var list = context.Organization.Where(o => o.ParentId == OrgParent).Select(o => o.OrganizationId).ToList();
            lstChid.Add(OrgParent);
            if (list.Count != 0)
            {
                list.ForEach(orgId =>
                {
                    GetOrgTreeId(orgId, lstChid);
                });
            }
        }

        public GetConvertRateResult GetConvertRate(GetConvertRateParameter parameter)
        {
            try
            {
                this.iAuditTrace.Trace(ActionName.GETCOUNT, ObjectName.LEAD, "Count Lead for Dashboard", parameter.UserId);
                List<Guid> orgList = new List<Guid>();
                var commonUser = context.User.ToList();
                var commonEmployee = context.Employee.ToList();
                var commonCategoryType = context.CategoryType.ToList();
                var commonCategory = context.Category.ToList();
                var commonLead = context.Lead.Where(w => w.Active == true).ToList();
                var commonContact = context.Contact.Where(w => w.Active == true).ToList();
                var commonOrganization = context.Organization.Where(o => o.Active == true).ToList();

                var categoryStatus = commonCategoryType.FirstOrDefault(cs => cs.CategoryTypeCode == "TLE");
                var listLeadStatusId = commonCategory.Where(tl => tl.CategoryTypeId == categoryStatus.CategoryTypeId && (tl.CategoryCode != "NDO" && tl.CategoryCode != "KHD")).Select(tl => tl.CategoryId);
                var empId = commonUser.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
                var manager = commonEmployee.FirstOrDefault(e => e.EmployeeId == empId);

                var listOrganizationId = new List<Guid>();
                var orgEmp = commonOrganization.FirstOrDefault(oe => oe.OrganizationId == manager.OrganizationId);
                if (orgEmp != null)
                {
                    if (orgEmp.Level == 0 && (orgEmp.ParentId == null || orgEmp.ParentId == Guid.Empty))
                    {
                        listOrganizationId = commonOrganization.Select(lo => lo.OrganizationId).ToList();
                    }
                    else
                    {
                        listOrganizationId.Add(orgEmp.OrganizationId);
                        int maxLevel = commonOrganization.Max(mo => mo.Level);
                        for (int i = orgEmp.Level + 1; i <= maxLevel; i++)
                        {
                            var lstOrgParent = commonOrganization.Where(cp => cp.Level == i && listOrganizationId.Contains(Guid.Parse(cp.ParentId.ToString()))).Select(cp => cp.OrganizationId).ToList();
                            lstOrgParent.ForEach(item =>
                            {
                                listOrganizationId.Add(item);
                            });
                        }
                    }
                }
                //ngoc edit 6/8/2020
                //var empInCurrentOrg = commonEmployee.Where(e => e.OrganizationId == manager.OrganizationId);  
                var empInCurrentOrg = commonEmployee.Where(e => listOrganizationId.Contains(Guid.Parse(e.OrganizationId.ToString()))).ToList();

                var userInCurrentOrg = new List<Guid>();
                var lstEmpInCurrentOrg = new List<Guid?>();
                empInCurrentOrg.ForEach(emp =>
                {
                    var userId = commonUser.FirstOrDefault(u => u.EmployeeId == emp.EmployeeId).UserId;
                    userInCurrentOrg.Add(userId);
                    lstEmpInCurrentOrg.Add(emp.EmployeeId);
                });

                #region Comment By Hung
                //var countNew = (from l in this.context.Lead
                //                where l.Status.CategoryCode == "MOI" && l.Status.CategoryType.CategoryTypeCode == "TLE" && (l.PersonInChargeId == empId
                //                || (manager.IsManager && userInCurrentOrg.Contains(Guid.Parse(l.CreatedById)))
                //                || (l.CreatedById == parameter.UserId.ToString() && (l.PersonInChargeId == null || l.PersonInChargeId == Guid.Empty)))
                //                select l).Count();
                //var countInProgress = (from l in this.context.Lead
                //                       where l.Status.CategoryCode == "DSA" && l.Status.CategoryType.CategoryTypeCode == "TLE" && (l.PersonInChargeId == empId
                //                       || (manager.IsManager && userInCurrentOrg.Contains(Guid.Parse(l.CreatedById)))
                //                       || (l.CreatedById == parameter.UserId.ToString() && (l.PersonInChargeId == null || l.PersonInChargeId == Guid.Empty)))
                //                       select l).Count();
                //var countQuotation = (from l in this.context.Lead
                //                      where l.Status.CategoryCode == "DGI" && l.Status.CategoryType.CategoryTypeCode == "TLE" && (l.PersonInChargeId == empId
                //                      || (manager.IsManager && userInCurrentOrg.Contains(Guid.Parse(l.CreatedById)))
                //                      || (l.CreatedById == parameter.UserId.ToString() && (l.PersonInChargeId == null || l.PersonInChargeId == Guid.Empty)))
                //                      select l).Count();
                //var countWaiting = (from l in this.context.Lead
                //                    where l.Status.CategoryCode == "CHO" && l.Status.CategoryType.CategoryTypeCode == "TLE" && (l.PersonInChargeId == empId
                //                    || (manager.IsManager && userInCurrentOrg.Contains(Guid.Parse(l.CreatedById)))
                //                    || (l.CreatedById == parameter.UserId.ToString() && (l.PersonInChargeId == null || l.PersonInChargeId == Guid.Empty)))
                //                    select l).Count();
                //var countUnfollowed = (from l in this.context.Lead
                //                       where l.Status.CategoryCode == "NDO" && l.Status.CategoryType.CategoryTypeCode == "TLE" && (l.PersonInChargeId == empId
                //                       || (manager.IsManager && userInCurrentOrg.Contains(Guid.Parse(l.CreatedById)))
                //                       || (l.CreatedById == parameter.UserId.ToString() && (l.PersonInChargeId == null || l.PersonInChargeId == Guid.Empty)))
                //                       select l).Count();
                //var countSigned = (from l in this.context.Lead
                //                   where l.Status.CategoryCode == "KHD" && l.Status.CategoryType.CategoryTypeCode == "TLE" && (l.PersonInChargeId == empId
                //                   || (manager.IsManager && userInCurrentOrg.Contains(Guid.Parse(l.CreatedById)))
                //                   || (l.CreatedById == parameter.UserId.ToString() && (l.PersonInChargeId == null || l.PersonInChargeId == Guid.Empty)))
                //                   select l).Count();
                #endregion

                #region Add By Hung
                List<LeadConvertRateEntityModel> leadConvertRateList = new List<LeadConvertRateEntityModel>();
                List<LeadRequirementRateEntityModel> leadRequirementRateList = new List<LeadRequirementRateEntityModel>();
                List<LeadPotentialRateEntityModel> leadPotentialRateList = new List<LeadPotentialRateEntityModel>();

                List<LeadEntityModel> listCHOLead = new List<LeadEntityModel>();
                List<LeadEntityModel> listNDOLead = new List<LeadEntityModel>();
                List<LeadEntityModel> listMOILead = new List<LeadEntityModel>();
                List<Entities.Lead> leadMoi = new List<Entities.Lead>();
                List<Entities.Lead> leadCHO = new List<Entities.Lead>();
                List<Entities.Lead> leadNDO = new List<Entities.Lead>();
                var categoryType = commonCategoryType.FirstOrDefault(f => f.CategoryTypeCode == "TLE");
                if (categoryType != null)
                {
                    var category = commonCategory.Where(w => w.CategoryTypeId == categoryType.CategoryTypeId).ToList();
                    if (category != null)
                    {
                        List<Guid> categoryIdList = new List<Guid>();
                        category.ForEach(item =>
                        {
                            categoryIdList.Add(item.CategoryId);
                        });
                        var leadList = commonLead.Where(w => categoryIdList.Contains(w.StatusId) &&
                                                 (w.PersonInChargeId == empId
                                        || (manager.IsManager && userInCurrentOrg.Contains(Guid.Parse(w.CreatedById)))
                                        || (w.CreatedById == parameter.UserId.ToString() && (w.PersonInChargeId == null || w.PersonInChargeId == Guid.Empty)))).ToList();

                        List<Guid> leadIdList = new List<Guid>();
                        List<Guid> personInChargeIdList = new List<Guid>();
                        category.ForEach(item =>
                        {
                            var tpmCount = 0;
                            //Ngọc edit 8/6/2020
                            //if (item.CategoryCode == "MOI") 
                            if (item.CategoryCode != "NDO" && item.CategoryCode != "KHD")
                            {
                                // Ngọc edit 8/6/2020
                                //leadMoi = leadList.Where(w => w.StatusId == item.CategoryId).ToList();
                                //leadMoi.ForEach(lead =>
                                var leadMoiTam = leadList.Where(w => w.StatusId == item.CategoryId).ToList();
                                leadMoiTam.ForEach(lead =>
                                {
                                    leadMoi.Add(lead);
                                    leadIdList.Add(lead.LeadId);
                                    if (lead.PersonInChargeId != null)
                                    {
                                        if (!personInChargeIdList.Contains(lead.PersonInChargeId.Value))
                                        {
                                            personInChargeIdList.Add(lead.PersonInChargeId.Value);
                                        }
                                    }
                                });
                                tpmCount = leadMoi.Count;
                            }
                            // ngọc edit 8/6/2020
                            //else if (item.CategoryCode == "DSA")
                            if (item.CategoryCode == "DSA")
                            {
                                leadCHO = leadList.Where(w => w.StatusId == item.CategoryId).ToList();
                                leadCHO.ForEach(lead =>
                                {
                                    leadIdList.Add(lead.LeadId);
                                    if (lead.PersonInChargeId != null)
                                    {
                                        if (!personInChargeIdList.Contains(lead.PersonInChargeId.Value))
                                        {
                                            personInChargeIdList.Add(lead.PersonInChargeId.Value);
                                        }
                                    }
                                });
                                tpmCount = leadCHO.Count;
                            }
                            // ngọc edit 8/6/2020
                            //else if (item.CategoryCode == "NDO") 
                            if (item.CategoryCode == "NDO")
                            {
                                //leadNDO = leadList.Where(w => w.StatusId == item.CategoryId).ToList();
                                leadNDO = leadList.Where(w => w.WaitingForApproval == true && w.StatusId != item.CategoryId).OrderByDescending(w => w.UpdatedDate).ToList();
                                leadNDO.ForEach(lead =>
                                {
                                    leadIdList.Add(lead.LeadId);
                                    if (lead.PersonInChargeId != null)
                                    {
                                        if (!personInChargeIdList.Contains(lead.PersonInChargeId.Value))
                                        {
                                            personInChargeIdList.Add(lead.PersonInChargeId.Value);
                                        }
                                    }
                                });
                                tpmCount = leadNDO.Count;
                            }
                            else
                            {
                                tpmCount = leadList.Where(w => w.StatusId == item.CategoryId).Count();
                            }

                            //LeadConvertRateEntityModel leadConvertRate = new LeadConvertRateEntityModel
                            //{
                            //    Code = item.CategoryCode,
                            //    Name = item.CategoryName,
                            //    Count = tpmCount
                            //};
                            //totalCount += leadConvertRate.Count;
                            //leadConvertRateList.Add(leadConvertRate);
                        });

                        var contacts = commonContact.Where(w => leadIdList.Contains(w.ObjectId) && w.ObjectType == ObjectType.LEAD).ToList();
                        var contactEmployeeList = commonContact.Where(w => personInChargeIdList.Contains(w.ObjectId) && w.ObjectType == ObjectType.EMPLOYEE).ToList();

                        #region Get Lead MOI //BẢNG TOP KHÁCH HÀNG MỚI
                        var ilead = 0;
                        leadMoi.OrderByDescending(o => o.CreatedDate).ToList().ForEach(item =>
                        {
                            ilead++;
                            var contact = contacts.FirstOrDefault(f => f.ObjectId == item.LeadId);
                            var contactEm = contactEmployeeList.FirstOrDefault(f => f.ObjectId == item.PersonInChargeId);
                            if (contact != null)
                            {
                                LeadEntityModel moi = new LeadEntityModel
                                {
                                    LeadId = item.LeadId,
                                    ContactId = contact.ContactId,
                                    FullName = contact.FirstName + " " + contact.LastName,
                                    PersonInChargeId = item.PersonInChargeId,
                                    PersonInChargeFullName = contactEm != null ? contactEm.FirstName + "" + contactEm.LastName : "",
                                    Email = contact.Email,
                                    Phone = contact.Phone,
                                    AvatarUrl = string.IsNullOrEmpty(contact.AvatarUrl) ? "" : contact.AvatarUrl,
                                    Active = item.Active
                                };
                                if (ilead <= 5)
                                    listMOILead.Add(moi);
                            }
                        });
                        #endregion

                        #region Get Lead DSA  //BẢNG TOP KHÁCH HÀNG CHỜ LÀM BÁO GIÁ
                        ilead = 0;
                        leadCHO.OrderByDescending(o => o.CreatedDate).ToList().ForEach(item =>
                        {
                            ilead++;
                            var contact = contacts.FirstOrDefault(f => f.ObjectId == item.LeadId);
                            var contactEm = contactEmployeeList.FirstOrDefault(f => f.ObjectId == item.PersonInChargeId);
                            if (contact != null)
                            {
                                LeadEntityModel cho = new LeadEntityModel
                                {
                                    LeadId = item.LeadId,
                                    ContactId = contact.ContactId,
                                    FullName = contact.FirstName + " " + contact.LastName,
                                    PersonInChargeFullName = contactEm != null ? contactEm.FirstName + "" + contactEm.LastName : "",
                                    PersonInChargeId = item.PersonInChargeId,
                                    Email = contact.Email,
                                    Phone = contact.Phone,
                                    AvatarUrl = string.IsNullOrEmpty(contact.AvatarUrl) ? "" : contact.AvatarUrl,
                                    Active = item.Active
                                };
                                if (ilead <= 5)
                                    listCHOLead.Add(cho);
                            }
                        });
                        #endregion

                        #region Get Lead NDO //BẢNG TOP KHÁCH HÀNG NGỪNG THEO DÕI CHỜ PHÊ DUYỆT
                        ilead = 0;
                        leadNDO.ForEach(item =>
                        {
                            ilead++;
                            var contact = contacts.FirstOrDefault(f => f.ObjectId == item.LeadId);
                            var contactEm = contactEmployeeList.FirstOrDefault(f => f.ObjectId == item.PersonInChargeId);
                            if (contact != null)
                            {
                                LeadEntityModel ndo = new LeadEntityModel
                                {
                                    LeadId = item.LeadId,
                                    ContactId = contact.ContactId,
                                    FullName = contact.FirstName + " " + contact.LastName,
                                    PersonInChargeFullName = contactEm != null ? contactEm.FirstName + "" + contactEm.LastName : "",
                                    PersonInChargeId = item.PersonInChargeId,
                                    Email = contact.Email,
                                    Phone = contact.Phone,
                                    AvatarUrl = string.IsNullOrEmpty(contact.AvatarUrl) ? "" : contact.AvatarUrl,
                                    Active = item.Active
                                };
                                if (ilead <= 10)
                                    listNDOLead.Add(ndo);
                            }
                        });
                        #endregion
                    }
                }

                #endregion

                #region Lấy biểu đồ mức độ chuyển đổi ( status lead)
                var totalCount = 0;
                var categoryTypeTLE = commonCategoryType.FirstOrDefault(f => f.CategoryTypeCode == "TLE");
                if (categoryTypeTLE != null)
                {
                    var category = commonCategory.Where(w => w.CategoryTypeId == categoryTypeTLE.CategoryTypeId).ToList();
                    if (category != null)
                    {
                        //var leadPresion = commonLead.Where(lp => lstEmpInCurrentOrg.Contains(Guid.Parse(lp.PersonInChargeId.ToString()))).ToList();
                        //var a = commonLead.Where(lp => lp.PersonInChargeId == null || lp.PersonInChargeId == Guid.Empty).ToList();
                        //var leadPresion = a.Where(p => lstEmpInCurrentOrg.Contains(p.PersonInChargeId)).ToList();

                        category.ForEach(item =>
                        {
                            var tpmCount = commonLead.Where(w => w.StatusId == item.CategoryId &&
                            (w.CreatedDate.Month == parameter.Month && w.CreatedDate.Year == parameter.Year) &&
                                                ( //w.PersonInChargeId == empId  // ngoc edit 6/8/2020
                                                lstEmpInCurrentOrg.Contains(w.PersonInChargeId)
                                       || (manager.IsManager && userInCurrentOrg.Contains(Guid.Parse(w.CreatedById)))
                                       || (w.CreatedById == parameter.UserId.ToString() && (w.PersonInChargeId == null || w.PersonInChargeId == Guid.Empty)))).Count();
                            LeadConvertRateEntityModel leadConvertRate = new LeadConvertRateEntityModel
                            {
                                Code = item.CategoryCode,
                                Name = item.CategoryName,
                                Count = tpmCount
                            };
                            totalCount += leadConvertRate.Count;
                            leadConvertRateList.Add(leadConvertRate);
                        });
                    }
                }
                #endregion


                #region Add by Hung PotentialRate
                var totalCountPotentialRate = 0;
                var categoryTypeMTN = commonCategoryType.FirstOrDefault(f => f.CategoryTypeCode == "MTN");
                if (categoryTypeMTN != null)
                {
                    var category = commonCategory.Where(w => w.CategoryTypeId == categoryTypeMTN.CategoryTypeId).ToList();
                    if (category != null)
                    {
                        category.ForEach(item =>
                        {
                            var tpmCount = commonLead.Where(w => w.PotentialId == item.CategoryId &&
                                (w.CreatedDate.Month == parameter.Month && w.CreatedDate.Year == parameter.Year) &&
                                        listLeadStatusId.Contains(w.StatusId) &&
                                                (//w.PersonInChargeId == empId  // ngoc edit 6/8/2020
                                                lstEmpInCurrentOrg.Contains(w.PersonInChargeId)
                                       || (manager.IsManager && userInCurrentOrg.Contains(Guid.Parse(w.CreatedById)))
                                       || (w.CreatedById == parameter.UserId.ToString() && (w.PersonInChargeId == null || w.PersonInChargeId == Guid.Empty)))).Count();
                            LeadPotentialRateEntityModel leadPotentialRate = new LeadPotentialRateEntityModel
                            {
                                Code = item.CategoryCode,
                                Name = item.CategoryName,
                                Count = tpmCount
                            };
                            totalCountPotentialRate += leadPotentialRate.Count;
                            leadPotentialRateList.Add(leadPotentialRate);
                        });

                        //var tpmCountNull = commonLead.Where(w => w.PotentialId == null &&
                        //                listLeadStatusId.Contains(w.StatusId) &&
                        //    (w.CreatedDate.Month == parameter.Month && w.CreatedDate.Year == parameter.Year) &&
                        //                    (//w.PersonInChargeId == empId  // ngoc edit 6/8/2020
                        //                    lstEmpInCurrentOrg.Contains(w.PersonInChargeId)
                        //           || (manager.IsManager && userInCurrentOrg.Contains(Guid.Parse(w.CreatedById)))
                        //           || (w.CreatedById == parameter.UserId.ToString() && (w.PersonInChargeId == null || w.PersonInChargeId == Guid.Empty)))).Count();
                        //LeadPotentialRateEntityModel leadPotentialRateNull = new LeadPotentialRateEntityModel
                        //{
                        //    Code = "",
                        //    Name = "",
                        //    Count = tpmCountNull
                        //};
                        //totalCountPotentialRate += leadPotentialRateNull.Count;
                        //leadPotentialRateList.Add(leadPotentialRateNull);
                    }
                }
                #endregion

                #region Add By Hung RequirementRateList                
                var totalCountRequirementRate = 0;
                var categoryTypeNHU = commonCategoryType.FirstOrDefault(f => f.CategoryTypeCode == "NHU");
                if (categoryTypeNHU != null)
                {
                    var category = commonCategory.Where(w => w.CategoryTypeId == categoryTypeNHU.CategoryTypeId).ToList();
                    if (category != null)
                    {
                        category.ForEach(item =>
                        {
                            LeadRequirementRateEntityModel leadRequirementRate = new LeadRequirementRateEntityModel
                            {
                                Code = item.CategoryCode,
                                Name = item.CategoryName,
                                Count = commonLead.Where(w => w.InterestedGroupId == item.CategoryId &&
                                        listLeadStatusId.Contains(w.StatusId) &&
                                    (w.CreatedDate.Month == parameter.Month && w.CreatedDate.Year == parameter.Year) &&
                                            (//w.PersonInChargeId == empId  // ngoc edit 6/8/2020
                                                lstEmpInCurrentOrg.Contains(w.PersonInChargeId)
                                    || (manager.IsManager && userInCurrentOrg.Contains(Guid.Parse(w.CreatedById)))
                                    || (w.CreatedById == parameter.UserId.ToString() && (w.PersonInChargeId == null || w.PersonInChargeId == Guid.Empty)))).Count()
                            };
                            totalCountRequirementRate += leadRequirementRate.Count;
                            leadRequirementRateList.Add(leadRequirementRate);
                        });
                    }
                }
                #endregion

                var result = new GetConvertRateResult
                {
                    Status = true,
                    ListCHOLead = listCHOLead,
                    ListNDOLead = listNDOLead,
                    ListMOILead = listMOILead,
                    TotalCountRequirementRate = totalCountRequirementRate,
                    LeadRequirementRateList = leadRequirementRateList.OrderByDescending(o => o.Count).ToList(),
                    TotalCountPotentialRate = totalCountPotentialRate,
                    LeadPotentialRateList = leadPotentialRateList.OrderByDescending(o => o.Count).ToList(),
                    TotalCount = totalCount,
                    LeadConvertRateList = leadConvertRateList.OrderByDescending(o => o.Count).ToList()
                };
                return result;
            }
            catch (Exception ex)
            {
                return new GetConvertRateResult
                {
                    Status = false,
                    TotalCount = 0,
                    ListCHOLead = new List<LeadEntityModel>(),
                    ListNDOLead = new List<LeadEntityModel>(),
                    ListMOILead = new List<LeadEntityModel>(),
                    LeadConvertRateList = new List<LeadConvertRateEntityModel>(),
                    Message = ex.Message
                };
            }

        }
        public GetRequirementRateResult GetRequirementRate(GetRequirementRateParemeter parameter)
        {
            try
            {
                this.iAuditTrace.Trace(ActionName.GETCOUNT, ObjectName.LEAD, "Count Lead for Dashboard", parameter.UserId);
                List<Guid> orgList = new List<Guid>();
                var commonUser = context.User.ToList();
                var commonEmployee = context.Employee.ToList();
                var commonCategoryType = context.CategoryType.ToList();
                var commomCategory = context.Category.ToList();
                var commonLead = context.Lead.ToList();
                var empId = commonUser.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
                var manager = commonEmployee.FirstOrDefault(e => e.EmployeeId == empId);
                var empInCurrentOrg = commonEmployee.Where(e => e.OrganizationId == manager.OrganizationId);

                //GetOrgTreeId(manager.OrganizationId.Value, orgList);
                var userInCurrentOrg = new List<Guid>();
                empInCurrentOrg.ToList().ForEach(emp =>
                {
                    var userId = commonUser.FirstOrDefault(u => u.EmployeeId == emp.EmployeeId).UserId;
                    userInCurrentOrg.Add(userId);
                });

                #region Add By Hung RequirementRateList
                List<LeadRequirementRateEntityModel> leadRequirementRateList = new List<LeadRequirementRateEntityModel>();
                var totalCountRequirementRate = 0;
                var categoryTypeNHU = commonCategoryType.FirstOrDefault(f => f.CategoryTypeCode == "NHU");
                if (categoryTypeNHU != null)
                {
                    var category = commomCategory.Where(w => w.CategoryTypeId == categoryTypeNHU.CategoryTypeId).ToList();
                    if (category != null)
                    {
                        category.ForEach(item =>
                        {
                            LeadRequirementRateEntityModel leadRequirementRate = new LeadRequirementRateEntityModel
                            {
                                Code = item.CategoryCode,
                                Name = item.CategoryName,
                                Count = commonLead.Where(w => w.InterestedGroupId == item.CategoryId &&
                                            (w.PersonInChargeId == empId || (manager.IsManager && userInCurrentOrg.Contains(Guid.Parse(w.CreatedById))) || (w.CreatedById == parameter.UserId.ToString() && (w.PersonInChargeId == null || w.PersonInChargeId == Guid.Empty)))).Count()
                            };
                            totalCountRequirementRate += leadRequirementRate.Count;
                            leadRequirementRateList.Add(leadRequirementRate);
                        });
                    }
                }
                #endregion

                var result = new GetRequirementRateResult
                {
                    Status = true,
                    LeadRequirementRateList = leadRequirementRateList.OrderByDescending(or => or.Count).ToList(),
                    TotalCount = totalCountRequirementRate
                };
                return result;
            }
            catch (Exception ex)
            {
                return new GetRequirementRateResult
                {
                    Status = false,
                    TotalCount = 0,
                    LeadRequirementRateList = new List<LeadRequirementRateEntityModel>(),
                    Message = ex.Message
                };
            }

        }
        public GetPotentialRateResult GetPotentialRate(GetPotentialRateParameter parameter)
        {
            try
            {
                this.iAuditTrace.Trace(ActionName.GETCOUNT, ObjectName.LEAD, "Count Lead for Dashboard", parameter.UserId);
                List<Guid> orgList = new List<Guid>();
                var commonUser = context.User.ToList();
                var commonEmployee = context.Employee.ToList();
                var commonCategoryType = context.CategoryType.ToList();
                var commomCategory = context.Category.ToList();
                var commonLead = context.Lead.ToList();

                var empId = commonUser.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
                var manager = commonEmployee.FirstOrDefault(e => e.EmployeeId == empId);
                var empInCurrentOrg = commonEmployee.Where(e => e.OrganizationId == manager.OrganizationId);
                //GetOrgTreeId(manager.OrganizationId.Value, orgList);
                var userInCurrentOrg = new List<Guid>();
                empInCurrentOrg.ToList().ForEach(emp =>
                {
                    var userId = commonUser.FirstOrDefault(u => u.EmployeeId == emp.EmployeeId).UserId;
                    userInCurrentOrg.Add(userId);
                });

                #region Add by Hung
                var totalCount = 0;
                List<LeadPotentialRateEntityModel> leadPotentialRateList = new List<LeadPotentialRateEntityModel>();
                var categoryType = commonCategoryType.FirstOrDefault(f => f.CategoryTypeCode == "MTN");
                if (categoryType != null)
                {
                    var category = commomCategory.Where(w => w.CategoryTypeId == categoryType.CategoryTypeId).ToList();
                    if (category != null)
                    {
                        category.ForEach(item =>
                        {
                            var tpmCount = commonLead.Where(w => w.PotentialId == item.CategoryId &&
                                                (w.PersonInChargeId == empId
                                       || (manager.IsManager && userInCurrentOrg.Contains(Guid.Parse(w.CreatedById)))
                                       || (w.CreatedById == parameter.UserId.ToString() && (w.PersonInChargeId == null || w.PersonInChargeId == Guid.Empty)))).Count();
                            LeadPotentialRateEntityModel leadPotentialRate = new LeadPotentialRateEntityModel
                            {
                                Code = item.CategoryCode,
                                Name = item.CategoryName,
                                Count = tpmCount
                            };
                            totalCount += leadPotentialRate.Count;
                            leadPotentialRateList.Add(leadPotentialRate);
                        });
                    }
                }
                #endregion

                var result = new GetPotentialRateResult
                {
                    Status = true,
                    TotalCount = totalCount,
                    LeadPotentialRateList = leadPotentialRateList,
                };
                return result;
            }
            catch (Exception ex)
            {
                return new GetPotentialRateResult
                {
                    Status = false,
                    TotalCount = 0,
                    LeadPotentialRateList = new List<LeadPotentialRateEntityModel>(),
                    Message = ex.Message
                };
            }

        }
        public GetTopLeadResult GetTopLead(GetTopLeadParameter parameter)
        {
            try
            {
                this.iAuditTrace.Trace(ActionName.GETTOP, ObjectName.LEAD, "Get Top Lead for Dashboard", parameter.UserId);
                var empId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
                var manager = context.Employee.FirstOrDefault(e => e.EmployeeId == empId);
                var empInCurrentOrg = context.Employee.Where(e => e.OrganizationId == manager.OrganizationId);
                var userInCurrentOrg = new List<Guid>();
                empInCurrentOrg.ToList().ForEach(emp =>
                {
                    var userId = context.User.FirstOrDefault(u => u.EmployeeId == emp.EmployeeId).UserId;
                    userInCurrentOrg.Add(userId);
                });

                //if (parameter.StatusCode.ToLower() == "NDO")
                //{
                //    leads = (from l in context.Lead
                //             join c in context.Contact on l.LeadId equals c.ObjectId
                //             join c1 in context.Contact on l.PersonInChargeId equals c1.ObjectId into gj
                //             from x in gj.DefaultIfEmpty()
                //             where l.WaitingForApproval && (empId == l.PersonInChargeId || (l.PersonInChargeId == Guid.Empty && l.CreatedById == parameter.UserId.ToString())
                //             || (manager.IsManager && userInCurrentOrg.Contains(Guid.Parse(l.CreatedById))))
                //             orderby l.CreatedDate descending
                //             select new LeadEntityModel
                //             {
                //                 LeadId = l.LeadId,
                //                 ContactId = c.ContactId,
                //                 FullName = c.FirstName + " " + c.LastName,
                //                 PersonInChargeFullName = x.ObjectType == "EMP" ? x.FirstName + " " + x.LastName : "",
                //                 Email = c.Email,
                //                 Phone = c.Phone,
                //                 AvatarUrl = string.IsNullOrEmpty(c.AvatarUrl) ? "" : c.AvatarUrl,
                //                 Active = l.Active
                //             }).Take(parameter.Count).ToList();
                //}
                //else
                //{
                //    leads = (from l in context.Lead
                //             join c in context.Contact on l.LeadId equals c.ObjectId
                //             join ctx in context.Category on l.StatusId equals ctx.CategoryId into cl
                //             from x in cl.DefaultIfEmpty()
                //             where l.Status.CategoryCode == parameter.StatusCode && l.Status.CategoryType.CategoryTypeCode == "TLE" &&
                //             (empId == l.PersonInChargeId || (l.CreatedById == parameter.UserId.ToString() && l.PersonInChargeId == Guid.Empty)
                //             || (manager.IsManager && userInCurrentOrg.Contains(Guid.Parse(l.CreatedById)))) &&
                //             !l.WaitingForApproval
                //             orderby l.CreatedDate descending
                //             select new LeadEntityModel
                //             {
                //                 LeadId = l.LeadId,
                //                 ContactId = c.ContactId,
                //                 FullName = c.FirstName + " " + c.LastName,
                //                 PersonInChargeFullName = c.ObjectType == "EMP" ? c.FirstName + " " + c.LastName : "",
                //                 PersonInChargeId = l.PersonInChargeId,
                //                 Email = c.Email,
                //                 Phone = c.Phone,
                //                 AvatarUrl = string.IsNullOrEmpty(c.AvatarUrl) ? "" : c.AvatarUrl,
                //                 Active = l.Active
                //             }).Take(parameter.Count).ToList();

                //    leads.ForEach(lead => {
                //        var picId = lead.PersonInChargeId;
                //        if (picId != Guid.Empty)
                //        {
                //            lead.PersonInChargeFullName = context.Employee.FirstOrDefault(e => e.EmployeeId == picId) != null ? lead.PersonInChargeFullName = context.Employee.FirstOrDefault(e => e.EmployeeId == picId).EmployeeName : string.Empty;
                //        }
                //    });
                //}

                #region Add By Hung
                var totalCount = 0;
                List<LeadConvertRateEntityModel> leadConvertRateList = new List<LeadConvertRateEntityModel>();
                List<LeadEntityModel> listCHOLead = new List<LeadEntityModel>();
                List<LeadEntityModel> listNDOLead = new List<LeadEntityModel>();
                List<LeadEntityModel> listMOILead = new List<LeadEntityModel>();
                List<Entities.Lead> leadMoi = new List<Entities.Lead>();
                List<Entities.Lead> leadCHO = new List<Entities.Lead>();
                List<Entities.Lead> leadNDO = new List<Entities.Lead>();
                var categoryType = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "TLE");
                if (categoryType != null)
                {
                    var category = context.Category.Where(w => w.CategoryTypeId == categoryType.CategoryTypeId).ToList();
                    if (category != null)
                    {
                        List<Guid> categoryIdList = new List<Guid>();
                        category.ForEach(item =>
                        {
                            categoryIdList.Add(item.CategoryId);
                        });
                        var leadList = context.Lead.Where(w => categoryIdList.Contains(w.StatusId) &&
                                                 (w.PersonInChargeId == empId
                                        || (manager.IsManager && userInCurrentOrg.Contains(Guid.Parse(w.CreatedById)))
                                        || (w.CreatedById == parameter.UserId.ToString() && (w.PersonInChargeId == null || w.PersonInChargeId == Guid.Empty)))).ToList();

                        List<Guid> leadIdList = new List<Guid>();
                        List<Guid> personInChargeIdList = new List<Guid>();
                        category.ForEach(item =>
                        {
                            if (item.CategoryCode == "MOI")
                            {
                                leadMoi = leadList.Where(w => w.StatusId == item.CategoryId).ToList();
                                leadMoi.ForEach(lead =>
                                {
                                    leadIdList.Add(lead.LeadId);
                                    if (lead.PersonInChargeId != null)
                                    {
                                        if (!personInChargeIdList.Contains(lead.PersonInChargeId.Value))
                                        {
                                            personInChargeIdList.Add(lead.PersonInChargeId.Value);
                                        }
                                    }
                                });
                            }

                        });

                        var contacts = context.Contact.Where(w => leadIdList.Contains(w.ObjectId) && w.ObjectType == ObjectType.LEAD).ToList();
                        var contactEmployeeList = context.Contact.Where(w => personInChargeIdList.Contains(w.ObjectId) && w.ObjectType == ObjectType.EMPLOYEE).ToList();
                        #region Get Lead MOI
                        var ilead = 0;
                        leadMoi.OrderByDescending(o => o.CreatedDate).ToList().ForEach(item =>
                        {
                            ilead++;
                            var contact = contacts.FirstOrDefault(f => f.ObjectId == item.LeadId);
                            var contactEm = contactEmployeeList.FirstOrDefault(f => f.ObjectId == item.PersonInChargeId);
                            if (contact != null)
                            {
                                LeadEntityModel moi = new LeadEntityModel
                                {
                                    LeadId = item.LeadId,
                                    ContactId = contact.ContactId,
                                    FullName = contact.FirstName + " " + contact.LastName,
                                    PersonInChargeId = item.PersonInChargeId,
                                    PersonInChargeFullName = contactEm != null ? contactEm.FirstName + "" + contactEm.LastName : "",
                                    Email = contact.Email,
                                    Phone = contact.Phone,
                                    AvatarUrl = string.IsNullOrEmpty(contact.AvatarUrl) ? "" : contact.AvatarUrl,
                                    Active = item.Active
                                };
                                if (ilead <= 4)
                                    listMOILead.Add(moi);
                            }
                        });
                        #endregion
                    }
                }

                #endregion
                return new GetTopLeadResult
                {
                    Status = true,
                    ListLead = listMOILead,
                };
            }
            catch (Exception ex)
            {
                return new GetTopLeadResult
                {
                    Status = false,
                    Message = ex.Message,
                    ListLead = new List<LeadEntityModel>()
                };
            }
        }

        public GetDataLeadDashboardResult GetDataLeadDashboard(GetDataLeadDashboardParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var listEmployee = context.Employee.ToList(); //Active = false ?
                var listContact = context.Contact.Where(x => x.ObjectType == "LEA").ToList();
                var employee = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                var leadDashBoard = new Models.Lead.LeadDashBoardEntityModel();
                leadDashBoard.ListLeadSumaryByMonth = new List<LeadSumaryByMonth>();

                var commonLead = context.Lead.Where(w => w.Active == true).ToList() ?? new List<Entities.Lead>();
                var commonLeadId = commonLead.Select(w => w.LeadId).ToList() ?? new List<Guid>();
                var commonContact = context.Contact.Where(w =>
                    commonLeadId.Contains(w.ObjectId) && w.ObjectType == "LEA" && w.Active == true).ToList();
                var commonLeadInterestedGroup = context.LeadInterestedGroupMapping.ToList() ??
                                                new List<Entities.LeadInterestedGroupMapping>();
                var commonEmployee = context.Employee.ToList();

                var commonCategoryType = context.CategoryType.Where(w => w.Active == true).ToList();
                var commonCategory = context.Category.Where(w => w.Active == true).OrderBy(w => w.CreatedDate).ToList();

                var INVEST_CODE = "IVF";  //nguon tiem nang code IVF
                var POTENTIAL_CODE = "MTN"; //muc do tiem nang
                var INTERESTED_CODE = "NHU"; //nhu cau san pham, dich vu
                var LEAD_STATUS = "CHS"; //trạng thái lead

                var investFundTypeId = commonCategoryType.FirstOrDefault(w => w.CategoryTypeCode == INVEST_CODE)
                    .CategoryTypeId;
                var potentialTypeId = commonCategoryType.FirstOrDefault(w => w.CategoryTypeCode == POTENTIAL_CODE)
                    .CategoryTypeId;
                var interestedTypeId = commonCategoryType.FirstOrDefault(w => w.CategoryTypeCode == INTERESTED_CODE)
                    .CategoryTypeId;
                var leadStatusType = commonCategoryType.FirstOrDefault(w => w.CategoryTypeCode == LEAD_STATUS)
                    .CategoryTypeId;

                var statusDraftId = commonCategory
                    .FirstOrDefault(c => c.CategoryCode == "DRAFT" && c.CategoryTypeId == leadStatusType).CategoryId;
                var statusConfirmId = commonCategory
                    .FirstOrDefault(c => c.CategoryCode == "APPR" && c.CategoryTypeId == leadStatusType).CategoryId;
                var statusCloseId = commonCategory
                    .FirstOrDefault(c => c.CategoryCode == "CLOSE" && c.CategoryTypeId == leadStatusType).CategoryId;

                var commonCategoryConfirm = commonCategory.Where(c => c.CategoryTypeId == leadStatusType).ToList();
                var commonCustomer = context.Customer.ToList();

                var investFundList = commonCategory.Where(w => w.CategoryTypeId == investFundTypeId).Select(w =>
                                         new Models.CategoryEntityModel
                                         {
                                             CategoryId = w.CategoryId,
                                             CategoryName = w.CategoryName,
                                             CategoryCode = w.CategoryCode,
                                             IsDefault = w.IsDefauld
                                         }).ToList() ?? new List<Models.CategoryEntityModel>();

                var listPotential = commonCategory.Where(w => w.CategoryTypeId == potentialTypeId).Select(w =>
                                        new Models.CategoryEntityModel
                                        {
                                            CategoryId = w.CategoryId,
                                            CategoryName = w.CategoryName,
                                            CategoryCode = w.CategoryCode,
                                            IsDefault = w.IsDefauld
                                        }).ToList() ?? new List<Models.CategoryEntityModel>();

                var listInterestedGroup = commonCategory.Where(w => w.CategoryTypeId == interestedTypeId).Select(w =>
                                              new Models.CategoryEntityModel
                                              {
                                                  CategoryId = w.CategoryId,
                                                  CategoryName = w.CategoryName,
                                                  CategoryCode = w.CategoryCode,
                                                  IsDefault = w.IsDefauld
                                              }).ToList() ?? new List<Models.CategoryEntityModel>();

                var listLeadStatus = commonCategory.Where(w => w.CategoryTypeId == leadStatusType).Select(w =>
                                         new Models.CategoryEntityModel
                                         {
                                             CategoryId = w.CategoryId,
                                             CategoryName = w.CategoryName,
                                             CategoryCode = w.CategoryCode,
                                             IsDefault = w.IsDefauld
                                         }).ToList() ?? new List<Models.CategoryEntityModel>();

                if (employee.IsManager)
                {
                    /*
                  * Lấy list phòng ban con của user
                  * List phòng ban bao gồm: chính phòng ban nó đang thuộc và các phòng ban cấp dưới của nó nếu có
                  */
                    List<Guid?> listGetAllChild = new List<Guid?>();
                    listGetAllChild.Add(employee.OrganizationId.Value);
                    listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);

                    listEmployee = listEmployee
                        .Where(x => listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId)).ToList();

                    var listEmployeeId = listEmployee.Select(y => y.EmployeeId).ToList();

                    //Lấy list UserId theo list EmployeeId
                    var listUser = context.User.Where(x => listEmployeeId.Contains(x.EmployeeId.Value)).ToList();

                    var listUserId = listUser.Select(y => y.UserId.ToString()).ToList();

                    var leadPersonInCharge = commonLead.Where(c => c.PersonInChargeId != null).ToList();
                    var leadNotPersonInCharge = commonLead.Where(c => c.PersonInChargeId == null).ToList();
                    var listLead = leadPersonInCharge.Where(c => listEmployeeId.Contains(c.PersonInChargeId.Value))
                        .ToList();
                    var listLead01 = leadNotPersonInCharge.Where(c => listUserId.Contains(c.CreatedById)).ToList();

                    listLead.AddRange(listLead01);
                    for (int i = 1; i <= 12; i++)
                    {
                        int currentMonth = i;
                        var currentTime = DateTime.Now;
                        var currentMonthDate = new DateTime(currentTime.Year, currentMonth, 1);
                        //nếu có update thì so sánh update, nếu không thì so sánh create 

                        //tổng số lead trong tháng
                        var listLeadByMonth = listLead.Where(w =>
                                                  ((w.UpdatedDate != null) &&
                                                   (GetYearAndMonth(w.UpdatedDate) == GetYearAndMonth(currentMonthDate))
                                                  )
                                                  || ((w.UpdatedDate == null) &&
                                                      (GetYearAndMonth(w.CreatedDate) ==
                                                       GetYearAndMonth(currentMonthDate)))
                                              ).ToList() ?? new List<Entities.Lead>();

                        // Lấy tất cả các lead trong tháng với trạng thái là xác nhận và đóng
                        var listLeadByMonthFollowStatus = listLeadByMonth
                            .Where(c => c.StatusId == statusConfirmId || c.StatusId == statusCloseId).ToList();
                        
                        var totalLeadInMonthInvest = listLeadByMonthFollowStatus.Where(c => c.InvestmentFundId != null)
                            .ToList().Count;
                        var totalLeadInMonthPotential =
                            listLeadByMonthFollowStatus.Where(c => c.PotentialId != null).ToList().Count;

                        #region Init sumary value

                        decimal totalInvestFundByMonth = 0;
                        decimal totalPotentialByMonth = 0;
                        decimal totalInterestedGroup = 0;

                        #endregion

                        #region Thống kê nguồn tiềm năng

                        investFundList?.ForEach(cate =>
                        {
                            var listInvestFundByCategory =
                                listLeadByMonthFollowStatus.Where(w =>
                                    w.InvestmentFundId != null && w.InvestmentFundId == cate.CategoryId).ToList() ??
                                new List<Entities.Lead>();
                            var count = listInvestFundByCategory.Count(); //số lượng nguồn tiềm năng trong tháng
                            totalInvestFundByMonth += count;
                            decimal percentValue = totalLeadInMonthInvest != 0
                                ? ((decimal) count / totalLeadInMonthInvest) * 100
                                : 0;
                            var leadReportByMonth = new LeadReportByMonthModel
                            {
                                CategoryId = cate.CategoryId,
                                CategoryName = cate.CategoryName,
                                Month = currentMonth,
                                PercentValue = percentValue,
                                Value = count,
                            };
                            leadDashBoard.ListInvestFundReport.Add(leadReportByMonth);
                        });

                        #endregion

                        #region Thống kê mức độ tiềm năng

                        listPotential?.ForEach(cate =>
                        {
                            var listPotentialByCategory =
                                listLeadByMonthFollowStatus
                                    .Where(w => w.PotentialId != null && w.PotentialId == cate.CategoryId).ToList() ??
                                new List<Entities.Lead>();
                            var count = listPotentialByCategory.Count(); //số lượng mức độ tiềm năng trong tháng
                            totalPotentialByMonth += count;
                            decimal percentValue = totalLeadInMonthPotential != 0
                                ? ((decimal) count / totalLeadInMonthPotential) * 100
                                : 0;
                            var leadReportByMonth = new LeadReportByMonthModel
                            {
                                CategoryId = cate.CategoryId,
                                CategoryName = cate.CategoryName,
                                Month = currentMonth,
                                PercentValue = percentValue,
                                Value = count
                            };
                            leadDashBoard.ListPotentialReport.Add(leadReportByMonth);
                        });

                        #endregion

                        #region Thống kê tỉ lệ nhu cầu

                        var listLeadByMonthId = listLeadByMonthFollowStatus.Select(w => w.LeadId).ToList() ??
                                                new List<Guid>();
                        var listInterestedGroupByMonth =
                            commonLeadInterestedGroup.Where(w => listLeadByMonthId.Contains(w.LeadId)).ToList() ??
                            new List<Entities.LeadInterestedGroupMapping>();
                        var totalLeadInterestedByMonth = listInterestedGroupByMonth.Count();

                        listInterestedGroup?.ForEach(cate =>
                        {
                            var listInterestedGroupByCategory =
                                listInterestedGroupByMonth.Where(w =>
                                    w.InterestedGroupId == cate.CategoryId && w.InterestedGroupId != null).ToList() ??
                                new List<Entities.LeadInterestedGroupMapping>();
                            var count = listInterestedGroupByCategory.Count(); //tổng nhu cầu trong tháng
                            totalInterestedGroup += count;
                            decimal percentValue = totalLeadInterestedByMonth != 0
                                ? ((decimal) count / totalLeadInterestedByMonth) * 100
                                : 0;
                            var leadReportByMonth = new LeadReportByMonthModel
                            {
                                CategoryId = cate.CategoryId,
                                CategoryName = cate.CategoryName,
                                Month = currentMonth,
                                PercentValue = percentValue,
                                Value = count
                            };
                            leadDashBoard.ListInterestedGroupReport.Add(leadReportByMonth);
                        });

                        #endregion
                        #region Get sumary section
                        leadDashBoard.ListLeadSumaryByMonth.Add(new LeadSumaryByMonth
                        {
                            Month = currentMonth,
                            TotalInvestFundByMonth = totalInvestFundByMonth,
                            TotalPotentialByMonth = totalPotentialByMonth,
                            TotalInterestedByMonth = totalInterestedGroup
                        });
                        #endregion
                    }

                    #region Lấy top 5 cơ hội mới nhất

                    var listNewestLead =
                        listLead.Where(w => w.StatusId == statusDraftId).OrderByDescending(w => w.CreatedDate).Take(5)
                            .ToList() ?? new List<Entities.Lead>();
                    listNewestLead?.ForEach(lead =>
                    {
                        var contact = commonContact.FirstOrDefault(f => f.ObjectId == lead.LeadId);
                        var personInChange = commonEmployee.FirstOrDefault(f => f.EmployeeId == lead.PersonInChargeId);
                        var leadReport = new LeadReportModel
                        {
                            LeadId = lead.LeadId,
                            ContactId = contact?.ContactId,
                            LeadName = contact.FirstName + " " + contact.LastName,
                            Email = contact?.Email ?? "",
                            Phone = contact?.Phone ?? "",
                            PersonInChangeId = lead?.PersonInChargeId,
                            PersonInChangeName = personInChange?.EmployeeName ?? "",
                            StatusId = lead.StatusId,
                            StatusName = commonCategoryConfirm.FirstOrDefault(c => c.CategoryId == lead.StatusId)
                                             ?.CategoryName ?? "",
                            CustomerId = lead.CustomerId,
                            CustomerName = commonCustomer.FirstOrDefault(c => c.CustomerId == lead.CustomerId)
                                               ?.CustomerName ?? ""
                        };
                        leadDashBoard.TopNewestLead.Add(leadReport);
                    });

                    #endregion

                    #region Lấy top 5 cơ hội ở trạng thái xác nhận

                    var approvalId = listLeadStatus.FirstOrDefault(f => f.CategoryCode == "APPR")?.CategoryId;
                    var listApprovalLead =
                        listLead.Where(w => w.StatusId == approvalId).OrderByDescending(w => w.UpdatedDate).Take(5)
                            .ToList() ?? new List<Entities.Lead>();

                    listApprovalLead?.ForEach(lead =>
                    {
                        var contact = commonContact.FirstOrDefault(f => f.ObjectId == lead.LeadId);
                        var personInChange = commonEmployee.FirstOrDefault(f => f.EmployeeId == lead.PersonInChargeId);
                        var leadReport = new LeadReportModel
                        {
                            LeadId = lead.LeadId,
                            LeadName = contact.FirstName + " " + contact.LastName,
                            ContactId = contact?.ContactId,
                            Email = contact?.Email ?? "",
                            Phone = contact?.Phone ?? "",
                            PersonInChangeId = lead?.PersonInChargeId,
                            PersonInChangeName = personInChange?.EmployeeName ?? "",
                            StatusId = lead.StatusId,
                            StatusName = commonCategoryConfirm.FirstOrDefault(c => c.CategoryId == lead.StatusId)
                                             ?.CategoryName ?? "",
                            CustomerId = lead.CustomerId,
                            CustomerName = commonCustomer.FirstOrDefault(c => c.CustomerId == lead.CustomerId)
                                               ?.CustomerName ?? ""
                        };
                        leadDashBoard.TopApprovalLead.Add(leadReport);
                    });

                    #endregion
                }
                else
                {
                    var leadPersonInCharge = commonLead.Where(c => c.PersonInChargeId != null).ToList();
                    var leadNotPersonInCharge = commonLead.Where(c => c.PersonInChargeId == null).ToList();
                    var listLead = leadPersonInCharge.Where(c => user.EmployeeId == c.PersonInChargeId.Value).ToList();
                    var listLead01 = leadNotPersonInCharge.Where(c => user.UserId.ToString() == c.CreatedById).ToList();

                    listLead.AddRange(listLead01);

                    for (int i = 1; i <= 12; i++)
                    {
                        var currentMonth = i;
                        var currentTime = DateTime.Now;
                        var currentMonthDate = new DateTime(currentTime.Year, currentMonth, 1);
                        //nếu có update thì so sánh update, nếu không thì so sánh create 

                        //tổng số lead trong tháng
                        var listLeadByMonth = listLead.Where(w =>
                                                  ((w.UpdatedDate != null) &&
                                                   (GetYearAndMonth(w.UpdatedDate) == GetYearAndMonth(currentMonthDate))
                                                  )
                                                  || ((w.UpdatedDate == null) &&
                                                      (GetYearAndMonth(w.CreatedDate) ==
                                                       GetYearAndMonth(currentMonthDate)))
                                              ).ToList() ?? new List<Entities.Lead>();

                        // Lấy tất cả các lead trong tháng với trạng thái là xác nhận và đóng
                        var listLeadByMonthFollowStatus = listLeadByMonth
                            .Where(c => c.StatusId == statusConfirmId || c.StatusId == statusCloseId).ToList();

                        var totalLeadInMonthInvest = listLeadByMonthFollowStatus.Where(c => c.InvestmentFundId != null)
                            .ToList().Count;
                        var totalLeadInMonthPotential =
                            listLeadByMonthFollowStatus.Where(c => c.PotentialId != null).ToList().Count;

                        #region Init sumary value

                        decimal totalInvestFundByMonth = 0;
                        decimal totalPotentialByMonth = 0;
                        decimal totalInterestedGroup = 0;

                        #endregion

                        #region Thống kê nguồn tiềm năng

                        investFundList?.ForEach(cate =>
                        {
                            var listInvestFundByCategory =
                                listLeadByMonthFollowStatus.Where(w =>
                                    w.InvestmentFundId != null && w.InvestmentFundId == cate.CategoryId).ToList() ??
                                new List<Entities.Lead>();
                            var count = listInvestFundByCategory.Count(); //số lượng nguồn tiềm năng trong tháng
                            totalInvestFundByMonth += count;
                            decimal percentValue = totalLeadInMonthInvest != 0
                                ? ((decimal) count / totalLeadInMonthInvest) * 100
                                : 0;
                            var leadReportByMonth = new LeadReportByMonthModel
                            {
                                CategoryId = cate.CategoryId,
                                CategoryName = cate.CategoryName,
                                Month = currentMonth,
                                PercentValue = percentValue,
                                Value = count,
                            };
                            leadDashBoard.ListInvestFundReport.Add(leadReportByMonth);
                        });

                        #endregion

                        #region Thống kê mức độ tiềm năng

                        listPotential?.ForEach(cate =>
                        {
                            var listPotentialByCategory =
                                listLeadByMonthFollowStatus
                                    .Where(w => w.PotentialId != null && w.PotentialId == cate.CategoryId).ToList() ??
                                new List<Entities.Lead>();
                            var count = listPotentialByCategory.Count(); //số lượng mức độ tiềm năng trong tháng
                            totalPotentialByMonth += count;
                            decimal percentValue = totalLeadInMonthPotential != 0
                                ? ((decimal) count / totalLeadInMonthPotential) * 100
                                : 0;
                            var leadReportByMonth = new LeadReportByMonthModel
                            {
                                CategoryId = cate.CategoryId,
                                CategoryName = cate.CategoryName,
                                Month = currentMonth,
                                PercentValue = percentValue,
                                Value = count
                            };
                            leadDashBoard.ListPotentialReport.Add(leadReportByMonth);
                        });

                        #endregion

                        #region Thống kê tỉ lệ nhu cầu

                        var listLeadByMonthId = listLeadByMonthFollowStatus.Select(w => w.LeadId).ToList() ??
                                                new List<Guid>();
                        var listInterestedGroupByMonth =
                            commonLeadInterestedGroup.Where(w => listLeadByMonthId.Contains(w.LeadId)).ToList() ??
                            new List<Entities.LeadInterestedGroupMapping>();
                        var totalLeadInterestedByMonth = listInterestedGroupByMonth.Count();

                        listInterestedGroup?.ForEach(cate =>
                        {
                            var listInterestedGroupByCategory =
                                listInterestedGroupByMonth.Where(w =>
                                    w.InterestedGroupId == cate.CategoryId && w.InterestedGroupId != null).ToList() ??
                                new List<Entities.LeadInterestedGroupMapping>();
                            var count = listInterestedGroupByCategory.Count(); //tổng nhu cầu trong tháng
                            totalInterestedGroup += count;
                            decimal percentValue = totalLeadInterestedByMonth != 0
                                ? ((decimal) count / totalLeadInterestedByMonth) * 100
                                : 0;
                            var leadReportByMonth = new LeadReportByMonthModel
                            {
                                CategoryId = cate.CategoryId,
                                CategoryName = cate.CategoryName,
                                Month = currentMonth,
                                PercentValue = percentValue,
                                Value = count
                            };
                            leadDashBoard.ListInterestedGroupReport.Add(leadReportByMonth);
                        });

                        #endregion

                        #region Get sumary section

                        leadDashBoard.ListLeadSumaryByMonth.Add(new LeadSumaryByMonth
                        {
                            Month = currentMonth,
                            TotalInvestFundByMonth = totalInvestFundByMonth,
                            TotalPotentialByMonth = totalPotentialByMonth,
                            TotalInterestedByMonth = totalInterestedGroup
                        });

                        #endregion
                    }

                    #region Lấy top 5 cơ hội mới nhất

                    var listNewestLead =
                        listLead.Where(w => w.StatusId == statusDraftId).OrderByDescending(w => w.CreatedDate).Take(5)
                            .ToList() ?? new List<Entities.Lead>();
                    listNewestLead?.ForEach(lead =>
                    {
                        var contact = commonContact.FirstOrDefault(f => f.ObjectId == lead.LeadId);
                        var personInChange = commonEmployee.FirstOrDefault(f => f.EmployeeId == lead.PersonInChargeId);
                        var leadReport = new LeadReportModel
                        {
                            LeadId = lead.LeadId,
                            ContactId = contact?.ContactId,
                            LeadName = contact.FirstName + " " + contact.LastName,
                            Email = contact?.Email ?? "",
                            Phone = contact?.Phone ?? "",
                            PersonInChangeId = lead?.PersonInChargeId,
                            PersonInChangeName = personInChange?.EmployeeName ?? "",
                            StatusId = lead.StatusId,
                            StatusName = commonCategoryConfirm.FirstOrDefault(c => c.CategoryId == lead.StatusId)
                                             ?.CategoryName ?? "",
                            CustomerId = lead.CustomerId,
                            CustomerName = commonCustomer.FirstOrDefault(c => c.CustomerId == lead.CustomerId)
                                               ?.CustomerName ?? ""
                        };
                        leadDashBoard.TopNewestLead.Add(leadReport);
                    });

                    #endregion

                    #region Lấy top 5 cơ hội ở trạng thái xác nhận

                    var approvalId = listLeadStatus.FirstOrDefault(f => f.CategoryCode == "APPR")?.CategoryId;
                    var listApprovalLead =
                        listLead.Where(w => w.StatusId == approvalId).OrderByDescending(w => w.UpdatedDate).Take(5)
                            .ToList() ?? new List<Entities.Lead>();
                    listApprovalLead?.ForEach(lead =>
                    {
                        var contact = commonContact.FirstOrDefault(f => f.ObjectId == lead.LeadId);
                        var personInChange = commonEmployee.FirstOrDefault(f => f.EmployeeId == lead.PersonInChargeId);
                        var leadReport = new LeadReportModel
                        {
                            LeadId = lead.LeadId,
                            LeadName = contact.FirstName + " " + contact.LastName,
                            ContactId = contact?.ContactId,
                            Email = contact?.Email ?? "",
                            Phone = contact?.Phone ?? "",
                            PersonInChangeId = lead?.PersonInChargeId,
                            PersonInChangeName = personInChange?.EmployeeName ?? "",
                            StatusId = lead.StatusId,
                            StatusName = commonCategoryConfirm.FirstOrDefault(c => c.CategoryId == lead.StatusId)
                                             ?.CategoryName ?? "",
                            CustomerId = lead.CustomerId,
                            CustomerName = commonCustomer.FirstOrDefault(c => c.CustomerId == lead.CustomerId)
                                               ?.CustomerName ?? "",
                        };
                        leadDashBoard.TopApprovalLead.Add(leadReport);
                    });

                    #endregion
                }

                leadDashBoard.ListInvestFundReport?.ForEach(e =>
                {
                    e.PercentValue = decimal.Round(e.PercentValue, 2);
                });
                leadDashBoard.ListPotentialReport?.ForEach(e =>
                {
                    e.PercentValue = decimal.Round(e.PercentValue, 2);
                });
                leadDashBoard.ListInterestedGroupReport?.ForEach(e =>
                {
                    e.PercentValue = decimal.Round(e.PercentValue, 2);
                });

                return new GetDataLeadDashboardResult
                {
                    LeadDashBoard = leadDashBoard,
                    Status = true,
                };
            }
            catch (Exception ex)
            {
                return new GetDataLeadDashboardResult
                {
                    Status = false,
                    Message = ex.Message,
                    LeadDashBoard = new LeadDashBoardEntityModel()
                };
            }
        }

        private List<Guid?> getOrganizationChildrenId(Guid? id, List<Guid?> list)
        {
            var Organization = context.Organization.Where(o => o.ParentId == id).ToList();
            Organization.ForEach(item =>
            {
                list.Add(item.OrganizationId);
                getOrganizationChildrenId(item.OrganizationId, list);
            });

            return list;
        }

        private List<Guid?> getOrganizationChildrenId(List<Organization> organizationList, Guid? id, List<Guid?> list)
        {
            var organizations = organizationList.Where(o => o.ParentId == id).ToList();
            organizations.ForEach(item =>
            {
                list.Add(item.OrganizationId);
                getOrganizationChildrenId(organizationList, item.OrganizationId, list);
            });

            return list;
        }

        public DateTime? GetYearAndMonth(DateTime? time)
        {
            if (time == null) return null;
            return new DateTime(time.Value.Year, time.Value.Month, 1).Date;
        }
    }
}
