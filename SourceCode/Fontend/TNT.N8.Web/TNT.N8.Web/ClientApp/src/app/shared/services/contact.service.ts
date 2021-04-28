
import {map} from 'rxjs/operators';
import { Injectable, Pipe } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ContactModel } from '../models/contact.model';

interface OtherContact {
  contactId: string,
  objectId: string,
  objectType: string,
  firstName: string,
  lastName: string,
  contactName: string,
  gender: string,
  genderName: string,
  dateOfBirth: Date,
  address: string,
  role: string, //Chức vụ
  phone: string,
  email: string,
  other: string,  //Thông tin khác
}

interface PersonalCustomerContact {
  ContactId: string,
  Email: string,
  WorkEmail: string,
  OtherEmail: string,
  Phone: string,
  WorkPhone: string,
  OtherPhone: string,
  ProvinceId: string,
  DistrictId: string,
  WardId: string,
  Address: string,
  Other: string
}

@Pipe({ name: 'ContactService' })
@Injectable()
export class ContactService {

    constructor(private httpClient: HttpClient) { }

  createContact(contact: OtherContact) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/contact/create';
    return this.httpClient.post(url, { Contact: contact }).pipe(
      map((response: Response) => {
        return response;
      }));
    }

  getContactById(contactId: string, userId: string) {
      const url = localStorage.getItem('ApiEndPoint') + '/api/contact/getContactById';
    return this.httpClient.post(url, { ContactId: contactId, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getContactByObjectId(objectId: string, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/contact/getContactByObjectId';
    return this.httpClient.post(url, { ObjectId: objectId, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  editContactById(contact: ContactModel, userId: string) {
      const url = localStorage.getItem('ApiEndPoint') + '/api/contact/editContactById';
    return this.httpClient.post(url, { Contact: contact, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  deleteContactById(contactId: string, objectId: string, objectType: string) {
      const url = localStorage.getItem('ApiEndPoint') + '/api/contact/deleteContactById';
      return this.httpClient.post(url, 
      { 
        ContactId: contactId, 
        ObjectId: objectId, 
        ObjectType: objectType 
      }).pipe(map((response: Response) => {
          return response;
        }));
  }

  getAllCountry(userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/country/getAllCountry';
    return this.httpClient.post(url, {  UserId: userId }).pipe(
    map((response: Response) => {
      return response;
    }));
  }

  updatePersonalCustomerContact(contact: PersonalCustomerContact) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/contact/updatePersonalCustomerContact';
    return this.httpClient.post(url, {  Contact: contact }).pipe(
    map((response: Response) => {
      return response;
    }));
  }

  getAddressByObject(objectId: string, objectType: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/contact/getAddressByObject';
    return this.httpClient.post(url, { ObjectId: objectId, ObjectType: objectType }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
}
