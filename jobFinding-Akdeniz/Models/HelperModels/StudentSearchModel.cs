using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace jobFinding_Akdeniz.Models.HelperModels
{
    public class StudentSearchModel
    {
        public Nullable<int> intrestedSectorId { get; set; }
        public string statusStd { get; set; }
        [Required]
        public string department { get; set; }
        public int languageID { get; set; }

    }
}