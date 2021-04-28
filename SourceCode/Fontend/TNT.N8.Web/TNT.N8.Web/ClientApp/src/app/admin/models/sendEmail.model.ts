export class SendEmailModel {
  //lead infor
  public LeadName: string;
  public LeadEmail: string;
  public LeadPhone: string;
  public LeadAddress: string;
  public LeadInterested: string;
  public LeadPotential: string;
  public LeadPicCode: string;
  public LeadPicName: string;
  public EmployeeCode: string;
  public EmployeeName: string;
  public LeadId: string;
  public LeadContactId: string;

  //create Quote
  public QuoteCode: string;
  public QuoteStatus: string;
  public CustomerType: string;
  public CustomerName: string;
  public CustomerEmail: string;
  public CustomerPhone: string;
  public Seller: string;
  public SendQuoteDate: string;
  public EffectiveQuoteDate: string;
  public CreatedEmployeeName: string;
  public CreatedEmployeeCode: string;
  public ListQuoteDetailToSendEmail: Array<QuoteDetailToSendEmail>;
  public SendDetailProduct: boolean; //gui bang detail product
  public SumAmountDiscount: string; //tổng tiền chiết khấu theo sản phẩm
  public AmountDiscountByQuote: string; //tổng tiền chiết khấu theo quote
  public SumAmount: string; //tổng tiền theo sản phẩm
  public SumAmountByQuote: string; //tổng tiền theo qute
  public SumAmountTransform: string;

  //gửi email sau khi gửi phê duyệt Phiếu đề xuất mua hàng
  public OrganizationName: string;
  public ProcurementCode: string;

  public SendDetailProductInOrder: boolean;
  public ListDetailToSendEmailInOrder: Array<OrderDetailToSendEmailModel>;
  public SumAmountDiscountByProductInOder: string; //tổng tiền chiết khấu theo sản phẩm
  public SumAmountDiscountByOrder: string; //chiết khấu theo order
  public SumAmountBeforeDiscount: string;//tổng tiền order truoc chiet khau tổng
  public SumAmountAfterDiscount: string;//tổng tiền sau chiet khau
  public SumAmountTransformInOrder: string;//chuyển thành tiền bằng chữ

  //chung
  public CompanyName: string;
  public CompanyAddress: string;
  public ListSendToEmail: Array<string>;
  constructor() {
    this.ListQuoteDetailToSendEmail = [];
    this.ListDetailToSendEmailInOrder = [];
  }
}

export class QuoteDetailToSendEmail {
  public ProductName: string;
  public ProductNameUnit: string;
  public UnitPrice: string;
  public Quantity: string;
  public Vat: string;
  public DiscountValue: string;
  public SumAmount: string;
  public AmountDiscountPerProduct: string;
}

export class OrderDetailToSendEmailModel {
  public ProductName: string;
  public ProductNameUnit: string;
  public UnitPrice: string;
  public Quantity: string;
  public Vat: string;
  public DiscountValue: string;
  public SumAmount: string;
  public AmountDiscountPerProduct: string;
}
