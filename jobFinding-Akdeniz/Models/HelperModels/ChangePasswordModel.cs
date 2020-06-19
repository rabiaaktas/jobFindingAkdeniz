using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace jobFinding_Akdeniz.Models.HelperModels
{
    public class ChangePasswordModel
    {
        public int ID { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

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