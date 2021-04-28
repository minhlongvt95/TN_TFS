using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Results.Lead
{
    public class GetLeadByIdResult : BaseResult
    {
        public Databases.Entities.Lead Lead { get; set; }
        public List<Company> ListCompany { get; set; }
        public Company Company { get; set; }
        public string InterestedGroupName { get; set; }
        public string Status_Code { get; set; }
        public string PotentialName { get; set; }
        public string Status_Name { get; set; }
        public string PIC_Name { get; set; }
        public string PositionName { get; set; }
        public string PaymentMethodName { get; set; }
        public string InterestedName { get; set; }
        public Guid? ResponsibleName { get; set; }
        public Databases.Entities.Contact Contact { get; set; }
        public List<Databases.Entities.Employee> Employee{ get; set; }
        public List<Category> StatusCategory{ get; set; }
        public List<Category> Potential { get; set; }
        public List<Category> InterestedList { get; set; }
        public List<Category> PaymentMethod { get; set; }
        public List<Category> Genders { get; set; }
        public string FullAddress { get; set; }
        public int CountLead { get; set; }
        public int StatusSaleBiddingAndQuote { get; set; }
    }
}
