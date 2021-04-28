
import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ProcurementRequestModel } from '../models/procurementRequest.model';
import { ProcurementRequestItemModel } from '../models/procurementRequestItem.model';

@Injectable()
export class ProcurementRequestService {
	constructor(private httpClient: HttpClient) { }

	createProcurementRequest(files: File[], pr: ProcurementRequestModel, prItemList: Array<ProcurementRequestItemModel>, userId: string) {
		const url = localStorage.getItem('ApiEndPoint') + '/api/procurement/createProcurementRequest';
		let formData: FormData = new FormData();
		let _string: string = ""
		prItemList.forEach(element => {
			_string = JSON.stringify({
				ProcurementRequestItemId: element.procurementRequestItemId,
				ProductId: element.productId,
				ProductName: element.productName,
				ProductCode: element.productCode,
				VendorId: element.vendorId,
				VendorName: element.vendorName,
				Quantity: element.quantity,
				UnitPrice: element.unitPrice,
				CurrencyUnit: element.currencyUnit,
				ExchangeRate: element.exchangeRate,
				Unit: element.unit,
				UnitString: element.unitString,
				TotalPrice: element.totalPrice,
				ProcurementRequestId: element.procurementRequestId,
				ProcurementPlanId: element.procurementPlanId,
				ProcurementPlanCode: element.procurementPlanCode,
				CreatedById: element.createdById,
				CreatedDate: element.createdDate,
				UpdatedById: element.updatedById,
				UpdatedDate: element.updatedDate,
				Description: element.description,
				DiscountType: element.discountType,
				DiscountValue: element.discountValue,
				IncurredUnit: element.incurredUnit,
				OrderDetailType: element.orderDetailType,
				Vat: element.vat,
				OrderNumber: element.orderNumber,
			})
			formData.append('ProcurementRequestItemList', _string);
		})

		formData.append('ProcurementRequest[ProcurementRequestId]', pr.procurementRequestId);
		formData.append('ProcurementRequest[ProcurementCode]', pr.procurementCode);
		formData.append('ProcurementRequest[ProcurementContent]', pr.procurementContent);
		formData.append('ProcurementRequest[RequestEmployeeId]', pr.requestEmployeeId);
		formData.append('ProcurementRequest[EmployeePhone]', pr.employeePhone);
		formData.append('ProcurementRequest[Unit]', pr.unit);
		formData.append('ProcurementRequest[ApproverId]', pr.approverId);
		formData.append('ProcurementRequest[ApproverPostion]', pr.approverPostion);
		formData.append('ProcurementRequest[Explain]', pr.explain);
		formData.append('ProcurementRequest[OrderId]', pr.orderId);
		formData.append('ProcurementRequest[StatusId]', pr.statusId);
		formData.append('ProcurementRequest[CreatedById]', pr.createdById);
		formData.append('ProcurementRequest[CreatedDate]', null);
		formData.append('ProcurementRequest[UpdatedById]', null);
		formData.append('ProcurementRequest[UpdatedDate]', null);

		for (var i = 0; i < files.length; i++) {
			formData.append('FileList', files[i]);
		}
		formData.append('UserId', userId);
		return this.httpClient.post(url, formData).pipe(
			map((response: Response) => {
				return response;
			}));
	}

	getAllProcurementPlan(userId: string) {
		const url = localStorage.getItem('ApiEndPoint') + '/api/budget/getAllBudget';
		return this.httpClient.post(url, { UserId: userId }).pipe(map((response: Response) => {
			return response;
		}));
	}
	searchProcurementRequest(code: string, fromDate: Date, toDate: Date, listRequester: Array<string>, listStatus: Array<string>, orgId: string, userId: string, listProductIds: Array<string>) {
		let url = localStorage.getItem('ApiEndPoint') + "/api/procurementRequest/search";
		return this.httpClient.post(url, {
			ProcurementRequestCode: code,
			FromDate: fromDate,
			ToDate: toDate,
			ListRequester: listRequester,
			ListStatus: listStatus,
			ListProduct: listProductIds,
			OrganizationId: orgId,
			UserId: userId
		}).pipe(
			map((response: Response) => {
				return response;
			}));
	}

