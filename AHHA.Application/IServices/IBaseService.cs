using AHHA.Core.Entities.Admin;
using AHHA.Core.Models;
using AHHA.Core.Models.Admin;

namespace AHHA.Application.IServices
{
    public interface IBaseService
    {
        UserGroupRightsViewModel ValidateScreen(string RegId,Int16 companyId, Int16 ModuleId, Int32 TransactionId, Int32 userId);
    }
}
