using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace jobFinding_Akdeniz.Models.HelperModels
{
    public class CompanyRegisterModel
    {
        [Required]
        [StringLength(200, MinimumLength = 4, ErrorMessage = "Şirket adı en az 4 karakter uzunluğunda olmalıdır.")]
        public string companyName { get; set; }

        [Required]
        public int businessID { get; set; }

        public string companyAddress { get; set; }

        [RegularExpression(@"^http(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_]*)?$", ErrorMessage = "URL türü yanlış.")]
        public string webSiteUrl { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail adresi geçerli değil.")]
        public string companyEmail { get; set; }

        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
                   ErrorMessage = "Telefon numarası geçerli değil.")]
        public string companyPhone { get; set; }

        [Required(ErrorMessage = "Parola gereklidir.", AllowEmptyStrings = false)]
        [StringLength(100, ErrorMessage = "Parola en az {2} karakter uzunluğunda olmalıdır.", MinimumLength = 6)]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{6,}$", ErrorMessage = "Parola en az 6 karakter uzunluğunda olmalıdır ve büyük harf (A-Z), küçük harf (a-z), sayı (0-9) ve özel karakter (e.g. !@#$%^&*) özelliklerinden en az 3 tanesini içermelidir.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Parola ve parola onay eşleşmiyor.")]
        public string ConfirmPassword { get; set; }
    }
}

