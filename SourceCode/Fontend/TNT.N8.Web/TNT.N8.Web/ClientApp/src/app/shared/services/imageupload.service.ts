
import {map} from 'rxjs/operators';
import { Injectable, Pipe } from '@angular/core';
import { HttpClient } from '@angular/common/http';
// tslint:disable-next-line:use-pipe-transform-interface
@Pipe({ name: 'ImageUploadService' })

@Injectable()
export class ImageUploadService {

  constructor(private httpClient: HttpClient) { }

  auth: any = JSON.parse(localStorage.getItem("auth"));

  uploadImage(base64: string, imageName: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/file/uploadImage';
    return this.httpClient.post(url, { Base64Img: base64, ImageName: imageName }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  uploadFile(fileList: File[]) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/file/uploadFile';
    let formData: FormData = new FormData();
    for (var i = 0; i < fileList.length; i++) {
      formData.append('fileList', fileList[i]);
    }
    return this.httpClient.post(url, formData).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  uploadFileForOptionAsync(fileList: File[], option: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/file/uploadFileForOption';
    let formData: FormData = new FormData();
    formData.append('Option', option);
    formData.append('UserId', this.auth.UserId);

    for (var i = 0; i < fileList.length; i++) {
      formData.append('fileList', fileList[i]);
    }

    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, formData).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  deleteFileForOptionAsync(option: string, fileName: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/file/deleteFileForOption';

    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { Option: option, FileName: fileName }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  uploadFileAsync(fileList: File[]) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/file/uploadFile';
    let formData: FormData = new FormData();
    for (var i = 0; i < fileList.length; i++) {
      formData.append('fileList', fileList[i]);
    }
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, formData).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  uploadProductImageAsync(fileList: File[]) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/file/uploadProductImage';
    let formData: FormData = new FormData();
    for (var i = 0; i < fileList.length; i++) {
      formData.append('fileList', fileList[i]);
    }
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, formData).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  downloadFile(filename: string, fileurl: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/file/downloadFile';
    return this.httpClient.post(url, { FileName: filename, FileUrl: fileurl }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
}
