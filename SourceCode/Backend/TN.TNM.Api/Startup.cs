using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using TN.TNM.Api.Extensions;
using TN.TNM.BusinessLogic.Factories;
using TN.TNM.BusinessLogic.Factories.Admin;
using TN.TNM.BusinessLogic.Factories.Admin.EmailConfig;
using TN.TNM.BusinessLogic.Factories.Admin.Category;
using TN.TNM.BusinessLogic.Factories.Admin.Company;
using TN.TNM.BusinessLogic.Factories.Admin.Country;
using TN.TNM.BusinessLogic.Factories.Admin.CustomerServiceLevel;
using TN.TNM.BusinessLogic.Factories.Admin.District;
using TN.TNM.BusinessLogic.Factories.Admin.Order_Status;
using TN.TNM.BusinessLogic.Factories.Admin.Organization;
using TN.TNM.BusinessLogic.Factories.Admin.Product;
using TN.TNM.BusinessLogic.Factories.Admin.ProductCategory;
using TN.TNM.BusinessLogic.Factories.Admin.Province;
using TN.TNM.BusinessLogic.Factories.Admin.Ward;
using TN.TNM.BusinessLogic.Factories.BankAccount;
using TN.TNM.BusinessLogic.Factories.BankBook;
using TN.TNM.BusinessLogic.Factories.CashBook;
using TN.TNM.BusinessLogic.Factories.Contact;
using TN.TNM.BusinessLogic.Factories.Customer;
using TN.TNM.BusinessLogic.Factories.CustomerCare;
using TN.TNM.BusinessLogic.Factories.DashboardRequest;
using TN.TNM.BusinessLogic.Factories.Document;
using TN.TNM.BusinessLogic.Factories.Email;
using TN.TNM.BusinessLogic.Factories.Employee;
using TN.TNM.BusinessLogic.Factories.Lead;
using TN.TNM.BusinessLogic.Factories.Note;
using TN.TNM.BusinessLogic.Factories.Notification;
using TN.TNM.BusinessLogic.Factories.Order;
using TN.TNM.BusinessLogic.Factories.PayableInvoice;
using TN.TNM.BusinessLogic.Factories.ProcurementPlan;
using TN.TNM.BusinessLogic.Factories.ProcurementRequest;
using TN.TNM.BusinessLogic.Factories.Promotion;
using TN.TNM.BusinessLogic.Factories.Quote;
using TN.TNM.BusinessLogic.Factories.ReceiptInvoice;
using TN.TNM.BusinessLogic.Factories.Receivable;
using TN.TNM.BusinessLogic.Factories.RequestPayment;
using TN.TNM.BusinessLogic.Factories.Vendor;
using TN.TNM.BusinessLogic.Factories.WareHouse;
using TN.TNM.BusinessLogic.Factories.Workflow;
using TN.TNM.BusinessLogic.Interfaces.Admin;
using TN.TNM.BusinessLogic.Interfaces.Admin.Email;
using TN.TNM.BusinessLogic.Interfaces.Admin.Category;
using TN.TNM.BusinessLogic.Interfaces.Admin.Company;
using TN.TNM.BusinessLogic.Interfaces.Admin.Country;
using TN.TNM.BusinessLogic.Interfaces.Admin.CustomerServiceLevel;
using TN.TNM.BusinessLogic.Interfaces.Admin.District;
using TN.TNM.BusinessLogic.Interfaces.Admin.OrderStatus;
using TN.TNM.BusinessLogic.Interfaces.Admin.Organization;
using TN.TNM.BusinessLogic.Interfaces.Admin.Product;
using TN.TNM.BusinessLogic.Interfaces.Admin.ProductCategory;
using TN.TNM.BusinessLogic.Interfaces.Admin.Province;
using TN.TNM.BusinessLogic.Interfaces.Admin.Ward;
using TN.TNM.BusinessLogic.Interfaces.BankAccount;
using TN.TNM.BusinessLogic.Interfaces.BankBook;
using TN.TNM.BusinessLogic.Interfaces.CashBook;
using TN.TNM.BusinessLogic.Interfaces.Contact;
using TN.TNM.BusinessLogic.Interfaces.Customer;
using TN.TNM.BusinessLogic.Interfaces.CustomerCare;
using TN.TNM.BusinessLogic.Interfaces.DashboardRequest;
using TN.TNM.BusinessLogic.Interfaces.Email;
using TN.TNM.BusinessLogic.Interfaces.Employee;
using TN.TNM.BusinessLogic.Interfaces.Lead;
using TN.TNM.BusinessLogic.Interfaces.MenuBuild;
using TN.TNM.BusinessLogic.Interfaces.Note;
using TN.TNM.BusinessLogic.Interfaces.Notification;
using TN.TNM.BusinessLogic.Interfaces.Order;
using TN.TNM.BusinessLogic.Interfaces.PayableInvoice;
using TN.TNM.BusinessLogic.Interfaces.ProcurementPlan;
using TN.TNM.BusinessLogic.Interfaces.ProcurementRequest;
using TN.TNM.BusinessLogic.Interfaces.Promotion;
using TN.TNM.BusinessLogic.Interfaces.Quote;
using TN.TNM.BusinessLogic.Interfaces.ReceiptInvoice;
using TN.TNM.BusinessLogic.Interfaces.Receivable;
using TN.TNM.BusinessLogic.Interfaces.RequestPayment;
using TN.TNM.BusinessLogic.Interfaces.Vendor;
using TN.TNM.BusinessLogic.Interfaces.WareHouse;
using TN.TNM.BusinessLogic.Interfaces.Workflow;
using TN.TNM.BusinessLogic.Models.Jwt;
using TN.TNM.DataAccess.Databases;
using TN.TNM.DataAccess.Databases.DAO;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.WebTenantProvider;

