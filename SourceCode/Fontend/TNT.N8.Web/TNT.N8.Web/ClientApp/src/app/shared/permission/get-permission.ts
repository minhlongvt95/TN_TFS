import { Injectable } from "@angular/core";
@Injectable()
export class GetPermission {
    getPermission(listPermissionResource: string, resource: string) {
        let listAllPermission: Array<string> = listPermissionResource.split(",");
        
        let listCurrentActionResource: Array<string> = [];
        listAllPermission.forEach(item => 
        {
            if (item.toLowerCase().indexOf(resource.toLowerCase()) != -1) 
            {
                let action = item.toLowerCase().slice(item.toLowerCase().lastIndexOf("/") + 1);
                listCurrentActionResource.push(action);
            }
        });
        if (listCurrentActionResource.length <= 0)
        {
            //Nếu User không có quyền gì thì chuyển về trang home
            return { status: false, listCurrentActionResource: listCurrentActionResource }
        }
        if (listCurrentActionResource.indexOf("view") == -1) 
        {
            //Nếu User không có quyền view thì chuyển về trang home
            return { status: false, listCurrentActionResource: listCurrentActionResource }
        }
        return { status: true, listCurrentActionResource: listCurrentActionResource }
    }
}
