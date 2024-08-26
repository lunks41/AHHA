﻿using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices
{
    public interface IMasterLookupService
    {
        public Task<IEnumerable<AccountSetupCategoryLookupModel>> GetAccountSetupCategoryLookupListAsync(string RegId, Int16 CompanyId, Int32 UserId);

        public Task<IEnumerable<AccountSetupLookupModel>> GetAccountSetupLookupListAsync(string RegId, Int16 CompanyId, Int32 UserId);

        public Task<IEnumerable<BankLookupModel>> GetBankLookupListAsync(string RegId, Int16 CompanyId, Int32 UserId);

        public Task<IEnumerable<CategoryLookupModel>> GetCategoryLookupListAsync(string RegId, Int16 CompanyId, Int32 UserId);

        public Task<IEnumerable<ChartOfAccountLookupModel>> GetChartOfAccountLookupListAsync(string RegId, Int16 CompanyId, Int32 UserId);

        public Task<IEnumerable<COACategoryLookupModel>> GetCOACategory1LookupListAsync(string RegId, Int16 CompanyId, Int32 UserId);

        public Task<IEnumerable<COACategoryLookupModel>> GetCOACategory2LookupListAsync(string RegId, Int16 CompanyId, Int32 UserId);

        public Task<IEnumerable<COACategoryLookupModel>> GetCOACategory3LookupListAsync(string RegId, Int16 CompanyId, Int32 UserId);

        public Task<IEnumerable<CountryLookupModel>> GetCountryLookupListAsync(string RegId, Int16 CompanyId, Int32 UserId);

        public Task<IEnumerable<CurrencyLookupModel>> GetCurrencyLookupListAsync(string RegId, Int16 CompanyId, Int32 UserId);

        public Task<IEnumerable<CustomerGroupCreditLimitLookupModel>> GetCustomerGroupCreditLimitLookupListAsync(string RegId, Int16 CompanyId, Int32 UserId);

        public Task<IEnumerable<CustomerLookupModel>> GetCustomerLookupListAsync(string RegId, Int16 CompanyId, Int32 UserId);

        public Task<IEnumerable<DepartmentLookupModel>> GetDepartmentLookupListAsync(string RegId, Int16 CompanyId, Int32 UserId);

        public Task<IEnumerable<DesignationLookupModel>> GetDesignationLookupListAsync(string RegId, Int16 CompanyId, Int32 UserId);

        public Task<IEnumerable<EmployeeLookupModel>> GetEmployeeLookupListAsync(string RegId, Int16 CompanyId, Int32 UserId);

        public Task<IEnumerable<GroupCreditLimitLookupModel>> GetGroupCreditLimitLookupListAsync(string RegId, Int16 CompanyId, Int32 UserId);

        public Task<IEnumerable<GstLookupModel>> GetGstLookupListAsync(string RegId, Int16 CompanyId, Int32 UserId);

        public Task<IEnumerable<GstCategoryLookupModel>> GetGstCategoryLookupListAsync(string RegId, Int16 CompanyId, Int32 UserId);

        public Task<IEnumerable<OrderTypeCategoryLookupModel>> GetOrderTypeCategoryLookupListAsync(string RegId, Int16 CompanyId, Int32 UserId);

        public Task<IEnumerable<OrderTypeLookupModel>> GetOrderTypeLookupListAsync(string RegId, Int16 CompanyId, Int32 UserId);

        public Task<IEnumerable<PaymentTypeLookupModel>> GetPaymentTypeLookupListAsync(string RegId, Int16 CompanyId, Int32 UserId);

        public Task<IEnumerable<PortLookupModel>> GetPortLookupListAsync(string RegId, Int16 CompanyId, Int32 UserId);

        public Task<IEnumerable<PortRegionLookupModel>> GetPortRegionLookupListAsync(string RegId, Int16 CompanyId, Int32 UserId);

        public Task<IEnumerable<ProductLookupModel>> GetProductLookupListAsync(string RegId, Int16 CompanyId, Int32 UserId);

        public Task<IEnumerable<SubCategoryLookupModel>> GetSubCategoryLookupListAsync(string RegId, Int16 CompanyId, Int32 UserId);

        public Task<IEnumerable<SupplierLookupModel>> GetSupplierLookupListAsync(string RegId, Int16 CompanyId, Int32 UserId);

        public Task<IEnumerable<UomLookupModel>> GetUomLookupListAsync(string RegId, Int16 CompanyId, Int32 UserId);

        public Task<IEnumerable<VoyageLookupModel>> GetVoyageLookupListAsync(string RegId, Int16 CompanyId, Int32 UserId);

        public Task<IEnumerable<VesselLookupModel>> GetVesselLookupListAsync(string RegId, Int16 CompanyId, Int32 UserId);

        public Task<IEnumerable<BargeLookupModel>> GetBargeLookupListAsync(string RegId, Int16 CompanyId, Int32 UserId);
    }
}