	searchVendorProductPrice(productId: string, vendorId: string, quantity: number) {
		let url = localStorage.getItem('ApiEndPoint') + '/api/procurementRequest/searchVendorProductPrice';
		return this.httpClient.post(url, { ProductId: productId, VendorId: vendorId, Quantity: quantity }).pipe(
		  map((response: Response) => {
			return response;
		  }));
	}

	searchProcurementRequestReport(code: string, fromDate: Date, toDate: Date, listRequester: Array<string>, listStatus: Array<string>, orgId: string, userId: string, listProductIds: Array<string>, 
		procurementRequestContent: string, listVendor: Array<string>, listApproval: Array<string>, listBudget: Array<string>) {
		let url = localStorage.getItem('ApiEndPoint') + "/api/procurementRequest/searchReport";
		return this.httpClient.post(url, {
			ProcurementRequestCode: code,
			FromDate: fromDate,
			ToDate: toDate,
			ListRequester: listRequester,
			ListStatus: listStatus,
			ListProduct: listProductIds,
			OrganizationId: orgId,
			ProcurementRequestContent: procurementRequestContent,
			ListVendor: listVendor,
			ListApproval: listApproval,
			ListBudget: listBudget,
			UserId: userId
		}).pipe(
			map((response: Response) => {
				return response;
			}));
	}
	getProcurementRequestById(procurementRequestId: string, userId: string) {
		const url = localStorage.getItem('ApiEndPoint') + '/api/procurement/getProcurementRequestById';
		return this.httpClient.post(url, { ProcurementRequestId: procurementRequestId, UserId: userId }).pipe(map((response: Response) => {
			return response;
		}));
	}
	getProcurementRequestByIdAsync(procurementRequestId: string, userId: string) {
		let url = localStorage.getItem('ApiEndPoint') + "/api/procurement/getProcurementRequestById";
		return new Promise((resolve, reject) => {
			return this.httpClient.post(url, { ProcurementRequestId: procurementRequestId, UserId: userId }).toPromise()
				.then((response: Response) => {
					resolve(response);
				});
		});
	}
	/* editProcurementRequest(procurementRequest: ProcurementRequestModel, lstProItem: Array<ProcurementRequestItemModel>, lstItemToDelete: Array<string>, lstRestOfDoc: Array<string>, listFile: File[], userId: string) {
		const url = localStorage.getItem('ApiEndPoint') + '/api/procurement/editProcurementRequest';
		let formData: FormData = new FormData();
		let _string: string = ""
		lstProItem.forEach(element => {
			_string = JSON.stringify({
				// ProcurementRequestItemId: element.ProcurementRequestItemId,
				// ProductId: element.ProductId,
				// VendorId: element.VendorId,
				// Quantity: element.Quantity,
				// UnitPrice: element.UnitPrice,
				// ProcurementRequestId: element.ProcurementRequestId,
				// ProcurementPlanId: element.ProcurementPlanId,
				// CreatedById: element.CreatedById,
				// CreatedDate: element.CreatedDate,
				// UpdatedById: element.UpdatedById,
				// UpdatedDate: element.UpdatedDate,
				ProcurementRequestItemId: element.ProcurementRequestItemId,
				ProductId: element.ProductId,
				ProductName: element.ProductName,
				ProductCode: element.ProductCode,
				VendorId: element.VendorId,
				VendorName: element.VendorName,
				Quantity: element.Quantity,
				UnitPrice: element.UnitPrice,
				CurrencyUnit: element.CurrencyUnit,
				ExchangeRate: element.ExchangeRate,
				Unit: element.Unit,
				UnitString: element.UnitString,
				TotalPrice: element.TotalPrice,
				ProcurementRequestId: element.ProcurementRequestId,
				ProcurementPlanId: element.ProcurementPlanId,
				ProcurementPlanCode: element.ProcurementPlanCode,
				CreatedById: element.CreatedById,
				CreatedDate: element.CreatedDate,
				UpdatedById: element.UpdatedById,
				UpdatedDate: element.UpdatedDate,
			})
			formData.append('ListProcurementRequestItem', _string);
		});
		formData.append('ProcurementRequest[ProcurementRequestId]', procurementRequest.ProcurementRequestId);
		formData.append('ProcurementRequest[ProcurementCode]', procurementRequest.ProcurementCode);
		formData.append('ProcurementRequest[ProcurementContent]', procurementRequest.ProcurementContent);
		formData.append('ProcurementRequest[RequestEmployeeId]', procurementRequest.RequestEmployeeId);
		formData.append('ProcurementRequest[EmployeePhone]', procurementRequest.EmployeePhone);
		formData.append('ProcurementRequest[Unit]', procurementRequest.Unit);
		formData.append('ProcurementRequest[ApproverId]', procurementRequest.ApproverId);
		formData.append('ProcurementRequest[ApproverPostion]', procurementRequest.ApproverPostion);
		formData.append('ProcurementRequest[Explain]', procurementRequest.Explain);
		formData.append('ProcurementRequest[StatusId]', procurementRequest.StatusId);
		formData.append('ProcurementRequest[CreatedById]', procurementRequest.CreatedById);
		formData.append('ProcurementRequest[CreatedDate]', null);
		formData.append('ProcurementRequest[UpdatedById]', null);
		formData.append('ProcurementRequest[UpdatedDate]', null);
		for (var i = 0; i < listFile.length; i++) {
			formData.append('FileList', listFile[i]);
		}
		for (var i = 0; i < lstRestOfDoc.length; i++) {
			formData.append('lstDocument', lstRestOfDoc[i]);
		}
		for (var i = 0; i < lstItemToDelete.length; i++) {
			formData.append('ListItemToDelete', lstItemToDelete[i]);
		}
		formData.append('UserId', userId);

		return this.httpClient.post(url, formData).pipe(map((response: Response) => {
			return response;
		}));
	} */

