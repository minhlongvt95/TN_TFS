import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { NgxLoadingComponent, ngxLoadingAnimationTypes } from 'ngx-loading';
import { Observable, Subscription } from 'rxjs';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatDialogRef, MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';

import { OrganizationpopupComponent } from '../../../../shared/components/organizationpopup/organizationpopup.component';
import { PaymentRequestModel } from '../../../models/paymentRequest.model';
import { Router, ActivatedRoute } from '@angular/router';
import { SuccessComponent } from '../../../../shared/toast/success/success.component';
import { FailComponent } from '../../../../shared/toast/fail/fail.component';

import { TranslateService } from '@ngx-translate/core';
import { PositionService } from '../../../../shared/services/position.service';
import { CategoryService } from '../../../../shared/services/category.service';
import { EmployeeService } from '../../../../employee/services/employee.service';
import { startWith, map } from 'rxjs/operators';
import { ImageUploadService } from '../../../../shared/services/imageupload.service';
import { ContactService } from '../../../../shared/services/contact.service';
import { PaymentRequestService } from '../../../services/payment-request.service';
import { WorkflowService } from '../../../../admin/components/workflow/services/workflow.service';

@Component({
	selector: 'app-payment-request-create',
	templateUrl: './payment-request-create.component.html',
	styleUrls: ['./payment-request-create.component.css']
})
export class PaymentRequestCreateComponent implements OnInit {

	@ViewChild('ngxLoading') ngxLoadingComponent: NgxLoadingComponent;
	public ngxLoadingAnimationTypes = ngxLoadingAnimationTypes;
	loadingConfig: any = {
		'animationType': ngxLoadingAnimationTypes.circle,
		'backdropBackgroundColour': 'rgba(0,0,0,0.1)',
		'backdropBorderRadius': '4px',
		'primaryColour': '#ffffff',
		'secondaryColour': '#999999',
		'tertiaryColour': '#ffffff'
	}
	loading: boolean = false;
	auth: any = JSON.parse(localStorage.getItem("auth"));
	pageFirstLoad: boolean = true;
	postionList: Array<any> = [];
	listStatus: Array<any> = [];
	listPaymenMethod: Array<any> = [];
	employeeList: Array<any> = [];
	filteredEmployee: Observable<string[]>;
	filteredEmployee2: Observable<string[]>;
	emptyString: string = '';
	emptyGuid: string = '00000000-0000-0000-0000-000000000000';
	createPRForm: FormGroup;
	dialogPopup: MatDialogRef<OrganizationpopupComponent>;
	selectedOrgName: string = '';
	totalPRprice: number = 0;
	submitted: boolean = false;
	failConfig: MatSnackBarConfig = { panelClass: 'fail-dialog', horizontalPosition: 'end', duration: 5000 };
	successConfig: MatSnackBarConfig = { panelClass: 'success-dialog', horizontalPosition: 'end', duration: 5000 };
	featureCode: string = 'PDDXTT';
	isApproved: boolean = false;
	isRejected: boolean = false;
	isInApprovalProgress: boolean = true;

	prModel = new PaymentRequestModel();

	//Khai bao cac variable de upload file
	accept = 'image/*, video/*, audio/*, .zip, .rar, .pdf, .xls, .xlsx, .doc, .docx, .ppt, .pptx, .txt';
	files: File[] = [];
	progress: number;
	hasBaseDropZoneOver = false;
	lastFileAt: Date;
	httpEmitter: Subscription;
	sendableFormData: any;
	maxSize: number = 11000000;
	lastInvalids: any;
	baseDropValid: any;
	dragFiles: any;

	constructor(private translate: TranslateService,
		public snackBar: MatSnackBar,
		private route: ActivatedRoute,
		private router: Router,
		public dialog: MatDialog,
		private el: ElementRef,
		private formBuilder: FormBuilder,
		private positionService: PositionService,
		private cateService: CategoryService,
		private employeeService: EmployeeService,
		private workflowService: WorkflowService,
		private imageService: ImageUploadService,
		private contactService: ContactService,
		private paymentRequestService: PaymentRequestService
	) { }

