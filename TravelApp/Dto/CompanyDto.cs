using System.ComponentModel.DataAnnotations;

namespace TravelApp.Dto
{
    // Input DTOs
    public class CreateCompanyDTO
    {
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public string CompanyAddress { get; set; }
        [Required]
        public string ContactInformation { get; set; }
        [Required]
        public string UserID { get; set; }
        public int? RequestID { get; set; }
    }

    public class AdminCreateCompanyDTO
    {
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public string CompanyAddress { get; set; }
        [Required]
        public string ContactInformation { get; set; }
        [Required]
        public string UserID { get; set; }
    }



    public class UpdateCompanyDTO
    {
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string ContactInformation { get; set; }
    }

    // Output DTOs
    public class CompanyDTO
    {
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string ContactInformation { get; set; }
        public string UserID { get; set; }
        public string LogoBase64 { get; set; }
    }

    public class UserCompaniesDTO
    {
        public ICollection<CompanyDTO> Companys { get; set; }
    }

    public class UserBookedWithCompanyDTO
    {
        public string? UserID { get; set; }
        public int AdId { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
    }

        //public class CompanyResponseDTO
        //{
        //    public int CompanyID { get; set; }
        //    public string CompanyName { get; set; }
        //    public string ContactInformation { get; set; }
        //    public bool IsCompanyValidated { get; set; }
        //}


    }
