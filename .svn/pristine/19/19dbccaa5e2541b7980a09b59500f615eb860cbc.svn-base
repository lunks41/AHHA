using AHHA.Core.Common;
using AHHA.Core.Entities.Setting;
using AHHA.Core.Models.Setting;

namespace AHHA.Application.IServices.Setting
{
    public interface IMandatoryFieldsServices
    {
        public Task<MandatoryFieldsViewModelCount> GetMandatoryFieldsListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<MandatoryFieldsViewModel> GetMandatoryFieldsByIdAsync(string RegId, Int16 CompanyId, Int16 ModuleId, Int32 TransactionId, Int16 UserId);

        public Task<SqlResponce> SaveMandatoryFieldsAsync(string RegId, Int16 CompanyId, S_MandatoryFields s_MandatoryFields, Int16 UserId);
    }
}