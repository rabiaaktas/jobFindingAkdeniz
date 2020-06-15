using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace jobFinding_Akdeniz.Models.HelperModels
{
    public class StudentRegisterModel
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
        public string statusStd { get; set; }

        [Required]
        public string degreeName { get; set; }

        [Required]
        public string universityName { get; set; }

        [Required]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "4 karakter uzunluğunda olmalıdır.")]
        public string startingDate { get; set; }

        [StringLength(4, MinimumLength = 4, ErrorMessage = "4 karakter uzunluğunda olmalıdır.")]
        public string endingDate { get; set; }

        [Required]
        public string department { get; set; }

        [Range(0, 4.0, ErrorMessage = "Geçerli bir GANO giriniz.")]
        public string GANO { get; set; }

        [RegularExpression("^[0-9]*$", ErrorMessage = "GANO value must be integer.")]
        public string GANOINT { get; set; }

    }
}