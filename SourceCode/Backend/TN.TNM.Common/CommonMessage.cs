namespace TN.TNM.Common
{
    public static class CommonMessage
    {
        public static class MasterData
        {
            public const string CREATE_SUCCESS = "Tạo Dữ liệu thành công.";
            public const string CREATE_FAIL = "Tạo Dữ liệu thất bại.";
            public const string EDIT_SUCCESS = "Chỉnh sửa Dữ liệu thành công.";
            public const string EDIT_FAIL = "Chỉnh sửa Dữ liệu thất bại.";
            public const string GET_FAIL = "Lấy thông tin Dữ liệu thất bại.";
            public const string GETALL_FAIL = "Có lỗi xảy ra khi lấy toàn bộ dữ liệu.";
            public const string DELETE_SUCCESS = "Xoá thành công.";
            public const string DELETE_FAIL = "Xóa thất bại.";
            public const string PLEASE_CHOOSE = "Vui lòng chọn loại dữ liệu.";
            public const string ONLY_2_LVL = "Chỉ được tạo dữ liệu cấp 2.";
            public const string CANNOT_DELETE_TYPE = "Không được xóa loại dữ liệu ";
            public const string CANNOT_EDIT_TYPE = "Không được chỉnh sửa loại dữ liệu.";
            public const string EMPLOYEE_EXIST = "Không thể xoá! Đã có nhân viên thuộc đơn vị này.";
            public const string HIGHEST_LEVEL = "Không thể xoá!";

        }

        public static class Organization
        {
            public const string CREATE_SUCCESS = "Tạo Đơn vị thành công.";
            public const string CREATE_FAIL = "Tạo Đơn vị thất bại.";
            public const string EDIT_SUCCESS = "Chỉnh sửa Đơn vị thành công.";
            public const string EDIT_FAIL = "Chỉnh sửa Đơn vị thất bại.";
            public const string GET_FAIL = "Lấy thông tin Đơn vị thất bại.";
            public const string GETALL_FAIL = "Có lỗi xảy ra khi lấy toàn bộ Đơn vị.";
            public const string DELETE_SUCCESS = "Xoá Đơn vị thành công.";
            public const string DELETE_FAIL = "Xóa Đơn vị thất bại.";
            public const string HAS_CHILD = "Không thể xoá! Đã có đơn vị con trực thuộc.";
            public const string DUPLICATE_CODE = "Đã có một đơn vị khác sử dụng mã này.";
        }

        public static class Lead
        {
            public const string CREATE_SUCCESS = "Tạo Cơ hội thành công.";
            public const string CREATE_FAIL = "Tạo Cơ hội thất bại.";
            public const string EDIT_SUCCESS = "Chỉnh sửa Cơ hội thành công.";
            public const string EDIT_FAIL = "Chỉnh sửa Cơ hội thất bại.";
            public const string DELETE_SUCCESS = "Xoá Cơ hội thành công.";
            public const string DELETE_FAIL = "Xoá Cơ hội thất bại.";
            public const string GET_FAIL = "Lấy thông tin Cơ hội thất bại.";
            public const string DELETE_FAIL_SALE_BIDDING_REFERENCES = "Xoá Cơ hội thất bại, Cơ hội đã gắn với hồ sơ thầu";
            public const string DELETE_FAIL_QUOTE_REFERENCES = "Xoá Cơ hội thất bại, Cơ hội đã gắn với báo giá";
            public const string LEAD_NOT_EXIST = "Cơ hội không tồn tại trên hệ thống";
        }

        public static class Contact
        {
            public const string CREATE_SUCCESS = "Tạo Người liên hệ thành công.";
            public const string CREATE_FAIL = "Tạo Người liên hệ thất bại.";
            public const string EDIT_SUCCESS = "Chỉnh sửa Người liên hệ thành công.";
            public const string EDIT_FAIL = "Chỉnh sửa Người liên hệ thất bại.";
            public const string DELETE_SUCCESS = "Xóa Người liên hệ thành công.";
            public const string DELETE_FAIL = "Xóa Người liên hệ thất bại.";
            public const string GET_FAIL = "Lấy thông tin Người liên hệ thất bại.";
            public const string EMAIL_DOES_NOT_EXIST = "Email của người dùng không tồn tại";
        }

        public static class Employee
        {
            public const string CREATE_SUCCESS = "Tạo Nhân viên thành công.";
            public const string CREATE_FAIL = "Tạo Nhân viên thất bại.";
            public const string EDIT_SUCCESS = "Chỉnh sửa Nhân viên thành công.";
            public const string EDIT_FAIL = "Chỉnh sửa Nhân viên thất bại.";
            public const string DELETE_SUCCESS = "Xoá Nhân viên thành công.";
            public const string DELETE_FAIL = "Xoá Nhân viên thất bại.";
            public const string GET_FAIL = "Lấy thông tin Nhân viên thất bại.";
            public const string GRAND_PERMISSION_SUCCESS = "Cấp quyền cho nhân viên thành công.";
            public const string GRAND_PERMISSION_FAIL = "Cấp quyền cho nhân viên thất bại.";
        }

        public static class EmployeeRequest
        {
            public const string CREATE_SUCCESS = "Tạo yêu cầu thành công.";
            public const string CREATE_FAIL = "Tạo yêu cầu thất bại.";
            public const string REACHED_MAX_LEAVE_DAY = "Số ngày xin nghỉ phép phải nhỏ hơn số ngày nghỉ phép còn lại.";
            public const string EDIT_SUCCESS = "Chỉnh sửa yêu cầu thành công.";
            public const string EDIT_FAIL = "Chỉnh sửa yêu cầu thất bại.";
            public const string GET_FAIL = "Lấy thông tin yêu cầu thất bại.";
        }
        public static class EmployeeSalary
        {
            public const string IMPORT_SUCCESS = "Import bảng chấm công thành công";
            public const string IMPORT_FAIL = "Import bảng chấm công thất bại";
            public const string Download_Fail = "Download template thất bại";
            public const string Search_Fail = "Tìm kiếm thất bại";
            public const string GetTeacherSalary_Fail = "Lấy thông tin lương giảng viên thất bại";
            public const string ExportEmployeeSalary_Fail = "Export bảng lương nhân viên thất bại";
            public const string ExportTeacherSalary_Fail = "Export bảng lương giảng viên thất bại";
            public const string ExportAssistantSalary_Fail = "Export bảng lương trợ giảng thất bại";
        }

        public static class Customer
        {
            public const string CREATE_SUCCESS = "Tạo Khách hàng thành công.";
            public const string CREATE_FAIL = "Tạo Khách hàng thất bại.";
            public const string EDIT_SUCCESS = "Chỉnh sửa Khách hàng thành công.";
            public const string EDIT_FAIL = "Chỉnh sửa Khách hàng thất bại.";
            public const string DELETE_SUCCESS = "Xóa Khách hàng thành công.";
            public const string DELETE_FAIL = "Xóa Khách hàng thất bại.";
            public const string GET_FAIL = "Lấy thông tin Khách hàng thất bại.";
            public const string NO_CUS = "Không tìm thấy khách hàng.";
            public const string SEARCH_FAIL = "Tìm kiếm khách hàng thất bại.";
            public const string TIME_ERROR = "Không thể tạo lịch hẹn có thời gian bé hơn ngày hiện tại";
        }

        public static class Note
        {
            public const string CREATE_SUCCESS = "Lưu ghi chú thành công.";
            public const string CREATE_FAIL = "Lưu ghi chú thất bại.";
            public const string DISABLE_SUCCESS = "Xóa ghi chú thành công.";
            public const string DISABLE_FAIL = "Xóa ghi chú thất bại.";
            public const string EDIT_SUCCESS = "Sửa ghi chú thành công.";
            public const string EDIT_FAIL = "Sửa ghi chú thất bại.";
            public const string NO_NOTE = "Không tìm thấy ghi chú tương ứng với điều kiện tìm kiếm.";
            public const string SEARCH_NOTE_FAIL = "Có lỗi xảy ra khi tìm kiếm ghi chú.";


            // Add note
            public const string NOTE_TITLE = "Đã thêm ghi chú";
            public const string NOTE_CONTENT_SEND_APPROVAL = "Đã gửi phê duyệt thành công";
            public const string NOTE_CONTENT_APPROVAL = " đã phê duyệt thành công.";
            public const string NOTE_CONTENT_APPROVAL_DESCRIPTION = "Đã phê duyệt thành công. Ghi chú: ";
            public const string NOTE_CONTENT_APPROVAL_SUCCESS = "Đã phê duyệt thành công.";
            public const string NOTE_CONTENT_REJECT = " đã từ chối do: ";
            public const string NOTE_CONTENT_CANCEL = " đã đổi sang trạng thái Hủy";
            public const string NOTE_CONTENT_EDIT_NEW = "Chuyển trạng thái về mới thành công";
            public const string NOTE_CONTENT_Close = "Chuyển trạng thái về đóng thành công";
            public const string NOTE_CONTENT_DELETE = "Đã xóa thành công";
        }

        public static class FileUpload
        {
            public const string UPLOAD_SUCCESS = "Lưu tài liệu thành công.";
            public const string NO_FILE = "Không có tài liệu để tải lên.";
        }

        public static class Login
        {
            public const string INACTIVE_USER = "Người dùng này không có quyền truy cập hệ thống.";
            public const string WRONG_USER_PASSWORD = "Tên đăng nhập hoặc mật khẩu không đúng.";
            public const string USER_NOT_EXIST = "Tên đăng nhập này không tồn tại trong hệ thống.";
            public const string DATELINE_RESET_PASS = "Đường dẫn này đã hết hạn sử dụng. Vui lòng quay lại chức năng Quên mật khẩu";
            public const string RESET_CODE_ERR = "Đường dẫn không đúng. Vui lòng kiểm tra lại email.";
            public const string POSITION_NOT_EXIST = "Người dùng không có chức vụ trong hệ thống";
        }

        public static class Permission
        {
            public const string CREATE_SUCCESS = "Tạo quyền thành công.";
            public const string CREATE_FAIL = "Tạo quyền thất bại.";
            public const string EDIT_SUCCESS = "Chỉnh sửa quyền thành công.";
            public const string EDIT_FAIL = "Chỉnh sửa quyền thất bại.";
            public const string DELETE_SUCCESS = "Xóa quyền thành công.";
            public const string DELETE_FAIL = "Xóa quyền thất bại.";
            public const string NOT_EXIST = "Quyền không tồn tại.";
            public const string HAS_USER = "Nhóm quyền này đang được sử dụng.";
        }

        public static class Password
        {
            public const string CHANGE_SUCCESS = "Sửa mật khẩu thành công.";
            public const string CHANGE_FAIL = "Sửa mật khẩu thất bại.";
            public const string NOT_CORRECT = "Sai mật khẩu cũ.";
            public const string DUPLICATE = "Trùng với mật khẩu cũ.";
        }

        public static class User
        {
            public const string CHANGE_SUCCESS = "Thay đổi thông tin cá nhân thành công.";
            public const string CHANGE_FAIL = "Thay đổi thông tin cá nhân thất bại.";
        }

        public static class Workflow
        {
            public const string CHANGE_SUCCESS = "Gửi phê duyệt thành công.";
            public const string CHANGE_FAIL = "Gửi phê duyệt thất bại.";
            public const string APPROVE = "Phê duyệt thành công.";
            public const string REJECT = "Từ chối phê duyệt thành công.";
            public const string CREATE_SUCCESS = "Tạo quy trình làm việc thành công.";
            public const string CREATE_FAIL = "Tạo quy trình làm việc thất bại.";
            public const string SEARCH_FAIL = "Tìm kiếm quy trình thất bại.";
            public const string GET_FAIL = "Lấy thông tin quy trình thất bại.";
            public const string EDIT_SUCCESS = "Chỉnh sửa quy trình làm việc thành công.";
            public const string EDIT_FAIL = "Chỉnh sửa quy trình làm việc thất bại.";
        }

        public static class Vendor
        {
            public const string CREATE_SUCCESS = "Tạo Nhà cung cấp thành công.";
            public const string CREATE_FAIL = "Tạo Nhà cung cấp thất bại.";
            public const string CREATE_ORDER_SUCCESS = "Tạo phiếu đặt SP/DV thành công.";
            public const string CREATE_ORDER_FAIL = "Tạo phiếu đặt SP/DV thất bại.";
            public const string CREATE_BANK_SUCCESS = "Thêm thông tin thành toán thành công.";
            public const string CREATE_BANK_FAIL = "Thêm thông tin thành toán thất bại.";
            public const string SEARCH_FAIL = "Tìm kiếm Nhà cung cấp thất bại.";
            public const string SEARCH_ORDER_FAIL = "Tìm kiếm phiếu đặt SP/DV thất bại.";
            public const string SEARCH_ORDER_EMPTY = "Không tim thấy phiếu đặt SP/DV tương ứng.";
            public const string GET_FAIL = "Lấy thông tin Nhà cung cấp thất bại.";
            public const string NO_ORDER = "Đơn hàng này không tồn tại trong hệ thống.";
            public const string GET_ORDER_FAIL = "Lấy thông tin đơn hàng thất bại.";
            public const string EDIT_SUCCESS = "Chỉnh sửa Nhà cung cấp thành công.";
            public const string EDIT_FAIL = "Chỉnh sửa Nhà cung cấp thất bại.";
            public const string EDIT_ORDER_SUCCESS = "Chỉnh sửa đơn hàng thành công.";
            public const string EDIT_ORDER_FAIL = "Chỉnh sửa đơn hàng thất bại.";
            public const string EDIT_BANK_SUCCESS = "Chỉnh sửa thông tin thanh toán thành công.";
            public const string EDIT_BANK_FAIL = "Chỉnh sửa thông tin thanh toán thất bại.";
            public const string DELETE_BANK_SUCCESS = "Xóa thông tin thanh toán thành công.";
            public const string DELETE_BANK_FAIL = "Xóa thông tin thanh toán thất bại.";
            public const string DELETE_SUCCESS = "Xóa Nhà cung cấp thành công.";
            public const string DELETE_FAIL = "Xóa Nhà cung cấp thất bại.";
        }
        public static class Order
        {
            public const string CREATE_SUCCESS = "Tạo hóa đơn thành công.";
            public const string CREATE_FAIL = "Tạo hóa đơn thất bại.";
            public const string SEARCH_FAIL = "Tìm kiếm hóa đơn thất bại.";
            public const string SEARCH_ORDER_FAIL = "Tìm kiếm hóa đơn thất bại.";
            public const string SEARCH_ORDER_EMPTY = "Không tim thấy hóa đơn tương ứng.";
            public const string GET_FAIL = "Lấy thông tin hóa đơn thất bại.";
            public const string EDIT_ORDER_SUCCESS = "Chỉnh sửa đơn hàng thành công.";
            public const string EDIT_ORDER_FAIL = "Chỉnh sửa đơn hàng thất bại.";
            public const string ORDER_EXIST = "Đơn hàng đã tồn tại.";
            public const string ORDER_NO_EXIST = "Đơn hàng không tồn tại trên hệ thống.";
            public const string ORDER_NO_PAY = "Đơn hàng đã phát sinh thanh toán không được đổi khách hàng.";
            public const string ORDER_NO_CHANGE_STATUS = "Đơn hàng không được phép chuyển sang trạng thái ";
            public const string ORDER_NO_CHANGE = "Đơn hàng không được phép thay đổi";
            public const string CANCEL_APPROVAL_SUCCESS = "Hủy yêu cầu phê duyệt thành công.";
        }

        public static class BankAccount
        {
            public const string CREATE_BANK_SUCCESS = "Thêm thông tin thành toán thành công.";
            public const string CREATE_BANK_FAIL = "Thêm thông tin thành toán thất bại.";
            public const string EDIT_BANK_SUCCESS = "Chỉnh sửa thông tin thanh toán thành công.";
            public const string EDIT_BANK_FAIL = "Chỉnh sửa thông tin thanh toán thất bại.";
            public const string DELETE_BANK_SUCCESS = "Xóa thông tin thanh toán thành công.";
            public const string DELETE_BANK_FAIL = "Xóa thông tin thanh toán thất bại.";
        }

        public static class ProductCategory
        {
            public const string CREATE_SUCCESS = "Tạo nhóm sản phẩm thành công.";
            public const string CREATE_FAIL = "Tạo nhóm sản phẩm thất bại.";
            public const string EDIT_SUCCESS = "Chỉnh sửa nhóm sản phẩm thành công.";
            public const string EDIT_FAIL = "Chỉnh sửa nhóm sản phẩm thất bại.";
            public const string GET_FAIL = "Lấy thông tin nhóm sản phẩm thất bại.";
            public const string GETALL_FAIL = "Có lỗi xảy ra khi lấy toàn bộ nhóm sản phẩm.";
            public const string DELETE_SUCCESS = "Xoá nhóm sản phẩm thành công.";
            public const string DELETE_FAIL = "Xóa nhóm sản phẩm thất bại.";
            public const string HAS_CHILD = "Không thể xoá! Đã có nhóm sản phẩm con trực thuộc.";
            public const string DUPLICATE_CODE = "Đã có một sản phẩm khác sử dụng mã này.";
            public const string NO_PRODUCTCATEGORY = "Không tìm thấy nhóm sản phẩm tương ứng với điều kiện tìm kiếm.";
        }

        public static class CustomerServiceLevel
        {
            public const string ADD_SUCCESS = "Thêm cấp độ mới thành công!";
        }

        public static class PayableInvoice
        {
            public const string ADD_SUCCESS = "Tạo phiếu chi thành công!";
            public const string ADD_FAIL = "Tạo phiếu chi thất bại!";
            public const string NO_INVOICE = "Không tìm thấy phiếu chi tương ứng.";
            public const string SEARCH_FAIL = "Có lỗi xảy ra.";
        }

        public static class BankPayableInvoice
        {
            public const string ADD_SUCCESS = "Tạo phiếu ủy nhiệm chi thành công!";
            public const string ADD_FAIL = "Tạo phiếu ủy nhiệm chi thất bại!";
            public const string NO_INVOICE = "Không tìm thấy phiếu chi tương ứng.";
            public const string SEARCH_FAIL = "Có lỗi xảy ra.";
        }

        public static class ReceiptInvoice
        {
            public const string ADD_SUCCESS = "Tạo phiếu thu thành công!";
            public const string ADD_FAIL = "Tạo phiếu thu thất bại!";
            public const string NO_INVOICE = "Không tìm thấy phiếu thu tương ứng.";
        }

        public static class BankReceiptInvoice
        {
            public const string ADD_SUCCESS = "Tạo báo có thành công!";
            public const string ADD_FAIL = "Tạo báo có thất bại!";
            public const string NO_INVOICE = "Không tìm thấy báo có tương ứng.";
            public const string SEARCH_FAIL = "Có lỗi xảy ra.";
        }

        public static class SalesReport
        {
            public const string NO_ITEM = "Không tìm thấy dữ liệu tương ứng.";
            public const string SEARCH_FAIL = "Có lỗi xảy ra.";
        }

        public static class BankBook
        {
            public const string NO_ITEM = "Không tìm thấy dữ liệu tương ứng.";
            public const string SEARCH_FAIL = "Có lỗi xảy ra.";
        }

        public static class ExportExcelReceiveable
        {
            public const string FAIL = "Xuất Excel thất bại.";
        }

        public static class ProcurementRequest
        {
            public const string ADD_SUCCESS = "Tạo yêu cầu mua sắm thành công!";
            public const string ADD_FAIL = "Tạo yêu cầu mua sắm thất bại!";
        }
        public static class ProcurementPlan
        {
            public const string ADD_SUCCESS = "Tạo dự toán thành công!";
            public const string ADD_FAIL = "Tạo dự toán thất bại!";
            public const string EDIT_FAIL = "Sửa dự toán thất bại!";
            public const string EDIT_SUCCESS = "Sửa dự toán thành công!";
        }
        public static class RequestPayment
        {
            public const string ADD_SUCCESS = "Tạo yêu cầu thanh toán thành công!";
            public const string ADD_FAIL = "Tạo yêu cầu thanh toán thất bại!";
            public const string EDIT_FAIL = "Sửa yêu cầu thanh toán thất bại!";
            public const string GET_FAIL = "Tìm kiếm yêu cầu thanh toán thất bại!";
        }
        public static class Document
        {
            public const string DOWNLOAD_SUCCESS = "Download tài liệu thành công.";
            public const string DOWNLOAD_FAIL = "Lỗi xảy ra khi down tài liệu.";
        }

        public static class Quote
        {
            public const string CREATE_SUCCESS = "Tạo báo giá thành công.";
            public const string CREATE_FAIL = "Tạo báo giá thất bại.";
            public const string SEARCH_SUCCESS = "Tìm kiếm báo giá thành công.";
            public const string SEARCH_QUOTE_FAIL = "Tìm kiếm báo giá thất bại.";
            public const string SEARCH_QUOTE_EMPTY = "Không tim thấy báo giá tương ứng.";
            public const string GET_FAIL = "Lấy thông tin báo giá thất bại.";
            public const string EDIT_QUOTE_SUCCESS = "Chỉnh sửa báo giá thành công.";
            public const string EDIT_QUOTE_FAIL = "Chỉnh sửa báo giá thất bại.";
            public const string DELETE_QUOTE_SUCCESS = "Xoá báo giá thành công.";
            public const string DELETE_QUOTE_FAIL = "Xoá báo giá thất bại.";
        }

        public static class Cost
        {
            public const string CREATE_SUCCESS = "Tạo chi phí thành công.";
            public const string CREATE_FAIL = "Tạo chi phí thất bại.";
        }

        public static class SaleBidding
        {
            public const string CREATE_SUCCESS = "Tạo hồ sơ thầu thành công.";
            public const string CREATE_FAIL = "Tạo hồ sơ thầu thất bại.";
            public const string EDIT_SUCCESS = "Chỉnh sửa hồ sơ thầu thành công.";
            public const string EDIT_FAIL = "Chỉnh sửa hồ sơ thầu thất bại.";
            public const string EDIT_FAIL_STATUS = "Không thể sửa hồ sơ thầu ở trạng thái này.";
            public const string DELETE_SUCCESS = "Xoá hồ sơ thầu thành công.";
            public const string DELETE_FAIL = "Xoá hồ sơ thầu thất bại.";
            public const string GET_FAIL = "Lấy thông tin hồ sơ thầu thất bại.";
            public const string GET_PERSON_FAIL = "Lấy thông tin người phụ trách hồ sơ thầu thất bại.";
            public const string NOT_FOUND_LIST_SALEBIDDING = "Không tìm thấy hồ sơ thầu nào.";
            public const string NOT_FOUND_LEAD = "Không tìm thấy cơ hội.";
            public const string NOT_CUSTOMER = "Cơ hội này chưa được gắn với khách hàng.";
            public const string NOT_PERSONINCHARGE = "Cơ hội này chưa gán người phụ trách.";
            public const string NOT_USER = "Không tìm người đăng nhập.";
            public const string NOT_EMPLOYEE = "Người đăng nhập không phải là nhân viên.";
            public const string CREATE_FAIL_ROLE = "Bạn không có quyền tạo.";
            public const string CREATE_FAIL_LEAD_NOT_EXIT = "Cơ hội này không tồn tại.";
            public const string CREATE_FAIL_QUOTE_EXIT = "Cơ hội này đã tạo báo giá.";
            public const string CREATE_FAIL_SALEBIDDING_EXIT = "Cơ hội này đã tạo hồ sơ thầu.";
            public const string CREATE_FAIL_STATUS = "Trạng thái của cơ hội này không thể tạo hồ sơ thầu";
            public const string EDIT_FAIL_ROLE = "Bạn không có quyền sửa.";
            public const string SEND_EMAIL_FAIL = "Gửi mail thất bại.";
            public const string SEND_EMAIL_SUCCESS = "Gửi mail thành công.";
            public const string END_DATE_FAIL = "Ngày có kết quả dự kiến phải lớn hơn hoặc bằng ngày mở thầu!";
        }

        public static class Product
        {                                         
            public const string IMPORT_SUCCESS = "Nhập excel sản phẩm thành công.";
            public const string IMPORT_FAIL = "Nhập excel sản phẩm thất bại.";
        }

        public static class PriceProduct
        {
            public const string CREATE_SUCCESS = "Tạo giá thành sản phẩm thành công.";
            public const string CREATE_FAIL = "Tạo giá thành sản phẩm thất bại.";
            public const string DELETE_SUCCESS = "Xóa giá thành sản phẩm thành công.";
            public const string DELETE_FAIL = "Xóa giá thành sản phẩm thất bại.";
        }

        public static class BillSale
        {
            public const string CREATE_SUCCESS = "Tạo hóa đơn thành công.";
            public const string CREATE_FAIL = "Tạo hóa đơn thất bại.";
            public const string EDIT_SUCCESS = "Chỉnh sửa hóa đơn thành công.";
            public const string EDIT_FAIL = "Chỉnh sửa hóa đơn thất bại.";
            public const string DELETE_SUCCESS = "Xoá hóa đơn thành công.";
            public const string DELETE_FAIL = "Xoá hóa đơn thất bại.";
            public const string GET_FAIL = "Lấy thông tin hóa đơn thất bại.";
            public const string NOT_FOUND_BILL = "Không tìm thấy hóa đơn.";
            public const string NOT_FOUND_STATUS_BILL = "Không tìm thấy trạng thái hóa đơn.";
            public const string CREATE_FAIL_ROLE = "Bạn không có quyền tạo.";
            public const string EDIT_FAIL_ROLE = "Bạn không có quyền sửa.";
            public const string SEND_EMAIL_FAIL = "Gửi mail thất bại.";
            public const string SEND_EMAIL_SUCCESS = "Gửi mail thành công.";
            public const string NOT_USER = "Không tìm người đăng nhập.";
            public const string NOT_EMPLOYEE = "Người đăng nhập không phải là nhân viên.";
        }
        public static class Contract
        {
            public const string GET_DATA_SUCCESS = "Lấy dữ liệu thành công.";
            public const string GET_DATA_FAIL = "Lấy dữ liệu thất bại.";
            public const string CREATE_SUCCESS = "Tạo hợp đồng thành công.";
            public const string CREATE_FAIL = "Tạo hợp đồng sản phẩm thất bại.";
            public const string EXIST = "Hợp đồng đã tồn tại trong hệ thống";
            public const string SEARCH_SUCCESS = "Tìm kiếm thành công";
            public const string DELETE_FAIL = "Xóa hợp đồng thất bại";
            public const string DELETE_SUCCESS = "Xóa hợp đồng thành công";
            public const string DUPLICATE_CODE = "Số hợp đồng đã tồn tại";

        }
        public static class VendorPrice
        {
            public const string CREATE_SUCCESS = "Tạo giá sản phẩm NCC thành công.";
            public const string CREATE_FAIL = "Tạo giá sản phẩm NCC thất bại.";
            public const string EDIT_SUCCESS = "Cập nhật giá sản phẩm NCC thành công.";
            public const string EDIT_FAIL = "Cập nhật giá sản phẩm NCC thất bại.";
            public const string DELETE_SUCCESS = "Xóa giá sản phẩm NCC thành công.";
            public const string DELETE_FAIL = "Xóa giá sản phẩm NCC thất bại.";
            public const string IMPORT_SUCCESS = "Nhập excel bảng giá NCC thành công.";
        }

        public static class SuggestedSupplierQuoteRequest
        {
            public const string CREATE_SUCCESS = "Tạo đề nghị báo giá nhà cung cấp thành công!";
            public const string CREATE_FAIL = "Tạo đề nghị báo giá nhà cung cấp thất bại!";
            public const string EDIT_SUCCESS = "Cập nhật đề nghị báo giá nhà cung cấp thành công.";
            public const string EDIT_FAIL = "Cập nhật đề nghị báo giá nhà cung cấp thất bại.";
            public const string DELETE_SUCCESS = "Xóa đề nghị báo giá NCC thành công.";
            public const string DELETE_FAIL = "Xóa đền nghị báo giá NCC thất bại.";
            public const string CHANGE_STATUS_SUCCESS = "Cập nhật trạng thái thành công";
            public const string CHANGE_STATUS_FAIL = "Cập nhật trạng thái thất bại";
        }
    }
}
