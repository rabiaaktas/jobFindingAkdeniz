using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace jobFinding_Akdeniz.Models.HelperModels
{
    public class TeacherRegisterModel
    {
        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail adresi geçerli değil.")]
        public string userEmail { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "İsim en az 4 karakter uzunluğunda olmalıdır.")]
        public string firstName { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Soyisim en az 4 karakter uzunluğunda olmalıdır.")]
        public string lastName { get; set; }

        [Required(ErrorMessage = "Parola gereklidir.", AllowEmptyStrings = false)]
        [StringLength(100, ErrorMessage = "Parola en az {2} karakter uzunluğunda olmalıdır.", MinimumLength = 6)]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{6,}$", ErrorMessage = "Parola en az 6 karakter uzunluğunda olmalıdır ve büyük harf (A-Z), küçük harf (a-z), sayı (0-9) ve özel karakter (e.g. !@#$%^&*) özelliklerinden en az 3 tanesini içermelidir.")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [DataType(DataType.Password)]
        [Compare("password", ErrorMessage = "Parola ve parola onay eşleşmiyor.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string degree { get; set; }

        [Required]
        public string universityName { get; set; }

        [Required]
        public string description { get; set; }

    }
}