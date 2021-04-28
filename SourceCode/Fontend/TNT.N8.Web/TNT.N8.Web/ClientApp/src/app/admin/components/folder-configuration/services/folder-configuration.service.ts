import { share, map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

class Folder {
  folderId: string;
  parentId: string;
  name: string;
  url: string;
  isDelete: boolean;
  active: boolean;
  hasChild: boolean;
  listFile: Array<FileInFolder>;
  folderType: string;
  numberFile: number;
}

class FileInFolder {
  fileInFolderId: string;
  folderId: string;
  fileName: string;
  objectId: string;
  objectType: string;
  size: string;
  active: boolean;
  fileExtension: string;
}

class FileUploadModel {
  FileInFolder: FileInFolder;
  FileSave: File;
}

@Injectable()
export class ForderConfigurationService {

  constructor(private httpClient: HttpClient) { }
  refreshLead: boolean = false;
  userId: string = JSON.parse(localStorage.getItem("auth")).UserId;


  getAllFolderDefault() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/folder/getAllFolderDefault';
    return this.httpClient.post(url, {}).pipe(map((response: Response) => {
      return response;
    }));
  }

  getAllFolderActive() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/folder/getAllFolderActive';
    return this.httpClient.post(url, {}).pipe(map((response: Response) => {
      return response;
    }));
  }

  addOrUpdateFolder(listFolder: Array<Folder>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/folder/addOrUpdateFolder';
    return this.httpClient.post(url, {
      ListFolder: listFolder
    }).pipe(map((response: Response) => {
      return response;
    }));
  }

  createFolder(folder: Folder, folderName: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/folder/createFolder';
    return this.httpClient.post(url, {
      FolderParent: folder,
      FolderName: folderName
    }).pipe(map((response: Response) => {
      return response;
    }));
  }

  deleteFolder(folder: Folder) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/folder/deleteFolder';
    return this.httpClient.post(url, {
      Folder: folder
    }).pipe(map((response: Response) => {
      return response;
    }));
  }
  downloadFile(fileInFolderId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/folder/downloadFile';
    return this.httpClient.post(url, {
      FileInFolderId: fileInFolderId
    }).pipe(map((response: Response) => {
      return response;
    }));
  }

  deleteFile(fileInFolderId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/folder/deleteFile';
    return this.httpClient.post(url, {
      FileInFolderId: fileInFolderId
    }).pipe(map((response: Response) => {
      return response;
    }));
  }

  uploadFileByFolderType(folderType: string, listFile: Array<FileUploadModel>, objectId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/folder/uploadFile';
    let formData: FormData = new FormData();
    formData.append("FolderType", folderType);
    formData.append("ObjectId", objectId);
    formData.append("UserId", this.userId);
    var index = 0;
    for (var pair of listFile) {
      formData.append("ListFile[" + index + "].FileInFolder.FileInFolderId", pair.FileInFolder.fileInFolderId);
      formData.append("ListFile[" + index + "].FileInFolder.FolderId", pair.FileInFolder.folderId);
      formData.append("ListFile[" + index + "].FileInFolder.FileName", pair.FileInFolder.fileName);
      formData.append("ListFile[" + index + "].FileInFolder.ObjectId", pair.FileInFolder.objectId);
      formData.append("ListFile[" + index + "].FileInFolder.ObjectType", pair.FileInFolder.objectType);
      formData.append("ListFile[" + index + "].FileInFolder.Size", pair.FileInFolder.size);
      formData.append("ListFile[" + index + "].FileInFolder.FileExtension", pair.FileInFolder.fileExtension);
      formData.append("ListFile[" + index + "].FileSave", pair.FileSave);
      index++;
    }
    return this.httpClient.post(url,
      formData
    ).pipe(map((response: Response) => {
      return response;
    }));
  }

  uploadFileeByFolderTypeAsync(folderType: string, listFile: Array<FileUploadModel>, objectId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/folder/uploadFile';
    let formData: FormData = new FormData();
    formData.append("FolderType", folderType);
    formData.append("ObjectId", objectId);
    formData.append("UserId", this.userId);
    var index = 0;
    for (var pair of listFile) {
      formData.append("ListFile[" + index + "].FileInFolder.FileInFolderId", pair.FileInFolder.fileInFolderId);
      formData.append("ListFile[" + index + "].FileInFolder.FolderId", pair.FileInFolder.folderId);
      formData.append("ListFile[" + index + "].FileInFolder.FileName", pair.FileInFolder.fileName);
      formData.append("ListFile[" + index + "].FileInFolder.ObjectId", pair.FileInFolder.objectId);
      formData.append("ListFile[" + index + "].FileInFolder.ObjectType", pair.FileInFolder.objectType);
      formData.append("ListFile[" + index + "].FileInFolder.Size", pair.FileInFolder.size);
      formData.append("ListFile[" + index + "].FileInFolder.FileExtension", pair.FileInFolder.fileExtension);
      formData.append("ListFile[" + index + "].FileSave", pair.FileSave);
      index++;
    }
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, formData).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  uploadFileByFolderId(folderId: string, listFile: Array<FileUploadModel>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/folder/uploadFileByFolderId';
    let formData: FormData = new FormData();
    formData.append("FolderId", folderId);
    formData.append("UserId", this.userId);
    var index = 0;
    for (var pair of listFile) {
      formData.append("ListFile[" + index + "].FileInFolder.FileInFolderId", pair.FileInFolder.fileInFolderId);
      formData.append("ListFile[" + index + "].FileInFolder.FolderId", pair.FileInFolder.folderId);
      formData.append("ListFile[" + index + "].FileInFolder.FileName", pair.FileInFolder.fileName);
      formData.append("ListFile[" + index + "].FileInFolder.Size", pair.FileInFolder.size);
      formData.append("ListFile[" + index + "].FileInFolder.FileExtension", pair.FileInFolder.fileExtension);
      formData.append("ListFile[" + index + "].FileSave", pair.FileSave);
      index++;
    }
    return this.httpClient.post(url,
      formData
    ).pipe(map((response: Response) => {
      return response;
    }));
  }
}
