"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var GetPermission = /** @class */ (function () {
    function GetPermission() {
    }
    GetPermission.prototype.getPermission = function (listPermissionResource, resource) {
        var listAllPermission = listPermissionResource.split(",");
        var listCurrentActionResource = [];
        listAllPermission.forEach(function (item) {
            if (item.toLowerCase().indexOf(resource.toLowerCase()) != -1) {
                var action = item.toLowerCase().slice(item.toLowerCase().lastIndexOf("/") + 1);
                listCurrentActionResource.push(action);
            }
        });
        if (listCurrentActionResource.length <= 0) {
            //Nếu User không có quyền gì thì chuyển về trang home
            return { status: false, listCurrentActionResource: listCurrentActionResource };
        }
        if (listCurrentActionResource.indexOf("view") == -1) {
            //Nếu User không có quyền view thì chuyển về trang home
            return { status: false, listCurrentActionResource: listCurrentActionResource };
        }
        return { status: true, listCurrentActionResource: listCurrentActionResource };
    };
    return GetPermission;
}());
exports.GetPermission = GetPermission;
//# sourceMappingURL=get-permission.js.map