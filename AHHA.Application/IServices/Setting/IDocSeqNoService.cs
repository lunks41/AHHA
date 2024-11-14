﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Setting;
using AHHA.Core.Models.Setting;

namespace AHHA.Application.IServices.Setting
{
    public interface IDocSeqNoService
    {
        //public Task<DocSeqNoViewModelCount> GetDocSeqNoListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<DocSeqNoViewModel> GetDocSeqNoByTransactionAsync(string RegId, Int16 CompanyId, Int16 ModuleId, Int16 TransactionId, Int16 UserId);

        //public Task<SqlResponce> SaveDocSeqNoAsync(string RegId, Int16 CompanyId, S_DocSeqNo s_DocSeqNo, Int16 UserId);
    }
}