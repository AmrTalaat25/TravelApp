using TravelApp.Dto;

namespace TravelApp.Models.Services.Interfaces
{
    public interface IPermissionRequestService
    {
        public Task<ResponseModel<PermissionRequestDTO>> CreatePermissionRequest(CreatePermissionRequestDTO request, string userId);
        public Task<ResponseModel<IEnumerable<PermissionRequestDTO>>> GetPermissionRequests();
        public Task<ResponseModel<PermissionRequestDetailedDto>> GetPermissionRequestsById(int requestID);
        public Task<ResponseModel<List<PermissionRequestDetailedDto>>> GetPermissionRequestsByUser(string userID);
        public Task<ResponseModel<byte[]>> GetPermissionRequestDocument(int requestID);
        public Task<ResponseModel<string>> ApprovePermissionRequest(int requestID);
        public Task<ResponseModel<string>> RejectPermissionRequest(int requestID);
        public Task<ResponseModel<string>> UpdatePermissionRequest(int requestID, UpdatePermissionRequestDTO request);
    }
}
