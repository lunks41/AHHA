using AutoMapper;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Entities.Masters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Application.CommonServices
{
    public class AuditLogServices : IAuditLogServices
    {
        private readonly IRepository<AdmAuditLog> _repository;
        public AuditLogServices(IRepository<AdmAuditLog> repository)
        {
            _repository = repository;
        }

        public async Task AddAuditLogAsync(AdmAuditLog auditLog)
        {
            try
            {
                await _repository.CreateAsync(auditLog);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

    }
}
