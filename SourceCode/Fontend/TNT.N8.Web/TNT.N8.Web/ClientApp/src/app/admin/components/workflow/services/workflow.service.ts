
import { map } from 'rxjs/operators';
import { Injectable, Pipe } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { WorkflowModel } from '../models/workflow.model';

@Pipe({ name: "WorkflowService" })
@Injectable()
export class WorkflowService {
  auth: any = JSON.parse(localStorage.getItem("auth"));

  constructor(private httpClient: HttpClient) { }

  createWorkflow(workflowModel: WorkflowModel, userId: string, workflowStepList: Array<any>) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/workflow/create";
    return this.httpClient.post(url, { Workflow: workflowModel, UserId: userId, WorkflowStepList: workflowStepList }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getAllWorkflowCode() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/workflow/create";
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getAllSystemFeature() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/workflow/getAllSystemFeature";
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  searchWorkflow(workflowName: string, systemFeatureIdList: Array<string>, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/workflow/searchWorkflow";
    return this.httpClient.post(url, { WorkflowName: workflowName, SystemFeatureIdList: systemFeatureIdList, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getWorkflowById(workflowId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/workflow/getWorkflowById";
    return this.httpClient.post(url, { WorkflowId: workflowId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateWorkflowById(workflowModel: WorkflowModel, userId: string, workflowStepList: Array<any>) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/workflow/updateWorkflowById";
    return this.httpClient.post(url, { Workflow: workflowModel, UserId: userId, WorkflowStepList: workflowStepList }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  nextWorkflowStep(featureCode: string, featureId: string, recordName: string, isReject: boolean,
    rejectComment: string, isApprove: boolean, isSendingApprove: boolean) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/workflow/nextWorkflowStep";
    return this.httpClient.post(url, {
      FeatureCode: featureCode, FeatureId: featureId, RecordName: recordName,
      IsReject: isReject, RejectComment: rejectComment, IsApprove: isApprove, IsSendingApprove: isSendingApprove,
      UserId: this.auth.UserId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataCreateWorkflow() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/workflow/getMasterDataCreateWorkflow";
    return this.httpClient.post(url, { }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
}