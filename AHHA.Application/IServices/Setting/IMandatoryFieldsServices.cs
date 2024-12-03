﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Setting;
using AHHA.Core.Models.Setting;

namespace AHHA.Application.IServices.Setting
{
    public interface IMandatoryFieldsServices
    {
        public Task<MandatoryFieldsViewModelCount> GetMandatoryFieldsListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<MandatoryFieldsViewModel>> GetMandatoryFieldsByIdAsyncV1(string RegId, Int16 CompanyId, Int16 ModuleId, Int16 TransactionId, Int16 UserId);

        public Task<MandatoryFieldsViewModel> GetMandatoryFieldsByIdAsync(string RegId, Int16 CompanyId, Int16 ModuleId, Int16 TransactionId, Int16 UserId);

        public Task<SqlResponce> SaveMandatoryFieldsAsyncV1(string RegId, Int16 CompanyId, List<S_MandatoryFields> s_MandatoryFields, Int16 UserId);

        public Task<SqlResponce> SaveMandatoryFieldsAsync(string RegId, Int16 CompanyId, S_MandatoryFields s_MandatoryFields, Int16 UserId);
    }
}