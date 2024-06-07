using Microsoft.AspNetCore.Mvc;
using TravelApp.Dto;

namespace TravelApp.Models.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<bool> CreateCompany(CreateCompanyDTO company);
        Task<ResponseModel<CompanyDTO>> AdminCreateCompany(AdminCreateCompanyDTO company);
        Task<ResponseModel<IEnumerable<CompanyDTO>>> GetAllCompanies();
        Task<ResponseModel<CompanyDTO>> GetByIdCompany(int id);
        Task<ResponseModel<string>> DeleteCompany(int id);
        Task<ResponseModel<CompanyDTO>> UpdateCompany(int id, UpdateCompanyDTO companyDTO);
        Task<ResponseModel<IEnumerable<UserBookedWithCompanyDTO>>> GetUsersForCompanyBooking(int companyId);
        Task<ResponseModel<IEnumerable<CompanyDTO>>> GetCompaniesByUserId(string userId);
        Task<ResponseModel<List<CompanyDTO>>> GetCompaniesByCompanyName(string companyname);
        Task<ResponseModel<IEnumerable<AdvertisementDTO>>> GetAllAdvertisementByIDCompany(int id);
        Task<ResponseModel<string>> SetCompanyLogo(int companyId, IFormFile image);
    }
}