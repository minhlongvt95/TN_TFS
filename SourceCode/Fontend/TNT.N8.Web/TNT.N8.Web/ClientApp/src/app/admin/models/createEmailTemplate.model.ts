/* REQUEST MODEL */
export class EmailTemplateModel {
    public EmailTemplateId: string;
    public EmailTemplateName: string;
    public EmailTemplateTitle: string;
    public EmailTemplateContent: string;
    public EmailTemplateTypeId: string;
    public EmailTemplateStatusId: string;
    public Active: boolean;
    public CreatedById: string;
    public CreatedDate: Date;
    public UpdatedById: string;
    public UpdatedDate: Date;
}