	async ngOnInit() {
		const phonePattern = '^([+84]|[84]|0)+[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$';
		this.createPRForm = this.formBuilder.group({
			contentControl: ['', [Validators.required]],
			ordererControl: [{ value: '', disabled: true }, [Validators.required]],
			phoneControl: ['', [Validators.pattern(phonePattern)]],
			orgControl: [{ value: '' }, [Validators.required]],
			approverControl: ['', [Validators.required]],
			positionControl: [{ disabled: true, value: '' }],
			descriptionControl: [''],
			totalMoneyControl: ['', [Validators.required]],
			paymentMethodControl: ['', [Validators.required]]
		});
		await this.setDefaultValueModel();
		await this.getMasterData();
	}
	// Khai báo bi?n ki?m tra option autocomplete có du?c ch?n dúng chua ???
	validationPickAutoComplete = true;
	checkPickedOption(event) {
		if (this.prModel.ApproverId == null || this.prModel.ApproverId == "") {
			this.validationPickAutoComplete = false;
		} else {
			const approver = this.employeeList.find(emp => emp.employeeId == this.prModel.ApproverId);
			if (approver != undefined) {
				if (event.target.value.trim() !== (approver.employeeCode.trim() + ' - ' + approver.employeeName.trim())) {
					this.validationPickAutoComplete = false;
				} else this.validationPickAutoComplete = true;
			}
		}
		if (!this.validationPickAutoComplete) {
			this.createPRForm.controls['approverControl'].setErrors({ 'incorrectPickerAutocomplete': true });
		} else {
			this.createPRForm.controls['approverControl'].setErrors(null);
		}
	}
	// End
	getDate() {
		return new Date();
	}
	setDefaultValueModel() {
		this.prModel.RequestPaymentId = this.emptyGuid;
		this.prModel.CreateDate = new Date();
		this.prModel.RequestPaymentCreateDate = new Date();
		this.prModel.TotalAmount = 0;
	}
	get f() { return this.createPRForm.controls; }

	openOrgPopup() {
		this.dialogPopup = this.dialog.open(OrganizationpopupComponent,
			{
				width: '550px',
				height: '580',
				autoFocus: false,
				data: {}
			});
		this.dialogPopup.afterClosed().subscribe(result => {
			if (result != undefined) {
				this.selectedOrgName = result.selectedOrgName;
				this.prModel.RequestBranch = result.selectedOrgId;
			}
		});
	}
	async getMasterData() {
		this.positionService.getAllPosition().subscribe(response => {
			let result = <any>response;
			this.postionList = result.listPosition;
		}, error => { });

		await this.getListEmployee();

		let rs: any = await this.cateService.getAllCategoryByCategoryTypeCodeAsyc('DDU');
		this.listStatus = rs.category;

		let rsmt: any = await this.cateService.getAllCategoryByCategoryTypeCodeAsyc('PTO');
		this.listPaymenMethod = rsmt.category;
		const pm = this.listPaymenMethod.find(mt => mt.categoryCode.trim() === "CASH");
	}
	async getListEmployee() {
		let result: any = await this.employeeService.searchEmployeeAsync(this.emptyString, this.emptyString, this.emptyString, this.emptyString, this.emptyString);
		this.employeeList = result.employeeList;
		// LO?I TR? CÁC CH?C V? NHÂN VIÊN,GI?NG VIÊN,TR? GI?NG,...
		let positionNotForApprove = [];
		this.postionList.forEach(ps => {
			if (ps.positionCode.trim() == "GV" || ps.positionCode.trim() == "CTV" || ps.positionCode.trim() == "NV" || ps.positionCode.trim() == "TG") {
				positionNotForApprove.push(ps.positionId.toString().toLowerCase().trim());
			}
		})
		let listEmpForApprove = this.employeeList.filter(function (emp) {
			if (emp.positionId == null || emp.positionId == "" || positionNotForApprove.indexOf(emp.positionId.toString().toLowerCase().trim()) >= 0) {
			} else return true;
		})
		// End
		let listMapEmpId = result.listIdentityMapEmpId;
		this.employeeList.forEach(function (item) {
			const toSelectOrderer = listMapEmpId.find(c => c.employeeId == item.employeeId);
			item.employeeCode = toSelectOrderer.employeeCode;
			item.contactId = toSelectOrderer.contactId;
		});
		const currentUser = listMapEmpId.find(emp => emp.employeeId == this.auth.EmployeeId);
		if (currentUser !== undefined) {
			this.prModel.RequestEmployee = currentUser.employeeId;
			this.createPRForm.controls['ordererControl'].setValue(currentUser.employeeCode + ' - ' + currentUser.employeeName);
			this.prModel.ApproverId = currentUser.employeeId;
			this.prModel.PostionApproverId = currentUser.positionId;
			this.createPRForm.controls['approverControl'].setValue(currentUser.employeeCode + ' - ' + currentUser.employeeName);
			this.contactService.getContactById(currentUser.contactId, this.auth.UserId).subscribe(response => {
				let result = <any>response;
				this.prModel.RequestEmployeePhone = result.contact.phone == null ? '' : result.contact.phone;
			});
			this.prModel.OrganizationName = currentUser.organizationName;
			this.prModel.RequestBranch = currentUser.organizationId;
			this.selectedOrgName = currentUser.organizationName;
		}
		this.filteredEmployee = this.createPRForm.controls['ordererControl'].valueChanges
			.pipe(
				startWith(''),
				map(state => state ? this._filter(state, this.employeeList) : this.employeeList.slice())
			);
		this.filteredEmployee2 = this.createPRForm.controls['approverControl'].valueChanges
			.pipe(
				startWith(''),
				map(state => state ? this._filter(state, listEmpForApprove) : listEmpForApprove.slice())
			);
	}

