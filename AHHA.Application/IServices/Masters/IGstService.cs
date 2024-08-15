using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IGstService
    {
        public Task<GstViewModelCount> GetGstListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_Gst> GetGstByIdAsync(Int16 CompanyId, Int16 COACategoryId, Int32 UserId);
        public Task<SqlResponce> AddGstAsync(Int16 CompanyId, M_Gst M_Gst, Int32 UserId);
        public Task<SqlResponce> UpdateGstAsync(Int16 CompanyId, M_Gst M_Gst, Int32 UserId);
        public Task<SqlResponce> DeleteGstAsync(Int16 CompanyId, M_Gst M_Gst, Int32 UserId);
    }
}
