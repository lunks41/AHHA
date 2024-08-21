using AHHA.Core.Entities.Admin;
using AHHA.Core.Entities.Masters;
using Microsoft.EntityFrameworkCore;

namespace AHHA.Infra.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions)
           : base(dbContextOptions)
        {
        }

        #region Admin

        public DbSet<AdmAccountGroup> AdmAccountGroup { get; set; }
        public DbSet<AdmAccountType> AdmAccountType { get; set; }
        public DbSet<AdmAuditLog> AdmAuditLog { get; set; }
        public DbSet<AdmErrorLog> AdmErrorLog { get; set; }
        public DbSet<AdmCompany> AdmCompany { get; set; }
        public DbSet<AdmModule> AdmModule { get; set; }
        public DbSet<AdmShareData> AdmShareData { get; set; }
        public DbSet<AdmTransaction> AdmTransaction { get; set; }
        public DbSet<AdmTransactionCategory> AdmTransactionCategory { get; set; }
        public DbSet<AdmUser> AdmUser { get; set; }
        public DbSet<AdmUserGroup> AdmUserGroup { get; set; }
        public DbSet<AdmUserGroupRights> AdmUserGroupRights { get; set; }
        public DbSet<AdmUserLog> AdmUserLog { get; set; }
        public DbSet<AdmUserRights> AdmUserRights { get; set; }

        #endregion Admin

        #region DbSet Section - Masters

        public DbSet<M_Product> M_Product { get; set; }
        public DbSet<M_Country> M_Country { get; set; }
        public DbSet<M_AccountSetup> M_AccountSetup { get; set; }
        public DbSet<M_AccountSetupCategory> M_AccountSetupCategory { get; set; }
        public DbSet<M_Bank> M_Bank { get; set; }
        public DbSet<M_Barge> M_Barge { get; set; }
        public DbSet<M_Category> M_Category { get; set; }
        public DbSet<M_ChartOfAccount> M_ChartOfAccount { get; set; }
        public DbSet<M_COACategory1> M_COACategory1 { get; set; }
        public DbSet<M_COACategory2> M_COACategory2 { get; set; }
        public DbSet<M_COACategory3> M_COACategory3 { get; set; }
        public DbSet<M_CreditTerm> M_CreditTerm { get; set; }
        public DbSet<M_CreditTermDt> M_CreditTermDt { get; set; }
        public DbSet<M_Currency> M_Currency { get; set; }
        public DbSet<M_CurrencyDt> M_CurrencyDt { get; set; }
        public DbSet<M_CurrencyLocalDt> M_CurrencyLocalDt { get; set; }
        public DbSet<M_CustomeGroupCreditLimt> M_CustomeGroupCreditLimt { get; set; }
        public DbSet<M_Customer> M_Customer { get; set; }
        public DbSet<M_CustomerAddress> M_CustomerAddress { get; set; }
        public DbSet<M_CustomerContact> M_CustomerContact { get; set; }
        public DbSet<M_CustomerCreditLimit> M_CustomerCreditLimit { get; set; }
        public DbSet<M_Department> M_Department { get; set; }
        public DbSet<M_Designation> M_Designation { get; set; }
        public DbSet<M_Employee> M_Employee { get; set; }
        public DbSet<M_GroupCreditLimt> M_GroupCreditLimt { get; set; }
        public DbSet<M_GroupCreditLimt_Customer> M_GroupCreditLimt_Customer { get; set; }
        public DbSet<M_GroupCreditLimtDt> M_GroupCreditLimtDt { get; set; }
        public DbSet<M_Gst> M_Gst { get; set; }
        public DbSet<M_GstCategory> M_GstCategory { get; set; }
        public DbSet<M_GstDt> M_GstDt { get; set; }
        public DbSet<M_OrderType> M_OrderType { get; set; }
        public DbSet<M_OrderTypeCategory> M_OrderTypeCategory { get; set; }
        public DbSet<M_PaymentType> M_PaymentType { get; set; }
        public DbSet<M_Port> M_Port { get; set; }
        public DbSet<M_PortRegion> M_PortRegion { get; set; }
        public DbSet<M_SubCategory> M_SubCategory { get; set; }
        public DbSet<M_Supplier> M_Supplier { get; set; }
        public DbSet<M_SupplierAddress> M_SupplierAddress { get; set; }
        public DbSet<M_SupplierContact> M_SupplierContact { get; set; }
        public DbSet<M_Tax> M_Tax { get; set; }
        public DbSet<M_TaxCategory> M_TaxCategory { get; set; }
        public DbSet<M_TaxDt> M_TaxDt { get; set; }
        public DbSet<M_Uom> M_Uom { get; set; }
        public DbSet<M_UomDt> M_UomDt { get; set; }
        public DbSet<M_Vessel> M_Vessel { get; set; }
        public DbSet<M_Voyage> M_Voyage { get; set; }

        #endregion DbSet Section - Masters
    }
}