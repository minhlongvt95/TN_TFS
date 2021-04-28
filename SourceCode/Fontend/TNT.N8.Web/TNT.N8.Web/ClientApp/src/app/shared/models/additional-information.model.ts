export class AdditionalInformationModel {
    additionalInformationId: string;
    objectId: string;
    objectType: string;
    title: string;
    content: string;
    ordinal: number;
    active: boolean;
    createdDate: Date;
    createdById: string;
    updatedDate: Date;
    updatedById: string;
    orderNumber: number;

    constructor() {
        this.additionalInformationId = '00000000-0000-0000-0000-000000000000';
        this.objectId = '00000000-0000-0000-0000-000000000000';
        this.objectType = '',
            this.title = '',
            this.content = '',
            this.ordinal = 0;
        this.active = true,
            this.createdDate = new Date(),
            this.createdById = '00000000-0000-0000-0000-000000000000',
            this.updatedDate = null,
            this.updatedById = null,
            this.orderNumber = 0;
    }
}