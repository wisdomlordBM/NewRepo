using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Jobportalwebsite.Models
{
    public class UploadCVViewModel
    {
        public int ApplicationId { get; set; }

        [Required(ErrorMessage = "Please upload your CV.")]
        [DataType(DataType.Upload)]
        public IFormFile CV { get; set; }
    }
}

