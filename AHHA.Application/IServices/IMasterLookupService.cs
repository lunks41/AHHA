﻿using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices
{
    public interface IMasterLookupService
    {
        public Task<IEnumerable<AccountSetupCategoryLookupModel>> GetAccountSetupCategoryLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<AccountSetupLookupModel>> GetAccountSetupLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<AccountGroupLookupModel>> GetAccountGroupLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<AccountTypeLookupModel>> GetAccountTypeLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<BankLookupModel>> GetBankLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<BankLookupModel>> GetBankLookup_SuppListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<CategoryLookupModel>> GetCategoryLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<ChartOfAccountLookupModel>> GetChartOfAccountLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<COACategoryLookupModel>> GetCOACategory1LookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<COACategoryLookupModel>> GetCOACategory2LookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<COACategoryLookupModel>> GetCOACategory3LookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<CountryLookupModel>> GetCountryLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<CurrencyLookupModel>> GetCurrencyLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<CustomerGroupCreditLimitLookupModel>> GetCustomerGroupCreditLimitLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<CustomerLookupModel>> GetCustomerLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<CustomerLookupModel>> GetCustomerLookupListAsync_V1(string RegId, Int16 CompanyId, string searchString, Int16 RecordCount, Int16 UserId);

        public Task<IEnumerable<DepartmentLookupModel>> GetDepartmentLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<DesignationLookupModel>> GetDesignationLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<EmployeeLookupModel>> GetEmployeeLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<GroupCreditLimitLookupModel>> GetGroupCreditLimitLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<GstLookupModel>> GetGstLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<GstCategoryLookupModel>> GetGstCategoryLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<OrderTypeCategoryLookupModel>> GetOrderTypeCategoryLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<OrderTypeLookupModel>> GetOrderTypeLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<PaymentTypeLookupModel>> GetPaymentTypeLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<PortLookupModel>> GetPortLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<PortRegionLookupModel>> GetPortRegionLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<ProductLookupModel>> GetProductLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<ProductLookupModel>> GetProductLookupListAsync_V1(string RegId, Int16 CompanyId, string searchString, Int16 RecordCount, Int16 UserId);

        public Task<IEnumerable<SubCategoryLookupModel>> GetSubCategoryLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<SupplierLookupModel>> GetSupplierLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<SupplierLookupModel>> GetSupplierLookupListAsync_V1(string RegId, Int16 CompanyId, string searchString, Int16 RecordCount, Int16 UserId);

        public Task<IEnumerable<UomLookupModel>> GetUomLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<VoyageLookupModel>> GetVoyageLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<VoyageLookupModel>> GetVoyageLookupListAsync_V1(string RegId, Int16 CompanyId, string searchString, Int16 RecordCount, Int16 UserId);

        public Task<IEnumerable<BargeLookupModel>> GetBargeLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<BargeLookupModel>> GetBargeLookupListAsync_V1(string RegId, Int16 CompanyId, string searchString, Int16 RecordCount, Int16 UserId);

        public Task<IEnumerable<CreditTermsLookupModel>> GetCreditTermsLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<TaxLookupModel>> GetTaxLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<TaxCategoryLookupModel>> GetTaxCategoryLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<VesselLookupModel>> GetVesselLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<VesselLookupModel>> GetVesselLookupListAsync_V1(string RegId, Int16 CompanyId, string searchString, Int16 RecordCount, Int16 UserId);

        public Task<IEnumerable<UserGroupLookupModel>> GetUserGroupLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<UserLookupModel>> GetUserLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<CustomerAddressLookupModel>> GetCustomerAddressLookup_FinListAsync(string RegId, Int16 CompanyId, Int16 UserId, Int16 CustomerId);

        public Task<IEnumerable<CustomerContactLookupModel>> GetCustomerContactLookup_FinListAsync(string RegId, Int16 CompanyId, Int16 UserId, Int16 CustomerId);

        public Task<IEnumerable<SupplierAddressLookupModel>> GetSupplierAddressLookup_FinListAsync(string RegId, Int16 CompanyId, Int16 UserId, Int16 SupplierId);

        public Task<IEnumerable<SupplierContactLookupModel>> GetSupplierContactLookup_FinListAsync(string RegId, Int16 CompanyId, Int16 UserId, Int16 SupplierId);

        public Task<IEnumerable<ModuleLookupModel>> GetModuleLookupAsync(string RegId, Int16 CompanyId, Int16 UserId, bool IsVisible, bool IsMandatory);

        public Task<IEnumerable<TransactionLookupModel>> GetTransactionLookupAsync(string RegId, Int16 CompanyId, Int16 UserId, Int16 ModuleId);

        public Task<IEnumerable<YearLookupModel>> GetPeriodCloseYearLookupAsync(string RegId, Int16 CompanyId, Int16 UserId, Int16 ModuleId);

        public Task<IEnumerable<YearLookupModel>> GetPeriodCloseNextYearLookupAsync(string RegId, Int16 CompanyId, Int16 UserId, Int16 ModuleId);

        public Task<IEnumerable<YearLookupModel>> GetNumberFormatNextYearLookupAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<DocumentTypeLookupModel>> GetDocumentTypeLookupAsync(string RegId, Int16 CompanyId, Int16 UserId, Int16 ModuleId);
    }
}