	editProcurementRequestAsync(procurementRequest: ProcurementRequestModel, listProcurementItem: Array<ProcurementRequestItemModel>, listDocumentId: Array<string>, listFile: File[], userId: string) {
		const url = localStorage.getItem('ApiEndPoint') + '/api/procurement/editProcurementRequest';
		let formData: FormData = new FormData();
		let _string: string = ""
		listProcurementItem.forEach(element => {
			_string = JSON.stringify({
				ProcurementRequestItemId: element.procurementRequestItemId,
				ProductId: element.productId,
				ProductName: element.productName,
				ProductCode: element.productCode,
				VendorId: element.vendorId,
				VendorName: element.vendorName,
				Quantity: element.quantity,
				QuantityApproval: element.quantityApproval,
				UnitPrice: element.unitPrice,
				CurrencyUnit: element.currencyUnit,
				ExchangeRate: element.exchangeRate,
				Unit: element.unit,
				UnitString: element.unitString,
				TotalPrice: element.totalPrice,
				ProcurementRequestId: element.procurementRequestId,
				ProcurementPlanId: element.procurementPlanId,
				ProcurementPlanCode: element.procurementPlanCode,
				CreatedById: element.createdById,
				CreatedDate: element.createdDate,
				UpdatedById: element.updatedById,
				UpdatedDate: element.updatedDate,
				Description: element.description,
				DiscountType: element.discountType,
				DiscountValue: element.discountValue,
				IncurredUnit: element.incurredUnit,
				OrderDetailType: element.orderDetailType,
				Vat: element.vat,
				OrderNumber: element.orderNumber,
			})
			formData.append('ListProcurementRequestItem', _string);
		});
		formData.append('ProcurementRequest[ProcurementRequestId]', procurementRequest.procurementRequestId);
		formData.append('ProcurementRequest[ProcurementCode]', procurementRequest.procurementCode);
		formData.append('ProcurementRequest[ProcurementContent]', procurementRequest.procurementContent);
		formData.append('ProcurementRequest[RequestEmployeeId]', procurementRequest.requestEmployeeId);
		formData.append('ProcurementRequest[EmployeePhone]', procurementRequest.employeePhone);
		formData.append('ProcurementRequest[Unit]', procurementRequest.unit);
		formData.append('ProcurementRequest[ApproverId]', procurementRequest.approverId);
		formData.append('ProcurementRequest[ApproverPostion]', procurementRequest.approverPostion);
		formData.append('ProcurementRequest[Explain]', procurementRequest.explain);
		formData.append('ProcurementRequest[StatusId]', procurementRequest.statusId);
		formData.append('ProcurementRequest[CreatedById]', procurementRequest.createdById);
		formData.append('ProcurementRequest[CreatedDate]', null);
		formData.append('ProcurementRequest[UpdatedById]', null);
		formData.append('ProcurementRequest[UpdatedDate]', null);
		for (var i = 0; i < listFile.length; i++) {
			formData.append('FileList', listFile[i]);
		}
		for (var i = 0; i < listDocumentId.length; i++) {
			formData.append('ListDocumentId', listDocumentId[i]);
		}
		// for (var i = 0; i < lstItemToDelete.length; i++) {
		// 	formData.append('ListItemToDelete', lstItemToDelete[i]);
		// }
		formData.append('UserId', userId);

		// return this.httpClient.post(url, formData).pipe(map((response: Response) => {
		// 	return response;
		// }));
		return new Promise((resolve, reject) => {
			return this.httpClient.post(url, formData).toPromise()
				.then((response: Response) => {
					resolve(response);
				});
		});
	}



