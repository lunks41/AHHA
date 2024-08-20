﻿using AHHA.Application.CommonServices;
using AHHA.Application.IServices;
using AHHA.Core.Models;

namespace AHHA.Infra.Services
{
    public class BaseService : IBaseService
    {
        private readonly IRepository<UserGroupRightsViewModel> _repository;

        public BaseService(IRepository<UserGroupRightsViewModel> repository)
        {
            _repository = repository;
        }

        public UserGroupRightsViewModel ValidateScreen(string RegId, Int16 companyId, Int16 ModuleId, Int32 TransactionId, Int32 userId)
        {
            try
            {
                //var albums = _context.AdmUserGroupRights.FromSqlRaw<AdmUserGroupRights>($"select GroupRights.ModuleId,GroupRights.TransactionId,GroupRights.IsRead,GroupRights.IsCreate,GroupRights.IsEdit,GroupRights.IsDelete,GroupRights.IsExport,GroupRights.IsPrint from AdmUserGroupRights GroupRights INNER Join AdmUser Auser on GroupRights.UserGroupId=Auser.UserGroupId inner join AdmUserRights UserRights on UserRights.UserId=AUser.UserId where UserRights.CompanyId={companyId} And UserRights.UserId= {userId}And GroupRights.ModuleId={ModuleId} And GroupRights.TransactionId={TransactionId}").FirstOrDefault();

                var userGroupRightsViewModels = _repository.GetQuerySingleOrDefaultAsync<UserGroupRightsViewModel>(RegId, $"select GroupRights.ModuleId,GroupRights.TransactionId,GroupRights.IsRead,GroupRights.IsCreate,GroupRights.IsEdit,GroupRights.IsDelete,GroupRights.IsExport,GroupRights.IsPrint from AdmUserGroupRights GroupRights INNER Join AdmUser Auser on GroupRights.UserGroupId=Auser.UserGroupId inner join AdmUserRights UserRights on UserRights.UserId=AUser.UserId where UserRights.CompanyId={companyId} And UserRights.UserId= {userId}And GroupRights.ModuleId={ModuleId} And GroupRights.TransactionId={TransactionId}");

                return userGroupRightsViewModels.Result;
            }
            catch
            {
                throw;
            }
        }
    }
}