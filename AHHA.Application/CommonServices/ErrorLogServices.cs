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
    public class ErrorLogServices : IErrorLogServices
    {
        private readonly IRepository<AdmErrorLog> _repository;
        public ErrorLogServices(IRepository<AdmErrorLog> repository)
        {
            _repository = repository;
        }

        public async Task AddErrorLogAsync(AdmErrorLog errorLog)
        {
            try
            {
                await _repository.CreateAsync(errorLog);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

    }
}