#region nuget

#endregion 

#region Data Access

#endregion

#region Business Logic

using IDocument = TN.TNM.BusinessLogic.Interfaces.Document.IDocument;
using TN.TNM.BusinessLogic.Interfaces.SaleBidding;
using TN.TNM.BusinessLogic.Factories.SaleBidding;
using TN.TNM.BusinessLogic.Interfaces.Folder;
using TN.TNM.BusinessLogic.Factories.Folder;
using TN.TNM.BusinessLogic.Interfaces.BillSale;
using TN.TNM.BusinessLogic.Factories.BillSale;
using TN.TNM.BusinessLogic.Interfaces.Contract;
using TN.TNM.BusinessLogic.Factories.Contract;
using TN.TNM.BusinessLogic.Interfaces.Admin.NotifiSetting;
using TN.TNM.BusinessLogic.Factories.Admin.NotifiSetting;
using TN.TNM.BusinessLogic.Factories.MenuBuild;

#endregion

#region MyRegion

using TN.TNM.DataAccess.HubConfig;

#endregion

namespace TN.TNM.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container. 
        /// </summary>
        /// <param name="services">Application service</param>
        public void ConfigureServices(IServiceCollection services)
        {
            if ("2".Equals(this.Configuration["LOGIN_OPTION"]))
            {
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                        .AddJwtBearer(options =>
                        {
                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuer = true,
                                ValidateAudience = true,
                                ValidateLifetime = true,
                                ValidateIssuerSigningKey = true,
                                ValidIssuer = Configuration["token-valid-issuer"],
                                ValidAudience = Configuration["token-valid-audience"],
                                IssuerSigningKey = JwtSecurityKey.Create(Configuration["secret-key-name"])
                            };

                            options.Events = new JwtBearerEvents
                            {
                                OnAuthenticationFailed = context =>
                                {
                                    Console.WriteLine("OnAuthenticationFailed: " + context.Exception.Message);
                                    return Task.CompletedTask;
                                },
                                OnTokenValidated = context =>
                                {
                                    Console.WriteLine("OnTokenValidated: " + context.SecurityToken);
                                    return Task.CompletedTask;
                                }
                            };
                        });

                services.AddAuthorization(options =>
                {
                    options.AddPolicy(
                        "Member",
                        policy => policy.RequireClaim("MembershipId"));
                });
            }
            else
            {
                services.AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddAzureAdBearer(options => ConfigurationBinder.Bind(Configuration, (string) "AzureAd", (object) options));

                services.AddAuthorization(options =>
                {
                    options.AddPolicy(
                        "Member",
                        policy => policy.RequireAuthenticatedUser());
                });
            }

            services.AddCors(options =>
            {
                //AllowSpecificOrigin
                options.AddPolicy(
                    "AllowSpecificOrigin",
                    builder => builder
                    .WithOrigins(this.Configuration["WEB_ENDPOINT"])
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });

            services.AddResponseCompression(options =>
            {
                IEnumerable<string> MimeTypes = new[]
                {
                    // General
                    "text/plain",
                    // Static files
                    "text/css",
                    "application/javascript",
                    // MVC
                    "text/html",
                    "application/xml",
                    "application/xhr",
                    "text/xml",
                    "application/json",
                    "text/json",
                    "font/woff2",
                    // WebAssembly
                    "application/wasm",
                    // Custom
                    "image/svg+xml",
                    "image/jpeg",
                    "image/x-icon",
                    "image/png"
                };

                options.EnableForHttps = true;
                options.MimeTypes = MimeTypes;
                options.Providers.Add<GzipCompressionProvider>();
            });
            services.AddSingleton(this.Configuration);
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            this.AddScopes(services);

            services.AddSignalR();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <param name="env">Hosting environment</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //app.UseCors("open");

            // Enable compression
            app.UseCors("AllowSpecificOrigin");
            app.UseResponseCompression();
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseMvc();

            app.UseSignalR(routes =>
            {
                routes.MapHub<Notification>("/notification");
            });
        }

        /// <summary>
        /// Add DJ to service scope
        /// </summary>
        /// <param name="services">App services</param>
        private void AddScopes(IServiceCollection services)
        {
            services.AddSingleton<ILogger>(s => s.GetService<ILogger<BaseFactory>>());

            var connection = this.Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<TenantContext>(options => options.UseSqlServer(connection));
            services.AddDbContext<TNTN8Context>(options => options.UseSqlServer(connection));

            services.AddScoped<ITenantProvider, WebTenantProvider>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            #region add bussiness logic dependencies
            services.AddScoped<ILeadSearch, LeadSearchFactory>();
            services.AddScoped<IAuth, AuthFactory>();
            services.AddScoped<ICompany, CompanyFactory>();
            services.AddScoped<ICategory, CategoryFactory>();
            services.AddScoped<IEmailConfig, EmailConfigFactory>();
            services.AddScoped<IContact, ContactFactory>();
            services.AddScoped<ILeadDashboard, LeadDashboardFactory>();
            services.AddScoped<IEmployee, EmployeeFactory>();
            services.AddScoped<IEmployeeRequest, EmployeeRequestFactory>();
            services.AddScoped<IEmployeeInsurance, EmployeeInsuranceFactory>();
            services.AddScoped<IOrganization, OrganizationFactory>();
            services.AddScoped<INote, NoteFactory>();
            services.AddScoped<IProvince, ProvinceFactory>();
            services.AddScoped<IDistrict, DistrictFactory>();
            services.AddScoped<IWard, WardFactory>();
            services.AddScoped<IPosition, PositionFactory>();
            services.AddScoped<IEmail, EmailFactory>();
            services.AddScoped<IVendor, VendorFactory>();
            services.AddScoped<IProductCategory,ProductCategoryFactory>();
            services.AddScoped<IProduct, ProductFactory>();
            services.AddScoped<ICustomer, CustomerFactory>();
            services.AddScoped<ICustomerOrder, CustomerOrderFactory>();
            services.AddScoped<IBankAccount, BankAccountFactory>();
            services.AddScoped<IOrderStatus, OrderStatusFactory>();
            services.AddScoped<IPayableInvoice, PayableInvoiceFactory>();
            services.AddScoped<IReceiptInvoice, ReceiptInvoiceFactory>();
            services.AddScoped<IReceivable, ReceivableFactory>();
            services.AddScoped<ICustomerServiceLevel, CustomerServiceLevelFactory>();
            services.AddScoped<IBankBook, BankBookFactory>();
            services.AddScoped<ICashBook, CashBookFactory>();
            services.AddScoped<IWorkflow, WorkflowFactory>();
            services.AddScoped<IEmployeeSalary, EmployeeSalaryFactory>();
            services.AddScoped<IEmployeeAllowance, EmployeeAllowanceFactory>();
            services.AddScoped<IEmployeeAssessment, EmployeeAssessmentFactory>();
            services.AddScoped<IEmployeeMonthySalary, EmployeeMonthySalaryFactory>();
			services.AddScoped<ICountry, CountryFactory>();
            services.AddScoped<IProcurementRequest, ProcurementRequestFactory>();
            services.AddScoped<IProcurementPlan, ProcurementPlanFactory>();
            services.AddScoped<IRequestPayment, RequestPaymentFactory>();
            services.AddScoped<IDocument, DocumentFactory>();
            services.AddScoped<IDashboardRequest, DashboardRequestFactory>();
            services.AddScoped<IQuote, QuoteFactory>();
            services.AddScoped<INotification, NotificationFactory>();
            services.AddScoped<ICustomerCare, CustomerCareFactory>();
            services.AddScoped<IWareHouse, WareHouseFactory>();
            services.AddScoped<ISaleBidding, SaleBiddingFactory>();
            services.AddScoped<IFolder, FolderFactory>();
            services.AddScoped<IBillSale, BillSaleFactory>();
            services.AddScoped<IContract, ContractFactory>();
            services.AddScoped<INotifiSetting, NotifiSettingFactory>();
            services.AddScoped<IPromotion, PromotionFactory>();
            services.AddScoped<IMenuBuild, MenuBuildFactory>();
            #endregion

            #region add data access dependencies
            services.AddScoped<IAuthDataAccess, AuthDAO>();
            services.AddScoped<IAuditTraceDataAccess, AuditTraceDAO>();
            services.AddScoped<ILeadSearchDataAccess, LeadSearchDAO>();
            services.AddScoped<ICompanyDataAccess, CompanyDAO>();
            services.AddScoped<IEmailConfigurationDataAccess, EmailConfigDAO>();
            services.AddScoped<ICategoryDataAccess, CategoryDAO>();
            services.AddScoped<IContactDataAccess, ContactDAO>();
            services.AddScoped<ILeadDashboardDataAccess, LeadDashboardDAO>();
            services.AddScoped<IEmployeeDataAccess, EmployeeDAO>();
            services.AddScoped<IEmployeeRequestDataAccess, EmployeeRequestDAO>();
            services.AddScoped<IEmployeeInsuranceDataAccess, EmployeeInsuranceDAO>();
            services.AddScoped<IOrganizationDataAccess, OrganizationDAO>();
            services.AddScoped<INoteDataAccess, NoteDAO>();
            services.AddScoped<IProvinceDataAccess, ProvinceDAO>();
            services.AddScoped<IDistrictDataAccess, DistrictDAO>();
            services.AddScoped<IWardDataAccess, WardDAO>();
            services.AddScoped<IPositionDataAccess, PositionDAO>();
            services.AddScoped<IEmailDataAccess, EmailDAO>();
            services.AddScoped<IVendorDataAccess, VendorDAO>();
            services.AddScoped<IProductCategoryDataAccess, ProductCategoryDAO>();
            services.AddScoped<IProductDataAccess, ProductDAO>();
            services.AddScoped<ICustomerDataAccess, CustomerDAO>();
            services.AddScoped<IBankAccountDataAccess, BankAccountDAO>();
            services.AddScoped<ICustomerOrderDataAccess, CustomerOrderDAO>();
            services.AddScoped<IOrderStatusDataAccess, OrderStatusDAO>();
            services.AddScoped<IPayableInvoiceDataAccess, PayableInvoiceDAO>();
            services.AddScoped<IReceiptInvoiceDataAccess, ReceiptInvoiceDAO>();
            services.AddScoped<IReceivableDataAccess, ReceivableDAO>();
            services.AddScoped<ICustomerServiceLevelDataAccess, CustomerServiceLevelDAO>();
            services.AddScoped<IBankBookDataAccess, BankBookDAO>();
            services.AddScoped<ICashBookDataAccess, CashBookDAO>();
            services.AddScoped<IWorkflowDataAccess, WorkflowDAO>();
            services.AddScoped<IEmployeeSalaryDataAccess, EmployeeSalaryDAO>();
            services.AddScoped<IEmployeeAllowanceDataAccess, EmployeeAllowanceDAO>();
            services.AddScoped<IEmployeeAssessmentDataAccess, EmployeeAssessmentDAO>();
            services.AddScoped<IEmployeeMonthySalaryDataAccess, EmployeeMonthySalaryDAO>();
			services.AddScoped<ICountryDataAccess, CountryDAO>();
            services.AddScoped<IProcurementPlanDataAccess, ProcurementPlanDAO>();
            services.AddScoped<IProcurementRequestDataAccess, ProcurementRequestDAO>();
            services.AddScoped<IRequestPaymentDataAccess, RequestPaymentDAO>();
            services.AddScoped<IDashboardRequestDataAccess, DashboardRequestDAO>();
            services.AddScoped<IDocumentDataAccess, DocumentDAO>();
            services.AddScoped<INotificationDataAccess, NotificationDAO>();
            services.AddScoped<IQuoteDataAccess, QuoteDAO>();
            services.AddScoped<ICustomerCareDataAccess, CustomerCareDAO>();
            services.AddScoped<IWareHouseDataAccess, WareHouseDAO>();
            services.AddScoped<ISaleBiddingDataAccess, SaleBiddingDAO>();
            services.AddScoped<IFolderDataAccess, FolderDAO>();
            services.AddScoped<IBillSaleDataAccess, BillSaleDAO>();
            services.AddScoped<IContractDataAccess, ContractDAO>();
            services.AddScoped<INotifiSettingDataAccess, NotifiSettingDAO>();
            services.AddScoped<IPromotionDataAccess, PromotionDAO>();
            services.AddScoped<IMenuBuildDataAccess, MenuBuildDAO>();
            #endregion
        }
    }
}