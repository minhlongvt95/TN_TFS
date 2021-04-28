export class NoteDocumentModel {
    NoteDocumentId: string;
    NoteId: string;
    DocumentName: string;
    DocumentSize: string;
    DocumentUrl: string;
    Base64Url: string;
    Active: boolean;
    CreatedById: string;
    CreatedDate: Date;
    UpdatedById: string;
    UpdatedDate: Date;
    constructor() { 
        this.NoteDocumentId = '00000000-0000-0000-0000-000000000000',
        this.NoteId = '00000000-0000-0000-0000-000000000000',
        this.DocumentName = '',
        this.DocumentSize = '',
        this.DocumentUrl = '',
        this.Base64Url = '',
        this.Active = true,
        this.CreatedById = '00000000-0000-0000-0000-000000000000',
        this.CreatedDate = new Date(),
        this.UpdatedById = null,
        this.UpdatedDate = null
    }
}