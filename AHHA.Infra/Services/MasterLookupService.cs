﻿using AHHA.Application.CommonServices;
using AHHA.Application.IServices;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Models.Masters;
using AHHA.Infra.Data;
using Microsoft.Extensions.Configuration;

namespace AHHA.Infra.Services.Masters
{
    internal sealed class MasterLookupService : IMasterLookupService
    {
        private readonly IRepository<dynamic> _repository;
        private ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private short recordCount = 0;

        public MasterLookupService(IRepository<dynamic> repository, ApplicationDbContext context, IConfiguration configuration)
        {
            _repository = repository;
            _context = context;
            _configuration = configuration;
            recordCount = Convert.ToInt16(_configuration["LookupDefault:RecordCount"]);
        }

        public async Task<IEnumerable<AccountSetupCategoryLookupModel>> GetAccountSetupCategoryLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<AccountSetupCategoryLookupModel>(RegId, $"select AccSetupCategoryId,AccSetupCategoryCode,AccSetupCategoryName from M_AccountSetupCategory where AccSetupCategoryId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.AccountSetupCategory})) order by AccSetupCategoryName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.AccountSetupCategory,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_AccountSetupCategory",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<AccountSetupLookupModel>> GetAccountSetupLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<AccountSetupLookupModel>(RegId, $"select AccSetupId,AccSetupCode,AccSetupName from M_AccountSetup where AccSetupId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.AccountSetup})) order by AccSetupName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.AccountSetup,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_AccountSetup",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<AccountTypeLookupModel>> GetAccountTypeLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<AccountTypeLookupModel>(RegId, $"select AccTypeId,AccTypeCode,AccTypeName from M_AccountType where AccTypeId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.AccountType})) order by AccTypeName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.AccountType,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_AccountType",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<AccountGroupLookupModel>> GetAccountGroupLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<AccountGroupLookupModel>(RegId, $"select AccGroupId,AccGroupCode,AccGroupName from M_AccountGroup where AccGroupId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.AccountGroup})) order by AccGroupName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.AccountGroup,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_AccountGroup",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<BankLookupModel>> GetBankLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<BankLookupModel>(RegId, $"select BankId,BankCode,BankName from M_Bank where BankId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Bank})) order by BankName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Bank,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_AccountSetup",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<BargeLookupModel>> GetBargeLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<BargeLookupModel>(RegId, $"SELECT BargeId,BargeCode,BargeName FROM M_Barge WHERE BargeId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Barge})) order by BargeName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Barge,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Barge",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<CategoryLookupModel>> GetCategoryLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<CategoryLookupModel>(RegId, $"select CategoryId,CategoryCode,CategoryName from M_Category where CategoryId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Category})) order by CategoryName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Category,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Category",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<ChartOfAccountLookupModel>> GetChartOfAccountLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<ChartOfAccountLookupModel>(RegId, $"select GLId,GLCode,GLName from M_ChartOfAccount where GLId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.ChartOfAccount})) order by GLName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.ChartOfAccount,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_ChartOfAccount",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<COACategoryLookupModel>> GetCOACategory1LookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<COACategoryLookupModel>(RegId, $"select COACategoryId,COACategoryCode,COACategoryName from M_COACategory1 where COACategoryId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.COACategory1})) order by COACategoryName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.COACategory1,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_COACategory1",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<COACategoryLookupModel>> GetCOACategory2LookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<COACategoryLookupModel>(RegId, $"select COACategoryId,COACategoryCode,COACategoryName from M_COACategory2 where COACategoryId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.COACategory2})) order by COACategoryName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.COACategory2,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_COACategory2",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<COACategoryLookupModel>> GetCOACategory3LookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<COACategoryLookupModel>(RegId, $"select COACategoryId,COACategoryCode,COACategoryName from M_COACategory3 where COACategoryId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.COACategory3})) order by COACategoryName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.COACategory3,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_COACategory3",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<CountryLookupModel>> GetCountryLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<CountryLookupModel>(RegId, $"SELECT CountryId,CountryCode,CountryName FROM M_Country WHERE CountryId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Country})) order by CountryName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Country,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Country",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<CurrencyLookupModel>> GetCurrencyLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<CurrencyLookupModel>(RegId, $"SELECT CurrencyId,CurrencyCode,CurrencyName,IsMultiply FROM M_Currency WHERE CurrencyId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Currency})) order by CurrencyName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Currency,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Currency",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<CustomerGroupCreditLimitLookupModel>> GetCustomerGroupCreditLimitLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<CustomerGroupCreditLimitLookupModel>(RegId, $"select GroupCreditLimitId,GroupCreditLimitCode,GroupCreditLimitName from M_CustomerGroupCreditLimit WHERE GroupCreditLimitId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.CustomerGroupCreditLimit})) order by GroupCreditLimitName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.CustomerGroupCreditLimit,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CustomerGroupCreditLimit",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<CustomerLookupModel>> GetCustomerLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<CustomerLookupModel>(RegId, $"select CustomerId,CustomerCode,CustomerName from M_Customer WHERE CustomerId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Customer})) order by CustomerName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Customer,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Customer",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<DepartmentLookupModel>> GetDepartmentLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<DepartmentLookupModel>(RegId, $"select DepartmentId,DepartmentCode,DepartmentName from M_Department WHERE DepartmentId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Department})) order by DepartmentName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Department,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Department",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<DesignationLookupModel>> GetDesignationLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<DesignationLookupModel>(RegId, $"select DesignationId,DesignationCode,DesignationName from M_Designation WHERE DesignationId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Designation})) order by DesignationName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Designation,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Designation",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<EmployeeLookupModel>> GetEmployeeLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<EmployeeLookupModel>(RegId, $"select EmployeeId,EmployeeCode,EmployeeName from M_Employee WHERE EmployeeId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Employee})) order by EmployeeName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Employee,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Employee",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<GroupCreditLimitLookupModel>> GetGroupCreditLimitLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<GroupCreditLimitLookupModel>(RegId, $"select GroupCreditLimitId,GroupCreditLimitCode,GroupCreditLimitName from M_GroupCreditLimit WHERE GroupCreditLimitId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.GroupCreditLimit})) order by GroupCreditLimitName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.GroupCreditLimit,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_GroupCreditLimit",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<GstLookupModel>> GetGstLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<GstLookupModel>(RegId, $"select GstId,GstCode,GstName from M_Gst WHERE GstId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Gst})) order by GstName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Gst,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Gst",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<GstCategoryLookupModel>> GetGstCategoryLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<GstCategoryLookupModel>(RegId, $"select GstCategoryId,GstCategoryCode,GstCategoryName from M_GstCategory WHERE GstCategoryId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.GstCategory})) order by GstCategoryName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.GstCategory,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_GstCategory",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<OrderTypeCategoryLookupModel>> GetOrderTypeCategoryLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<OrderTypeCategoryLookupModel>(RegId, $"select OrderTypeCategoryId,OrderTypeCategoryCode,OrderTypeCategoryName from M_OrderTypeCategory WHERE OrderTypeCategoryId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.OrderTypeCategory})) order by OrderTypeCategoryName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.OrderTypeCategory,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_OrderTypeCategory",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<OrderTypeLookupModel>> GetOrderTypeLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<OrderTypeLookupModel>(RegId, $"select OrderTypeId,OrderTypeCode,OrderTypeName from M_OrderType WHERE OrderTypeId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.OrderType})) order by OrderTypeName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.OrderType,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_OrderType",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<PaymentTypeLookupModel>> GetPaymentTypeLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<PaymentTypeLookupModel>(RegId, $"select PaymentTypeId,PaymentTypeCode,PaymentTypeName from M_PaymentType WHERE PaymentTypeId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.PaymentType})) order by PaymentTypeName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.PaymentType,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_PaymentType",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<PortLookupModel>> GetPortLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<PortLookupModel>(RegId, $"select PortId,PortCode,PortName from M_Port WHERE PortId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Port})) order by PortName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Port,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Port",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<PortRegionLookupModel>> GetPortRegionLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<PortRegionLookupModel>(RegId, $"select PortRegionId,PortRegionCode,PortRegionName from M_PortRegion WHERE PortRegionId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.PortRegion})) order by PortRegionName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.PortRegion,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_PortRegion",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<ProductLookupModel>> GetProductLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<ProductLookupModel>(RegId, $"select ProductId,ProductCode,ProductName from M_Product WHERE ProductId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Product})) order by ProductName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Product,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Product",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<SubCategoryLookupModel>> GetSubCategoryLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<SubCategoryLookupModel>(RegId, $"select SubCategoryId,SubCategoryCode,SubCategoryName from M_SubCategory WHERE SubCategoryId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.SubCategory})) order by SubCategoryName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.SubCategory,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_SubCategory",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<SupplierLookupModel>> GetSupplierLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<SupplierLookupModel>(RegId, $"select SupplierId,SupplierCode,SupplierName from M_Supplier WHERE SupplierId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Supplier})) order by SupplierName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Supplier,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Supplier",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<UomLookupModel>> GetUomLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<UomLookupModel>(RegId, $"select UomId,UomCode,UomName from M_Uom WHERE UomId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Uom})) order by UomName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Uom,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Uom",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<VoyageLookupModel>> GetVoyageLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<VoyageLookupModel>(RegId, $"select VoyageId,VoyageNo,ReferenceNo from M_Voyage WHERE VoyageId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Voyage})) order by VoyageNo");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Voyage,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Voyage",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<CreditTermsLookupModel>> GetCreditTermsLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<CreditTermsLookupModel>(RegId, $"SELECT CreditTermId,CreditTermCode,CreditTermName FROM dbo.M_CreditTerm WHERE CreditTermId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.CreditTerms})) order by CreditTermName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.CreditTerms,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CreditTerm",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<TaxLookupModel>> GetTaxLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<TaxLookupModel>(RegId, $"SELECT TaxId,TaxCode,TaxName FROM dbo.M_Tax WHERE TaxId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Tax})) order by TaxName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Tax,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CreditTerm",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<VesselLookupModel>> GetVesselLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<VesselLookupModel>(RegId, $"SELECT VesselId,VesselCode,VesselName FROM M_Vessel WHERE VesselId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Vessel})) order by VesselName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Vessel,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Vessel",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<VesselLookupModel>> GetVesselLookupListAsync_V1(string RegId, Int16 CompanyId, string searchString, Int16 RecordCount, Int16 UserId)
        {
            RecordCount = RecordCount == 0 ? recordCount : RecordCount;
            try
            {
                var result = await _repository.GetQueryAsync<VesselLookupModel>(RegId, $"SELECT TOP {RecordCount} VesselId,VesselCode,VesselName FROM M_Vessel WHERE VesselId<>0 And IsActive=1 And (VesselName like '%{searchString}%' OR VesselCode like '%{searchString}%') And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Vessel})) order by VesselName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Vessel,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Vessel",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<UserGroupLookupModel>> GetUserGroupLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<UserGroupLookupModel>(RegId, $"SELECT UserGroupId,UserGroupCode,UserGroupName FROM AdmUserGroup WHERE UserGroupId<>0 And IsActive=1  order by UserGroupName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Admin,
                    TransactionId = (short)E_Admin.UserGroup,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "AdmUserGroup",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<UserLookupModel>> GetUserLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<UserLookupModel>(RegId, $"SELECT UserId,UserCode,UserName FROM AdmUser WHERE UserId<>0 And IsActive=1  order by UserName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Admin,
                    TransactionId = (short)E_Admin.User,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "AdmUser",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<CustomerAddressLookupModel>> GetCustomerAddressLookup_FinListAsync(string RegId, Int16 CompanyId, Int16 UserId, Int16 CustomerId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<CustomerAddressLookupModel>(RegId, $"select AddressId, Address1, Address2, Address3, Address4, PinCode, CountryId, PhoneNo, FaxNo, EmailAdd, WebUrl from M_CustomerAddress where IsActive=1 And CustomerId={CustomerId} order by IsFinAdd Desc,IsDefaultAdd desc");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Customer,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CustomerAddress",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<CustomerContactLookupModel>> GetCustomerContactLookup_FinListAsync(string RegId, Int16 CompanyId, Int16 UserId, Int16 CustomerId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<CustomerContactLookupModel>(RegId, $"Select ContactId, ContactName, OtherName, MobileNo, OffNo, FaxNo, EmailAdd, MessId, ContactMessType from M_CustomerContact where IsActive=1 And CustomerId={CustomerId} Order by IsFinance Desc,IsDefault desc");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Customer,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CustomerContact",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<ModuleLookupModel>> GetModuleLookupAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<ModuleLookupModel>(RegId, $"Select ModuleId,ModuleCode,ModuleName from AdmModule where IsActive=1 Order by ModuleName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Admin,
                    TransactionId = (short)E_Admin.Modules,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "AdmModule",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<TransactionLookupModel>> GetTransactionLookupAsync(string RegId, Int16 CompanyId, Int16 UserId, Int16 ModuleId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<TransactionLookupModel>(RegId, $"Select TransactionId,TransactionCode,TransactionName from AdmTransaction where IsActive=1 And ModuleId={ModuleId} Order by TransactionName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Admin,
                    TransactionId = (short)E_Admin.Transaction,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "AdmTransaction",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<TaxCategoryLookupModel>> GetTaxCategoryLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<TaxCategoryLookupModel>(RegId, $"SELECT TaxCategoryId,TaxCategoryCode,TaxCategoryName FROM dbo.M_TaxCategory WHERE TaxId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.TaxCategory})) order by TaxCategoryName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.TaxCategory,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CreditTerm",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        //public async Task<IEnumerable<BargeLookupModel>> GetBargeLookupListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        //{
        //    try
        //    {
        //        var result = await _repository.GetQueryAsync<BargeLookupModel>(RegId, $"SELECT BargeId,BargeCode,BargeName FROM M_Barge WHERE BargeId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Master.Barge},{(short)E_Modules.Master})) order by BargeName");

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        var errorLog = new AdmErrorLog
        //        {
        //            CompanyId = CompanyId,
        //            ModuleId = (short)E_Modules.Master,
        //            TransactionId = (short)E_Master.Barge,
        //            DocumentId = 0,
        //            DocumentNo = "",
        //            TblName = "M_Barge",
        //            ModeId = (short)E_Mode.Lookup,
        //            Remarks = ex.Message + ex.InnerException,
        //            CreateById = UserId
        //        };

        //        _context.Add(errorLog);
        //        _context.SaveChanges();

        //        throw new Exception(ex.ToString());
        //    }
        //}
    }
}