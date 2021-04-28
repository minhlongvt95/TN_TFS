let emptyId = "00000000-0000-0000-0000-000000000000";
export class QueueModel {
    QueueId: string;
    FromTo: string;
    SendTo: string;
    SendContent: string;
    Title: string;
    Method: string;
    IsSend: boolean;
    SenDate: string;
    CustomerCareId: string;
    CustomerId: string;
    StatusId: string;
    CreateDate: string;
    CreateById: string;
    UpdateDate: string;
    UpdateById: string;

    constructor() {
        this.QueueId = emptyId;
        this.FromTo = null;
        this.SendTo = null;
        this.SendContent = '';
        this.Title = null;
        this.Method = null;
        this.IsSend = false;
        this.SenDate = null;
        this.CustomerCareId = null;
        this.CustomerId = null;
        this.StatusId = null;
        this.CreateDate = null;
        this.CreateById = null;
        this.UpdateDate = null;
        this.UpdateById = null;
    }
}

