
import { map } from 'rxjs/operators';
import { Injectable, Pipe } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ContactModel } from '../models/contact.model';
import { OrganizationModel } from '../models/organization.model';

@Pipe({ name: 'OrganizationService' })
@Injectable()
export class OrganizationService {

  constructor(private httpClient: HttpClient) { }

  getAllOrganization() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/organization/getAllOrganization";
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getAllOrganizationAsync() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/organization/getAllOrganization";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createOrganization(organizationName: string,
    organizationCode: string,
    phone: string,
    address: string,
    level: Number,
    parentId: string,
    isFI: boolean,
    geographicalAreaId: string,
    provinceId: string,
    districtId: string,
    wardId: string,
    satelliteId: string,
    organizationOtherCode: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/organization/createOrganization";
    return this.httpClient.post(url, {
      OrganizationName: organizationName, OrganizationCode: organizationCode, Phone: phone,
      Address: address, Level: level, ParentId: parentId, IsFinancialIndependence: isFI,
      GeographicalAreaId: geographicalAreaId,
      ProvinceId: provinceId,
      DistrictId: districtId,
      WardId: wardId,
      SatelliteId: satelliteId,
      OrganizationOtherCode: organizationOtherCode
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getOrganizationById(orgId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/organization/getOrganizationById";
    return this.httpClient.post(url, { OrganizationId: orgId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  editOrganizationById(orgId: string, orgName: string, orgCode: string, orgPhone: string, orgAddress: string, isFI: boolean) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/organization/editOrganizationById";
    return this.httpClient.post(url, {
      OrganizationId: orgId, OrganizationName: orgName, OrganizationCode: orgCode,
      Address: orgAddress, Phone: orgPhone, IsFinancialIndependence: isFI
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  deleteOrganizationById(orgId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/organization/deleteOrganizationById";
    return this.httpClient.post(url, { OrganizationId: orgId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getAllOrganizationCode() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/organization/getAllOrganizationCode";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getFinancialindependenceOrg() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/organization/getFinancialindependenceOrg";
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getFinancialindependenceOrgAsync() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/organization/getFinancialindependenceOrg";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getOrganizationByEmployeeId(employeeId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/organization/getOrganizationByEmployeeId';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { EmployeeId: employeeId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getChildrenByOrganizationId(organizationId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/organization/getChildrenByOrganizationId";
    return this.httpClient.post(url, { OrganizationId: organizationId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateOrganizationById(organizationId: string, organizationName: string, organizationCode: string,
    organizationPhone: string, organizationAddress: string, isFinancialIndependence: boolean,
    GeographicalAreaId: string,
    ProvinceId: string,
    DistrictId: string,
    WardId: string,
    SatelliteId: string,
    organizationOtherCode: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/organization/updateOrganizationById";
    return this.httpClient.post(url, {
      OrganizationId: organizationId,
      OrganizationName: organizationName,
      OrganizationCode: organizationCode,
      OrganizationPhone: organizationPhone,
      OrganizationAddress: organizationAddress,
      IsFinancialIndependence: isFinancialIndependence,
      GeographicalAreaId: GeographicalAreaId,
      ProvinceId: ProvinceId,
      DistrictId: DistrictId,
      WardId: WardId,
      SatelliteId: SatelliteId,
      organizationOtherCode: organizationOtherCode
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getOrganizationByUser(UserId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/organization/getOrganizationByUser';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        UserId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }
}
