using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ts.ictu.Models
{
    public class LearningRegisterModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập họ tên")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập sđt")]
        public string Phone { get; set; }

        public string Email { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập địa chỉ")]
        public string Address { get; set; }

        public DateTime Birthday { get; set; }

        public DateTime Created { get; set; }

        public int Status { get; set; }

        public string Class { get; set; }

        public bool Gender { get; set; }
    }
}