	selectOrderer(event: MatAutocompleteSelectedEvent) {
		this.createPRForm.controls['ordererControl'].setValue(event.option.viewValue);
		this.prModel.RequestEmployee = event.option.value.employeeId;
		this.contactService.getContactById(event.option.value.contactId, this.auth.UserId).subscribe(response => {
			let result = <any>response;
			this.prModel.RequestEmployeePhone = result.contact.phone == null ? '' : result.contact.phone;
		})
	}

	selectApprover(event: MatAutocompleteSelectedEvent) {
		this.createPRForm.controls['approverControl'].setValue(event.option.viewValue);
		this.prModel.ApproverId = event.option.value;
		let emp = this.employeeList.find(e => e.employeeId === event.option.value);
		this.prModel.PostionApproverId = emp.positionId;
	}

	private _filter(value: string, array: any) {
		const _value = <string>value.toString().toLowerCase();
		return array.filter(state =>
			state.employeeName.toLowerCase().includes(_value) || state.employeeCode.toLowerCase().includes(_value));
	}

	// Upload file to server
	uploadFiles(files: File[]) {
		this.imageService.uploadFile(files).subscribe(response => { }, error => { });
	}
	onRemoveFile(index: number) {
		this.files.splice(index, 1);
	}
	cancel() {
		this.router.navigate(['/accounting/payment-request-list']);
	}

	save(isSendApprove: boolean) {
		if (this.createPRForm.invalid) {
			return;
		} else {
			this.paymentRequestService.createRequestPayment(this.files, this.prModel, this.auth.UserId).subscribe(response => {
				let result = <any>response;
				if (result.statusCode === 202 || result.statusCode === 200) {
					//Tao buoc dau tien trong FeatureWorkflowProgress
					this.workflowService.nextWorkflowStep(this.featureCode, result.requestPaymentId, '', this.isRejected,
						'', this.isApproved, this.isInApprovalProgress).subscribe(response => {
							const presult = <any>response;
							if (presult.statusCode === 202 || presult.statusCode === 200) {
								if (isSendApprove) {
									this.workflowService.nextWorkflowStep(this.featureCode, result.requestPaymentId, '', this.isRejected,
										'', this.isApproved, this.isInApprovalProgress).subscribe(response => {
											const resultS = <any>response;
											if (resultS.statusCode === 202 || resultS.statusCode === 200) {
												this.snackBar.openFromComponent(SuccessComponent, { data: resultS.messageCode, ...this.successConfig });
											}
										}, error => {
											const result = <any>error;
											this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
										});
								} else {
									this.snackBar.openFromComponent(SuccessComponent, { data: result.messageCode, ...this.successConfig });
								}
							}
						}, error => {
							const result = <any>error;
							this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
						});
					this.router.navigate(['/accounting/payment-request-list']);
				}
				else {
					this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
				}
			}, error => {
				let result = <any>error;
				this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
			});
		}
	}

}