	getProcurementPlanById(procurementPlanId: string, userId: string) {
		const url = localStorage.getItem('ApiEndPoint') + '/api/budget/getBudgetById';
		return this.httpClient.post(url, { ProcurementPlanId: procurementPlanId, UserId: userId }).pipe(map((response: Response) => {
			return response;
		}));
	}

	getDataCreateProcurementRequest(useId: string) {
		let url = localStorage.getItem('ApiEndPoint') + '/api/procurement/getDataCreateProcurementRequest';
		return new Promise((resolve, reject) => {
			return this.httpClient.post(url, {
				UserId: useId
			}).toPromise()
				.then((response: Response) => {
					resolve(response);
				});
		});
	}

	getDataCreateProcurementRequestItem(useId: string) {
		let url = localStorage.getItem('ApiEndPoint') + '/api/procurement/getDataCreateProcurementRequestItem';
		return new Promise((resolve, reject) => {
			return this.httpClient.post(url, {
				UserId: useId
			}).toPromise()
				.then((response: Response) => {
					resolve(response);
				});
		});
	}

	getDataEditProcurementRequest(procurementRequestId: string, useId: string) {
		let url = localStorage.getItem('ApiEndPoint') + '/api/procurement/getDataEditProcurementRequest';
		return new Promise((resolve, reject) => {
			return this.httpClient.post(url, {
				ProcurementRequestId: procurementRequestId,
				UserId: useId
			}).toPromise()
				.then((response: Response) => {
					resolve(response);
				});
		});
	}

	approvalOrReject(procurementRequestId: string, useId: string, isApproval: boolean, description: string, listProcurementItem: Array<ProcurementRequestItemModel>) {
		let url = localStorage.getItem('ApiEndPoint') + '/api/procurement/approvalOrReject';
		
		return new Promise((resolve, reject) => {
			return this.httpClient.post(url, {
				ProcurementRequestId: procurementRequestId,
				IsAprroval: isApproval,
				Description: description,
				ListProcurementRequestItem: listProcurementItem,
				UserId: useId
			}).toPromise()
				.then((response: Response) => {
					resolve(response);
				});
		});
	}

	changeStatus(procurementRequestId: string, useId: string, type: string) {
		let url = localStorage.getItem('ApiEndPoint') + '/api/procurement/changeStatus';
		return new Promise((resolve, reject) => {
			return this.httpClient.post(url, {
				ProcurementRequestId: procurementRequestId,
				Description: type,
				UserId: useId
			}).toPromise()
				.then((response: Response) => {
					resolve(response);
				});
		});
	}

	getMasterDataSearchProcurementRequest() {
		let url = localStorage.getItem('ApiEndPoint') + '/api/procurement/getMasterDataSearchProcurementRequest';
		return this.httpClient.post(url, { }).pipe(
			map((response: Response) => {
			  return response;
			}));
	}

}