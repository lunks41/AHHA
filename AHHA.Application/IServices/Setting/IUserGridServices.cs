﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Setting;
using AHHA.Core.Models.Setting;

namespace AHHA.Application.IServices.Setting
{
    public interface IUserGridServices
    {
        public Task<UserGridViewModelCount> GetUserGridAsync(string RegId, Int16 CompanyId, Int16 ModuleId, Int16 TransactionId, Int16 UserId);

        public Task<UserGridViewModel> GetUserGridByIdAsync(string RegId, Int16 CompanyId, UserGridViewModel userGridViewModel, Int16 UserId);

        public Task<IEnumerable<UserGridViewModel>> GetUserGridByUserIdAsync(string RegId, Int16 CompanyId, Int16 SelectedUserId, Int16 UserId);

        public Task<SqlResponce> SaveUserGridAsync(string RegId, Int16 CompanyId, S_UserGrdFormat s_UserGrdFormat, Int16 UserId);
        public Task<SqlResponce> CloneUserGridSettingAsync(string RegId, Int16 CompanyId, Int16 FromUserId, Int16 ToUserId, Int16 UserId);
    }
}