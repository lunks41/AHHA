﻿using AHHA.Core.Entities.Accounts.AR;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Entities.Setting;
using AHHA.Core.Models.Account.AR;
using AHHA.Core.Models.Admin;
using AHHA.Core.Models.Masters;
using AHHA.Core.Models.Setting;
using AutoMapper;

namespace AHHA.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region Admin

            CreateMap<UserViewModel, AdmUser>().ReverseMap();
            CreateMap<UserGroupViewModel, M_Uom>().ReverseMap();
            CreateMap<UserGroupRightsViewModel, AdmUserGroupRights>().ReverseMap();
            CreateMap<UserRightsViewModel, AdmUserRights>().ReverseMap();

            #endregion Admin

            #region Masters

            CreateMap<AccountGroupViewModel, M_AccountGroup>().ReverseMap();
            CreateMap<AccountSetupCategoryViewModel, M_AccountSetupCategory>().ReverseMap();
            CreateMap<AccountTypeViewModel, M_AccountType>().ReverseMap();
            CreateMap<AccountSetupViewModel, M_AccountSetup>().ReverseMap();
            CreateMap<BankViewModel, M_Bank>().ReverseMap();
            CreateMap<BargeViewModel, M_Barge>().ReverseMap();
            CreateMap<CategoryViewModel, M_Category>().ReverseMap();
            CreateMap<ChartOfAccountViewModel, M_ChartOfAccount>().ReverseMap();
            CreateMap<COACategoryViewModel, M_COACategory1>().ReverseMap();
            CreateMap<COACategoryViewModel, M_COACategory2>().ReverseMap();
            CreateMap<COACategoryViewModel, M_COACategory3>().ReverseMap();
            CreateMap<CountryLookupModel, M_Country>().ReverseMap();
            CreateMap<CreditTermViewModel, M_CreditTerm>().ReverseMap();
            CreateMap<CurrencyViewModel, M_Currency>().ReverseMap();
            CreateMap<CustomerGroupCreditLimitViewModel, M_CustomerGroupCreditLimit>().ReverseMap();
            CreateMap<CustomerAddressViewModel, M_CustomerAddress>().ReverseMap();
            CreateMap<CustomerContactViewModel, M_CustomerContact>().ReverseMap();
            CreateMap<CustomerViewModel, M_Customer>().ReverseMap();
            CreateMap<CustomerCreditLimitViewModel, M_CustomerCreditLimit>().ReverseMap();
            CreateMap<DepartmentViewModel, M_Department>().ReverseMap();
            CreateMap<DesignationViewModel, M_Designation>().ReverseMap();
            CreateMap<EmployeeViewModel, M_Employee>().ReverseMap();
            CreateMap<GroupCreditLimit_CustomerViewModel, M_GroupCreditLimit_Customer>().ReverseMap();
            CreateMap<GroupCreditLimitViewModel, M_GroupCreditLimit>().ReverseMap();
            CreateMap<GstCategoryViewModel, M_GstCategory>().ReverseMap();
            CreateMap<GstViewModel, M_Gst>().ReverseMap();
            CreateMap<OrderTypeCategoryViewModel, M_OrderTypeCategory>().ReverseMap();
            CreateMap<OrderTypeViewModel, M_OrderType>().ReverseMap();
            CreateMap<PaymentTypeViewModel, M_PaymentType>().ReverseMap();
            CreateMap<PortViewModel, M_Port>().ReverseMap();
            CreateMap<PortRegionViewModel, M_PortRegion>().ReverseMap();
            CreateMap<ProductViewModel, M_Product>().ReverseMap();
            CreateMap<SubCategoryViewModel, M_SubCategory>().ReverseMap();
            CreateMap<SupplierAddressViewModel, M_SupplierAddress>().ReverseMap();
            CreateMap<SupplierContactViewModel, M_SupplierContact>().ReverseMap();
            CreateMap<SupplierBankViewModel, M_SupplierBank>().ReverseMap();
            CreateMap<SupplierViewModel, M_Supplier>().ReverseMap();
            CreateMap<TaxCategoryViewModel, M_TaxCategory>().ReverseMap();
            CreateMap<TaxViewModel, M_Tax>().ReverseMap();
            CreateMap<UomViewModel, M_Uom>().ReverseMap();
            CreateMap<VesselViewModel, M_Vessel>().ReverseMap();
            CreateMap<VoyageViewModel, M_Voyage>().ReverseMap();

            #endregion Masters

            #region ACCOUNTS

            CreateMap<ARInvoiceViewModel, ArInvoiceHd>().ReverseMap();

            #endregion ACCOUNTS

            #region Settings

            CreateMap<VisibleFieldsViewModel, S_VisibleFields>().ReverseMap();
            CreateMap<MandatoryFieldsViewModel, S_MandatoryFields>().ReverseMap();
            CreateMap<DynamicLookupViewModel, S_DynamicLookup>().ReverseMap();
            CreateMap<DocSeqNoViewModel, S_DocSeqNo>().ReverseMap();

            #endregion Settings
        }
    }
}