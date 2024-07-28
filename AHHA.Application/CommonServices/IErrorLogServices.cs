
using AHHA.Core.Entities.Admin;


namespace AHHA.Application.CommonServices
{
    public interface IErrorLogServices
    {
        public Task AddErrorLogAsync(AdmErrorLog errorLog);
    }
}
