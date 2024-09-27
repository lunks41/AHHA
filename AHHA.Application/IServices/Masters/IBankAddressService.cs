﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IBankAddressService
    {
        public Task<IEnumerable<BankAddressViewModel>> GetBankAddressByBankIdAsync(string RegId, Int16 CompanyId, Int16 BankId, Int16 UserId);

        public Task<BankAddressViewModel> GetBankAddressByIdAsync(string RegId, Int16 CompanyId, Int16 BankId, Int16 AddressId, Int16 UserId);

        public Task<SqlResponce> SaveBankAddressAsync(string RegId, Int16 CompanyId, M_BankAddress m_BankAddress, Int16 UserId);

        public Task<SqlResponce> DeleteBankAddressAsync(string RegId, Int16 CompanyId, Int16 BankId, Int16 AddressId, Int16 UserId);
    }
}