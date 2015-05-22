using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ts.ictu.Models
{
    public class BackupModel
    {
        public int ID { get; set; }
        public string FilePath { get; set; }

        [Required(ErrorMessage = "You must enter file name")]
        [RegularExpression(@"[^/\:*?""<>|]{1,}", ErrorMessage = " File name must not contain one of the following characters /\\:*?\"<>|")]
        [Display(Name = "Filename backup")]
        public string Name { get; set; }
        public string Date { get; set; }
    }

    public class BackupImageModel : BackupModel
    {
        [Required(ErrorMessage = "Please select a start date to backup")]
        [Display(Name = "Start date")]
        public string DateStart { get; set; }

        [Required(ErrorMessage = "Please select a end date want to backup")]
        [Display(Name = "End date")]
        public string DateEnd { get; set; }
    }
    public class SystemModel
    {
        public string PathBackupFolder { get; set; }
    }
}