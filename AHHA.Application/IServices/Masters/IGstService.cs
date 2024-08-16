using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IGstService
    {
        public Task<GstViewModelCount> GetGstListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_Gst> GetGstByIdAsync(string RegId, Int16 CompanyId, Int16 COACategoryId, Int32 UserId);
        public Task<SqlResponce> AddGstAsync(string RegId, Int16 CompanyId, M_Gst M_Gst, Int32 UserId);
        public Task<SqlResponce> UpdateGstAsync(string RegId, Int16 CompanyId, M_Gst M_Gst, Int32 UserId);
        public Task<SqlResponce> DeleteGstAsync(string RegId, Int16 CompanyId, M_Gst M_Gst, Int32 UserId);
    }
}
