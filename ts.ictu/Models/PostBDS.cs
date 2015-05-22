using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ts.ictu.Models
{
    public class PostBDS
    {
        [Required(ErrorMessage = "Trường này phải có dữ liệu")]
        //[StringLength(100, ErrorMessage = "{0} phải có ít nhất {2} ký tự.", MinimumLength = 6)]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; }

        [Display(Name = "Nội dung")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Trường này phải có dữ liệu")]
        [Display(Name = "Họ tên")]
        public string ContactName { get; set; }

        [Required(ErrorMessage = "Trường này phải có dữ liệu")]
        [Display(Name = "Số điện thoại")]
        public string ContactPhone { get; set; }

        [Display(Name = "Email")]
        public string ContactEmail { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Nhập đúng giá tiền, giá tiền là số và phải > 0")]
        [Display(Name = "Số tiền")]
        public double? Money { get; set; }

        [Required(ErrorMessage = "Trường này phải có dữ liệu")]
        [Display(Name = "Tỉnh / Thành phố")]
        public int Province { get; set; }

        [Required(ErrorMessage = "Trường này phải có dữ liệu")]
        [Display(Name = "Quận / Huyện")]
        public int District { get; set; }

        [Display(Name = "Phường / Xã")]
        public int? Ward { get; set; }

        [Required(ErrorMessage = "Trường này phải có dữ liệu")]
        [Display(Name = "Loại bất động sản")]
        public int LoaiBDSID { get; set; }

        [Required(ErrorMessage = "Trường này phải có dữ liệu")]
        [Display(Name = "Hình thức")]
        public int HinhThuc { get; set; }
    }
}