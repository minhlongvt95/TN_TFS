using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Category
    {
        public Category()
        {
            BankPayableInvoiceBankPayableInvoicePriceCurrencyNavigation = new HashSet<BankPayableInvoice>();
            BankPayableInvoiceBankPayableInvoiceReasonNavigation = new HashSet<BankPayableInvoice>();
            BankPayableInvoiceStatus = new HashSet<BankPayableInvoice>();
            BankReceiptInvoiceBankReceiptInvoicePriceCurrencyNavigation = new HashSet<BankReceiptInvoice>();
            BankReceiptInvoiceBankReceiptInvoiceReasonNavigation = new HashSet<BankReceiptInvoice>();
            BankReceiptInvoiceStatus = new HashSet<BankReceiptInvoice>();
            Contact = new HashSet<Contact>();
            CustomerField = new HashSet<Customer>();
            CustomerOrderDetail = new HashSet<CustomerOrderDetail>();
            CustomerScale = new HashSet<Customer>();
            CustomerStatus = new HashSet<Customer>();
            Inventory = new HashSet<Inventory>();
            LeadInterestedGroup = new HashSet<Lead>();
            LeadPaymentMethod = new HashSet<Lead>();
            LeadPotential = new HashSet<Lead>();
            LeadStatus = new HashSet<Lead>();
            PayableInvoiceCurrencyUnitNavigation = new HashSet<PayableInvoice>();
            PayableInvoicePayableInvoicePriceCurrencyNavigation = new HashSet<PayableInvoice>();
            PayableInvoicePayableInvoiceReasonNavigation = new HashSet<PayableInvoice>();
            PayableInvoiceRegisterTypeNavigation = new HashSet<PayableInvoice>();
            PayableInvoiceStatus = new HashSet<PayableInvoice>();
            Product = new HashSet<Product>();
            ReceiptInvoiceCurrencyUnitNavigation = new HashSet<ReceiptInvoice>();
            ReceiptInvoiceReceiptInvoiceReasonNavigation = new HashSet<ReceiptInvoice>();
            ReceiptInvoiceRegisterTypeNavigation = new HashSet<ReceiptInvoice>();
            ReceiptInvoiceStatus = new HashSet<ReceiptInvoice>();
            VendorOrderDetail = new HashSet<VendorOrderDetail>();
            VendorPayment = new HashSet<Vendor>();
            VendorVendorGroup = new HashSet<Vendor>();
        }

        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryCode { get; set; }
        public Guid CategoryTypeId { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDefauld { get; set; }
        public Guid? TenantId { get; set; }

        public CategoryType CategoryType { get; set; }
        public ICollection<BankPayableInvoice> BankPayableInvoiceBankPayableInvoicePriceCurrencyNavigation { get; set; }
        public ICollection<BankPayableInvoice> BankPayableInvoiceBankPayableInvoiceReasonNavigation { get; set; }
        public ICollection<BankPayableInvoice> BankPayableInvoiceStatus { get; set; }
        public ICollection<BankReceiptInvoice> BankReceiptInvoiceBankReceiptInvoicePriceCurrencyNavigation { get; set; }
        public ICollection<BankReceiptInvoice> BankReceiptInvoiceBankReceiptInvoiceReasonNavigation { get; set; }
        public ICollection<BankReceiptInvoice> BankReceiptInvoiceStatus { get; set; }
        public ICollection<Contact> Contact { get; set; }
        public ICollection<Customer> CustomerField { get; set; }
        public ICollection<CustomerOrderDetail> CustomerOrderDetail { get; set; }
        public ICollection<Customer> CustomerScale { get; set; }
        public ICollection<Customer> CustomerStatus { get; set; }
        public ICollection<Inventory> Inventory { get; set; }
        public ICollection<Lead> LeadInterestedGroup { get; set; }
        public ICollection<Lead> LeadPaymentMethod { get; set; }
        public ICollection<Lead> LeadPotential { get; set; }
        public ICollection<Lead> LeadStatus { get; set; }
        public ICollection<PayableInvoice> PayableInvoiceCurrencyUnitNavigation { get; set; }
        public ICollection<PayableInvoice> PayableInvoicePayableInvoicePriceCurrencyNavigation { get; set; }
        public ICollection<PayableInvoice> PayableInvoicePayableInvoiceReasonNavigation { get; set; }
        public ICollection<PayableInvoice> PayableInvoiceRegisterTypeNavigation { get; set; }
        public ICollection<PayableInvoice> PayableInvoiceStatus { get; set; }
        public ICollection<Product> Product { get; set; }
        public ICollection<ReceiptInvoice> ReceiptInvoiceCurrencyUnitNavigation { get; set; }
        public ICollection<ReceiptInvoice> ReceiptInvoiceReceiptInvoiceReasonNavigation { get; set; }
        public ICollection<ReceiptInvoice> ReceiptInvoiceRegisterTypeNavigation { get; set; }
        public ICollection<ReceiptInvoice> ReceiptInvoiceStatus { get; set; }
        public ICollection<VendorOrderDetail> VendorOrderDetail { get; set; }
        public ICollection<Vendor> VendorPayment { get; set; }
        public ICollection<Vendor> VendorVendorGroup { get; set; }
    }
}
