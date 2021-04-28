let emptyId = "00000000-0000-0000-0000-000000000000";
export class QueueModel {
    queueId: string;
    fromTo: string;
    sendTo: string;
    sendContent: string;
    title: string;
    method: string;
    isSend: boolean;
    senDate: string;
    customerCareId: string;
    customerId: string;
    statusId: string;
    createDate: string;
    createById: string;
    updateDate: string;
    updateById: string;

    constructor() {
        this.queueId = emptyId;
        this.fromTo = null;
        this.sendTo = null;
        this.sendContent = '';
        this.title = null;
        this.method = null;
        this.isSend = false;
        this.senDate = null;
        this.customerCareId = null;
        this.customerId = null;
        this.statusId = null;
        this.createDate = null;
        this.createById = null;
        this.updateDate = null;
        this.updateById = null;
    }
}

