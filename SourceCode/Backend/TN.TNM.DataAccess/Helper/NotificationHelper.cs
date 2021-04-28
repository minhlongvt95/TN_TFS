using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using KellermanSoftware.CompareNetObjects;
using Microsoft.AspNetCore.Hosting.Internal;
using TN.TNM.Common;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.Common.NotificationSetting;
using TN.TNM.DataAccess.Databases;
using TN.TNM.DataAccess.Models.BillSale;

namespace TN.TNM.DataAccess.Helper
{
    public static class NotificationHelper
    {
        /*
         * checkChange (true: thực hiện kiểm tra thay đổi của olModel và newModel,
         *              false: không thực hiện kiểm tra thay đổi) 
         */
        public static void AccessNotification(TNTN8Context context, string typeModel, string actionCode,
            object oldModel, object newModel, bool checkChange, object note = null)
        {
            if (checkChange)
            {
                var configEntity = context.SystemParameter.ToList();

                #region Kiểm tra xem đã có cấu hình cho thông báo chưa?

                var screenId = context.Screen.FirstOrDefault(x => x.ScreenCode == typeModel)?.ScreenId;
                var NotifiActionId = context.NotifiAction.FirstOrDefault(x => x.NotifiActionCode == actionCode && x.ScreenId == screenId)
                    ?.NotifiActionId;

                var notifiSetting =
                    context.NotifiSetting.FirstOrDefault(x => x.ScreenId == screenId &&
                                                              x.NotifiActionId == NotifiActionId && x.Active);

                #endregion

                if (notifiSetting == null) return;
                {
                    //Tạo mới đơn hàng
                    if (typeModel == TypeModel.CustomerOrder)
                    {
                        var _customerOrder = newModel as CustomerOrder;

                        #region Kiểm tra xem cấu hình có điều kiện không?

                        var listNotifiCondition = context.NotifiSettingCondition
                            .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                        #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                        var listAllInforScreen = context.InforScreen.Where(x => x.ScreenId == screenId).ToList();
                        var listAllCondition = context.NotifiCondition.Where(x => x.ScreenId == screenId).ToList();

                        var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                        var listAllQuote = context.Quote.Where(x => x.Active == true).ToList();
                        var listAllOrderDetail =
                            context.CustomerOrderDetail.Where(x => x.OrderId == _customerOrder.OrderId);
                        var listAllOrderDetailId = listAllOrderDetail.Select(y => y.ProductId).Distinct().ToList();
                        var listProduct = context.Product.Where(x => listAllOrderDetailId.Contains(x.ProductId))
                            .ToList();
                        var listAllContact = context.Contact.Where(x =>
                            x.Active == true && (x.ObjectType == "CUS" ||
                                                 x.ObjectType == "CUS_CON" ||
                                                 x.ObjectType == "EMP")).ToList();

                        #endregion

                        //Nếu có điều kiện
                        if (listNotifiCondition.Count > 0)
                        {
                            var check_condition =
                                true; //true: tất cả điều kiện đều thõa mãn, false: có điều kiện không thỏa mãn
                            for (var i = 0; i < listNotifiCondition.Count; i++)
                            {
                                var _conditionMapping = listNotifiCondition[i];

                                //Trường thông tin
                                var inforScreen =
                                    listAllInforScreen.FirstOrDefault(x =>
                                        x.InforScreenId == _conditionMapping.InforScreenId);

                                var condition = listAllCondition.FirstOrDefault(x =>
                                    x.NotifiConditionId == _conditionMapping.NotifiSettingConditionId);

                                //Nếu có điều kiện trên db
                                if (condition != null)
                                {
                                    //Nhân viên bán hàng
                                    if (inforScreen?.InforScreenCode == "NVBH")
                                    {
                                        var seller = listAllEmployee.FirstOrDefault(x =>
                                            x.EmployeeId == _customerOrder?.Seller);

                                        check_condition = CheckStringCondition(seller?.EmployeeName?.Trim(),
                                            _conditionMapping.StringValue?.Trim(), condition.TypeCondition);
                                    }

                                    //Số hợp đồng
                                    if (inforScreen?.InforScreenCode == "SHD")
                                    {
                                        check_condition = CheckStringCondition(_customerOrder?.OrderCode?.Trim(),
                                            _conditionMapping.StringValue?.Trim(), condition.TypeCondition);
                                    }

                                    //Số báo giá
                                    if (inforScreen?.InforScreenCode == "SBG")
                                    {
                                        var quote = listAllQuote.FirstOrDefault(
                                            x => x.QuoteId == _customerOrder.QuoteId);

                                        if (quote == null)
                                        {
                                            check_condition = false;
                                        }
                                        else
                                        {
                                            check_condition = CheckStringCondition(quote.QuoteCode?.Trim(),
                                                _conditionMapping.StringValue?.Trim(), condition.TypeCondition);
                                        }
                                    }

                                    //Mã sản phẩm
                                    if (inforScreen?.InforScreenCode == "MSP")
                                    {
                                        var listProductCode = listProduct.Select(y => y.ProductCode?.Trim()).ToList();
                                        check_condition = CheckListStringCondition(listProductCode,
                                            _conditionMapping.StringValue?.Trim(), condition.TypeCondition);
                                    }

                                    //Tổng thanh toán
                                    if (inforScreen?.InforScreenCode == "TTT")
                                    {
                                        var tong_thanh_toan = _customerOrder.Amount;
                                        check_condition = CheckNumberCondition(tong_thanh_toan,
                                            _conditionMapping.NumberValue.Value, condition.TypeCondition);
                                    }

                                    if (check_condition == false) break;
                                }
                                //Nếu không có điều kiện trên db
                                else
                                {
                                    check_condition = false;
                                }
                            }

                            //Nếu tất cả điều kiện đều thỏa mãn
                            if (check_condition)
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();

                                        #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                        if (notifiSetting.IsParticipant)
                                        {
                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt (Tạo mới thì chưa cần gửi email cho người phê duyệt)

                                        if (notifiSetting.IsApproved)
                                        {
                                            //Người phê duyệt đơn hàng là người được phân quyền dữ liệu là Quản lý
                                            var listManager = listAllEmployee.Where(x => x.IsManager)
                                                .Select(y => y.EmployeeId).ToList();
                                            var listEmailManager = listAllContact
                                                .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                .Select(y => y.Email).ToList();

                                            listEmailManager.ForEach(emailManager =>
                                            {
                                                if (!String.IsNullOrEmpty(emailManager))
                                                {
                                                    listEmailSendTo.Add(emailManager.Trim());
                                                }
                                            });
                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            //Người tạo
                                            var employeeId =
                                                context.User.FirstOrDefault(x => x.UserId == _customerOrder.CreatedById)
                                                    ?.EmployeeId;

                                            var email_created = "";

                                            if (employeeId != null)
                                            {
                                                email_created = listAllContact.FirstOrDefault(x =>
                                                    x.ObjectId == employeeId && x.ObjectType == "EMP")?.Email;

                                                if (!String.IsNullOrEmpty(email_created))
                                                {
                                                    listEmailSendTo.Add(email_created.Trim());
                                                }
                                            }
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách (Nhân viên bán hàng)

                                        if (notifiSetting.IsPersonIncharge)
                                        {
                                            var email_seller = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == _customerOrder.Seller && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_seller))
                                            {
                                                listEmailSendTo.Add(email_seller.Trim());
                                            }
                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmployeeId = context.NotifiSpecial
                                            .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId)
                                            .Select(y => y.EmployeeId).ToList();

                                        var listEmailSpecial = listAllContact.Where(x =>
                                                listEmployeeId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                            .Select(y => y.Email)
                                            .ToList();

                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _customerOrder,
                                            notifiSetting.EmailTitle, configEntity);
                                        var content = ReplaceTokenForContent(context, typeModel, _customerOrder,
                                            notifiSetting.EmailContent, configEntity);
                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), subject, content);

                                        //Đặt lịch gửi
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {

                                        }

                                        #endregion
                                    }
                                }
                            }
                        }
                        //Nếu không có điều kiện
                        else
                        {
                            //Nếu gửi nội bộ
                            if (notifiSetting.SendInternal)
                            {
                                //Nếu gửi bằng email
                                if (notifiSetting.IsEmail)
                                {
                                    #region Lấy danh sách email cần gửi thông báo

                                    var listEmailSendTo = new List<string>();

                                    #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                    if (notifiSetting.IsParticipant)
                                    {
                                    }

                                    #endregion

                                    #region Lấy email người phê duyệt

                                    if (notifiSetting.IsApproved)
                                    {
                                        //Người phê duyệt đơn hàng là người được phân quyền dữ liệu là Quản lý
                                        var listManager = listAllEmployee.Where(x => x.IsManager)
                                            .Select(y => y.EmployeeId).ToList();
                                        var listEmailManager = listAllContact
                                            .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                            .Select(y => y.Email).ToList();

                                        listEmailManager.ForEach(emailManager =>
                                        {
                                            if (!String.IsNullOrEmpty(emailManager))
                                            {
                                                listEmailSendTo.Add(emailManager.Trim());
                                            }
                                        });
                                    }

                                    #endregion

                                    #region Lấy email người tạo

                                    if (notifiSetting.IsCreated)
                                    {
                                        //Người tạo
                                        var employeeId =
                                            context.User.FirstOrDefault(x => x.UserId == _customerOrder.CreatedById)
                                                ?.EmployeeId;

                                        var email_created = "";

                                        if (employeeId != null)
                                        {
                                            email_created = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == employeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_created))
                                            {
                                                listEmailSendTo.Add(email_created.Trim());
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Lấy email người phụ trách (Nhân viên bán hàng)

                                    if (notifiSetting.IsPersonIncharge)
                                    {
                                        var email_seller = listAllContact.FirstOrDefault(x =>
                                            x.ObjectId == _customerOrder.Seller && x.ObjectType == "EMP")?.Email;

                                        if (!String.IsNullOrEmpty(email_seller))
                                        {
                                            listEmailSendTo.Add(email_seller.Trim());
                                        }
                                    }

                                    #endregion

                                    #region Lấy email của danh sách người đặc biệt

                                    var listEmployeeId = context.NotifiSpecial
                                        .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId)
                                        .Select(y => y.EmployeeId).ToList();

                                    var listEmailSpecial = listAllContact.Where(x =>
                                            listEmployeeId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                        .Select(y => y.Email)
                                        .ToList();

                                    listEmailSpecial.ForEach(email =>
                                    {
                                        if (!String.IsNullOrEmpty(email))
                                        {
                                            listEmailSendTo.Add(email.Trim());
                                        }
                                    });

                                    #endregion

                                    listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                    #endregion

                                    #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                    //Gửi ngay
                                    var subject = ReplaceTokenForContent(context, typeModel, _customerOrder,
                                        notifiSetting.EmailTitle, configEntity);
                                    var content = ReplaceTokenForContent(context, typeModel, _customerOrder,
                                        notifiSetting.EmailContent, configEntity);
                                    Emailer.SendEmail(context, listEmailSendTo, new List<string>(), subject, content);

                                    //Đặt lịch gửi
                                    if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                    {
                                    }

                                    #endregion
                                }
                            }
                        }

                        #endregion
                    }
                    //Chi tiết đơn hàng
                    else if (typeModel == TypeModel.CustomerOrderDetail)
                    {
                        var _customerOrder = newModel as CustomerOrder;

                        #region Kiểm tra xem cấu hình có điều kiện không?

                        var listNotifiCondition = context.NotifiSettingCondition
                            .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                        #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                        var listAllInforScreen = context.InforScreen.Where(x => x.ScreenId == screenId).ToList();
                        var listAllCondition = context.NotifiCondition.Where(x => x.ScreenId == screenId).ToList();

                        var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                        var listAllQuote = context.Quote.Where(x => x.Active == true).ToList();
                        var listAllOrderDetail =
                            context.CustomerOrderDetail.Where(x => x.OrderId == _customerOrder.OrderId);
                        var listAllOrderDetailId = listAllOrderDetail.Select(y => y.ProductId).Distinct().ToList();
                        var listProduct = context.Product.Where(x => listAllOrderDetailId.Contains(x.ProductId))
                            .ToList();
                        var listAllContact = context.Contact.Where(x =>
                            x.Active == true && (x.ObjectType == "CUS" ||
                                                 x.ObjectType == "CUS_CON" ||
                                                 x.ObjectType == "EMP")).ToList();

                        #endregion

                        //Nếu có điều kiện
                        if (listNotifiCondition.Count > 0)
                        {
                            var check_condition =
                                true; //true: tất cả điều kiện đều thõa mãn, false: có điều kiện không thỏa mãn
                            for (var i = 0; i < listNotifiCondition.Count; i++)
                            {
                                var _conditionMapping = listNotifiCondition[i];

                                //Trường thông tin
                                var inforScreen =
                                    listAllInforScreen.FirstOrDefault(x =>
                                        x.InforScreenId == _conditionMapping.InforScreenId);

                                var condition = listAllCondition.FirstOrDefault(x =>
                                    x.NotifiConditionId == _conditionMapping.NotifiSettingConditionId);

                                //Nếu có điều kiện trên db
                                if (condition != null)
                                {
                                    //Nhân viên bán hàng
                                    if (inforScreen?.InforScreenCode == "NVBH")
                                    {
                                        var seller = listAllEmployee.FirstOrDefault(x =>
                                            x.EmployeeId == _customerOrder?.Seller);

                                        check_condition = CheckStringCondition(seller?.EmployeeName?.Trim(),
                                            _conditionMapping.StringValue?.Trim(), condition.TypeCondition);
                                    }

                                    //Số hợp đồng
                                    if (inforScreen?.InforScreenCode == "SHD")
                                    {
                                        check_condition = CheckStringCondition(_customerOrder?.OrderCode?.Trim(),
                                            _conditionMapping.StringValue?.Trim(), condition.TypeCondition);
                                    }

                                    //Số báo giá
                                    if (inforScreen?.InforScreenCode == "SBG")
                                    {
                                        var quote = listAllQuote.FirstOrDefault(
                                            x => x.QuoteId == _customerOrder.QuoteId);

                                        if (quote == null)
                                        {
                                            check_condition = false;
                                        }
                                        else
                                        {
                                            check_condition = CheckStringCondition(quote.QuoteCode?.Trim(),
                                                _conditionMapping.StringValue?.Trim(), condition.TypeCondition);
                                        }
                                    }

                                    //Mã sản phẩm
                                    if (inforScreen?.InforScreenCode == "MSP")
                                    {
                                        var listProductCode = listProduct.Select(y => y.ProductCode?.Trim()).ToList();
                                        check_condition = CheckListStringCondition(listProductCode,
                                            _conditionMapping.StringValue?.Trim(), condition.TypeCondition);
                                    }

                                    //Tổng thanh toán
                                    if (inforScreen?.InforScreenCode == "TTT")
                                    {
                                        var tong_thanh_toan = _customerOrder.Amount;
                                        check_condition = CheckNumberCondition(tong_thanh_toan,
                                            _conditionMapping.NumberValue.Value, condition.TypeCondition);
                                    }

                                    if (check_condition == false) break;
                                }
                                //Nếu không có điều kiện trên db
                                else
                                {
                                    check_condition = false;
                                }
                            }

                            //Nếu tất cả điều kiện đều thỏa mãn
                            if (check_condition)
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    #region Lấy danh sách email cần gửi thông báo

                                    var listEmailSendTo = new List<string>();

                                    #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                    if (notifiSetting.IsParticipant)
                                    {
                                    }

                                    #endregion

                                    #region Lấy email người phê duyệt

                                    if (notifiSetting.IsApproved)
                                    {
                                        //Người phê duyệt đơn hàng là người được phân quyền dữ liệu là Quản lý
                                        var listManager = listAllEmployee.Where(x => x.IsManager)
                                            .Select(y => y.EmployeeId).ToList();
                                        var listEmailManager = listAllContact
                                            .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                            .Select(y => y.Email).ToList();

                                        listEmailManager.ForEach(emailManager =>
                                        {
                                            if (!String.IsNullOrEmpty(emailManager))
                                            {
                                                listEmailSendTo.Add(emailManager.Trim());
                                            }
                                        });
                                    }

                                    #endregion

                                    #region Lấy email người tạo

                                    if (notifiSetting.IsCreated)
                                    {
                                        //Người tạo
                                        var employeeId =
                                            context.User.FirstOrDefault(x => x.UserId == _customerOrder.CreatedById)
                                                ?.EmployeeId;

                                        var email_created = "";

                                        if (employeeId != null)
                                        {
                                            email_created = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == employeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_created))
                                            {
                                                listEmailSendTo.Add(email_created.Trim());
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Lấy email người phụ trách (Nhân viên bán hàng)

                                    if (notifiSetting.IsPersonIncharge)
                                    {
                                        var email_seller = listAllContact.FirstOrDefault(x =>
                                            x.ObjectId == _customerOrder.Seller && x.ObjectType == "EMP")?.Email;

                                        if (!String.IsNullOrEmpty(email_seller))
                                        {
                                            listEmailSendTo.Add(email_seller.Trim());
                                        }
                                    }

                                    #endregion

                                    #region Lấy email của danh sách người đặc biệt

                                    var listEmployeeId = context.NotifiSpecial
                                        .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId)
                                        .Select(y => y.EmployeeId).ToList();

                                    var listEmailSpecial = listAllContact.Where(x =>
                                            listEmployeeId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                        .Select(y => y.Email)
                                        .ToList();

                                    listEmailSpecial.ForEach(email =>
                                    {
                                        if (!String.IsNullOrEmpty(email))
                                        {
                                            listEmailSendTo.Add(email.Trim());
                                        }
                                    });

                                    #endregion

                                    listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                    #endregion

                                    #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                    //Gửi ngay
                                    var subject = ReplaceTokenForContent(context, typeModel, _customerOrder,
                                        notifiSetting.EmailTitle, configEntity);
                                    var content = ReplaceTokenForContent(context, typeModel, _customerOrder,
                                        notifiSetting.EmailContent, configEntity);
                                    Emailer.SendEmail(context, listEmailSendTo, new List<string>(), subject, content);

                                    //Đặt lịch gửi
                                    if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                    {
                                    }

                                    #endregion
                                }
                            }
                        }
                        //Nếu không có điều kiện
                        else
                        {
                            //Nếu gửi nội bộ
                            if (notifiSetting.SendInternal)
                            {
                                //Nếu gửi bằng email
                                if (notifiSetting.IsEmail)
                                {
                                    #region Lấy danh sách email cần gửi thông báo

                                    var listEmailSendTo = new List<string>();

                                    #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                    if (notifiSetting.IsParticipant)
                                    {
                                    }

                                    #endregion

                                    #region Lấy email người phê duyệt

                                    if (notifiSetting.IsApproved)
                                    {
                                        //Người phê duyệt đơn hàng là người được phân quyền dữ liệu là Quản lý
                                        var listManager = listAllEmployee.Where(x => x.IsManager)
                                            .Select(y => y.EmployeeId).ToList();
                                        var listEmailManager = listAllContact
                                            .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                            .Select(y => y.Email).ToList();

                                        listEmailManager.ForEach(emailManager =>
                                        {
                                            if (!String.IsNullOrEmpty(emailManager))
                                            {
                                                listEmailSendTo.Add(emailManager.Trim());
                                            }
                                        });
                                    }

                                    #endregion

                                    #region Lấy email người tạo

                                    if (notifiSetting.IsCreated)
                                    {
                                        //Người tạo
                                        var employeeId =
                                            context.User.FirstOrDefault(x => x.UserId == _customerOrder.CreatedById)
                                                ?.EmployeeId;

                                        var email_created = "";

                                        if (employeeId != null)
                                        {
                                            email_created = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == employeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_created))
                                            {
                                                listEmailSendTo.Add(email_created.Trim());
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Lấy email người phụ trách (Nhân viên bán hàng)

                                    if (notifiSetting.IsPersonIncharge)
                                    {
                                        var email_seller = listAllContact.FirstOrDefault(x =>
                                            x.ObjectId == _customerOrder.Seller && x.ObjectType == "EMP")?.Email;

                                        if (!String.IsNullOrEmpty(email_seller))
                                        {
                                            listEmailSendTo.Add(email_seller.Trim());
                                        }
                                    }

                                    #endregion

                                    #region Lấy email của danh sách người đặc biệt

                                    var listEmployeeId = context.NotifiSpecial
                                        .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId)
                                        .Select(y => y.EmployeeId).ToList();

                                    var listEmailSpecial = listAllContact.Where(x =>
                                            listEmployeeId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                        .Select(y => y.Email)
                                        .ToList();

                                    listEmailSpecial.ForEach(email =>
                                    {
                                        if (!String.IsNullOrEmpty(email))
                                        {
                                            listEmailSendTo.Add(email.Trim());
                                        }
                                    });

                                    #endregion

                                    #region Lấy email người tạo bình luận

                                    if (note != null)
                                    {
                                        var _note = note as Note;

                                        var comment_employeeId =
                                            context.User.FirstOrDefault(x => x.UserId == _note.CreatedById)?.EmployeeId;

                                        var email_comment = "";

                                        if (comment_employeeId != null)
                                        {
                                            email_comment = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == comment_employeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_comment))
                                            {
                                                listEmailSendTo.Add(email_comment.Trim());
                                            }
                                        }
                                    }

                                    #endregion

                                    listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                    #endregion

                                    #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                    //Gửi ngay
                                    var subject = ReplaceTokenForContent(context, typeModel, _customerOrder,
                                        notifiSetting.EmailTitle, configEntity, note);
                                    var content = ReplaceTokenForContent(context, typeModel, _customerOrder,
                                        notifiSetting.EmailContent, configEntity, note);

                                    #region Build nội dung thay đổi

                                    //string contentModelChange = "";

                                    ////Nếu phải kiểm tra thay đổi của model
                                    //if (checkChange)
                                    //{
                                    //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                    //}

                                    #endregion

                                    Emailer.SendEmail(context, listEmailSendTo, new List<string>(), subject, content);

                                    //Đặt lịch gửi             
                                    if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                    {
                                    }

                                    #endregion
                                }
                            }
                        }

                        #endregion
                    }
                    //Tạo báo giá
                    else if (typeModel == TypeModel.Quote)
                    {
                        var _quote = newModel as Quote;

                        #region Kiểm tra xem cấu hình có điều kiện không?

                        var listNotifiCondition = context.NotifiSettingCondition
                            .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                        #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                        var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                        var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                        #endregion

                        //Nếu có điều kiện
                        if (listNotifiCondition.Count > 0)
                        {
                            //Do something...
                        }
                        //Nếu không có điều kiện
                        else
                        {
                            //Nếu gửi nội bộ
                            if (notifiSetting.SendInternal)
                            {
                                //Nếu gửi bằng email
                                if (notifiSetting.IsEmail)
                                {
                                    #region Lấy danh sách email cần gửi thông báo

                                    var listEmailSendTo = new List<string>();

                                    #region Lấy email người tham gia

                                    if (notifiSetting.IsParticipant)
                                    {
                                        var listParticipantId = context.QuoteParticipantMapping
                                            .Where(x => x.QuoteId == _quote.QuoteId).Select(y => y.EmployeeId).ToList();

                                        if (listParticipantId.Count > 0)
                                        {
                                            var listEmailParticipant = listAllContact.Where(x =>
                                                    listParticipantId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                .Select(y => y.Email)
                                                .ToList();

                                            listEmailParticipant.ForEach(email =>
                                            {
                                                if (!String.IsNullOrEmpty(email))
                                                {
                                                    listEmailSendTo.Add(email.Trim());
                                                }
                                            });
                                        }
                                    }

                                    #endregion

                                    #region Lấy email người phê duyệt

                                    if (notifiSetting.IsApproved)
                                    {
                                        //Người phê duyệt báo giá sẽ phải kiểm tra theo Quy trình phê duyệt báo giá

                                        var listApproved =
                                            GetListEmployeeApproved(context, "PDBG", _quote.ApprovalStep,
                                                listAllEmployee);

                                        var listEmailManager = listAllContact
                                            .Where(x => listApproved.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                            .Select(y => y.Email).ToList();

                                        listEmailManager.ForEach(emailManager =>
                                        {
                                            if (!String.IsNullOrEmpty(emailManager))
                                            {
                                                listEmailSendTo.Add(emailManager.Trim());
                                            }
                                        });
                                    }

                                    #endregion

                                    #region Lấy email người tạo

                                    if (notifiSetting.IsCreated)
                                    {
                                        //Người tạo
                                        var employeeId =
                                            context.User.FirstOrDefault(x => x.UserId == _quote.CreatedById)
                                                ?.EmployeeId;

                                        var email_created = "";

                                        if (employeeId != null)
                                        {
                                            email_created = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == employeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_created))
                                            {
                                                listEmailSendTo.Add(email_created.Trim());
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Lấy email người phụ trách (Nhân viên bán hàng)

                                    if (notifiSetting.IsPersonIncharge)
                                    {
                                        var email_seller = listAllContact.FirstOrDefault(x =>
                                            x.ObjectId == _quote.Seller && x.ObjectType == "EMP")?.Email;

                                        if (!String.IsNullOrEmpty(email_seller))
                                        {
                                            listEmailSendTo.Add(email_seller.Trim());
                                        }
                                    }

                                    #endregion

                                    #region Lấy email của danh sách người đặc biệt

                                    var listEmployeeId = context.NotifiSpecial
                                        .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId)
                                        .Select(y => y.EmployeeId).ToList();

                                    var listEmailSpecial = listAllContact.Where(x =>
                                            listEmployeeId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                        .Select(y => y.Email)
                                        .ToList();

                                    listEmailSpecial.ForEach(email =>
                                    {
                                        if (!String.IsNullOrEmpty(email))
                                        {
                                            listEmailSendTo.Add(email.Trim());
                                        }
                                    });

                                    #endregion

                                    listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                    #endregion

                                    #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                    //Gửi ngay
                                    var subject = ReplaceTokenForContent(context, typeModel, _quote,
                                        notifiSetting.EmailTitle, configEntity);
                                    var content = ReplaceTokenForContent(context, typeModel, _quote,
                                        notifiSetting.EmailContent, configEntity);

                                    #region Build nội dung thay đổi

                                    //string contentModelChange = "";

                                    ////Nếu phải kiểm tra thay đổi của model
                                    //if (checkChange)
                                    //{
                                    //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                    //}

                                    #endregion

                                    Emailer.SendEmail(context, listEmailSendTo, new List<string>(), subject, content);

                                    //Đặt lịch gửi             
                                    if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                    {
                                    }

                                    #endregion
                                }
                            }
                        }

                        #endregion
                    }
                    //Chi tiết báo giá
                    else if (typeModel == TypeModel.QuoteDetail)
                    {
                        var _quote = newModel as Quote;

                        #region Kiểm tra xem cấu hình có điều kiện không?

                        var listNotifiCondition = context.NotifiSettingCondition
                            .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                        #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                        var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                        var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                        #endregion

                        //Nếu có điều kiện
                        if (listNotifiCondition.Count > 0)
                        {
                            //Do something...
                        }
                        //Nếu không có điều kiện
                        else
                        {
                            //Nếu gửi nội bộ
                            if (notifiSetting.SendInternal)
                            {
                                //Nếu gửi bằng email
                                if (notifiSetting.IsEmail)
                                {
                                    #region Lấy danh sách email cần gửi thông báo

                                    var listEmailSendTo = new List<string>();

                                    #region Lấy email người tham gia

                                    if (notifiSetting.IsParticipant)
                                    {
                                        var listParticipantId = context.QuoteParticipantMapping
                                            .Where(x => x.QuoteId == _quote.QuoteId).Select(y => y.EmployeeId).ToList();

                                        if (listParticipantId.Count > 0)
                                        {
                                            var listEmailParticipant = listAllContact.Where(x =>
                                                    listParticipantId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                .Select(y => y.Email)
                                                .ToList();

                                            listEmailParticipant.ForEach(email =>
                                            {
                                                if (!String.IsNullOrEmpty(email))
                                                {
                                                    listEmailSendTo.Add(email.Trim());
                                                }
                                            });
                                        }
                                    }

                                    #endregion

                                    #region Lấy email người phê duyệt

                                    if (notifiSetting.IsApproved)
                                    {
                                        //Người phê duyệt báo giá sẽ phải kiểm tra theo Quy trình phê duyệt báo giá

                                        var listApproved =
                                            GetListEmployeeApproved(context, "PDBG", _quote.ApprovalStep,
                                                listAllEmployee);

                                        var listEmailManager = listAllContact
                                            .Where(x => listApproved.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                            .Select(y => y.Email).ToList();

                                        listEmailManager.ForEach(emailManager =>
                                        {
                                            if (!String.IsNullOrEmpty(emailManager))
                                            {
                                                listEmailSendTo.Add(emailManager.Trim());
                                            }
                                        });
                                    }

                                    #endregion

                                    #region Lấy email người tạo

                                    if (notifiSetting.IsCreated)
                                    {
                                        //Người tạo
                                        var employeeId =
                                            context.User.FirstOrDefault(x => x.UserId == _quote.CreatedById)
                                                ?.EmployeeId;

                                        var email_created = "";

                                        if (employeeId != null)
                                        {
                                            email_created = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == employeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_created))
                                            {
                                                listEmailSendTo.Add(email_created.Trim());
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Lấy email người phụ trách (Nhân viên bán hàng)

                                    if (notifiSetting.IsPersonIncharge)
                                    {
                                        var email_seller = listAllContact.FirstOrDefault(x =>
                                            x.ObjectId == _quote.Seller && x.ObjectType == "EMP")?.Email;

                                        if (!String.IsNullOrEmpty(email_seller))
                                        {
                                            listEmailSendTo.Add(email_seller.Trim());
                                        }
                                    }

                                    #endregion

                                    #region Lấy email của danh sách người đặc biệt

                                    var listEmployeeId = context.NotifiSpecial
                                        .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId)
                                        .Select(y => y.EmployeeId).ToList();

                                    var listEmailSpecial = listAllContact.Where(x =>
                                            listEmployeeId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                        .Select(y => y.Email)
                                        .ToList();

                                    listEmailSpecial.ForEach(email =>
                                    {
                                        if (!String.IsNullOrEmpty(email))
                                        {
                                            listEmailSendTo.Add(email.Trim());
                                        }
                                    });

                                    #endregion

                                    #region Lấy email người tạo bình luận

                                    if (note != null)
                                    {
                                        var _note = note as Note;

                                        var comment_employeeId =
                                            context.User.FirstOrDefault(x => x.UserId == _note.CreatedById)?.EmployeeId;

                                        var email_comment = "";

                                        if (comment_employeeId != null)
                                        {
                                            email_comment = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == comment_employeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_comment))
                                            {
                                                listEmailSendTo.Add(email_comment.Trim());
                                            }
                                        }
                                    }

                                    #endregion

                                    listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                    #endregion

                                    #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                    //Gửi ngay
                                    var subject = ReplaceTokenForContent(context, typeModel, _quote,
                                        notifiSetting.EmailTitle, configEntity, note);
                                    var content = ReplaceTokenForContent(context, typeModel, _quote,
                                        notifiSetting.EmailContent, configEntity, note);

                                    #region Build nội dung thay đổi

                                    //string contentModelChange = "";

                                    ////Nếu phải kiểm tra thay đổi của model
                                    //if (checkChange)
                                    //{
                                    //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                    //}

                                    #endregion

                                    Emailer.SendEmail(context, listEmailSendTo, new List<string>(), subject, content);

                                    //Đặt lịch gửi             
                                    if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                    {
                                    }

                                    #endregion
                                }
                            }
                        }

                        #endregion
                    }
                    //Tạo Cơ hội
                    else if (typeModel == TypeModel.Lead)
                    {
                        var _lead = newModel as Lead;

                        #region Kiểm tra xem cấu hình có điều kiện không?

                        var listNotifiCondition = context.NotifiSettingCondition
                            .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                        #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                        var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                        var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                        #endregion

                        //Nếu có điều kiện
                        if (listNotifiCondition.Count > 0)
                        {
                            //Do something...
                        }
                        //Nếu không có điều kiện
                        else
                        {
                            //Nếu gửi nội bộ
                            if (notifiSetting.SendInternal)
                            {
                                //Nếu gửi bằng email
                                if (notifiSetting.IsEmail)
                                {
                                    #region Lấy danh sách email cần gửi thông báo

                                    var listEmailSendTo = new List<string>();

                                    #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                    if (notifiSetting.IsParticipant)
                                    {
                                    }

                                    #endregion

                                    #region Lấy email người phê duyệt

                                    if (notifiSetting.IsApproved)
                                    {
                                        //Người phê duyệt đơn hàng là người được phân quyền dữ liệu là Quản lý
                                        var listManager = listAllEmployee.Where(x => x.IsManager)
                                            .Select(y => y.EmployeeId).ToList();
                                        var listEmailManager = listAllContact
                                            .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                            .Select(y => y.Email).ToList();

                                        listEmailManager.ForEach(emailManager =>
                                        {
                                            if (!String.IsNullOrEmpty(emailManager))
                                            {
                                                listEmailSendTo.Add(emailManager.Trim());
                                            }
                                        });
                                    }

                                    #endregion

                                    #region Lấy email người tạo

                                    if (notifiSetting.IsCreated)
                                    {
                                        //Người tạo
                                        var employeeId =
                                            context.User.FirstOrDefault(x => x.UserId.ToString() == _lead.CreatedById)
                                                ?.EmployeeId;

                                        var email_created = "";

                                        if (employeeId != null)
                                        {
                                            email_created = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == employeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_created))
                                            {
                                                listEmailSendTo.Add(email_created.Trim());
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Lấy email người phụ trách (Nhân viên bán hàng)

                                    if (notifiSetting.IsPersonIncharge)
                                    {
                                        var email_seller = listAllContact.FirstOrDefault(x =>
                                            x.ObjectId == _lead.PersonInChargeId && x.ObjectType == "EMP")?.Email;

                                        if (!String.IsNullOrEmpty(email_seller))
                                        {
                                            listEmailSendTo.Add(email_seller.Trim());
                                        }
                                    }

                                    #endregion

                                    #region Lấy email của danh sách người đặc biệt

                                    var listEmployeeId = context.NotifiSpecial
                                        .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId)
                                        .Select(y => y.EmployeeId).ToList();

                                    var listEmailSpecial = listAllContact.Where(x =>
                                            listEmployeeId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                        .Select(y => y.Email)
                                        .ToList();

                                    listEmailSpecial.ForEach(email =>
                                    {
                                        if (!String.IsNullOrEmpty(email))
                                        {
                                            listEmailSendTo.Add(email.Trim());
                                        }
                                    });

                                    #endregion

                                    listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                    #endregion

                                    #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                    //Gửi ngay
                                    var subject = ReplaceTokenForContent(context, typeModel, _lead,
                                        notifiSetting.EmailTitle, configEntity);
                                    var content = ReplaceTokenForContent(context, typeModel, _lead,
                                        notifiSetting.EmailContent, configEntity);

                                    #region Build nội dung thay đổi

                                    //string contentModelChange = "";

                                    ////Nếu phải kiểm tra thay đổi của model
                                    //if (checkChange)
                                    //{
                                    //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                    //}

                                    #endregion

                                    Emailer.SendEmail(context, listEmailSendTo, new List<string>(), subject, content);

                                    //Đặt lịch gửi             
                                    if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                    {
                                    }


                                    #endregion
                                }
                            }
                        }

                        #endregion
                    }
                    //Chi tiết cơ hội
                    else if (typeModel == TypeModel.LeadDetail)
                    {
                        var _lead = newModel as Lead;

                        #region Kiểm tra xem cấu hình có điều kiện không?

                        var listNotifiCondition = context.NotifiSettingCondition
                            .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                        #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                        var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                        var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                        #endregion

                        //Nếu có điều kiện
                        if (listNotifiCondition.Count > 0)
                        {
                            //Do something...
                        }
                        //Nếu không có điều kiện
                        else
                        {
                            //Nếu gửi nội bộ
                            if (notifiSetting.SendInternal)
                            {
                                //Nếu gửi bằng email
                                if (notifiSetting.IsEmail)
                                {
                                    #region Lấy danh sách email cần gửi thông báo

                                    var listEmailSendTo = new List<string>();

                                    #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                    if (notifiSetting.IsParticipant)
                                    {
                                    }

                                    #endregion

                                    #region Lấy email người phê duyệt (Tạo mới thì chưa cần gửi email cho người phê duyệt)

                                    if (notifiSetting.IsApproved)
                                    {
                                        //Người phê duyệt đơn hàng là người được phân quyền dữ liệu là Quản lý
                                        var listManager = listAllEmployee.Where(x => x.IsManager)
                                            .Select(y => y.EmployeeId).ToList();
                                        var listEmailManager = listAllContact
                                            .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                            .Select(y => y.Email).ToList();

                                        listEmailManager.ForEach(emailManager =>
                                        {
                                            if (!String.IsNullOrEmpty(emailManager))
                                            {
                                                listEmailSendTo.Add(emailManager.Trim());
                                            }
                                        });
                                    }

                                    #endregion

                                    #region Lấy email người tạo

                                    if (notifiSetting.IsCreated)
                                    {
                                        //Người tạo
                                        var employeeId =
                                            context.User.FirstOrDefault(x => x.UserId.ToString() == _lead.CreatedById)
                                                ?.EmployeeId;

                                        var email_created = "";

                                        if (employeeId != null)
                                        {
                                            email_created = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == employeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_created))
                                            {
                                                listEmailSendTo.Add(email_created.Trim());
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Lấy email người phụ trách (Nhân viên bán hàng)

                                    if (notifiSetting.IsPersonIncharge)
                                    {
                                        var email_seller = listAllContact.FirstOrDefault(x =>
                                            x.ObjectId == _lead.PersonInChargeId && x.ObjectType == "EMP")?.Email;

                                        if (!String.IsNullOrEmpty(email_seller))
                                        {
                                            listEmailSendTo.Add(email_seller.Trim());
                                        }
                                    }

                                    #endregion

                                    #region Lấy email của danh sách người đặc biệt

                                    var listEmployeeId = context.NotifiSpecial
                                        .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId)
                                        .Select(y => y.EmployeeId).ToList();

                                    var listEmailSpecial = listAllContact.Where(x =>
                                            listEmployeeId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                        .Select(y => y.Email)
                                        .ToList();

                                    listEmailSpecial.ForEach(email =>
                                    {
                                        if (!String.IsNullOrEmpty(email))
                                        {
                                            listEmailSendTo.Add(email.Trim());
                                        }
                                    });

                                    #endregion

                                    #region Lấy email người tạo bình luận

                                    if (note != null)
                                    {
                                        var _note = note as Note;

                                        var comment_employeeId =
                                            context.User.FirstOrDefault(x => x.UserId == _note.CreatedById)?.EmployeeId;

                                        var email_comment = "";

                                        if (comment_employeeId != null)
                                        {
                                            email_comment = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == comment_employeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_comment))
                                            {
                                                listEmailSendTo.Add(email_comment.Trim());
                                            }
                                        }
                                    }

                                    #endregion

                                    listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                    #endregion

                                    #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                    //Gửi ngay
                                    var subject = ReplaceTokenForContent(context, typeModel, _lead,
                                        notifiSetting.EmailTitle, configEntity, note);
                                    var content = ReplaceTokenForContent(context, typeModel, _lead,
                                        notifiSetting.EmailContent, configEntity, note);

                                    #region Build nội dung thay đổi

                                    //string contentModelChange = "";

                                    ////Nếu phải kiểm tra thay đổi của model
                                    //if (checkChange)
                                    //{
                                    //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                    //}

                                    #endregion

                                    Emailer.SendEmail(context, listEmailSendTo, new List<string>(), subject, content);

                                    //Đặt lịch gửi             
                                    if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                    {
                                    }

                                    #endregion
                                }
                            }
                        }

                        #endregion
                    }
                    //Tạo hồ sơ thầu
                    else if (typeModel == TypeModel.SaleBidding)
                    {
                        var _saleBidding = newModel as SaleBidding;

                        #region Kiểm tra xem cấu hình có điều kiện không?

                        var listNotifiCondition = context.NotifiSettingCondition
                            .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                        #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                        var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                        var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                        #endregion

                        //Nếu có điều kiện
                        if (listNotifiCondition.Count > 0)
                        {
                            //Do something...
                        }
                        //Nếu không có điều kiện
                        else
                        {
                            //Nếu gửi nội bộ
                            if (notifiSetting.SendInternal)
                            {
                                //Nếu gửi bằng email
                                if (notifiSetting.IsEmail)
                                {
                                    #region Lấy danh sách email cần gửi thông báo

                                    var listEmailSendTo = new List<string>();

                                    #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                    if (notifiSetting.IsParticipant)
                                    {
                                    }

                                    #endregion

                                    #region Lấy email người phê duyệt (Tạo mới thì chưa cần gửi email cho người phê duyệt)

                                    if (notifiSetting.IsApproved)
                                    {
                                        //Người phê duyệt đơn hàng là người được phân quyền dữ liệu là Quản lý
                                        var listManager = listAllEmployee.Where(x => x.IsManager)
                                            .Select(y => y.EmployeeId).ToList();
                                        var listEmailManager = listAllContact
                                            .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                            .Select(y => y.Email).ToList();

                                        listEmailManager.ForEach(emailManager =>
                                        {
                                            if (!String.IsNullOrEmpty(emailManager))
                                            {
                                                listEmailSendTo.Add(emailManager.Trim());
                                            }
                                        });
                                    }

                                    #endregion

                                    #region Lấy email người tạo

                                    if (notifiSetting.IsCreated)
                                    {
                                        //Người tạo
                                        var employeeId =
                                            context.User.FirstOrDefault(x => x.UserId == _saleBidding.CreatedById)?.EmployeeId;

                                        var email_created = "";

                                        if (employeeId != null)
                                        {
                                            email_created = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == employeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_created))
                                            {
                                                listEmailSendTo.Add(email_created.Trim());
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Lấy email người phụ trách (Nhân viên bán hàng)

                                    if (notifiSetting.IsPersonIncharge)
                                    {
                                        var email_seller = listAllContact.FirstOrDefault(x =>
                                            x.ObjectId == _saleBidding.PersonInChargeId && x.ObjectType == "EMP")?.Email;

                                        if (!String.IsNullOrEmpty(email_seller))
                                        {
                                            listEmailSendTo.Add(email_seller.Trim());
                                        }
                                    }

                                    #endregion

                                    #region Lấy email của danh sách người đặc biệt

                                    var listEmployeeId = context.NotifiSpecial
                                        .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId)
                                        .Select(y => y.EmployeeId).ToList();

                                    var listEmailSpecial = listAllContact.Where(x =>
                                            listEmployeeId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                        .Select(y => y.Email)
                                        .ToList();

                                    listEmailSpecial.ForEach(email =>
                                    {
                                        if (!String.IsNullOrEmpty(email))
                                        {
                                            listEmailSendTo.Add(email.Trim());
                                        }
                                    });

                                    #endregion

                                    listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                    #endregion

                                    #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                    //Gửi ngay
                                    var subject = ReplaceTokenForContent(context, typeModel, _saleBidding,
                                        notifiSetting.EmailTitle, configEntity);
                                    var content = ReplaceTokenForContent(context, typeModel, _saleBidding,
                                        notifiSetting.EmailContent, configEntity);

                                    #region Build nội dung thay đổi

                                    //string contentModelChange = "";

                                    ////Nếu phải kiểm tra thay đổi của model
                                    //if (checkChange)
                                    //{
                                    //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                    //}

                                    #endregion

                                    Emailer.SendEmail(context, listEmailSendTo, new List<string>(), subject, content);

                                    //Đặt lịch gửi             
                                    if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                    {
                                    }

                                    #endregion
                                }
                            }
                        }

                        #endregion
                    }
                    //Chi tiết hồ sơ thầu
                    else if (typeModel == TypeModel.SaleBiddingDetail)
                    {
                        var _saleBidding = newModel as SaleBidding;

                        #region Kiểm tra xem cấu hình có điều kiện không?

                        var listNotifiCondition = context.NotifiSettingCondition
                            .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                        #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                        var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                        var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                        #endregion

                        //Nếu có điều kiện
                        if (listNotifiCondition.Count > 0)
                        {
                            //Do something...
                        }
                        //Nếu không có điều kiện
                        else
                        {
                            //Nếu gửi nội bộ
                            if (notifiSetting.SendInternal)
                            {
                                //Nếu gửi bằng email
                                if (notifiSetting.IsEmail)
                                {
                                    #region Lấy danh sách email cần gửi thông báo

                                    var listEmailSendTo = new List<string>();

                                    #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                    if (notifiSetting.IsParticipant)
                                    {
                                    }

                                    #endregion

                                    #region Lấy email người phê duyệt (Tạo mới thì chưa cần gửi email cho người phê duyệt)

                                    if (notifiSetting.IsApproved)
                                    {
                                        //Người phê duyệt đơn hàng là người được phân quyền dữ liệu là Quản lý
                                        var listManager = listAllEmployee.Where(x => x.IsManager)
                                            .Select(y => y.EmployeeId).ToList();
                                        var listEmailManager = listAllContact
                                            .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                            .Select(y => y.Email).ToList();

                                        listEmailManager.ForEach(emailManager =>
                                        {
                                            if (!String.IsNullOrEmpty(emailManager))
                                            {
                                                listEmailSendTo.Add(emailManager.Trim());
                                            }
                                        });
                                    }

                                    #endregion

                                    #region Lấy email người tạo

                                    if (notifiSetting.IsCreated)
                                    {
                                        //Người tạo
                                        var employeeId =
                                            context.User.FirstOrDefault(x => x.UserId == _saleBidding.CreatedById)
                                                ?.EmployeeId;

                                        var email_created = "";

                                        if (employeeId != null)
                                        {
                                            email_created = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == employeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_created))
                                            {
                                                listEmailSendTo.Add(email_created.Trim());
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Lấy email người phụ trách (Nhân viên bán hàng)

                                    if (notifiSetting.IsPersonIncharge)
                                    {
                                        var email_seller = listAllContact.FirstOrDefault(x =>
                                            x.ObjectId == _saleBidding.PersonInChargeId && x.ObjectType == "EMP")?.Email;

                                        if (!String.IsNullOrEmpty(email_seller))
                                        {
                                            listEmailSendTo.Add(email_seller.Trim());
                                        }
                                    }

                                    #endregion

                                    #region Lấy email của danh sách người đặc biệt

                                    var listEmployeeId = context.NotifiSpecial
                                        .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId)
                                        .Select(y => y.EmployeeId).ToList();

                                    var listEmailSpecial = listAllContact.Where(x =>
                                            listEmployeeId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                        .Select(y => y.Email)
                                        .ToList();

                                    listEmailSpecial.ForEach(email =>
                                    {
                                        if (!String.IsNullOrEmpty(email))
                                        {
                                            listEmailSendTo.Add(email.Trim());
                                        }
                                    });

                                    #endregion

                                    #region Lấy email người tạo bình luận

                                    if (note != null)
                                    {
                                        var _note = note as Note;

                                        var comment_employeeId =
                                            context.User.FirstOrDefault(x => x.UserId == _note.CreatedById)?.EmployeeId;

                                        var email_comment = "";

                                        if (comment_employeeId != null)
                                        {
                                            email_comment = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == comment_employeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_comment))
                                            {
                                                listEmailSendTo.Add(email_comment.Trim());
                                            }
                                        }
                                    }

                                    #endregion

                                    listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                    #endregion

                                    #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                    //Gửi ngay
                                    var subject = ReplaceTokenForContent(context, typeModel, _saleBidding,
                                        notifiSetting.EmailTitle, configEntity, note);
                                    var content = ReplaceTokenForContent(context, typeModel, _saleBidding,
                                        notifiSetting.EmailContent, configEntity, note);

                                    #region Build nội dung thay đổi

                                    //string contentModelChange = "";

                                    ////Nếu phải kiểm tra thay đổi của model
                                    //if (checkChange)
                                    //{
                                    //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                    //}

                                    #endregion

                                    Emailer.SendEmail(context, listEmailSendTo, new List<string>(), subject, content);

                                    //Đặt lịch gửi             
                                    if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                    {
                                    }

                                    #endregion
                                }
                            }
                        }

                        #endregion
                    }
                    //Tạo hợp đồng bán
                    else if (typeModel == TypeModel.Contract)
                    {
                        var _contract = newModel as Contract;

                        #region Kiểm tra xem cấu hình có điều kiện không?

                        var listNotifiCondition = context.NotifiSettingCondition
                            .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                        #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                        var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                        var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                        #endregion

                        //Nếu có điều kiện
                        if (listNotifiCondition.Count > 0)
                        {
                            //Do something...
                        }
                        //Nếu không có điều kiện
                        else
                        {
                            //Nếu gửi nội bộ
                            if (notifiSetting.SendInternal)
                            {
                                //Nếu gửi bằng email
                                if (notifiSetting.IsEmail)
                                {
                                    #region Lấy danh sách email cần gửi thông báo

                                    var listEmailSendTo = new List<string>();

                                    #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                    if (notifiSetting.IsParticipant)
                                    {
                                    }

                                    #endregion

                                    #region Lấy email người phê duyệt (Tạo mới thì chưa cần gửi email cho người phê duyệt)

                                    if (notifiSetting.IsApproved)
                                    {
                                        //Người phê duyệt đơn hàng là người được phân quyền dữ liệu là Quản lý
                                        var listManager = listAllEmployee.Where(x => x.IsManager)
                                            .Select(y => y.EmployeeId).ToList();
                                        var listEmailManager = listAllContact
                                            .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                            .Select(y => y.Email).ToList();

                                        listEmailManager.ForEach(emailManager =>
                                        {
                                            if (!String.IsNullOrEmpty(emailManager))
                                            {
                                                listEmailSendTo.Add(emailManager.Trim());
                                            }
                                        });
                                    }

                                    #endregion

                                    #region Lấy email người tạo

                                    if (notifiSetting.IsCreated)
                                    {
                                        //Người tạo
                                        var employeeId =
                                            context.User.FirstOrDefault(x => x.UserId == _contract.CreatedById)?.EmployeeId;

                                        var email_created = "";

                                        if (employeeId != null)
                                        {
                                            email_created = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == employeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_created))
                                            {
                                                listEmailSendTo.Add(email_created.Trim());
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Lấy email người phụ trách (Nhân viên bán hàng)

                                    if (notifiSetting.IsPersonIncharge)
                                    {
                                        var email_seller = listAllContact.FirstOrDefault(x =>
                                            x.ObjectId == _contract.EmployeeId && x.ObjectType == "EMP")?.Email;

                                        if (!String.IsNullOrEmpty(email_seller))
                                        {
                                            listEmailSendTo.Add(email_seller.Trim());
                                        }
                                    }

                                    #endregion

                                    #region Lấy email của danh sách người đặc biệt

                                    var listEmployeeId = context.NotifiSpecial
                                        .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId)
                                        .Select(y => y.EmployeeId).ToList();

                                    var listEmailSpecial = listAllContact.Where(x =>
                                            listEmployeeId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                        .Select(y => y.Email)
                                        .ToList();

                                    listEmailSpecial.ForEach(email =>
                                    {
                                        if (!String.IsNullOrEmpty(email))
                                        {
                                            listEmailSendTo.Add(email.Trim());
                                        }
                                    });

                                    #endregion

                                    listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                    #endregion

                                    #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                    //Gửi ngay
                                    var subject = ReplaceTokenForContent(context, typeModel, _contract,
                                        notifiSetting.EmailTitle, configEntity);
                                    var content = ReplaceTokenForContent(context, typeModel, _contract,
                                        notifiSetting.EmailContent, configEntity);

                                    #region Build nội dung thay đổi

                                    //string contentModelChange = "";

                                    ////Nếu phải kiểm tra thay đổi của model
                                    //if (checkChange)
                                    //{
                                    //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                    //}

                                    #endregion

                                    Emailer.SendEmail(context, listEmailSendTo, new List<string>(), subject, content);

                                    //Đặt lịch gửi             
                                    if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                    {
                                    }

                                    #endregion
                                }
                            }
                        }

                        #endregion
                    }
                    //Chi tiết hợp đồng bán
                    else if (typeModel == TypeModel.ContractDetail)
                    {
                        var _contract = newModel as Contract;

                        #region Kiểm tra xem cấu hình có điều kiện không?

                        var listNotifiCondition = context.NotifiSettingCondition
                            .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                        #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                        var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                        var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                        #endregion

                        //Nếu có điều kiện
                        if (listNotifiCondition.Count > 0)
                        {
                            //Do something...
                        }
                        //Nếu không có điều kiện
                        else
                        {
                            //Nếu gửi nội bộ
                            if (notifiSetting.SendInternal)
                            {
                                //Nếu gửi bằng email
                                if (notifiSetting.IsEmail)
                                {
                                    #region Lấy danh sách email cần gửi thông báo

                                    var listEmailSendTo = new List<string>();

                                    #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                    if (notifiSetting.IsParticipant)
                                    {
                                    }

                                    #endregion

                                    #region Lấy email người phê duyệt (Tạo mới thì chưa cần gửi email cho người phê duyệt)

                                    if (notifiSetting.IsApproved)
                                    {
                                        //Người phê duyệt đơn hàng là người được phân quyền dữ liệu là Quản lý
                                        var listManager = listAllEmployee.Where(x => x.IsManager)
                                            .Select(y => y.EmployeeId).ToList();
                                        var listEmailManager = listAllContact
                                            .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                            .Select(y => y.Email).ToList();

                                        listEmailManager.ForEach(emailManager =>
                                        {
                                            if (!String.IsNullOrEmpty(emailManager))
                                            {
                                                listEmailSendTo.Add(emailManager.Trim());
                                            }
                                        });
                                    }

                                    #endregion

                                    #region Lấy email người tạo

                                    if (notifiSetting.IsCreated)
                                    {
                                        //Người tạo
                                        var employeeId =
                                            context.User.FirstOrDefault(x => x.UserId == _contract.CreatedById)
                                                ?.EmployeeId;

                                        var email_created = "";

                                        if (employeeId != null)
                                        {
                                            email_created = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == employeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_created))
                                            {
                                                listEmailSendTo.Add(email_created.Trim());
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Lấy email người phụ trách (Nhân viên bán hàng)

                                    if (notifiSetting.IsPersonIncharge)
                                    {
                                        var email_seller = listAllContact.FirstOrDefault(x =>
                                            x.ObjectId == _contract.EmployeeId && x.ObjectType == "EMP")?.Email;

                                        if (!String.IsNullOrEmpty(email_seller))
                                        {
                                            listEmailSendTo.Add(email_seller.Trim());
                                        }
                                    }

                                    #endregion

                                    #region Lấy email của danh sách người đặc biệt

                                    var listEmployeeId = context.NotifiSpecial
                                        .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId)
                                        .Select(y => y.EmployeeId).ToList();

                                    var listEmailSpecial = listAllContact.Where(x =>
                                            listEmployeeId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                        .Select(y => y.Email)
                                        .ToList();

                                    listEmailSpecial.ForEach(email =>
                                    {
                                        if (!String.IsNullOrEmpty(email))
                                        {
                                            listEmailSendTo.Add(email.Trim());
                                        }
                                    });

                                    #endregion

                                    #region Lấy email người tạo bình luận

                                    if (note != null)
                                    {
                                        var _note = note as Note;

                                        var comment_employeeId =
                                            context.User.FirstOrDefault(x => x.UserId == _note.CreatedById)?.EmployeeId;

                                        var email_comment = "";

                                        if (comment_employeeId != null)
                                        {
                                            email_comment = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == comment_employeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_comment))
                                            {
                                                listEmailSendTo.Add(email_comment.Trim());
                                            }
                                        }
                                    }

                                    #endregion

                                    listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                    #endregion

                                    #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                    //Gửi ngay
                                    var subject = ReplaceTokenForContent(context, typeModel, _contract,
                                        notifiSetting.EmailTitle, configEntity, note);
                                    var content = ReplaceTokenForContent(context, typeModel, _contract,
                                        notifiSetting.EmailContent, configEntity, note);

                                    #region Build nội dung thay đổi

                                    //string contentModelChange = "";

                                    ////Nếu phải kiểm tra thay đổi của model
                                    //if (checkChange)
                                    //{
                                    //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                    //}

                                    #endregion

                                    Emailer.SendEmail(context, listEmailSendTo, new List<string>(), subject, content);

                                    //Đặt lịch gửi             
                                    if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                    {
                                    }

                                    #endregion
                                }
                            }
                        }

                        #endregion
                    }
                    //Tạo hóa đơn
                    else if (typeModel == TypeModel.BillSale)
                    {
                        var _bill = newModel as BillOfSale;

                        #region Kiểm tra xem cấu hình có điều kiện không?

                        var listNotifiCondition = context.NotifiSettingCondition
                            .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                        #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                        var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                        var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                        #endregion

                        //Nếu có điều kiện
                        if (listNotifiCondition.Count > 0)
                        {
                            //Do something...
                        }
                        //Nếu không có điều kiện
                        else
                        {
                            //Nếu gửi nội bộ
                            if (notifiSetting.SendInternal)
                            {
                                //Nếu gửi bằng email
                                if (notifiSetting.IsEmail)
                                {
                                    #region Lấy danh sách email cần gửi thông báo

                                    var listEmailSendTo = new List<string>();

                                    #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                    if (notifiSetting.IsParticipant)
                                    {
                                    }

                                    #endregion

                                    #region Lấy email người phê duyệt (Tạo mới thì chưa cần gửi email cho người phê duyệt)

                                    if (notifiSetting.IsApproved)
                                    {
                                        //Người phê duyệt đơn hàng là người được phân quyền dữ liệu là Quản lý
                                        var listManager = listAllEmployee.Where(x => x.IsManager)
                                            .Select(y => y.EmployeeId).ToList();
                                        var listEmailManager = listAllContact
                                            .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                            .Select(y => y.Email).ToList();

                                        listEmailManager.ForEach(emailManager =>
                                        {
                                            if (!String.IsNullOrEmpty(emailManager))
                                            {
                                                listEmailSendTo.Add(emailManager.Trim());
                                            }
                                        });
                                    }

                                    #endregion

                                    #region Lấy email người tạo

                                    if (notifiSetting.IsCreated)
                                    {
                                        //Người tạo
                                        var employeeId =
                                            context.User.FirstOrDefault(x => x.UserId == _bill.CreatedById)?.EmployeeId;

                                        var email_created = "";

                                        if (employeeId != null)
                                        {
                                            email_created = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == employeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_created))
                                            {
                                                listEmailSendTo.Add(email_created.Trim());
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Lấy email người phụ trách (Nhân viên bán hàng)

                                    if (notifiSetting.IsPersonIncharge)
                                    {
                                        var email_seller = listAllContact.FirstOrDefault(x =>
                                            x.ObjectId == _bill.EmployeeId && x.ObjectType == "EMP")?.Email;

                                        if (!String.IsNullOrEmpty(email_seller))
                                        {
                                            listEmailSendTo.Add(email_seller.Trim());
                                        }
                                    }

                                    #endregion

                                    #region Lấy email của danh sách người đặc biệt

                                    var listEmployeeId = context.NotifiSpecial
                                        .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId)
                                        .Select(y => y.EmployeeId).ToList();

                                    var listEmailSpecial = listAllContact.Where(x =>
                                            listEmployeeId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                        .Select(y => y.Email)
                                        .ToList();

                                    listEmailSpecial.ForEach(email =>
                                    {
                                        if (!String.IsNullOrEmpty(email))
                                        {
                                            listEmailSendTo.Add(email.Trim());
                                        }
                                    });

                                    #endregion

                                    listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                    #endregion

                                    #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                    //Gửi ngay
                                    var subject = ReplaceTokenForContent(context, typeModel, _bill,
                                        notifiSetting.EmailTitle, configEntity);
                                    var content = ReplaceTokenForContent(context, typeModel, _bill,
                                        notifiSetting.EmailContent, configEntity);

                                    #region Build nội dung thay đổi

                                    //string contentModelChange = "";

                                    ////Nếu phải kiểm tra thay đổi của model
                                    //if (checkChange)
                                    //{
                                    //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                    //}

                                    #endregion

                                    Emailer.SendEmail(context, listEmailSendTo, new List<string>(), subject, content);

                                    //Đặt lịch gửi             
                                    if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                    {

                                    }

                                    #endregion
                                }
                            }
                        }

                        #endregion
                    }
                    //Chi tiết hóa đơn
                    else if (typeModel == TypeModel.BillSaleDetail)
                    {
                        var _bill = newModel as BillOfSale;

                        #region Kiểm tra xem cấu hình có điều kiện không?

                        var listNotifiCondition = context.NotifiSettingCondition
                            .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                        #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                        var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                        var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                        #endregion

                        //Nếu có điều kiện
                        if (listNotifiCondition.Count > 0)
                        {
                            //Do something...
                        }
                        //Nếu không có điều kiện
                        else
                        {
                            //Nếu gửi nội bộ
                            if (notifiSetting.SendInternal)
                            {
                                //Nếu gửi bằng email
                                if (notifiSetting.IsEmail)
                                {
                                    #region Lấy danh sách email cần gửi thông báo

                                    var listEmailSendTo = new List<string>();

                                    #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                    if (notifiSetting.IsParticipant)
                                    {
                                    }

                                    #endregion

                                    #region Lấy email người phê duyệt (Tạo mới thì chưa cần gửi email cho người phê duyệt)

                                    if (notifiSetting.IsApproved)
                                    {
                                        //Người phê duyệt đơn hàng là người được phân quyền dữ liệu là Quản lý
                                        var listManager = listAllEmployee.Where(x => x.IsManager)
                                            .Select(y => y.EmployeeId).ToList();
                                        var listEmailManager = listAllContact
                                            .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                            .Select(y => y.Email).ToList();

                                        listEmailManager.ForEach(emailManager =>
                                        {
                                            if (!String.IsNullOrEmpty(emailManager))
                                            {
                                                listEmailSendTo.Add(emailManager.Trim());
                                            }
                                        });
                                    }

                                    #endregion

                                    #region Lấy email người tạo

                                    if (notifiSetting.IsCreated)
                                    {
                                        //Người tạo
                                        var employeeId =
                                            context.User.FirstOrDefault(x => x.UserId == _bill.CreatedById)
                                                ?.EmployeeId;

                                        var email_created = "";

                                        if (employeeId != null)
                                        {
                                            email_created = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == employeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_created))
                                            {
                                                listEmailSendTo.Add(email_created.Trim());
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Lấy email người phụ trách (Nhân viên bán hàng)

                                    if (notifiSetting.IsPersonIncharge)
                                    {
                                        var email_seller = listAllContact.FirstOrDefault(x =>
                                            x.ObjectId == _bill.EmployeeId && x.ObjectType == "EMP")?.Email;

                                        if (!String.IsNullOrEmpty(email_seller))
                                        {
                                            listEmailSendTo.Add(email_seller.Trim());
                                        }
                                    }

                                    #endregion

                                    #region Lấy email của danh sách người đặc biệt

                                    var listEmployeeId = context.NotifiSpecial
                                        .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId)
                                        .Select(y => y.EmployeeId).ToList();

                                    var listEmailSpecial = listAllContact.Where(x =>
                                            listEmployeeId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                        .Select(y => y.Email)
                                        .ToList();

                                    listEmailSpecial.ForEach(email =>
                                    {
                                        if (!String.IsNullOrEmpty(email))
                                        {
                                            listEmailSendTo.Add(email.Trim());
                                        }
                                    });

                                    #endregion

                                    #region Lấy email người tạo bình luận

                                    if (note != null)
                                    {
                                        var _note = note as Note;

                                        var comment_employeeId =
                                            context.User.FirstOrDefault(x => x.UserId == _note.CreatedById)?.EmployeeId;

                                        var email_comment = "";

                                        if (comment_employeeId != null)
                                        {
                                            email_comment = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == comment_employeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_comment))
                                            {
                                                listEmailSendTo.Add(email_comment.Trim());
                                            }
                                        }
                                    }

                                    #endregion

                                    listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                    #endregion

                                    #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                    //Gửi ngay
                                    var subject = ReplaceTokenForContent(context, typeModel, _bill,
                                        notifiSetting.EmailTitle, configEntity, note);
                                    var content = ReplaceTokenForContent(context, typeModel, _bill,
                                        notifiSetting.EmailContent, configEntity, note);

                                    #region Build nội dung thay đổi

                                    //string contentModelChange = "";

                                    ////Nếu phải kiểm tra thay đổi của model
                                    //if (checkChange)
                                    //{
                                    //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                    //}

                                    #endregion

                                    Emailer.SendEmail(context, listEmailSendTo, new List<string>(), subject, content);

                                    //Đặt lịch gửi             
                                    if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                    {
                                    }

                                    #endregion
                                }
                            }
                        }

                        #endregion
                    }
                    //Tạo đề xuất mua hàng
                    else if (typeModel == TypeModel.ProcurementRequest)
                    {
                        var _procurementRequest = newModel as ProcurementRequest;

                        #region Kiểm tra xem cấu hình có điều kiện không?

                        var listNotifiCondition = context.NotifiSettingCondition
                            .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                        #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                        var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                        var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                        #endregion

                        //Nếu có điều kiện
                        if (listNotifiCondition.Count > 0)
                        {
                            //Do something...
                        }
                        //Nếu không có điều kiện
                        else
                        {
                            //Nếu gửi nội bộ
                            if (notifiSetting.SendInternal)
                            {
                                //Nếu gửi bằng email
                                if (notifiSetting.IsEmail)
                                {
                                    #region Lấy danh sách email cần gửi thông báo

                                    var listEmailSendTo = new List<string>();

                                    #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                    if (notifiSetting.IsParticipant)
                                    {
                                    }

                                    #endregion

                                    #region Lấy email người phê duyệt (Tạo mới thì chưa cần gửi email cho người phê duyệt)

                                    if (notifiSetting.IsApproved)
                                    {
                                        //Người phê duyệt đơn hàng là người được phân quyền dữ liệu là Quản lý
                                        var listManager = listAllEmployee.Where(x => x.IsManager)
                                            .Select(y => y.EmployeeId).ToList();
                                        var listEmailManager = listAllContact
                                            .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                            .Select(y => y.Email).ToList();

                                        listEmailManager.ForEach(emailManager =>
                                        {
                                            if (!String.IsNullOrEmpty(emailManager))
                                            {
                                                listEmailSendTo.Add(emailManager.Trim());
                                            }
                                        });
                                    }

                                    #endregion

                                    #region Lấy email người tạo

                                    if (notifiSetting.IsCreated)
                                    {
                                        //Người tạo
                                        var employeeId =
                                            context.User.FirstOrDefault(x => x.UserId == _procurementRequest.CreatedById)?.EmployeeId;

                                        var email_created = "";

                                        if (employeeId != null)
                                        {
                                            email_created = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == employeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_created))
                                            {
                                                listEmailSendTo.Add(email_created.Trim());
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Lấy email người phụ trách (Nhân viên bán hàng)

                                    if (notifiSetting.IsPersonIncharge)
                                    {
                                        var email_seller = listAllContact.FirstOrDefault(x =>
                                            x.ObjectId == _procurementRequest.RequestEmployeeId && x.ObjectType == "EMP")?.Email;

                                        if (!String.IsNullOrEmpty(email_seller))
                                        {
                                            listEmailSendTo.Add(email_seller.Trim());
                                        }
                                    }

                                    #endregion

                                    #region Lấy email của danh sách người đặc biệt

                                    var listEmployeeId = context.NotifiSpecial
                                        .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId)
                                        .Select(y => y.EmployeeId).ToList();

                                    var listEmailSpecial = listAllContact.Where(x =>
                                            listEmployeeId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                        .Select(y => y.Email)
                                        .ToList();

                                    listEmailSpecial.ForEach(email =>
                                    {
                                        if (!String.IsNullOrEmpty(email))
                                        {
                                            listEmailSendTo.Add(email.Trim());
                                        }
                                    });

                                    #endregion

                                    listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                    #endregion

                                    #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                    //Gửi ngay
                                    var subject = ReplaceTokenForContent(context, typeModel, _procurementRequest,
                                        notifiSetting.EmailTitle, configEntity);
                                    var content = ReplaceTokenForContent(context, typeModel, _procurementRequest,
                                        notifiSetting.EmailContent, configEntity);

                                    #region Build nội dung thay đổi

                                    //string contentModelChange = "";

                                    ////Nếu phải kiểm tra thay đổi của model
                                    //if (checkChange)
                                    //{
                                    //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                    //}

                                    #endregion

                                    Emailer.SendEmail(context, listEmailSendTo, new List<string>(), subject, content);

                                    //Đặt lịch gửi             
                                    if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                    {
                                    }

                                    #endregion
                                }
                            }
                        }

                        #endregion
                    }
                    //Chi tiết đề xuất mua hàng
                    else if (typeModel == TypeModel.ProcurementRequestDetail)
                    {
                        var _procurementRequest = newModel as ProcurementRequest;

                        #region Kiểm tra xem cấu hình có điều kiện không?

                        var listNotifiCondition = context.NotifiSettingCondition
                            .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                        #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                        var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                        var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                        #endregion

                        //Nếu có điều kiện
                        if (listNotifiCondition.Count > 0)
                        {
                            //Do something...
                        }
                        //Nếu không có điều kiện
                        else
                        {
                            //Nếu gửi nội bộ
                            if (notifiSetting.SendInternal)
                            {
                                //Nếu gửi bằng email
                                if (notifiSetting.IsEmail)
                                {
                                    #region Lấy danh sách email cần gửi thông báo

                                    var listEmailSendTo = new List<string>();

                                    #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                    if (notifiSetting.IsParticipant)
                                    {
                                    }

                                    #endregion

                                    #region Lấy email người phê duyệt (Tạo mới thì chưa cần gửi email cho người phê duyệt)

                                    if (notifiSetting.IsApproved)
                                    {
                                        //Người phê duyệt đơn hàng là người được phân quyền dữ liệu là Quản lý
                                        var listManager = listAllEmployee.Where(x => x.IsManager)
                                            .Select(y => y.EmployeeId).ToList();
                                        var listEmailManager = listAllContact
                                            .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                            .Select(y => y.Email).ToList();

                                        listEmailManager.ForEach(emailManager =>
                                        {
                                            if (!String.IsNullOrEmpty(emailManager))
                                            {
                                                listEmailSendTo.Add(emailManager.Trim());
                                            }
                                        });
                                    }

                                    #endregion

                                    #region Lấy email người tạo

                                    if (notifiSetting.IsCreated)
                                    {
                                        //Người tạo
                                        var employeeId =
                                            context.User.FirstOrDefault(x => x.UserId == _procurementRequest.CreatedById)
                                                ?.EmployeeId;

                                        var email_created = "";

                                        if (employeeId != null)
                                        {
                                            email_created = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == employeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_created))
                                            {
                                                listEmailSendTo.Add(email_created.Trim());
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Lấy email người phụ trách (Nhân viên bán hàng)

                                    if (notifiSetting.IsPersonIncharge)
                                    {
                                        var email_seller = listAllContact.FirstOrDefault(x =>
                                            x.ObjectId == _procurementRequest.RequestEmployeeId && x.ObjectType == "EMP")?.Email;

                                        if (!String.IsNullOrEmpty(email_seller))
                                        {
                                            listEmailSendTo.Add(email_seller.Trim());
                                        }
                                    }

                                    #endregion

                                    #region Lấy email của danh sách người đặc biệt

                                    var listEmployeeId = context.NotifiSpecial
                                        .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId)
                                        .Select(y => y.EmployeeId).ToList();

                                    var listEmailSpecial = listAllContact.Where(x =>
                                            listEmployeeId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                        .Select(y => y.Email)
                                        .ToList();

                                    listEmailSpecial.ForEach(email =>
                                    {
                                        if (!String.IsNullOrEmpty(email))
                                        {
                                            listEmailSendTo.Add(email.Trim());
                                        }
                                    });

                                    #endregion

                                    #region Lấy email người tạo bình luận

                                    if (note != null)
                                    {
                                        var _note = note as Note;

                                        var comment_employeeId =
                                            context.User.FirstOrDefault(x => x.UserId == _note.CreatedById)?.EmployeeId;

                                        var email_comment = "";

                                        if (comment_employeeId != null)
                                        {
                                            email_comment = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == comment_employeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_comment))
                                            {
                                                listEmailSendTo.Add(email_comment.Trim());
                                            }
                                        }
                                    }

                                    #endregion

                                    listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                    #endregion

                                    #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                    //Gửi ngay
                                    var subject = ReplaceTokenForContent(context, typeModel, _procurementRequest,
                                        notifiSetting.EmailTitle, configEntity, note);
                                    var content = ReplaceTokenForContent(context, typeModel, _procurementRequest,
                                        notifiSetting.EmailContent, configEntity, note);

                                    #region Build nội dung thay đổi

                                    //string contentModelChange = "";

                                    ////Nếu phải kiểm tra thay đổi của model
                                    //if (checkChange)
                                    //{
                                    //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                    //}

                                    #endregion

                                    Emailer.SendEmail(context, listEmailSendTo, new List<string>(), subject, content);

                                    //Đặt lịch gửi             
                                    if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                    {
                                    }

                                    #endregion
                                }
                            }
                        }

                        #endregion
                    }
                    //Tạo khách hàng tiềm năng
                    else if (typeModel == TypeModel.PotentialCustomer)
                    {
                        var _customer = newModel as Customer;

                        #region Kiểm tra xem cấu hình có điều kiện không?

                        var listNotifiCondition = context.NotifiSettingCondition
                            .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                        #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                        var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                        var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                        #endregion

                        //Nếu có điều kiện
                        if (listNotifiCondition.Count > 0)
                        {
                            //Do something...
                        }
                        //Nếu không có điều kiện
                        else
                        {
                            //Nếu gửi nội bộ
                            if (notifiSetting.SendInternal)
                            {
                                //Nếu gửi bằng email
                                if (notifiSetting.IsEmail)
                                {
                                    #region Lấy danh sách email cần gửi thông báo

                                    var listEmailSendTo = new List<string>();

                                    #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                    if (notifiSetting.IsParticipant)
                                    {
                                    }

                                    #endregion

                                    #region Lấy email người phê duyệt (Khách hàng tiềm năng không có action phê duyệt)

                                    if (notifiSetting.IsApproved)
                                    {
                                        //Người phê duyệt đơn hàng là người được phân quyền dữ liệu là Quản lý
                                        //var listManager = listAllEmployee.Where(x => x.IsManager)
                                        //    .Select(y => y.EmployeeId).ToList();
                                        //var listEmailManager = listAllContact
                                        //    .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                        //    .Select(y => y.Email).ToList();

                                        //listEmailManager.ForEach(emailManager =>
                                        //{
                                        //    if (!String.IsNullOrEmpty(emailManager))
                                        //    {
                                        //        listEmailSendTo.Add(emailManager.Trim());
                                        //    }
                                        //});
                                    }

                                    #endregion

                                    #region Lấy email người tạo

                                    if (notifiSetting.IsCreated)
                                    {
                                        //Người tạo
                                        var employeeId =
                                            context.User.FirstOrDefault(x => x.UserId == _customer.CreatedById)?.EmployeeId;

                                        var email_created = "";

                                        if (employeeId != null)
                                        {
                                            email_created = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == employeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_created))
                                            {
                                                listEmailSendTo.Add(email_created.Trim());
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Lấy email người phụ trách (Nhân viên bán hàng)

                                    if (notifiSetting.IsPersonIncharge)
                                    {
                                        var email_seller = listAllContact.FirstOrDefault(x =>
                                            x.ObjectId == _customer.PersonInChargeId && x.ObjectType == "EMP")?.Email;

                                        if (!String.IsNullOrEmpty(email_seller))
                                        {
                                            listEmailSendTo.Add(email_seller.Trim());
                                        }
                                    }

                                    #endregion

                                    #region Lấy email của danh sách người đặc biệt

                                    var listEmployeeId = context.NotifiSpecial
                                        .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId)
                                        .Select(y => y.EmployeeId).ToList();

                                    var listEmailSpecial = listAllContact.Where(x =>
                                            listEmployeeId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                        .Select(y => y.Email)
                                        .ToList();

                                    listEmailSpecial.ForEach(email =>
                                    {
                                        if (!String.IsNullOrEmpty(email))
                                        {
                                            listEmailSendTo.Add(email.Trim());
                                        }
                                    });

                                    #endregion

                                    listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                    #endregion

                                    #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                    //Gửi ngay
                                    var subject = ReplaceTokenForContent(context, typeModel, _customer,
                                        notifiSetting.EmailTitle, configEntity);
                                    var content = ReplaceTokenForContent(context, typeModel, _customer,
                                        notifiSetting.EmailContent, configEntity);

                                    #region Build nội dung thay đổi

                                    //string contentModelChange = "";

                                    ////Nếu phải kiểm tra thay đổi của model
                                    //if (checkChange)
                                    //{
                                    //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                    //}

                                    #endregion

                                    Emailer.SendEmail(context, listEmailSendTo, new List<string>(), subject, content);

                                    //Đặt lịch gửi 
                                    if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                    {

                                    }

                                    #endregion
                                }
                            }
                        }

                        #endregion
                    }
                    //Chi tiết khách hàng tiềm năng
                    else if (typeModel == TypeModel.PotentialCustomerDetail)
                    {
                        var _customer = newModel as Customer;

                        #region Kiểm tra xem cấu hình có điều kiện không?

                        var listNotifiCondition = context.NotifiSettingCondition
                            .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                        #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                        var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                        var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                        #endregion

                        //Nếu có điều kiện
                        if (listNotifiCondition.Count > 0)
                        {
                            //Do something...
                        }
                        //Nếu không có điều kiện
                        else
                        {
                            //Nếu gửi nội bộ
                            if (notifiSetting.SendInternal)
                            {
                                //Nếu gửi bằng email
                                if (notifiSetting.IsEmail)
                                {
                                    #region Lấy danh sách email cần gửi thông báo

                                    var listEmailSendTo = new List<string>();

                                    #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                    if (notifiSetting.IsParticipant)
                                    {
                                    }

                                    #endregion

                                    #region Lấy email người phê duyệt (Khách hàng tiềm năng không có action phê duyệt)

                                    if (notifiSetting.IsApproved)
                                    {
                                        //Người phê duyệt đơn hàng là người được phân quyền dữ liệu là Quản lý
                                        //var listManager = listAllEmployee.Where(x => x.IsManager)
                                        //    .Select(y => y.EmployeeId).ToList();
                                        //var listEmailManager = listAllContact
                                        //    .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                        //    .Select(y => y.Email).ToList();

                                        //listEmailManager.ForEach(emailManager =>
                                        //{
                                        //    if (!String.IsNullOrEmpty(emailManager))
                                        //    {
                                        //        listEmailSendTo.Add(emailManager.Trim());
                                        //    }
                                        //});
                                    }

                                    #endregion

                                    #region Lấy email người tạo

                                    if (notifiSetting.IsCreated)
                                    {
                                        //Người tạo
                                        var employeeId =
                                            context.User.FirstOrDefault(x => x.UserId == _customer.CreatedById)
                                                ?.EmployeeId;

                                        var email_created = "";

                                        if (employeeId != null)
                                        {
                                            email_created = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == employeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_created))
                                            {
                                                listEmailSendTo.Add(email_created.Trim());
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Lấy email người phụ trách (Nhân viên bán hàng)

                                    if (notifiSetting.IsPersonIncharge)
                                    {
                                        var email_seller = listAllContact.FirstOrDefault(x =>
                                            x.ObjectId == _customer.PersonInChargeId && x.ObjectType == "EMP")?.Email;

                                        if (!String.IsNullOrEmpty(email_seller))
                                        {
                                            listEmailSendTo.Add(email_seller.Trim());
                                        }
                                    }

                                    #endregion

                                    #region Lấy email của danh sách người đặc biệt

                                    var listEmployeeId = context.NotifiSpecial
                                        .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId)
                                        .Select(y => y.EmployeeId).ToList();

                                    var listEmailSpecial = listAllContact.Where(x =>
                                            listEmployeeId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                        .Select(y => y.Email)
                                        .ToList();

                                    listEmailSpecial.ForEach(email =>
                                    {
                                        if (!String.IsNullOrEmpty(email))
                                        {
                                            listEmailSendTo.Add(email.Trim());
                                        }
                                    });

                                    #endregion

                                    #region Lấy email người tạo bình luận

                                    if (note != null)
                                    {
                                        var _note = note as Note;

                                        var comment_employeeId =
                                            context.User.FirstOrDefault(x => x.UserId == _note.CreatedById)?.EmployeeId;

                                        var email_comment = "";

                                        if (comment_employeeId != null)
                                        {
                                            email_comment = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == comment_employeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_comment))
                                            {
                                                listEmailSendTo.Add(email_comment.Trim());
                                            }
                                        }
                                    }

                                    #endregion

                                    listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                    #endregion

                                    #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                    //Gửi ngay
                                    var subject = ReplaceTokenForContent(context, typeModel, _customer,
                                        notifiSetting.EmailTitle, configEntity, note);
                                    var content = ReplaceTokenForContent(context, typeModel, _customer,
                                        notifiSetting.EmailContent, configEntity, note);

                                    #region Build nội dung thay đổi

                                    //string contentModelChange = "";

                                    ////Nếu phải kiểm tra thay đổi của model
                                    //if (checkChange)
                                    //{
                                    //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                    //}

                                    #endregion

                                    Emailer.SendEmail(context, listEmailSendTo, new List<string>(), subject, content);

                                    //Đặt lịch gửi             
                                    if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                    {

                                    }

                                    #endregion
                                }
                            }
                        }

                        #endregion
                    }
                    //Tạo khách hàng
                    else if (typeModel == TypeModel.Customer)
                    {
                        var _customer = newModel as Customer;

                        #region Kiểm tra xem cấu hình có điều kiện không?

                        var listNotifiCondition = context.NotifiSettingCondition
                            .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                        #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                        var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                        #endregion

                        //Nếu có điều kiện
                        if (listNotifiCondition.Count > 0)
                        {
                            //Do something...
                        }
                        //Nếu không có điều kiện
                        else
                        {
                            //Nếu gửi nội bộ
                            if (notifiSetting.SendInternal)
                            {
                                //Nếu gửi bằng email
                                if (notifiSetting.IsEmail)
                                {
                                    #region Lấy danh sách email cần gửi thông báo

                                    var listEmailSendTo = new List<string>();

                                    #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                    if (notifiSetting.IsParticipant)
                                    {
                                    }

                                    #endregion

                                    #region Lấy email người phê duyệt (Tạo mới thì chưa cần gửi email cho người phê duyệt)

                                    //if (notifiSetting.IsApproved)
                                    //{
                                    //    //Người phê duyệt đơn hàng là người được phân quyền dữ liệu là Quản lý
                                    //var listManager = listAllEmployee.Where(x => x.IsManager)
                                    //    .Select(y => y.EmployeeId).ToList();
                                    //var listEmailManager = listAllContact
                                    //    .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                    //    .Select(y => y.Email).ToList();

                                    //    listEmailManager.ForEach(emailManager =>
                                    //    {
                                    //        if (!String.IsNullOrEmpty(emailManager))
                                    //        {
                                    //            listEmailSendTo.Add(emailManager.Trim());
                                    //        }
                                    //    });
                                    //}

                                    #endregion

                                    #region Lấy email người tạo

                                    if (notifiSetting.IsCreated)
                                    {
                                        //Người tạo
                                        var employeeId =
                                            context.User.FirstOrDefault(x => x.UserId == _customer.CreatedById)?.EmployeeId;

                                        var email_created = "";

                                        if (employeeId != null)
                                        {
                                            email_created = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == employeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_created))
                                            {
                                                listEmailSendTo.Add(email_created.Trim());
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Lấy email người phụ trách (Nhân viên bán hàng)

                                    if (notifiSetting.IsPersonIncharge)
                                    {
                                        var email_seller = listAllContact.FirstOrDefault(x =>
                                            x.ObjectId == _customer.PersonInChargeId && x.ObjectType == "EMP")?.Email;

                                        if (!String.IsNullOrEmpty(email_seller))
                                        {
                                            listEmailSendTo.Add(email_seller.Trim());
                                        }
                                    }

                                    #endregion

                                    #region Lấy email của danh sách người đặc biệt

                                    var listEmployeeId = context.NotifiSpecial
                                        .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId)
                                        .Select(y => y.EmployeeId).ToList();

                                    var listEmailSpecial = listAllContact.Where(x =>
                                            listEmployeeId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                        .Select(y => y.Email)
                                        .ToList();

                                    listEmailSpecial.ForEach(email =>
                                    {
                                        if (!String.IsNullOrEmpty(email))
                                        {
                                            listEmailSendTo.Add(email.Trim());
                                        }
                                    });

                                    #endregion

                                    listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                    #endregion

                                    #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                    //Gửi ngay
                                    var subject = ReplaceTokenForContent(context, typeModel, _customer,
                                        notifiSetting.EmailTitle, configEntity);
                                    var content = ReplaceTokenForContent(context, typeModel, _customer,
                                        notifiSetting.EmailContent, configEntity);

                                    #region Build nội dung thay đổi

                                    //string contentModelChange = "";

                                    ////Nếu phải kiểm tra thay đổi của model
                                    //if (checkChange)
                                    //{
                                    //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                    //}

                                    #endregion

                                    Emailer.SendEmail(context, listEmailSendTo, new List<string>(), subject, content);

                                    //Đặt lịch gửi 
                                    if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                    {

                                    }

                                    #endregion
                                }
                            }
                        }

                        #endregion
                    }
                    //Chi tiết khách hàng
                    else if (typeModel == TypeModel.CustomerDetail)
                    {
                        var _customer = newModel as Customer;

                        #region Kiểm tra xem cấu hình có điều kiện không?

                        var listNotifiCondition = context.NotifiSettingCondition
                            .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                        #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                        var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                        var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                        #endregion

                        //Nếu có điều kiện
                        if (listNotifiCondition.Count > 0)
                        {
                            //Do something...
                        }
                        //Nếu không có điều kiện
                        else
                        {
                            //Nếu gửi nội bộ
                            if (notifiSetting.SendInternal)
                            {
                                //Nếu gửi bằng email
                                if (notifiSetting.IsEmail)
                                {
                                    #region Lấy danh sách email cần gửi thông báo

                                    var listEmailSendTo = new List<string>();

                                    #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                    if (notifiSetting.IsParticipant)
                                    {
                                    }

                                    #endregion

                                    #region Lấy email người phê duyệt (Tạo mới thì chưa cần gửi email cho người phê duyệt)

                                    if (notifiSetting.IsApproved)
                                    {
                                        ////Người phê duyệt đơn hàng là người được phân quyền dữ liệu là Quản lý
                                        //var listManager = listAllEmployee.Where(x => x.IsManager)
                                        //    .Select(y => y.EmployeeId).ToList();
                                        //var listEmailManager = listAllContact
                                        //    .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                        //    .Select(y => y.Email).ToList();

                                        //listEmailManager.ForEach(emailManager =>
                                        //{
                                        //    if (!String.IsNullOrEmpty(emailManager))
                                        //    {
                                        //        listEmailSendTo.Add(emailManager.Trim());
                                        //    }
                                        //});
                                    }

                                    #endregion

                                    #region Lấy email người tạo

                                    if (notifiSetting.IsCreated)
                                    {
                                        //Người tạo
                                        var employeeId =
                                            context.User.FirstOrDefault(x => x.UserId == _customer.CreatedById)
                                                ?.EmployeeId;

                                        var email_created = "";

                                        if (employeeId != null)
                                        {
                                            email_created = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == employeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_created))
                                            {
                                                listEmailSendTo.Add(email_created.Trim());
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Lấy email người phụ trách (Nhân viên bán hàng)

                                    if (notifiSetting.IsPersonIncharge)
                                    {
                                        var email_seller = listAllContact.FirstOrDefault(x =>
                                            x.ObjectId == _customer.PersonInChargeId && x.ObjectType == "EMP")?.Email;

                                        if (!String.IsNullOrEmpty(email_seller))
                                        {
                                            listEmailSendTo.Add(email_seller.Trim());
                                        }
                                    }

                                    #endregion

                                    #region Lấy email của danh sách người đặc biệt

                                    var listEmployeeId = context.NotifiSpecial
                                        .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId)
                                        .Select(y => y.EmployeeId).ToList();

                                    var listEmailSpecial = listAllContact.Where(x =>
                                            listEmployeeId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                        .Select(y => y.Email)
                                        .ToList();

                                    listEmailSpecial.ForEach(email =>
                                    {
                                        if (!String.IsNullOrEmpty(email))
                                        {
                                            listEmailSendTo.Add(email.Trim());
                                        }
                                    });

                                    #endregion

                                    #region Lấy email người tạo bình luận

                                    if (note != null)
                                    {
                                        var _note = note as Note;

                                        var comment_employeeId =
                                            context.User.FirstOrDefault(x => x.UserId == _note.CreatedById)?.EmployeeId;

                                        var email_comment = "";

                                        if (comment_employeeId != null)
                                        {
                                            email_comment = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == comment_employeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_comment))
                                            {
                                                listEmailSendTo.Add(email_comment.Trim());
                                            }
                                        }
                                    }

                                    #endregion

                                    listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                    #endregion

                                    #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                    //Gửi ngay
                                    var subject = ReplaceTokenForContent(context, typeModel, _customer,
                                        notifiSetting.EmailTitle, configEntity, note);
                                    var content = ReplaceTokenForContent(context, typeModel, _customer,
                                        notifiSetting.EmailContent, configEntity, note);

                                    #region Build nội dung thay đổi

                                    //string contentModelChange = "";

                                    ////Nếu phải kiểm tra thay đổi của model
                                    //if (checkChange)
                                    //{
                                    //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                    //}

                                    #endregion

                                    Emailer.SendEmail(context, listEmailSendTo, new List<string>(), subject, content);

                                    //Đặt lịch gửi             
                                    if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                    {

                                    }

                                    #endregion
                                }
                            }
                        }

                        #endregion
                    }
                    //Tạo đơn hàng mua
                    else if (typeModel == TypeModel.VendorOrder)
                    {
                        var _vendorOrder = newModel as VendorOrder;

                        #region Kiểm tra xem cấu hình có điều kiện không?

                        var listNotifiCondition = context.NotifiSettingCondition
                            .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                        #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                        var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                        var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                        #endregion

                        //Nếu có điều kiện
                        if (listNotifiCondition.Count > 0)
                        {
                            //Do something...
                        }
                        //Nếu không có điều kiện
                        else
                        {
                            //Nếu gửi nội bộ
                            if (notifiSetting.SendInternal)
                            {
                                //Nếu gửi bằng email
                                if (notifiSetting.IsEmail)
                                {
                                    #region Lấy danh sách email cần gửi thông báo

                                    var listEmailSendTo = new List<string>();

                                    #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                    if (notifiSetting.IsParticipant)
                                    {
                                    }

                                    #endregion

                                    #region Lấy email người phê duyệt (Tạo mới thì chưa cần gửi email cho người phê duyệt)

                                    if (notifiSetting.IsApproved)
                                    {
                                        //Người phê duyệt đơn hàng là người được phân quyền dữ liệu là Quản lý
                                        var listManager = listAllEmployee.Where(x => x.IsManager)
                                            .Select(y => y.EmployeeId).ToList();
                                        var listEmailManager = listAllContact
                                            .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                            .Select(y => y.Email).ToList();

                                        listEmailManager.ForEach(emailManager =>
                                        {
                                            if (!String.IsNullOrEmpty(emailManager))
                                            {
                                                listEmailSendTo.Add(emailManager.Trim());
                                            }
                                        });
                                    }

                                    #endregion

                                    #region Lấy email người tạo

                                    if (notifiSetting.IsCreated)
                                    {
                                        //Người tạo
                                        var employeeId =
                                            context.User.FirstOrDefault(x => x.UserId == _vendorOrder.CreatedById)?.EmployeeId;

                                        var email_created = "";

                                        if (employeeId != null)
                                        {
                                            email_created = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == employeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_created))
                                            {
                                                listEmailSendTo.Add(email_created.Trim());
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Lấy email người phụ trách (Nhân viên bán hàng - đơn hàng mua không có người phụ trách)

                                    if (notifiSetting.IsPersonIncharge)
                                    {
                                        //var email_seller = listAllContact.FirstOrDefault(x =>
                                        //    x.ObjectId == _vendorOrder.PersonInChargeId && x.ObjectType == "EMP")?.Email;

                                        //if (!String.IsNullOrEmpty(email_seller))
                                        //{
                                        //    listEmailSendTo.Add(email_seller.Trim());
                                        //}
                                    }

                                    #endregion

                                    #region Lấy email của danh sách người đặc biệt

                                    var listEmployeeId = context.NotifiSpecial
                                        .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId)
                                        .Select(y => y.EmployeeId).ToList();

                                    var listEmailSpecial = listAllContact.Where(x =>
                                            listEmployeeId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                        .Select(y => y.Email)
                                        .ToList();

                                    listEmailSpecial.ForEach(email =>
                                    {
                                        if (!String.IsNullOrEmpty(email))
                                        {
                                            listEmailSendTo.Add(email.Trim());
                                        }
                                    });

                                    #endregion

                                    listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                    #endregion

                                    #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                    //Gửi ngay
                                    var subject = ReplaceTokenForContent(context, typeModel, _vendorOrder,
                                        notifiSetting.EmailTitle, configEntity);
                                    var content = ReplaceTokenForContent(context, typeModel, _vendorOrder,
                                        notifiSetting.EmailContent, configEntity);

                                    #region Build nội dung thay đổi

                                    //string contentModelChange = "";

                                    ////Nếu phải kiểm tra thay đổi của model
                                    //if (checkChange)
                                    //{
                                    //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                    //}

                                    #endregion

                                    Emailer.SendEmail(context, listEmailSendTo, new List<string>(), subject, content);

                                    //Đặt lịch gửi 
                                    if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                    {

                                    }

                                    #endregion
                                }
                            }
                        }

                        #endregion
                    }
                    //Chi tiết đơn hàng mua
                    else if (typeModel == TypeModel.VendorOrderDetail)
                    {
                        var _vendorOrder = newModel as VendorOrder;

                        #region Kiểm tra xem cấu hình có điều kiện không?

                        var listNotifiCondition = context.NotifiSettingCondition
                            .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                        #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                        var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                        var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                        #endregion

                        //Nếu có điều kiện
                        if (listNotifiCondition.Count > 0)
                        {
                            //Do something...
                        }
                        //Nếu không có điều kiện
                        else
                        {
                            //Nếu gửi nội bộ
                            if (notifiSetting.SendInternal)
                            {
                                //Nếu gửi bằng email
                                if (notifiSetting.IsEmail)
                                {
                                    #region Lấy danh sách email cần gửi thông báo

                                    var listEmailSendTo = new List<string>();

                                    #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                    if (notifiSetting.IsParticipant)
                                    {
                                    }

                                    #endregion

                                    #region Lấy email người phê duyệt (Tạo mới thì chưa cần gửi email cho người phê duyệt)

                                    if (notifiSetting.IsApproved)
                                    {
                                        //Người phê duyệt đơn hàng là người được phân quyền dữ liệu là Quản lý
                                        var listManager = listAllEmployee.Where(x => x.IsManager)
                                            .Select(y => y.EmployeeId).ToList();
                                        var listEmailManager = listAllContact
                                            .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                            .Select(y => y.Email).ToList();

                                        listEmailManager.ForEach(emailManager =>
                                        {
                                            if (!String.IsNullOrEmpty(emailManager))
                                            {
                                                listEmailSendTo.Add(emailManager.Trim());
                                            }
                                        });
                                    }

                                    #endregion

                                    #region Lấy email người tạo

                                    if (notifiSetting.IsCreated)
                                    {
                                        //Người tạo
                                        var employeeId =
                                            context.User.FirstOrDefault(x => x.UserId == _vendorOrder.CreatedById)
                                                ?.EmployeeId;

                                        var email_created = "";

                                        if (employeeId != null)
                                        {
                                            email_created = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == employeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_created))
                                            {
                                                listEmailSendTo.Add(email_created.Trim());
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Lấy email người phụ trách (Nhân viên bán hàng - đơn hàng mua không có người phụ trách)

                                    if (notifiSetting.IsPersonIncharge)
                                    {
                                        //var email_seller = listAllContact.FirstOrDefault(x =>
                                        //    x.ObjectId == _vendorOrder.PersonInChargeId && x.ObjectType == "EMP")?.Email;

                                        //if (!String.IsNullOrEmpty(email_seller))
                                        //{
                                        //    listEmailSendTo.Add(email_seller.Trim());
                                        //}
                                    }

                                    #endregion

                                    #region Lấy email của danh sách người đặc biệt

                                    var listEmployeeId = context.NotifiSpecial
                                        .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId)
                                        .Select(y => y.EmployeeId).ToList();

                                    var listEmailSpecial = listAllContact.Where(x =>
                                            listEmployeeId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                        .Select(y => y.Email)
                                        .ToList();

                                    listEmailSpecial.ForEach(email =>
                                    {
                                        if (!String.IsNullOrEmpty(email))
                                        {
                                            listEmailSendTo.Add(email.Trim());
                                        }
                                    });

                                    #endregion

                                    #region Lấy email người tạo bình luận

                                    if (note != null)
                                    {
                                        var _note = note as Note;

                                        var comment_employeeId =
                                            context.User.FirstOrDefault(x => x.UserId == _note.CreatedById)?.EmployeeId;

                                        var email_comment = "";

                                        if (comment_employeeId != null)
                                        {
                                            email_comment = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == comment_employeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_comment))
                                            {
                                                listEmailSendTo.Add(email_comment.Trim());
                                            }
                                        }
                                    }

                                    #endregion

                                    listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                    #endregion

                                    #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                    //Gửi ngay
                                    var subject = ReplaceTokenForContent(context, typeModel, _vendorOrder,
                                        notifiSetting.EmailTitle, configEntity, note);
                                    var content = ReplaceTokenForContent(context, typeModel, _vendorOrder,
                                        notifiSetting.EmailContent, configEntity, note);

                                    #region Build nội dung thay đổi

                                    //string contentModelChange = "";

                                    ////Nếu phải kiểm tra thay đổi của model
                                    //if (checkChange)
                                    //{
                                    //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                    //}

                                    #endregion

                                    Emailer.SendEmail(context, listEmailSendTo, new List<string>(), subject, content);

                                    //Đặt lịch gửi             
                                    if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                    {

                                    }

                                    #endregion
                                }
                            }
                        }

                        #endregion
                    }
                }
            }
        }

        private static string ContentModelChange(TNTN8Context context, object oldModel, object newModel, string typeModel)
        {
            string result = "";

            if (typeModel == TypeModel.CustomerOrder)
            {
                var _oldModel = oldModel as CustomerOrder;
                var _newModel = newModel as CustomerOrder;

                #region Kiểm tra thay đổi trên object

                CompareLogic compareLogic = new CompareLogic();
                var listIgnorField = new List<string> { "OrderId", "CreatedDate", "CreatedById" };
                compareLogic.Config.MembersToIgnore = listIgnorField;
                ComparisonResult compare = compareLogic.Compare(_oldModel, _newModel);

                //Nếu có thay đổi
                if (compare.AreEqual)
                {

                }

                #endregion

                #region Kiểm tra thay đổi trên list object detail

                //var _newListDetail =
                //    context.CustomerOrderDetail.Where(co => co.OrderId == _newModel.OrderId).ToList();
                //var listIgnorFieldDetail = new List<string> { "OrderDetailId", "CreatedDate", "CreatedById" };
                //compareLogic.Config.MembersToIgnore = listIgnorFieldDetail;
                //ComparisonResult detailCompare = compareLogic.Compare(_oldListDetail, _newListDetail);

                #endregion
            }
            else if (typeModel == TypeModel.Quote)
            {
                var _oldModel = oldModel as Quote;
                var _newModel = newModel as Quote;

                #region Kiểm tra thay đổi trên object

                CompareLogic compareLogic = new CompareLogic();
                var listIgnorField = new List<string> { "QuoteId", "CreatedDate", "CreatedById" };
                compareLogic.Config.MembersToIgnore = listIgnorField;
                ComparisonResult compare = compareLogic.Compare(_oldModel, _newModel);

                //Nếu có thay đổi
                if (compare.AreEqual)
                {

                }

                #endregion

                #region Kiểm tra thay đổi trên list object detail

                //var _newListDetail =
                //    context.QuoteDetail.Where(co => co.QuoteId == _newModel.QuoteId).ToList();
                //var listIgnorFieldDetail = new List<string> { "QuoteDetailId", "CreatedDate", "CreatedById" };
                //compareLogic.Config.MembersToIgnore = listIgnorFieldDetail;
                //ComparisonResult detailCompare = compareLogic.Compare(_oldListDetail, _newListDetail);

                #endregion
            }

            return result;
        }

        //param_1 là giá trị hiện tại của object, param_2 là giá trị trong setting
        private static bool CheckStringCondition(string param_1, string param_2, int typeCondition)
        {
            bool result = true;

            switch (typeCondition)
            {
                case TypeCondition.EQUAL:
                    if (param_1 != param_2) result = false;
                    break;
                case TypeCondition.NOT_EQUAL:
                    if (param_1 == param_2) result = false;
                    break;
                case TypeCondition.CONTAINS:
                    if (!param_1.Contains(param_2)) result = false;
                    break;
                case TypeCondition.NOT_CONTAINS:
                    if (param_1.Contains(param_2)) result = false;
                    break;
                case TypeCondition.NULL:
                    if (!String.IsNullOrEmpty(param_1)) result = false;
                    break;
                case TypeCondition.NOT_NULL:
                    if (String.IsNullOrEmpty(param_1)) result = false;
                    break;
            }

            return result;
        }

        //param_1 là giá trị hiện tại của object, param_2 là giá trị trong setting
        private static bool CheckListStringCondition(List<string> list_param_1, string param_2, int typeCondition)
        {
            bool result = true;

            switch (typeCondition)
            {
                case TypeCondition.EQUAL:
                    if (!list_param_1.Contains(param_2)) result = false;
                    break;
                case TypeCondition.NOT_EQUAL:
                    if (list_param_1.Contains(param_2)) result = false;
                    break;
                case TypeCondition.CONTAINS:
                    var count = list_param_1.Count;
                    var temp = 0;
                    list_param_1.ForEach(item =>
                    {
                        if (!item.Contains(param_2)) temp++;
                    });

                    if (temp == count) result = false;
                    break;
                case TypeCondition.NOT_CONTAINS:
                    list_param_1.ForEach(item =>
                    {
                        if (item.Contains(param_2)) result = false;
                    });
                    break;
            }

            return result;
        }

        //param_1 là giá trị hiện tại của object, param_2 là giá trị trong setting
        private static bool CheckNumberCondition(decimal? param_1, decimal param_2, int typeCondition)
        {
            bool result = true;

            switch (typeCondition)
            {
                case TypeCondition.EQUAL:
                    if (param_1 != param_2) result = false;
                    break;
                case TypeCondition.NOT_EQUAL:
                    if (param_1 == param_2) result = false;
                    break;
                case TypeCondition.NULL:
                    if (param_1 != null) result = false;
                    break;
                case TypeCondition.NOT_NULL:
                    if (param_1 == null) result = false;
                    break;
            }

            return result;
        }

        private static string ReplaceTokenForContent(TNTN8Context context, string typeModel, object model,
            string emailContent, List<SystemParameter> configEntity, object note = null)
        {
            var result = emailContent;

            #region Common Token

            const string Logo = "[LOGO]";
            const string CustomerName = "[CUSTOMER_NAME]";
            const string EmployeeCode = "[EMPLOYEE_CODE]";
            const string EmployeeName = "[EMPLOYEE_NAME]";
            const string Url_Login = "[URL]";
            const string CreatedDate = "[CREATED_DATE]";
            const string UpdatedDate = "[UPDATED_DATE]";
            const string CommentEmployeeName = "[COMM_EMP_NAME]";
            const string CommentEmployeeCode = "[COMM_EMP_CODE]";
            const string CommentCreatedDate = "[COMM_CREATED_DATE]";
            const string CommentContent = "[COMM_CONTENT]";
            const string Description = "[DESCRIPTION]";

            #endregion

            #region Token CustomerOrder

            const string OrderCode = "[ORDER_CODE]";

            #endregion

            #region Token Contract

            const string ContractCode = "[CONTRACT_CODE]";

            #endregion

            #region Token Quote

            const string QuoteName = "[QUOTE_NAME]";
            const string QuoteCode = "[QUOTE_CODE]";

            #endregion

            #region Token LEAD

            const string LeadName = "[LEAD_NAME]";
            const string LeadCode = "[LEAD_CODE]";

            #endregion

            #region Token SALE_BIDDING

            const string SaleBiddingName = "[SALE_BIDDING_NAME]";
            const string SaleBiddingCode = "[SALE_BIDDING_CODE]";

            #endregion

            #region BILL_SALE

            const string BillCode = "[BILL_CODE]";
            const string BillName = "[INVOICE_SYMBOL]";

            #endregion

            #region PROCUREMENT_REQUEST

            const string requestCode = "[PROCUREMENT_CODE]";
            const string requestEmployee = "[REQUEST_EMPLOYEE]";

            #endregion

            #region POTENTIAL_CUSTOMER

            const string PotentialCustomerName = "[POTENTIAL_CUSTOMER_NAME]";

            #endregion

            #region CUSTOMER

            const string CustomerCode = "[CUSTOMER_CODE]";

            #endregion

            #region VENDOR_ORDER

            const string vendorOrderCode = "[ORDER_CODE]";
            const string vendorName = "[VENDOR_NAME]";

            #endregion


            //Tạo đơn hàng
            if (typeModel == TypeModel.CustomerOrder)
            {
                var _model = model as CustomerOrder;

                if (result.Contains(Logo))
                {
                    var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                    if (!String.IsNullOrEmpty(logo))
                    {
                        var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                        result = result.Replace(Logo, temp_logo);
                    }
                    else
                    {
                        result = result.Replace(Logo, "");
                    }
                }

                if (result.Contains(OrderCode) && _model.OrderCode != null)
                {
                    result = result.Replace(OrderCode, _model.OrderCode);
                }

                if (result.Contains(CustomerName))
                {
                    var _customerName = context.Customer.FirstOrDefault(x => x.CustomerId == _model.CustomerId)
                        ?.CustomerName;

                    if (!String.IsNullOrEmpty(_customerName))
                    {
                        result = result.Replace(CustomerName, _customerName);
                    }
                    else
                    {
                        result = result.Replace(CustomerName, "");
                    }
                }

                if (result.Contains(EmployeeCode))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                    var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                    if (!String.IsNullOrEmpty(employeeCode))
                    {
                        result = result.Replace(EmployeeCode, employeeCode);
                    }
                    else
                    {
                        result = result.Replace(EmployeeCode, "");
                    }
                }

                if (result.Contains(EmployeeName))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                    var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                    if (!String.IsNullOrEmpty(employeeName))
                    {
                        result = result.Replace(EmployeeName, employeeName);
                    }
                    else
                    {
                        result = result.Replace(EmployeeName, "");
                    }
                }

                if (result.Contains(CreatedDate))
                {
                    result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                }

                if (result.Contains(UpdatedDate))
                {
                    result = result.Replace(UpdatedDate, FormatDateToString(_model.UpdatedDate));
                }

                if (result.Contains(Url_Login))
                {
                    var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                    var loginLink = Domain + @"/login?returnUrl=%2Fhome";

                    if (!String.IsNullOrEmpty(loginLink))
                    {
                        result = result.Replace(Url_Login, loginLink);
                    }
                }
            }
            //Chi tiết đơn hàng
            else if (typeModel == TypeModel.CustomerOrderDetail)
            {
                var _model = model as CustomerOrder;

                if (result.Contains(Logo))
                {
                    var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                    if (!String.IsNullOrEmpty(logo))
                    {
                        var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                        result = result.Replace(Logo, temp_logo);
                    }
                    else
                    {
                        result = result.Replace(Logo, "");
                    }
                }

                if (result.Contains(OrderCode) && _model.OrderCode != null)
                {
                    result = result.Replace(OrderCode, _model.OrderCode);
                }

                if (result.Contains(CustomerName))
                {
                    var _customerName = context.Customer.FirstOrDefault(x => x.CustomerId == _model.CustomerId)
                        ?.CustomerName;

                    if (!String.IsNullOrEmpty(_customerName))
                    {
                        result = result.Replace(CustomerName, _customerName);
                    }
                    else
                    {
                        result = result.Replace(CustomerName, "");
                    }
                }

                if (result.Contains(EmployeeCode))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                    var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                    if (!String.IsNullOrEmpty(employeeCode))
                    {
                        result = result.Replace(EmployeeCode, employeeCode);
                    }
                    else
                    {
                        result = result.Replace(EmployeeCode, "");
                    }
                }

                if (result.Contains(EmployeeName))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                    var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                    if (!String.IsNullOrEmpty(employeeName))
                    {
                        result = result.Replace(EmployeeName, employeeName);
                    }
                    else
                    {
                        result = result.Replace(EmployeeName, "");
                    }
                }

                if (result.Contains(CreatedDate))
                {
                    result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                }

                if (result.Contains(UpdatedDate))
                {
                    result = result.Replace(UpdatedDate, FormatDateToString(_model.UpdatedDate));
                }

                if (result.Contains(Url_Login))
                {
                    var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                    var loginLink = Domain + @"/login?returnUrl=%2Fhome";

                    if (!String.IsNullOrEmpty(loginLink))
                    {
                        result = result.Replace(Url_Login, loginLink);
                    }
                }

                if (note != null)
                {
                    var newNote = note as Note;

                    if (result.Contains(CommentEmployeeName))
                    {
                        var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                        if (commentUser != null)
                        {
                            var commentEmployee =
                                context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                            var _commentEmployeeName = commentEmployee?.EmployeeName;

                            if (!String.IsNullOrEmpty(_commentEmployeeName))
                            {
                                result = result.Replace(CommentEmployeeName, _commentEmployeeName);
                            }
                            else
                            {
                                result = result.Replace(CommentEmployeeName, "");
                            }
                        }
                    }

                    if (result.Contains(CommentEmployeeCode))
                    {
                        var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                        if (commentUser != null)
                        {
                            var commentEmployee =
                                context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                            var _commentEmployeeCode = commentEmployee?.EmployeeCode;

                            if (!String.IsNullOrEmpty(_commentEmployeeCode))
                            {
                                result = result.Replace(CommentEmployeeCode, _commentEmployeeCode);
                            }
                            else
                            {
                                result = result.Replace(CommentEmployeeCode, "");
                            }
                        }
                    }

                    if (result.Contains(CommentCreatedDate))
                    {
                        result = result.Replace(CommentCreatedDate, FormatDateToString(newNote.CreatedDate));
                    }

                    if (result.Contains(CommentContent))
                    {
                        var _commentContent = newNote.Description?.Trim();
                        if (!String.IsNullOrEmpty(_commentContent))
                        {
                            result = result.Replace(CommentContent, _commentContent);
                        }
                        else
                        {
                            result = result.Replace(CommentContent, "");
                        }
                    }
                }
            }
            //Tạo mới báo giá
            else if (typeModel == TypeModel.Quote)
            {
                var _model = model as Quote;

                if (result.Contains(Logo))
                {
                    var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                    if (!String.IsNullOrEmpty(logo))
                    {
                        var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                        result = result.Replace(Logo, temp_logo);
                    }
                    else
                    {
                        result = result.Replace(Logo, "");
                    }
                }

                if (result.Contains(QuoteName) && _model.QuoteName != null)
                {
                    result = result.Replace(QuoteName, _model.QuoteName.Trim());
                }

                if (result.Contains(QuoteCode) && _model.QuoteCode != null)
                {
                    result = result.Replace(QuoteCode, _model.QuoteCode.Trim());
                }

                if (result.Contains(EmployeeCode))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                    var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                    if (!String.IsNullOrEmpty(employeeCode))
                    {
                        result = result.Replace(EmployeeCode, employeeCode);
                    }
                    else
                    {
                        result = result.Replace(EmployeeCode, "");
                    }
                }

                if (result.Contains(EmployeeName))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                    var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                    if (!String.IsNullOrEmpty(employeeName))
                    {
                        result = result.Replace(EmployeeName, employeeName);
                    }
                    else
                    {
                        result = result.Replace(EmployeeName, "");
                    }
                }

                if (result.Contains(CreatedDate))
                {
                    result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                }

                if (result.Contains(Url_Login))
                {
                    var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                    var loginLink = Domain + @"/login?returnUrl=%2Fhome";

                    if (!String.IsNullOrEmpty(loginLink))
                    {
                        result = result.Replace(Url_Login, loginLink);
                    }
                }

                if (note != null)
                {
                    var newNote = note as Note;

                    if (result.Contains(CommentEmployeeName))
                    {
                        var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                        if (commentUser != null)
                        {
                            var commentEmployee =
                                context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                            var _commentEmployeeName = commentEmployee?.EmployeeName;

                            if (!String.IsNullOrEmpty(_commentEmployeeName))
                            {
                                result = result.Replace(CommentEmployeeName, _commentEmployeeName);
                            }
                            else
                            {
                                result = result.Replace(CommentEmployeeName, "");
                            }
                        }
                    }

                    if (result.Contains(CommentEmployeeCode))
                    {
                        var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                        if (commentUser != null)
                        {
                            var commentEmployee =
                                context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                            var _commentEmployeeCode = commentEmployee?.EmployeeCode;

                            if (!String.IsNullOrEmpty(_commentEmployeeCode))
                            {
                                result = result.Replace(CommentEmployeeCode, _commentEmployeeCode);
                            }
                            else
                            {
                                result = result.Replace(CommentEmployeeCode, "");
                            }
                        }
                    }

                    if (result.Contains(CommentCreatedDate))
                    {
                        result = result.Replace(CommentCreatedDate, FormatDateToString(newNote.CreatedDate));
                    }

                    if (result.Contains(CommentContent))
                    {
                        var _commentContent = newNote.Description?.Trim();
                        if (!String.IsNullOrEmpty(_commentContent))
                        {
                            result = result.Replace(CommentContent, _commentContent);
                        }
                        else
                        {
                            result = result.Replace(CommentContent, "");
                        }
                    }
                }
            }
            //Chi tiết báo giá
            else if (typeModel == TypeModel.QuoteDetail)
            {
                var _model = model as Quote;

                if (result.Contains(Logo))
                {
                    var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                    if (!String.IsNullOrEmpty(logo))
                    {
                        var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                        result = result.Replace(Logo, temp_logo);
                    }
                    else
                    {
                        result = result.Replace(Logo, "");
                    }
                }

                if (result.Contains(QuoteName) && _model.QuoteName != null)
                {
                    result = result.Replace(QuoteName, _model.QuoteName.Trim());
                }

                if (result.Contains(QuoteCode) && _model.QuoteCode != null)
                {
                    result = result.Replace(QuoteCode, _model.QuoteCode.Trim());
                }

                if (result.Contains(EmployeeCode))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                    var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                    if (!String.IsNullOrEmpty(employeeCode))
                    {
                        result = result.Replace(EmployeeCode, employeeCode);
                    }
                    else
                    {
                        result = result.Replace(EmployeeCode, "");
                    }
                }

                if (result.Contains(EmployeeName))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                    var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                    if (!String.IsNullOrEmpty(employeeName))
                    {
                        result = result.Replace(EmployeeName, employeeName);
                    }
                    else
                    {
                        result = result.Replace(EmployeeName, "");
                    }
                }

                if (result.Contains(CreatedDate))
                {
                    result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                }

                if (result.Contains(UpdatedDate))
                {
                    result = result.Replace(UpdatedDate, FormatDateToString(_model.UpdatedDate));
                }

                if (result.Contains(Url_Login))
                {
                    var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                    var loginLink = Domain + @"/login?returnUrl=%2Fhome";

                    if (!String.IsNullOrEmpty(loginLink))
                    {
                        result = result.Replace(Url_Login, loginLink);
                    }
                }

                if (note != null)
                {
                    var newNote = note as Note;

                    if (result.Contains(CommentEmployeeName))
                    {
                        var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                        if (commentUser != null)
                        {
                            var commentEmployee =
                                context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                            var _commentEmployeeName = commentEmployee?.EmployeeName;

                            if (!String.IsNullOrEmpty(_commentEmployeeName))
                            {
                                result = result.Replace(CommentEmployeeName, _commentEmployeeName);
                            }
                            else
                            {
                                result = result.Replace(CommentEmployeeName, "");
                            }
                        }
                    }

                    if (result.Contains(CommentEmployeeCode))
                    {
                        var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                        if (commentUser != null)
                        {
                            var commentEmployee =
                                context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                            var _commentEmployeeCode = commentEmployee?.EmployeeCode;

                            if (!String.IsNullOrEmpty(_commentEmployeeCode))
                            {
                                result = result.Replace(CommentEmployeeCode, _commentEmployeeCode);
                            }
                            else
                            {
                                result = result.Replace(CommentEmployeeCode, "");
                            }
                        }
                    }

                    if (result.Contains(CommentCreatedDate))
                    {
                        result = result.Replace(CommentCreatedDate, FormatDateToString(newNote.CreatedDate));
                    }

                    if (result.Contains(CommentContent))
                    {
                        var _commentContent = newNote.Description?.Trim();
                        if (!String.IsNullOrEmpty(_commentContent))
                        {
                            result = result.Replace(CommentContent, _commentContent);
                        }
                        else
                        {
                            result = result.Replace(CommentContent, "");
                        }
                    }

                    if (result.Contains(Description))
                    {
                        var _description = newNote.Description?.Trim();
                        if (!String.IsNullOrEmpty(_description))
                        {
                            result = result.Replace(Description, _description);
                        }
                        else
                        {
                            result = result.Replace(Description, "");
                        }
                    }
                }
            }
            //Tạo mới cơ hội
            else if (typeModel == TypeModel.Lead)
            {
                var _model = model as Lead;

                if (result.Contains(Logo))
                {
                    var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                    if (!String.IsNullOrEmpty(logo))
                    {
                        var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                        result = result.Replace(Logo, temp_logo);
                    }
                    else
                    {
                        result = result.Replace(Logo, "");
                    }
                }

                if (result.Contains(LeadName))
                {
                    var _leadName = context.Contact.FirstOrDefault(c => c.ObjectId == _model.LeadId && c.ObjectType == "LEA")?.FirstName;
                    if (!String.IsNullOrEmpty(_leadName))
                    {
                        result = result.Replace(LeadName, _leadName.Trim());
                    }
                }

                if (result.Contains(LeadCode) && _model.LeadCode != null)
                {
                    result = result.Replace(LeadCode, _model.LeadCode.Trim());
                }

                if (result.Contains(EmployeeCode))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId.ToString() == _model.CreatedById)?.EmployeeId;
                    var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                    if (!String.IsNullOrEmpty(employeeCode))
                    {
                        result = result.Replace(EmployeeCode, employeeCode);
                    }
                    else
                    {
                        result = result.Replace(EmployeeCode, "");
                    }
                }

                if (result.Contains(EmployeeName))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId.ToString() == _model.CreatedById)?.EmployeeId;
                    var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                    if (!String.IsNullOrEmpty(employeeName))
                    {
                        result = result.Replace(EmployeeName, employeeName);
                    }
                    else
                    {
                        result = result.Replace(EmployeeName, "");
                    }
                }

                if (result.Contains(CreatedDate))
                {
                    result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                }

                if (result.Contains(Url_Login))
                {
                    var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                    var loginLink = Domain + @"/login?returnUrl=%2Fhome";

                    if (!String.IsNullOrEmpty(loginLink))
                    {
                        result = result.Replace(Url_Login, loginLink);
                    }
                }
            }
            //Chi tiết cơ hội
            else if (typeModel == TypeModel.LeadDetail)
            {
                var _model = model as Lead;

                if (result.Contains(Logo))
                {
                    var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                    if (!String.IsNullOrEmpty(logo))
                    {
                        var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                        result = result.Replace(Logo, temp_logo);
                    }
                    else
                    {
                        result = result.Replace(Logo, "");
                    }
                }

                if (result.Contains(LeadName))
                {
                    var _leadName = context.Contact
                        .FirstOrDefault(c => c.ObjectId == _model.LeadId && c.ObjectType == "LEA").FirstName;
                    if (!String.IsNullOrEmpty(_leadName))
                    {
                        result = result.Replace(LeadName, _leadName.Trim());
                    }
                }

                if (result.Contains(LeadCode) && _model.LeadCode != null)
                {
                    result = result.Replace(LeadCode, _model.LeadCode.Trim());
                }

                if (result.Contains(EmployeeCode))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId.ToString() == _model.UpdatedById)?.EmployeeId;
                    var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                    if (!String.IsNullOrEmpty(employeeCode))
                    {
                        result = result.Replace(EmployeeCode, employeeCode);
                    }
                    else
                    {
                        result = result.Replace(EmployeeCode, "");
                    }
                }

                if (result.Contains(EmployeeName))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId.ToString() == _model.UpdatedById)?.EmployeeId;
                    var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                    if (!String.IsNullOrEmpty(employeeName))
                    {
                        result = result.Replace(EmployeeName, employeeName);
                    }
                    else
                    {
                        result = result.Replace(EmployeeName, "");
                    }
                }

                if (result.Contains(CreatedDate))
                {
                    result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                }

                if (result.Contains(UpdatedDate))
                {
                    result = result.Replace(UpdatedDate, FormatDateToString(_model.UpdatedDate));
                }

                if (result.Contains(Url_Login))
                {
                    var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                    var loginLink = Domain + @"/login?returnUrl=%2Fhome";

                    if (!String.IsNullOrEmpty(loginLink))
                    {
                        result = result.Replace(Url_Login, loginLink);
                    }
                }

                if (note != null)
                {
                    var newNote = note as Note;

                    if (result.Contains(CommentEmployeeName))
                    {
                        var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                        if (commentUser != null)
                        {
                            var commentEmployee =
                                context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                            var _commentEmployeeName = commentEmployee?.EmployeeName;

                            if (!String.IsNullOrEmpty(_commentEmployeeName))
                            {
                                result = result.Replace(CommentEmployeeName, _commentEmployeeName);
                            }
                            else
                            {
                                result = result.Replace(CommentEmployeeName, "");
                            }
                        }
                    }

                    if (result.Contains(CommentEmployeeCode))
                    {
                        var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                        if (commentUser != null)
                        {
                            var commentEmployee =
                                context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                            var _commentEmployeeCode = commentEmployee?.EmployeeCode;

                            if (!String.IsNullOrEmpty(_commentEmployeeCode))
                            {
                                result = result.Replace(CommentEmployeeCode, _commentEmployeeCode);
                            }
                            else
                            {
                                result = result.Replace(CommentEmployeeCode, "");
                            }
                        }
                    }

                    if (result.Contains(CommentCreatedDate))
                    {
                        result = result.Replace(CommentCreatedDate, FormatDateToString(newNote.CreatedDate));
                    }

                    if (result.Contains(CommentContent))
                    {
                        var _commentContent = newNote.Description?.Trim();
                        if (!String.IsNullOrEmpty(_commentContent))
                        {
                            result = result.Replace(CommentContent, _commentContent);
                        }
                        else
                        {
                            result = result.Replace(CommentContent, "");
                        }
                    }
                }
            }
            //Tạo mới hồ sơ thầu
            else if (typeModel == TypeModel.SaleBidding)
            {
                var _model = model as SaleBidding;

                if (result.Contains(Logo))
                {
                    var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                    if (!String.IsNullOrEmpty(logo))
                    {
                        var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                        result = result.Replace(Logo, temp_logo);
                    }
                    else
                    {
                        result = result.Replace(Logo, "");
                    }
                }

                if (result.Contains(SaleBiddingName) && _model.SaleBiddingName != null)
                {
                    result = result.Replace(SaleBiddingName, _model.SaleBiddingName.Trim());
                }

                if (result.Contains(SaleBiddingCode) && _model.SaleBiddingCode != null)
                {
                    result = result.Replace(SaleBiddingCode, _model.SaleBiddingCode.Trim());
                }

                if (result.Contains(EmployeeCode))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                    var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                    if (!String.IsNullOrEmpty(employeeCode))
                    {
                        result = result.Replace(EmployeeCode, employeeCode);
                    }
                    else
                    {
                        result = result.Replace(EmployeeCode, "");
                    }
                }

                if (result.Contains(EmployeeName))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                    var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                    if (!String.IsNullOrEmpty(employeeName))
                    {
                        result = result.Replace(EmployeeName, employeeName);
                    }
                    else
                    {
                        result = result.Replace(EmployeeName, "");
                    }
                }

                if (result.Contains(CreatedDate))
                {
                    result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                }

                if (result.Contains(Url_Login))
                {
                    var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                    var loginLink = Domain + @"/login?returnUrl=%2Fhome";

                    if (!String.IsNullOrEmpty(loginLink))
                    {
                        result = result.Replace(Url_Login, loginLink);
                    }
                }
            }
            //Chi tiết hồ sơ thầu
            else if (typeModel == TypeModel.SaleBiddingDetail)
            {
                var _model = model as SaleBidding;

                if (result.Contains(Logo))
                {
                    var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                    if (!String.IsNullOrEmpty(logo))
                    {
                        var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                        result = result.Replace(Logo, temp_logo);
                    }
                    else
                    {
                        result = result.Replace(Logo, "");
                    }
                }

                if (result.Contains(SaleBiddingName) && _model.SaleBiddingName != null)
                {
                    result = result.Replace(SaleBiddingName, _model.SaleBiddingName.Trim());
                }

                if (result.Contains(SaleBiddingCode) && _model.SaleBiddingCode != null)
                {
                    result = result.Replace(SaleBiddingCode, _model.SaleBiddingCode.Trim());
                }

                if (result.Contains(EmployeeCode))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                    var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                    if (!String.IsNullOrEmpty(employeeCode))
                    {
                        result = result.Replace(EmployeeCode, employeeCode);
                    }
                    else
                    {
                        result = result.Replace(EmployeeCode, "");
                    }
                }

                if (result.Contains(EmployeeName))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                    var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                    if (!String.IsNullOrEmpty(employeeName))
                    {
                        result = result.Replace(EmployeeName, employeeName);
                    }
                    else
                    {
                        result = result.Replace(EmployeeName, "");
                    }
                }

                if (result.Contains(CreatedDate))
                {
                    result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                }

                if (result.Contains(UpdatedDate))
                {
                    result = result.Replace(UpdatedDate, FormatDateToString(_model.UpdatedDate));
                }

                if (result.Contains(Url_Login))
                {
                    var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                    var loginLink = Domain + @"/login?returnUrl=%2Fhome";

                    if (!String.IsNullOrEmpty(loginLink))
                    {
                        result = result.Replace(Url_Login, loginLink);
                    }
                }

                if (note != null)
                {
                    var newNote = note as Note;

                    if (result.Contains(CommentEmployeeName))
                    {
                        var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                        if (commentUser != null)
                        {
                            var commentEmployee =
                                context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                            var _commentEmployeeName = commentEmployee?.EmployeeName;

                            if (!String.IsNullOrEmpty(_commentEmployeeName))
                            {
                                result = result.Replace(CommentEmployeeName, _commentEmployeeName);
                            }
                            else
                            {
                                result = result.Replace(CommentEmployeeName, "");
                            }
                        }
                    }

                    if (result.Contains(CommentEmployeeCode))
                    {
                        var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                        if (commentUser != null)
                        {
                            var commentEmployee =
                                context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                            var _commentEmployeeCode = commentEmployee?.EmployeeCode;

                            if (!String.IsNullOrEmpty(_commentEmployeeCode))
                            {
                                result = result.Replace(CommentEmployeeCode, _commentEmployeeCode);
                            }
                            else
                            {
                                result = result.Replace(CommentEmployeeCode, "");
                            }
                        }
                    }

                    if (result.Contains(CommentCreatedDate))
                    {
                        result = result.Replace(CommentCreatedDate, FormatDateToString(newNote.CreatedDate));
                    }

                    if (result.Contains(CommentContent))
                    {
                        var _commentContent = newNote.Description?.Trim();
                        if (!String.IsNullOrEmpty(_commentContent))
                        {
                            result = result.Replace(CommentContent, _commentContent);
                        }
                        else
                        {
                            result = result.Replace(CommentContent, "");
                        }
                    }

                    if (result.Contains(Description))
                    {
                        var _description = newNote.Description?.Trim();
                        if (!String.IsNullOrEmpty(_description))
                        {
                            result = result.Replace(Description, _description);
                        }
                        else
                        {
                            result = result.Replace(Description, "");
                        }
                    }
                }
            }
            //Tạo hợp đồng bán
            else if (typeModel == TypeModel.Contract)
            {
                var _model = model as Contract;

                if (result.Contains(Logo))
                {
                    var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                    if (!String.IsNullOrEmpty(logo))
                    {
                        var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                        result = result.Replace(Logo, temp_logo);
                    }
                    else
                    {
                        result = result.Replace(Logo, "");
                    }
                }

                if (result.Contains(CustomerName))
                {
                    var customerName = context.Customer.FirstOrDefault(c => c.CustomerId == _model.CustomerId)
                        .CustomerName;
                    if (!String.IsNullOrEmpty(customerName))
                    {
                        result = result.Replace(CustomerName, customerName.Trim());
                    }
                }

                if (result.Contains(ContractCode) && _model.ContractCode != null)
                {
                    result = result.Replace(ContractCode, _model.ContractCode.Trim());
                }

                if (result.Contains(EmployeeCode))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                    var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                    if (!String.IsNullOrEmpty(employeeCode))
                    {
                        result = result.Replace(EmployeeCode, employeeCode);
                    }
                    else
                    {
                        result = result.Replace(EmployeeCode, "");
                    }
                }

                if (result.Contains(EmployeeName))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                    var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                    if (!String.IsNullOrEmpty(employeeName))
                    {
                        result = result.Replace(EmployeeName, employeeName);
                    }
                    else
                    {
                        result = result.Replace(EmployeeName, "");
                    }
                }

                if (result.Contains(CreatedDate))
                {
                    result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                }

                if (result.Contains(Url_Login))
                {
                    var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                    var loginLink = Domain + @"/login?returnUrl=%2Fhome";

                    if (!String.IsNullOrEmpty(loginLink))
                    {
                        result = result.Replace(Url_Login, loginLink);
                    }
                }
            }
            //Chi tiết hợp đồng bán
            else if (typeModel == TypeModel.ContractDetail)
            {
                var _model = model as Contract;

                if (result.Contains(Logo))
                {
                    var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                    if (!String.IsNullOrEmpty(logo))
                    {
                        var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                        result = result.Replace(Logo, temp_logo);
                    }
                    else
                    {
                        result = result.Replace(Logo, "");
                    }
                }

                if (result.Contains(CustomerName))
                {
                    var customerName = context.Customer.FirstOrDefault(c => c.CustomerId == _model.CustomerId)
                        .CustomerName;
                    if (!String.IsNullOrEmpty(customerName))
                    {
                        result = result.Replace(CustomerName, customerName.Trim());
                    }
                }

                if (result.Contains(ContractCode) && _model.ContractCode != null)
                {
                    result = result.Replace(ContractCode, _model.ContractCode.Trim());
                }


                if (result.Contains(EmployeeCode))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                    var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                    if (!String.IsNullOrEmpty(employeeCode))
                    {
                        result = result.Replace(EmployeeCode, employeeCode);
                    }
                    else
                    {
                        result = result.Replace(EmployeeCode, "");
                    }
                }

                if (result.Contains(EmployeeName))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                    var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                    if (!String.IsNullOrEmpty(employeeName))
                    {
                        result = result.Replace(EmployeeName, employeeName);
                    }
                    else
                    {
                        result = result.Replace(EmployeeName, "");
                    }
                }

                if (result.Contains(CreatedDate))
                {
                    result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                }

                if (result.Contains(UpdatedDate))
                {
                    result = result.Replace(UpdatedDate, FormatDateToString(_model.UpdatedDate));
                }

                if (result.Contains(Url_Login))
                {
                    var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                    var loginLink = Domain + @"/login?returnUrl=%2Fhome";

                    if (!String.IsNullOrEmpty(loginLink))
                    {
                        result = result.Replace(Url_Login, loginLink);
                    }
                }

                if (note != null)
                {
                    var newNote = note as Note;

                    if (result.Contains(CommentEmployeeName))
                    {
                        var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                        if (commentUser != null)
                        {
                            var commentEmployee =
                                context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                            var _commentEmployeeName = commentEmployee?.EmployeeName;

                            if (!String.IsNullOrEmpty(_commentEmployeeName))
                            {
                                result = result.Replace(CommentEmployeeName, _commentEmployeeName);
                            }
                            else
                            {
                                result = result.Replace(CommentEmployeeName, "");
                            }
                        }
                    }

                    if (result.Contains(CommentEmployeeCode))
                    {
                        var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                        if (commentUser != null)
                        {
                            var commentEmployee =
                                context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                            var _commentEmployeeCode = commentEmployee?.EmployeeCode;

                            if (!String.IsNullOrEmpty(_commentEmployeeCode))
                            {
                                result = result.Replace(CommentEmployeeCode, _commentEmployeeCode);
                            }
                            else
                            {
                                result = result.Replace(CommentEmployeeCode, "");
                            }
                        }
                    }

                    if (result.Contains(CommentCreatedDate))
                    {
                        result = result.Replace(CommentCreatedDate, FormatDateToString(newNote.CreatedDate));
                    }

                    if (result.Contains(CommentContent))
                    {
                        var _commentContent = newNote.Description?.Trim();
                        if (!String.IsNullOrEmpty(_commentContent))
                        {
                            result = result.Replace(CommentContent, _commentContent);
                        }
                        else
                        {
                            result = result.Replace(CommentContent, "");
                        }
                    }
                }
            }
            //Tạo hóa đơn
            else if (typeModel == TypeModel.BillSale)
            {
                var _model = model as BillOfSale;

                if (result.Contains(Logo))
                {
                    var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                    if (!String.IsNullOrEmpty(logo))
                    {
                        var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                        result = result.Replace(Logo, temp_logo);
                    }
                    else
                    {
                        result = result.Replace(Logo, "");
                    }
                }

                if (result.Contains(BillName) && _model.InvoiceSymbol != null)
                {
                    result = result.Replace(BillName, _model.InvoiceSymbol.Trim());
                }

                if (result.Contains(BillCode) && _model.BillOfSaLeCode != null)
                {
                    result = result.Replace(BillCode, _model.BillOfSaLeCode.Trim());
                }

                if (result.Contains(EmployeeCode))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                    var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                    if (!String.IsNullOrEmpty(employeeCode))
                    {
                        result = result.Replace(EmployeeCode, employeeCode);
                    }
                    else
                    {
                        result = result.Replace(EmployeeCode, "");
                    }
                }

                if (result.Contains(EmployeeName))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                    var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                    if (!String.IsNullOrEmpty(employeeName))
                    {
                        result = result.Replace(EmployeeName, employeeName);
                    }
                    else
                    {
                        result = result.Replace(EmployeeName, "");
                    }
                }

                if (result.Contains(CreatedDate))
                {
                    result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                }

                if (result.Contains(Url_Login))
                {
                    var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                    var loginLink = Domain + @"/login?returnUrl=%2Fhome";

                    if (!String.IsNullOrEmpty(loginLink))
                    {
                        result = result.Replace(Url_Login, loginLink);
                    }
                }
            }
            //Chi tiết hóa đơn
            else if (typeModel == TypeModel.BillSaleDetail)
            {
                var _model = model as BillOfSale;

                if (result.Contains(Logo))
                {
                    var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                    if (!String.IsNullOrEmpty(logo))
                    {
                        var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                        result = result.Replace(Logo, temp_logo);
                    }
                    else
                    {
                        result = result.Replace(Logo, "");
                    }
                }

                if (result.Contains(BillName) && _model.InvoiceSymbol != null)
                {
                    result = result.Replace(BillName, _model.InvoiceSymbol.Trim());
                }

                if (result.Contains(BillCode) && _model.BillOfSaLeCode != null)
                {
                    result = result.Replace(BillCode, _model.BillOfSaLeCode.Trim());
                }


                if (result.Contains(EmployeeCode))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                    var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                    if (!String.IsNullOrEmpty(employeeCode))
                    {
                        result = result.Replace(EmployeeCode, employeeCode);
                    }
                    else
                    {
                        result = result.Replace(EmployeeCode, "");
                    }
                }

                if (result.Contains(EmployeeName))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                    var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                    if (!String.IsNullOrEmpty(employeeName))
                    {
                        result = result.Replace(EmployeeName, employeeName);
                    }
                    else
                    {
                        result = result.Replace(EmployeeName, "");
                    }
                }

                if (result.Contains(CreatedDate))
                {
                    result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                }

                if (result.Contains(UpdatedDate))
                {
                    result = result.Replace(UpdatedDate, FormatDateToString(_model.UpdatedDate));
                }

                if (result.Contains(Url_Login))
                {
                    var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                    var loginLink = Domain + @"/login?returnUrl=%2Fhome";

                    if (!String.IsNullOrEmpty(loginLink))
                    {
                        result = result.Replace(Url_Login, loginLink);
                    }
                }

                if (note != null)
                {
                    var newNote = note as Note;

                    if (result.Contains(CommentEmployeeName))
                    {
                        var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                        if (commentUser != null)
                        {
                            var commentEmployee =
                                context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                            var _commentEmployeeName = commentEmployee?.EmployeeName;

                            if (!String.IsNullOrEmpty(_commentEmployeeName))
                            {
                                result = result.Replace(CommentEmployeeName, _commentEmployeeName);
                            }
                            else
                            {
                                result = result.Replace(CommentEmployeeName, "");
                            }
                        }
                    }

                    if (result.Contains(CommentEmployeeCode))
                    {
                        var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                        if (commentUser != null)
                        {
                            var commentEmployee =
                                context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                            var _commentEmployeeCode = commentEmployee?.EmployeeCode;

                            if (!String.IsNullOrEmpty(_commentEmployeeCode))
                            {
                                result = result.Replace(CommentEmployeeCode, _commentEmployeeCode);
                            }
                            else
                            {
                                result = result.Replace(CommentEmployeeCode, "");
                            }
                        }
                    }

                    if (result.Contains(CommentCreatedDate))
                    {
                        result = result.Replace(CommentCreatedDate, FormatDateToString(newNote.CreatedDate));
                    }

                    if (result.Contains(CommentContent))
                    {
                        var _commentContent = newNote.Description?.Trim();
                        if (!String.IsNullOrEmpty(_commentContent))
                        {
                            result = result.Replace(CommentContent, _commentContent);
                        }
                        else
                        {
                            result = result.Replace(CommentContent, "");
                        }
                    }
                }
            }
            //Tạo đề xuất mua hàng
            else if (typeModel == TypeModel.ProcurementRequest)
            {
                var _model = model as ProcurementRequest;

                if (result.Contains(Logo))
                {
                    var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                    if (!String.IsNullOrEmpty(logo))
                    {
                        var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                        result = result.Replace(Logo, temp_logo);
                    }
                    else
                    {
                        result = result.Replace(Logo, "");
                    }
                }

                if (result.Contains(requestCode) && _model.ProcurementCode != null)
                {
                    result = result.Replace(requestCode, _model.ProcurementCode.Trim());
                }

                if (result.Contains(requestEmployee))
                {
                    var _requestEmployee = context.Employee
                        .FirstOrDefault(x => x.EmployeeId == _model.RequestEmployeeId).EmployeeName;
                    if (!String.IsNullOrEmpty(_requestEmployee))
                    {
                        result = result.Replace(requestEmployee, _requestEmployee);
                    }
                }

                if (result.Contains(EmployeeCode))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                    var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                    if (!String.IsNullOrEmpty(employeeCode))
                    {
                        result = result.Replace(EmployeeCode, employeeCode);
                    }
                    else
                    {
                        result = result.Replace(EmployeeCode, "");
                    }
                }

                if (result.Contains(EmployeeName))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                    var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                    if (!String.IsNullOrEmpty(employeeName))
                    {
                        result = result.Replace(EmployeeName, employeeName);
                    }
                    else
                    {
                        result = result.Replace(EmployeeName, "");
                    }
                }

                if (result.Contains(CreatedDate))
                {
                    result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                }

                if (result.Contains(Url_Login))
                {
                    var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                    var loginLink = Domain + @"/login?returnUrl=%2Fhome";

                    if (!String.IsNullOrEmpty(loginLink))
                    {
                        result = result.Replace(Url_Login, loginLink);
                    }
                }
            }
            //Chi tiết đề xuất mua hàng
            else if (typeModel == TypeModel.ProcurementRequestDetail)
            {
                var _model = model as ProcurementRequest;

                if (result.Contains(Logo))
                {
                    var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                    if (!String.IsNullOrEmpty(logo))
                    {
                        var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                        result = result.Replace(Logo, temp_logo);
                    }
                    else
                    {
                        result = result.Replace(Logo, "");
                    }
                }

                if (result.Contains(requestCode) && _model.ProcurementCode != null)
                {
                    result = result.Replace(requestCode, _model.ProcurementCode.Trim());
                }

                if (result.Contains(requestEmployee))
                {
                    var _requestEmployee = context.Employee
                        .FirstOrDefault(x => x.EmployeeId == _model.RequestEmployeeId).EmployeeName;
                    if (!String.IsNullOrEmpty(_requestEmployee))
                    {
                        result = result.Replace(requestEmployee, _requestEmployee);
                    }
                }

                if (result.Contains(EmployeeCode))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                    var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                    if (!String.IsNullOrEmpty(employeeCode))
                    {
                        result = result.Replace(EmployeeCode, employeeCode);
                    }
                    else
                    {
                        result = result.Replace(EmployeeCode, "");
                    }
                }

                if (result.Contains(EmployeeName))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                    var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                    if (!String.IsNullOrEmpty(employeeName))
                    {
                        result = result.Replace(EmployeeName, employeeName);
                    }
                    else
                    {
                        result = result.Replace(EmployeeName, "");
                    }
                }

                if (result.Contains(CreatedDate))
                {
                    result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                }

                if (result.Contains(UpdatedDate))
                {
                    result = result.Replace(UpdatedDate, FormatDateToString(_model.UpdatedDate));
                }

                if (result.Contains(Url_Login))
                {
                    var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                    var loginLink = Domain + @"/login?returnUrl=%2Fhome";

                    if (!String.IsNullOrEmpty(loginLink))
                    {
                        result = result.Replace(Url_Login, loginLink);
                    }
                }

                if (note != null)
                {
                    var newNote = note as Note;

                    if (result.Contains(CommentEmployeeName))
                    {
                        var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                        if (commentUser != null)
                        {
                            var commentEmployee =
                                context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                            var _commentEmployeeName = commentEmployee?.EmployeeName;

                            if (!String.IsNullOrEmpty(_commentEmployeeName))
                            {
                                result = result.Replace(CommentEmployeeName, _commentEmployeeName);
                            }
                            else
                            {
                                result = result.Replace(CommentEmployeeName, "");
                            }
                        }
                    }

                    if (result.Contains(CommentEmployeeCode))
                    {
                        var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                        if (commentUser != null)
                        {
                            var commentEmployee =
                                context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                            var _commentEmployeeCode = commentEmployee?.EmployeeCode;

                            if (!String.IsNullOrEmpty(_commentEmployeeCode))
                            {
                                result = result.Replace(CommentEmployeeCode, _commentEmployeeCode);
                            }
                            else
                            {
                                result = result.Replace(CommentEmployeeCode, "");
                            }
                        }
                    }

                    if (result.Contains(CommentCreatedDate))
                    {
                        result = result.Replace(CommentCreatedDate, FormatDateToString(newNote.CreatedDate));
                    }

                    if (result.Contains(CommentContent))
                    {
                        var _commentContent = newNote.Description?.Trim();
                        if (!String.IsNullOrEmpty(_commentContent))
                        {
                            result = result.Replace(CommentContent, _commentContent);
                        }
                        else
                        {
                            result = result.Replace(CommentContent, "");
                        }
                    }
                }
            }
            //Tạo khách hàng tiềm năng
            else if (typeModel == TypeModel.PotentialCustomer)
            {
                var _model = model as Customer;

                if (result.Contains(Logo))
                {
                    var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                    if (!String.IsNullOrEmpty(logo))
                    {
                        var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                        result = result.Replace(Logo, temp_logo);
                    }
                    else
                    {
                        result = result.Replace(Logo, "");
                    }
                }

                if (result.Contains(PotentialCustomerName) && _model.CustomerName != null)
                {
                    result = result.Replace(PotentialCustomerName, _model.CustomerName);
                }

                if (result.Contains(EmployeeCode))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                    var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                    if (!String.IsNullOrEmpty(employeeCode))
                    {
                        result = result.Replace(EmployeeCode, employeeCode);
                    }
                    else
                    {
                        result = result.Replace(EmployeeCode, "");
                    }
                }

                if (result.Contains(EmployeeName))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                    var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                    if (!String.IsNullOrEmpty(employeeName))
                    {
                        result = result.Replace(EmployeeName, employeeName);
                    }
                    else
                    {
                        result = result.Replace(EmployeeName, "");
                    }
                }

                if (result.Contains(CreatedDate))
                {
                    result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                }

                if (result.Contains(Url_Login))
                {
                    var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                    var loginLink = Domain + @"/login?returnUrl=%2Fhome";

                    if (!String.IsNullOrEmpty(loginLink))
                    {
                        result = result.Replace(Url_Login, loginLink);
                    }
                }
            }
            //Chi tiết khách hàng tiềm năng
            else if (typeModel == TypeModel.PotentialCustomerDetail)
            {
                var _model = model as Customer;

                if (result.Contains(Logo))
                {
                    var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                    if (!String.IsNullOrEmpty(logo))
                    {
                        var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                        result = result.Replace(Logo, temp_logo);
                    }
                    else
                    {
                        result = result.Replace(Logo, "");
                    }
                }

                if (result.Contains(PotentialCustomerName) && _model.CustomerName != null)
                {
                    result = result.Replace(PotentialCustomerName, _model.CustomerName);
                }

                if (result.Contains(EmployeeCode))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                    var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                    if (!String.IsNullOrEmpty(employeeCode))
                    {
                        result = result.Replace(EmployeeCode, employeeCode);
                    }
                    else
                    {
                        result = result.Replace(EmployeeCode, "");
                    }
                }

                if (result.Contains(EmployeeName))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                    var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                    if (!String.IsNullOrEmpty(employeeName))
                    {
                        result = result.Replace(EmployeeName, employeeName);
                    }
                    else
                    {
                        result = result.Replace(EmployeeName, "");
                    }
                }

                if (result.Contains(CreatedDate))
                {
                    result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                }

                if (result.Contains(UpdatedDate))
                {
                    result = result.Replace(UpdatedDate, FormatDateToString(_model.UpdatedDate));
                }

                if (result.Contains(Url_Login))
                {
                    var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                    var loginLink = Domain + @"/login?returnUrl=%2Fhome";

                    if (!String.IsNullOrEmpty(loginLink))
                    {
                        result = result.Replace(Url_Login, loginLink);
                    }
                }

                if (note != null)
                {
                    var newNote = note as Note;

                    if (result.Contains(CommentEmployeeName))
                    {
                        var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                        if (commentUser != null)
                        {
                            var commentEmployee =
                                context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                            var _commentEmployeeName = commentEmployee?.EmployeeName;

                            if (!String.IsNullOrEmpty(_commentEmployeeName))
                            {
                                result = result.Replace(CommentEmployeeName, _commentEmployeeName);
                            }
                            else
                            {
                                result = result.Replace(CommentEmployeeName, "");
                            }
                        }
                    }

                    if (result.Contains(CommentEmployeeCode))
                    {
                        var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                        if (commentUser != null)
                        {
                            var commentEmployee =
                                context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                            var _commentEmployeeCode = commentEmployee?.EmployeeCode;

                            if (!String.IsNullOrEmpty(_commentEmployeeCode))
                            {
                                result = result.Replace(CommentEmployeeCode, _commentEmployeeCode);
                            }
                            else
                            {
                                result = result.Replace(CommentEmployeeCode, "");
                            }
                        }
                    }

                    if (result.Contains(CommentCreatedDate))
                    {
                        result = result.Replace(CommentCreatedDate, FormatDateToString(newNote.CreatedDate));
                    }

                    if (result.Contains(CommentContent))
                    {
                        var _commentContent = newNote.Description?.Trim();
                        if (!String.IsNullOrEmpty(_commentContent))
                        {
                            result = result.Replace(CommentContent, _commentContent);
                        }
                        else
                        {
                            result = result.Replace(CommentContent, "");
                        }
                    }
                }
            }
            //Tạo khách hàng
            else if (typeModel == TypeModel.Customer)
            {
                var _model = model as Customer;

                if (result.Contains(Logo))
                {
                    var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                    if (!String.IsNullOrEmpty(logo))
                    {
                        var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                        result = result.Replace(Logo, temp_logo);
                    }
                    else
                    {
                        result = result.Replace(Logo, "");
                    }
                }

                if (result.Contains(CustomerCode) && _model.CustomerCode != null)
                {
                    result = result.Replace(CustomerCode, _model.CustomerCode);
                }

                if (result.Contains(CustomerName) && _model.CustomerName != null)
                {
                    result = result.Replace(CustomerName, _model.CustomerName);
                }

                if (result.Contains(EmployeeCode))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                    var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                    if (!String.IsNullOrEmpty(employeeCode))
                    {
                        result = result.Replace(EmployeeCode, employeeCode);
                    }
                    else
                    {
                        result = result.Replace(EmployeeCode, "");
                    }
                }

                if (result.Contains(EmployeeName))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                    var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                    if (!String.IsNullOrEmpty(employeeName))
                    {
                        result = result.Replace(EmployeeName, employeeName);
                    }
                    else
                    {
                        result = result.Replace(EmployeeName, "");
                    }
                }

                if (result.Contains(CreatedDate))
                {
                    result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                }

                if (result.Contains(Url_Login))
                {
                    var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                    var loginLink = Domain + @"/login?returnUrl=%2Fhome";

                    if (!String.IsNullOrEmpty(loginLink))
                    {
                        result = result.Replace(Url_Login, loginLink);
                    }
                }
            }
            //Chi tiết khách hàng
            else if (typeModel == TypeModel.CustomerDetail)
            {
                var _model = model as Customer;

                if (result.Contains(Logo))
                {
                    var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                    if (!String.IsNullOrEmpty(logo))
                    {
                        var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                        result = result.Replace(Logo, temp_logo);
                    }
                    else
                    {
                        result = result.Replace(Logo, "");
                    }
                }

                if (result.Contains(CustomerCode) && _model.CustomerCode != null)
                {
                    result = result.Replace(CustomerCode, _model.CustomerCode);
                }

                if (result.Contains(CustomerName) && _model.CustomerName != null)
                {
                    result = result.Replace(CustomerName, _model.CustomerName);
                }

                if (result.Contains(EmployeeCode))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                    var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                    if (!String.IsNullOrEmpty(employeeCode))
                    {
                        result = result.Replace(EmployeeCode, employeeCode);
                    }
                    else
                    {
                        result = result.Replace(EmployeeCode, "");
                    }
                }

                if (result.Contains(EmployeeName))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                    var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                    if (!String.IsNullOrEmpty(employeeName))
                    {
                        result = result.Replace(EmployeeName, employeeName);
                    }
                    else
                    {
                        result = result.Replace(EmployeeName, "");
                    }
                }

                if (result.Contains(CreatedDate))
                {
                    result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                }

                if (result.Contains(UpdatedDate))
                {
                    result = result.Replace(UpdatedDate, FormatDateToString(_model.UpdatedDate));
                }

                if (result.Contains(Url_Login))
                {
                    var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                    var loginLink = Domain + @"/login?returnUrl=%2Fhome";

                    if (!String.IsNullOrEmpty(loginLink))
                    {
                        result = result.Replace(Url_Login, loginLink);
                    }
                }

                if (note != null)
                {
                    var newNote = note as Note;

                    if (result.Contains(CommentEmployeeName))
                    {
                        var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                        if (commentUser != null)
                        {
                            var commentEmployee =
                                context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                            var _commentEmployeeName = commentEmployee?.EmployeeName;

                            if (!String.IsNullOrEmpty(_commentEmployeeName))
                            {
                                result = result.Replace(CommentEmployeeName, _commentEmployeeName);
                            }
                            else
                            {
                                result = result.Replace(CommentEmployeeName, "");
                            }
                        }
                    }

                    if (result.Contains(CommentEmployeeCode))
                    {
                        var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                        if (commentUser != null)
                        {
                            var commentEmployee =
                                context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                            var _commentEmployeeCode = commentEmployee?.EmployeeCode;

                            if (!String.IsNullOrEmpty(_commentEmployeeCode))
                            {
                                result = result.Replace(CommentEmployeeCode, _commentEmployeeCode);
                            }
                            else
                            {
                                result = result.Replace(CommentEmployeeCode, "");
                            }
                        }
                    }

                    if (result.Contains(CommentCreatedDate))
                    {
                        result = result.Replace(CommentCreatedDate, FormatDateToString(newNote.CreatedDate));
                    }

                    if (result.Contains(CommentContent))
                    {
                        var _commentContent = newNote.Description?.Trim();
                        if (!String.IsNullOrEmpty(_commentContent))
                        {
                            result = result.Replace(CommentContent, _commentContent);
                        }
                        else
                        {
                            result = result.Replace(CommentContent, "");
                        }
                    }
                }
            }
            //Tạo đơn hàng mua
            else if (typeModel == TypeModel.VendorOrder)
            {
                var _model = model as VendorOrder;

                if (result.Contains(Logo))
                {
                    var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                    if (!String.IsNullOrEmpty(logo))
                    {
                        var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                        result = result.Replace(Logo, temp_logo);
                    }
                    else
                    {
                        result = result.Replace(Logo, "");
                    }
                }

                if (result.Contains(vendorOrderCode) && _model.VendorOrderCode != null)
                {
                    result = result.Replace(vendorOrderCode, _model.VendorOrderCode);
                }

                if (result.Contains(vendorName))
                {
                    var _vendorName = context.Vendor.FirstOrDefault(x => x.VendorId == _model.VendorId).VendorName;
                    if (!String.IsNullOrEmpty(_vendorName))
                    {
                        result = result.Replace(vendorName, _vendorName);
                    }
                }

                if (result.Contains(EmployeeCode))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                    var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                    if (!String.IsNullOrEmpty(employeeCode))
                    {
                        result = result.Replace(EmployeeCode, employeeCode);
                    }
                    else
                    {
                        result = result.Replace(EmployeeCode, "");
                    }
                }

                if (result.Contains(EmployeeName))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                    var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                    if (!String.IsNullOrEmpty(employeeName))
                    {
                        result = result.Replace(EmployeeName, employeeName);
                    }
                    else
                    {
                        result = result.Replace(EmployeeName, "");
                    }
                }

                if (result.Contains(CreatedDate))
                {
                    result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                }

                if (result.Contains(Url_Login))
                {
                    var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                    var loginLink = Domain + @"/login?returnUrl=%2Fhome";

                    if (!String.IsNullOrEmpty(loginLink))
                    {
                        result = result.Replace(Url_Login, loginLink);
                    }
                }
            }
            //Chi tiết đơn hàng mua
            else if (typeModel == TypeModel.VendorOrderDetail)
            {
                var _model = model as VendorOrder;

                if (result.Contains(Logo))
                {
                    var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                    if (!String.IsNullOrEmpty(logo))
                    {
                        var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                        result = result.Replace(Logo, temp_logo);
                    }
                    else
                    {
                        result = result.Replace(Logo, "");
                    }
                }

                if (result.Contains(vendorOrderCode) && _model.VendorOrderCode != null)
                {
                    result = result.Replace(vendorOrderCode, _model.VendorOrderCode);
                }

                if (result.Contains(vendorName))
                {
                    var _vendorName = context.Vendor.FirstOrDefault(x => x.VendorId == _model.VendorId).VendorName;
                    if (!String.IsNullOrEmpty(_vendorName))
                    {
                        result = result.Replace(vendorName, _vendorName);
                    }
                }

                if (result.Contains(EmployeeCode))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                    var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                    if (!String.IsNullOrEmpty(employeeCode))
                    {
                        result = result.Replace(EmployeeCode, employeeCode);
                    }
                    else
                    {
                        result = result.Replace(EmployeeCode, "");
                    }
                }

                if (result.Contains(EmployeeName))
                {
                    var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                    var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                    if (!String.IsNullOrEmpty(employeeName))
                    {
                        result = result.Replace(EmployeeName, employeeName);
                    }
                    else
                    {
                        result = result.Replace(EmployeeName, "");
                    }
                }

                if (result.Contains(CreatedDate))
                {
                    result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                }

                if (result.Contains(UpdatedDate))
                {
                    result = result.Replace(UpdatedDate, FormatDateToString(_model.UpdatedDate));
                }

                if (result.Contains(Url_Login))
                {
                    var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                    var loginLink = Domain + @"/login?returnUrl=%2Fhome";

                    if (!String.IsNullOrEmpty(loginLink))
                    {
                        result = result.Replace(Url_Login, loginLink);
                    }
                }

                if (note != null)
                {
                    var newNote = note as Note;

                    if (result.Contains(CommentEmployeeName))
                    {
                        var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                        if (commentUser != null)
                        {
                            var commentEmployee =
                                context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                            var _commentEmployeeName = commentEmployee?.EmployeeName;

                            if (!String.IsNullOrEmpty(_commentEmployeeName))
                            {
                                result = result.Replace(CommentEmployeeName, _commentEmployeeName);
                            }
                            else
                            {
                                result = result.Replace(CommentEmployeeName, "");
                            }
                        }
                    }

                    if (result.Contains(CommentEmployeeCode))
                    {
                        var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                        if (commentUser != null)
                        {
                            var commentEmployee =
                                context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                            var _commentEmployeeCode = commentEmployee?.EmployeeCode;

                            if (!String.IsNullOrEmpty(_commentEmployeeCode))
                            {
                                result = result.Replace(CommentEmployeeCode, _commentEmployeeCode);
                            }
                            else
                            {
                                result = result.Replace(CommentEmployeeCode, "");
                            }
                        }
                    }

                    if (result.Contains(CommentCreatedDate))
                    {
                        result = result.Replace(CommentCreatedDate, FormatDateToString(newNote.CreatedDate));
                    }

                    if (result.Contains(CommentContent))
                    {
                        var _commentContent = newNote.Description?.Trim();
                        if (!String.IsNullOrEmpty(_commentContent))
                        {
                            result = result.Replace(CommentContent, _commentContent);
                        }
                        else
                        {
                            result = result.Replace(CommentContent, "");
                        }
                    }

                    if (result.Contains(Description))
                    {
                        var _description = newNote.Description?.Trim();
                        if (!String.IsNullOrEmpty(_description))
                        {
                            result = result.Replace(Description, _description);
                        }
                        else
                        {
                            result = result.Replace(Description, "");
                        }
                    }
                }
            }

            return result;
        }

        private static string FormatDateToString(DateTime? date)
        {
            var result = "";

            if (date != null)
            {
                result = date.Value.Day.ToString("00") + "/" +
                         date.Value.Month.ToString("00") + "/" +
                         date.Value.Year.ToString("0000") + " " +
                         date.Value.Hour.ToString("00") + ":" +
                         date.Value.Minute.ToString("00");
            }

            return result;
        }

        private static List<Guid> GetListEmployeeApproved(TNTN8Context context, string WorkflowCode, int? StepNumber, List<Employee> ListAllEmployee)
        {
            var result = new List<Guid>();

            var workflow = context.WorkFlows.FirstOrDefault(x => x.WorkflowCode == WorkflowCode);

            if (workflow != null)
            {
                var workflowSteps = context.WorkFlowSteps.Where(x => x.WorkflowId == workflow.WorkFlowId).ToList();

                if (workflowSteps.Count > 0)
                {
                    //Nếu đối tượng có trạng thái Mới tạo (StepNumber == null) hoặc có trạng thái Từ chối (StepNumber == 0)
                    if (StepNumber == 0 || StepNumber == null)
                    {
                        var workflowStep = workflowSteps.FirstOrDefault(x => x.StepNumber == 2);

                        if (workflowStep != null)
                        {
                            bool approvalByPosition = workflowStep.ApprovebyPosition;

                            if (approvalByPosition)
                            {
                                #region Lấy danh sách người phê duyệt theo chức vụ

                                var positionId = workflowStep.ApproverPositionId;

                                if (positionId != null && positionId != Guid.Empty)
                                {
                                    result = ListAllEmployee.Where(x => x.PositionId == positionId).Select(y => y.EmployeeId)
                                        .ToList();
                                }

                                #endregion
                            }
                            else
                            {
                                #region Lấy người phê duyệt theo chỉ định

                                var approvedId = workflowStep.ApproverId;

                                if (approvedId != null && approvedId != Guid.Empty)
                                {
                                    result.Add(approvedId.Value);
                                }

                                #endregion
                            }
                        }
                    }
                    else
                    {
                        var workflowStep = workflowSteps.FirstOrDefault(x => x.StepNumber == StepNumber);

                        if (workflowStep != null)
                        {
                            bool approvalByPosition = workflowStep.ApprovebyPosition;

                            if (approvalByPosition)
                            {
                                #region Lấy danh sách người phê duyệt theo chức vụ

                                var positionId = workflowStep.ApproverPositionId;

                                if (positionId != null && positionId != Guid.Empty)
                                {
                                    result = ListAllEmployee.Where(x => x.PositionId == positionId).Select(y => y.EmployeeId)
                                        .ToList();
                                }

                                #endregion
                            }
                            else
                            {
                                #region Lấy người phê duyệt theo chỉ định

                                var approvedId = workflowStep.ApproverId;

                                if (approvedId != null && approvedId != Guid.Empty)
                                {
                                    result.Add(approvedId.Value);
                                }

                                #endregion
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
