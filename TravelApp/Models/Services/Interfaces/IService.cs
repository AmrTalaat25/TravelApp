using Microsoft.AspNetCore.Mvc;
using TravelApp.Dto;

namespace TravelApp.Models.Services.Interfaces
{
    public interface IService<T, TCreateDTO, TUpdateDTO, TDTO>
    {
        Task<ActionResult<IEnumerable<TDTO>>> GetAllT();
        Task<ActionResult<TDTO>> GetById(int id);
        Task<ActionResult<TDTO>> Create(TCreateDTO dto);
        ActionResult<string> Update(int id, TUpdateDTO dto);
        ActionResult<string> Delete(int id);
        ActionResult<IEnumerable<TDTO>> GetByUsername(string username);

    }
    //public class CompanyService : IService<Company, CreateCompanyDTO, UpdateCompanyDTO, CompanyDTO>
    //{
    //}

    //public class AdvertisementService : IService<Advertisement, CreateAdvertisementDTO, UpdateAdvertisementDTO, AdvertisementDTO>
    //{
    //}

}
