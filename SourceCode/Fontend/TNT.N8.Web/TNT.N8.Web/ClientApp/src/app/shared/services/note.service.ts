
import {map} from 'rxjs/operators';
import { Injectable, Pipe } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { NoteModel } from '../models/note.model';
import { NoteDocumentModel } from '../models/note-document.model';
// tslint:disable-next-line:use-pipe-transform-interface

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

@Pipe({ name: 'NoteService' })

@Injectable()
export class NoteService {
  constructor(private httpClient: HttpClient) { }
  userId: string = JSON.parse(localStorage.getItem("auth")).UserId;

  // Create note
  createNote(noteModel: NoteModel, leadId: string, fileList: File[], userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/createNote';
    let formData: FormData = new FormData();
    if (fileList !== null) {
      for (var i = 0; i < fileList.length; i++) {
        formData.append("fileList", fileList[i]);
      }
    }

    formData.append("Note[Description]", noteModel.Description);
    formData.append("Note[Type]", noteModel.Type);
    formData.append('Note[NoteTitle]', noteModel.NoteTitle);
    formData.append('LeadId', leadId);
    formData.append('UserId', userId);

    return this.httpClient.post(url, formData).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  // Disable note
  disableNote(noteId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/disableNote';
    return this.httpClient.post(url, { NoteId: noteId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createNoteAndNoteDocument(leadId: string, fileList: File[], userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/createNoteAndNoteDocument';
    let formData: FormData = new FormData();
    for (var i = 0; i < fileList.length; i++) {
      formData.append('fileList', fileList[i]);
    }

    formData.append('LeadId', leadId);
    formData.append('UserId', userId);

    return this.httpClient.post(url, formData).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  editNoteById(noteId: string, noteDescription: string, fileList: File[], leadId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/editNoteById';
    const formData: FormData = new FormData();
    for (let i = 0; i < fileList.length; i++) {
      formData.append('FileList', fileList[i]);
    }
    formData.append('LeadId', leadId);
    formData.append('NoteId', noteId);
    formData.append('NoteDescription', noteDescription);

    return this.httpClient.post(url, formData).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  searchNote(keyword: string, fromDate: Date, toDate: Date, leadId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/searchNote';
    return this.httpClient.post(url, { Keyword: keyword, FromDate: fromDate, ToDate: toDate, LeadId: leadId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createNoteForCustomerDetail(noteModel: NoteModel, listNoteDocumentModel: Array<NoteDocumentModel>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/createNoteForCustomerDetail';
    return this.httpClient.post(url, { Note: noteModel, ListNoteDocument: listNoteDocumentModel }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createNoteForLeadDetail(noteModel: NoteModel, listNoteDocumentModel: Array<NoteDocumentModel>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/createNoteForLeadDetail';
    return this.httpClient.post(url, { Note: noteModel, ListNoteDocument: listNoteDocumentModel }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createNoteForOrderDetail(noteModel: NoteModel, listNoteDocumentModel: Array<NoteDocumentModel>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/createNoteForOrderDetail';
    return this.httpClient.post(url, { Note: noteModel, ListNoteDocument: listNoteDocumentModel }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createNoteForQuoteDetail(noteModel : NoteModel, listNoteDocumentModel : Array<NoteDocumentModel>){
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/createNoteForQuoteDetail';
    return this.httpClient.post(url, {Note: noteModel, ListNoteDocument : listNoteDocumentModel}).pipe(
      map((response : Response) =>{
        return response;
      }));
  }
  createNoteForSaleBiddingDetail(noteModel : NoteModel, listNoteDocumentModel : Array<NoteDocumentModel>){
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/createNoteForSaleBiddingDetail';
    return this.httpClient.post(url, {Note: noteModel, ListNoteDocument : listNoteDocumentModel}).pipe(
      map((response : Response) =>{
        return response;
      }));
  }
  createNoteForBillSaleDetail(noteModel : NoteModel, listNoteDocumentModel : Array<NoteDocumentModel>){
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/createNoteForBillSaleDetail';
    return this.httpClient.post(url, {Note: noteModel, ListNoteDocument : listNoteDocumentModel}).pipe(
      map((response : Response) =>{
        return response;
      }));
  }
  createNoteForContract(noteModel : NoteModel, listNoteDocumentModel : Array<NoteDocumentModel>){
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/createNoteForContract';
    return this.httpClient.post(url, {Note: noteModel, ListNoteDocument : listNoteDocumentModel}).pipe(
      map((response : Response) =>{
        return response;
      }));
  }

  createNoteForObject(noteModel : NoteModel, listFile : Array<FileUploadModel>, folderType: string){
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/createNoteForObject';
    let formData: FormData = new FormData();

    formData.append("UserId", this.userId);

    formData.append('Note[NoteId]', noteModel.NoteId);
    formData.append('Note[Description]', noteModel.Description);
    formData.append('Note[Type]', noteModel.Type);
    formData.append('Note[ObjectId]', noteModel.ObjectId);
    formData.append('Note[ObjectType]', noteModel.ObjectType);
    formData.append('Note[NoteTitle]', noteModel.NoteTitle);
    formData.append('Note[CreatedById]', noteModel.CreatedById);
    formData.append('Note[CreatedDate]', null);

    formData.append("FolderType", folderType);

    var index = 0;
    for (var pair of listFile) {
      formData.append("ListFile[" + index + "].FileInFolder.FileInFolderId",pair.FileInFolder.fileInFolderId);
      formData.append("ListFile[" + index + "].FileInFolder.FolderId", pair.FileInFolder.folderId);
      formData.append("ListFile[" + index + "].FileInFolder.FileName", pair.FileInFolder.fileName);
      formData.append("ListFile[" + index + "].FileInFolder.ObjectId", pair.FileInFolder.objectId);
      formData.append("ListFile[" + index + "].FileInFolder.ObjectType", pair.FileInFolder.objectType);
      formData.append("ListFile[" + index + "].FileInFolder.Size", pair.FileInFolder.size);
      formData.append("ListFile[" + index + "].FileInFolder.FileExtension", pair.FileInFolder.fileExtension);
      formData.append("ListFile[" + index + "].FileSave", pair.FileSave);
      index++;
    }

    return this.httpClient.post(url, formData).pipe(
      map((response : Response) =>{
        return response;
      }));
  }
  
}
