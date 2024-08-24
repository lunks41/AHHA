
using AHHA.Core.Entities.Admin;


namespace AHHA.Application.CommonServices
{
    public interface IAuditLogServices
    {
        public Task AddAuditLogAsync(AdmAuditLog auditLog);
    }
}
